//Josip Medved <jmedved@jmedved.com>   www.medo64.com

//2018-02-24: Added option to raise event on read/write instead of using registry.
//2010-10-31: Added option to skip registry writes (NoRegistryWrites).
//2010-10-17: Limited all loaded forms to screen's working area.
//            Changed LoadNowAndSaveOnClose to use SetupOnLoadAndClose.
//2009-07-04: Compatibility with Mono 2.4.
//2008-12-27: Added LoadNowAndSaveOnClose method.
//2008-07-10: Fixed resize on load when window is maximized.
//2008-05-11: Windows with fixed borders are no longer resized.
//2008-04-11: Cleaned code to match FxCop 1.36 beta 2 (CompoundWordsShouldBeCasedCorrectly).
//2008-02-15: Fixed bug with positioning of centered forms.
//2008-01-31: Fixed bug that caused only first control to be loaded/saved.
//2008-01-10: Moved private methods to Helper class.
//2008-01-05: Removed obsoleted methods.
//2008-01-02: Calls to MoveSplitterTo method are now checked.
//2007-12-27: Added Load overloads for multiple controls.
//            Obsoleted Subscribe method.
//2007-12-24: Changed SubKeyPath to include full path.
//2007-11-21: Initial version.


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Medo.Windows.Forms {

    /// <summary>
    /// Enables storing and loading of windows control's state.
    /// It is written in State key at HKEY_CURRENT_USER branch withing SubKeyPath of Settings class.
    /// This class is not thread-safe since it should be called only from one thread - one that has interface.
    /// </summary>
    public static class State {

        private static string _subkeyPath;
        /// <summary>
        /// Gets/sets subkey used for registry storage.
        /// </summary>
        public static string SubkeyPath {
            get {
                if (_subkeyPath == null) {
                    var assembly = Assembly.GetEntryAssembly();

                    string company = null;
                    var companyAttributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
                    if ((companyAttributes != null) && (companyAttributes.Length >= 1)) {
                        company = ((AssemblyCompanyAttribute)companyAttributes[companyAttributes.Length - 1]).Company;
                    }

                    string product = null;
                    var productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
                    if ((productAttributes != null) && (productAttributes.Length >= 1)) {
                        product = ((AssemblyProductAttribute)productAttributes[productAttributes.Length - 1]).Product;
                    } else {
                        var titleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
                        if ((titleAttributes != null) && (titleAttributes.Length >= 1)) {
                            product = ((AssemblyTitleAttribute)titleAttributes[titleAttributes.Length - 1]).Title;
                        } else {
                            product = assembly.GetName().Name;
                        }
                    }

                    var path = "Software";
                    if (!string.IsNullOrEmpty(company)) { path += "\\" + company; }
                    if (!string.IsNullOrEmpty(product)) { path += "\\" + product; }

                    _subkeyPath = path + "\\State";
                }
                return _subkeyPath;
            }
            set { _subkeyPath = value; }
        }

        /// <summary>
        /// Gets/sets whether settings should be written to registry.
        /// </summary>
        public static bool NoRegistryWrites { get; set; }


        #region Load Save - All

        /// <summary>
        /// Loads previous state.
        /// Supported controls are Form, PropertyGrid, ListView and SplitContainer.
        /// </summary>
        /// <param name="form">Form on which's FormClosing handler this function will attach. State will not be altered for this parameter.</param>
        /// <param name="controls">Controls to load and to save.</param>
        /// <exception cref="ArgumentNullException">Form is null.</exception>
        /// <exception cref="NotSupportedException">This control's parents cannot be resolved using Name property.</exception>
        /// <exception cref="ArgumentException">Form already used.</exception>
        [Obsolete("Use SetupOnLoadAndClose instead.")]
        public static void LoadNowAndSaveOnClose(Form form, params Control[] controls) {
            SetupOnLoadAndClose(form, controls);
        }

        /// <summary>
        /// Loads previous state.
        /// Supported controls are Form, PropertyGrid, ListView and SplitContainer.
        /// </summary>
        /// <param name="form">Form on which's FormClosing handler this function will attach. State will not be altered for this parameter.</param>
        /// <param name="controls">Controls to load and to save.</param>
        /// <exception cref="ArgumentNullException">Form is null.</exception>
        /// <exception cref="NotSupportedException">This control's parents cannot be resolved using Name property.</exception>
        /// <exception cref="ArgumentException">Form setup already done.</exception>
        public static void SetupOnLoadAndClose(Form form, params Control[] controls) {
            if (form == null) { throw new ArgumentNullException("form", "Form is null."); }

            if (formSetup.ContainsKey(form)) { throw new ArgumentException("Form setup already done.", "form"); }

            Load(form);
            if (controls != null) { Load(controls); }

            formSetup.Add(form, controls);
            form.Load += new EventHandler(form_Load);
            form.FormClosed += new FormClosedEventHandler(form_FormClosed);
        }

        private static Dictionary<Form, Control[]> formSetup = new Dictionary<Form, Control[]>();

        private static void form_Load(object sender, EventArgs e) {
            var form = sender as Form;
            if (formSetup.ContainsKey(form)) {
                Load(form);
                Load(formSetup[form]);
            }
        }

        private static void form_FormClosed(object sender, FormClosedEventArgs e) {
            var form = sender as Form;
            if (formSetup.ContainsKey(form)) {
                Save(form);
                Save(formSetup[form]);
                formSetup.Remove(form);
            }
        }

        /// <summary>
        /// Loads previous state.
        /// Supported controls are Form, PropertyGrid, ListView and SplitContainer.
        /// </summary>
        /// <param name="controls">Controls to load.</param>
        /// <exception cref="NotSupportedException">This control's parents cannot be resolved using Name property.</exception>
        public static void Load(params Control[] controls) {
            if (controls == null) { return; }
            for (var i = 0; i < controls.Length; ++i) {
                if (controls[i] is Form form) {
                    Load(form);
                } else if (controls[i] is PropertyGrid propertyGrid) {
                    Load(propertyGrid);
                } else if (controls[i] is ListView listView) {
                    Load(listView);
                } else if (controls[i] is SplitContainer splitContainer) {
                    Load(splitContainer);
                }
            }
        }

        /// <summary>
        /// Saves control's state.
        /// Supported controls are Form, PropertyGrid, ListView and SplitContainer.
        /// </summary>
        /// <param name="controls">Controls to save.</param>
        /// <exception cref="NotSupportedException">This control's parents cannot be resolved using Name property.</exception>
        public static void Save(params Control[] controls) {
            if (controls == null) { return; }
            for (var i = 0; i < controls.Length; ++i) {
                if (controls[i] is Form form) {
                    Save(form);
                } else if (controls[i] is PropertyGrid propertyGrid) {
                    Save(propertyGrid);
                } else if (controls[i] is ListView listView) {
                    Save(listView);
                } else if (controls[i] is SplitContainer splitContainer) {
                    Save(splitContainer);
                }
            }
        }

        #endregion


        #region Load Save - Form

        /// <summary>
        /// Saves Form state (Left,Top,Width,Height,WindowState).
        /// </summary>
        /// <param name="form">Form.</param>
        /// <exception cref="ArgumentNullException">Form is null.</exception>
        /// <exception cref="NotSupportedException">This control's parents cannot be resolved using Name property.</exception>
        public static void Save(Form form) {
            if (form == null) { throw new ArgumentNullException("form", "Form is null."); }

            var baseValueName = Helper.GetControlPath(form);

            Write(baseValueName + ".WindowState", Convert.ToInt32(form.WindowState, CultureInfo.InvariantCulture));
            if (form.WindowState == FormWindowState.Normal) {
                Write(baseValueName + ".Left", form.Bounds.Left);
                Write(baseValueName + ".Top", form.Bounds.Top);
                Write(baseValueName + ".Width", form.Bounds.Width);
                Write(baseValueName + ".Height", form.Bounds.Height);
            } else {
                Write(baseValueName + ".Left", form.RestoreBounds.Left);
                Write(baseValueName + ".Top", form.RestoreBounds.Top);
                Write(baseValueName + ".Width", form.RestoreBounds.Width);
                Write(baseValueName + ".Height", form.RestoreBounds.Height);
            }
        }

        /// <summary>
        /// Loads previous Form state (Left,Top,Width,Height,WindowState).
        /// If StartupPosition value is Manual, saved settings are used, for other types, it tryes to resemble original behaviour.
        /// </summary>
        /// <param name="form">Form.</param>
        /// <exception cref="ArgumentNullException">Form is null.</exception>
        /// <exception cref="NotSupportedException">This control's parents cannot be resolved using Name property.</exception>
        public static void Load(Form form) {
            if (form == null) { throw new ArgumentNullException("form", "Form is null."); }

            var baseValueName = Helper.GetControlPath(form);

            var currWindowState = Convert.ToInt32(form.WindowState, CultureInfo.InvariantCulture);
            int currLeft, currTop, currWidth, currHeight;
            if (form.WindowState == FormWindowState.Normal) {
                currLeft = form.Bounds.Left;
                currTop = form.Bounds.Top;
                currWidth = form.Bounds.Width;
                currHeight = form.Bounds.Height;
            } else {
                currLeft = form.RestoreBounds.Left;
                currTop = form.RestoreBounds.Top;
                currWidth = form.RestoreBounds.Width;
                currHeight = form.RestoreBounds.Height;
            }

            var newLeft = Read(baseValueName + ".Left", currLeft);
            var newTop = Read(baseValueName + ".Top", currTop);
            var newWidth = Read(baseValueName + ".Width", currWidth);
            var newHeight = Read(baseValueName + ".Height", currHeight);
            var newWindowState = Read(baseValueName + ".WindowState", currWindowState);

            if ((form.FormBorderStyle == FormBorderStyle.Fixed3D) || (form.FormBorderStyle == FormBorderStyle.FixedDialog) || (form.FormBorderStyle == FormBorderStyle.FixedSingle) || (form.FormBorderStyle == FormBorderStyle.FixedToolWindow)) {
                newWidth = currWidth;
                newHeight = currHeight;
            }

            var screen = Screen.FromRectangle(new Rectangle(newLeft, newTop, newWidth, newHeight));

            switch (form.StartPosition) {

                case FormStartPosition.CenterParent: {
                        if (form.Parent != null) {
                            newLeft = form.Parent.Left + (form.Parent.Width - newWidth) / 2;
                            newTop = form.Parent.Top + (form.Parent.Height - newHeight) / 2;
                        } else if (form.Owner != null) {
                            newLeft = form.Owner.Left + (form.Owner.Width - newWidth) / 2;
                            newTop = form.Owner.Top + (form.Owner.Height - newHeight) / 2;
                        } else {
                            newLeft = screen.WorkingArea.Left + (screen.WorkingArea.Width - newWidth) / 2;
                            newTop = screen.WorkingArea.Top + (screen.WorkingArea.Height - newHeight) / 2;
                        }
                    }
                    break;

                case FormStartPosition.CenterScreen: {
                        newLeft = screen.WorkingArea.Left + (screen.WorkingArea.Width - newWidth) / 2;
                        newTop = screen.WorkingArea.Top + (screen.WorkingArea.Height - newHeight) / 2;
                    }
                    break;

            }

            if (newWidth <= 0) { newWidth = currWidth; }
            if (newHeight <= 0) { newHeight = currHeight; }
            if (newWidth > screen.WorkingArea.Width) { newWidth = screen.WorkingArea.Width; }
            if (newHeight > screen.WorkingArea.Height) { newHeight = screen.WorkingArea.Height; }
            if (newLeft + newWidth > screen.WorkingArea.Right) { newLeft = screen.WorkingArea.Left + (screen.WorkingArea.Width - newWidth); }
            if (newTop + newHeight > screen.WorkingArea.Bottom) { newTop = screen.WorkingArea.Top + (screen.WorkingArea.Height - newHeight); }
            if (newLeft < screen.WorkingArea.Left) { newLeft = screen.WorkingArea.Left; }
            if (newTop < screen.WorkingArea.Top) { newTop = screen.WorkingArea.Top; }

            form.Location = new Point(newLeft, newTop);
            form.Size = new Size(newWidth, newHeight);

            if (newWindowState == Convert.ToInt32(FormWindowState.Maximized, CultureInfo.InvariantCulture)) {
                form.WindowState = FormWindowState.Maximized;
            } //no need for any code - it is already either in normal state or minimized (will be restored to normal).
        }

        #endregion

        #region Load Save - PropertyGrid

        /// <summary>
        /// Loads previous PropertyGrid state (LabelWidth, PropertySort).
        /// </summary>
        /// <param name="control">PropertyGrid.</param>
        /// <exception cref="ArgumentNullException">Control is null.</exception>
        /// <exception cref="NotSupportedException">This control's parents cannot be resolved using Name property.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "PropertyGrid is passed because of reflection upon its member.")]
        public static void Load(PropertyGrid control) {
            if (control == null) { throw new ArgumentNullException("control", "Control is null."); }

            var baseValueName = Helper.GetControlPath(control);

            try {
                control.PropertySort = (PropertySort)(Read(baseValueName + ".PropertySort", Convert.ToInt32(control.PropertySort, CultureInfo.InvariantCulture)));
            } catch (InvalidEnumArgumentException) { }

            var fieldGridView = control.GetType().GetField("gridView", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
            var gridViewObject = fieldGridView.GetValue(control);
            if (gridViewObject != null) {
                var currentlabelWidth = 0;
                var propertyInternalLabelWidth = gridViewObject.GetType().GetProperty("InternalLabelWidth", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
                if (propertyInternalLabelWidth != null) {
                    var val = propertyInternalLabelWidth.GetValue(gridViewObject, null);
                    if (val is int) {
                        currentlabelWidth = (int)val;
                    }
                }

                var labelWidth = Read(baseValueName + ".LabelWidth", currentlabelWidth);
                if ((labelWidth > 0) && (labelWidth < control.Width)) {
                    var methodMoveSplitterToFlags = BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance;
                    var methodMoveSplitterTo = gridViewObject.GetType().GetMethod("MoveSplitterTo", methodMoveSplitterToFlags);
                    if (methodMoveSplitterTo != null) {
                        methodMoveSplitterTo.Invoke(gridViewObject, methodMoveSplitterToFlags, null, new object[] { labelWidth }, CultureInfo.CurrentCulture);
                    }
                }
            }
        }

        /// <summary>
        /// Saves PropertyGrid state (LabelWidth).
        /// </summary>
        /// <param name="control">PropertyGrid.</param>
        /// <exception cref="ArgumentNullException">Control is null.</exception>
        /// <exception cref="NotSupportedException">This control's parents cannot be resolved using Name property.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "PropertyGrid is passed because of reflection upon its member.")]
        public static void Save(PropertyGrid control) {
            if (control == null) { throw new ArgumentNullException("control", "Control is null."); }

            var baseValueName = Helper.GetControlPath(control);

            Write(baseValueName + ".PropertySort", Convert.ToInt32(control.PropertySort, CultureInfo.InvariantCulture));

            var fieldGridView = control.GetType().GetField("gridView", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
            var gridViewObject = fieldGridView.GetValue(control);
            if (gridViewObject != null) {
                var propertyInternalLabelWidth = gridViewObject.GetType().GetProperty("InternalLabelWidth", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
                if (propertyInternalLabelWidth != null) {
                    var val = propertyInternalLabelWidth.GetValue(gridViewObject, null);
                    if (val is int) {
                        Write(baseValueName + ".LabelWidth", (int)val);
                    }
                }
            }
        }

        #endregion

        #region Load Save - ListView

        /// <summary>
        /// Loads previous ListView state (Column header width).
        /// </summary>
        /// <param name="control">ListView.</param>
        /// <exception cref="ArgumentNullException">Control is null.</exception>
        public static void Load(ListView control) {
            if (control == null) { throw new ArgumentNullException("control", "Control is null."); }

            var baseValueName = Helper.GetControlPath(control);

            for (var i = 0; i < control.Columns.Count; i++) {
                var width = Read(baseValueName + ".ColumnHeaderWidth[" + i.ToString(CultureInfo.InvariantCulture) + "]", control.Columns[i].Width);
                if (width > control.ClientRectangle.Width) { width = control.ClientRectangle.Width; }
                control.Columns[i].Width = width;
            }
        }

        /// <summary>
        /// Saves ListView state (Column header width).
        /// </summary>
        /// <param name="control">ListView.</param>
        /// <exception cref="ArgumentNullException">Control is null.</exception>
        public static void Save(ListView control) {
            if (control == null) { throw new ArgumentNullException("control", "Control is null."); }

            var baseValueName = Helper.GetControlPath(control);

            for (var i = 0; i < control.Columns.Count; i++) {
                Write(baseValueName + ".ColumnHeaderWidth[" + i.ToString(CultureInfo.InvariantCulture) + "]", control.Columns[i].Width);
            }
        }

        #endregion

        #region Load Save - SplitContainer

        /// <summary>
        /// Loads previous SplitContainer state.
        /// </summary>
        /// <param name="control">SplitContainer.</param>
        /// <exception cref="ArgumentNullException">Control is null.</exception>
        public static void Load(SplitContainer control) {
            if (control == null) { throw new ArgumentNullException("control", "Control is null."); }

            var baseValueName = Helper.GetControlPath(control);

            try {
                control.Orientation = (Orientation)(Read(baseValueName + ".Orientation", Convert.ToInt32(control.Orientation, CultureInfo.InvariantCulture)));
            } catch (InvalidEnumArgumentException) { }
            try {
                var distance = Read(baseValueName + ".SplitterDistance", control.SplitterDistance);
                try {
                    control.SplitterDistance = distance;
                } catch (ArgumentOutOfRangeException) { }
            } catch (InvalidEnumArgumentException) { }
        }

        /// <summary>
        /// Saves SplitContainer state.
        /// </summary>
        /// <param name="control">SplitContainer.</param>
        /// <exception cref="ArgumentNullException">Control is null.</exception>
        public static void Save(SplitContainer control) {
            if (control == null) { throw new ArgumentNullException("control", "Control is null."); }

            var baseValueName = Helper.GetControlPath(control);

            Write(baseValueName + ".Orientation", Convert.ToInt32(control.Orientation, CultureInfo.InvariantCulture));
            Write(baseValueName + ".SplitterDistance", control.SplitterDistance);
        }

        #endregion


        #region Store

        /// <summary>
        /// Event handler used to read state.
        /// If used, registry is not read.
        /// </summary>
        public static event EventHandler<StateReadEventArgs> ReadState;

        /// <summary>
        /// Event handler used to write state.
        /// If used, registry is not written to.
        /// </summary>
        public static event EventHandler<StateWriteEventArgs> WriteState;


        private static void Write(string valueName, int value) {
            var ev = WriteState;
            if (ev != null) {
                ev.Invoke(null, new StateWriteEventArgs(valueName, value));
            } else {
                Helper.RegistryWrite(valueName, value);
            }
        }

        private static int Read(string valueName, int defaultValue) {
            var ev = ReadState;
            if (ev != null) {
                var state = new StateReadEventArgs(valueName, defaultValue);
                ev.Invoke(null, state);
                return state.Value;
            } else {
                return Helper.RegistryRead(valueName, defaultValue);
            }
        }


        #endregion Store

        private static class Helper {

            internal static void RegistryWrite(string valueName, int value) {
                if (State.NoRegistryWrites == false) {
                    try {
                        if (State.SubkeyPath.Length == 0) { return; }
                        using (var rk = Registry.CurrentUser.CreateSubKey(State.SubkeyPath)) {
                            if (rk != null) {
                                rk.SetValue(valueName, value, RegistryValueKind.DWord);
                            }
                        }
                    } catch (IOException) { //key is deleted. 
                    } catch (UnauthorizedAccessException) { } //key is write protected. 
                }
            }

            internal static int RegistryRead(string valueName, int defaultValue) {
                try {
                    using (var rk = Registry.CurrentUser.OpenSubKey(State.SubkeyPath, false)) {
                        if (rk != null) {
                            var value = rk.GetValue(valueName, null);
                            if (value == null) { return defaultValue; }
                            var valueKind = RegistryValueKind.DWord;
                            if (!State.Helper.IsRunningOnMono) { valueKind = rk.GetValueKind(valueName); }
                            if ((value != null) && (valueKind == RegistryValueKind.DWord)) {
                                return (int)value;
                            }
                        }
                    }
                } catch (SecurityException) { }
                return defaultValue;
            }

            internal static string GetControlPath(Control control) {
                var sbPath = new StringBuilder();

                var currControl = control;
                while (true) {
                    var parentControl = currControl.Parent;

                    if (parentControl == null) {
                        if (sbPath.Length > 0) { sbPath.Insert(0, "."); }
                        sbPath.Insert(0, currControl.GetType().FullName);
                        break;
                    } else {
                        if (string.IsNullOrEmpty(currControl.Name)) {
                            throw new NotSupportedException("This control's parents cannot be resolved using Name property.");
                        } else {
                            if (sbPath.Length > 0) { sbPath.Insert(0, "."); }
                            sbPath.Insert(0, currControl.Name);
                        }
                    }

                    currControl = parentControl;
                }

                return sbPath.ToString();
            }

            private static bool IsRunningOnMono {
                get {
                    return (Type.GetType("Mono.Runtime") != null);
                }
            }

        }

    }



    /// <summary>
    /// State read event arguments.
    /// </summary>
    public class StateReadEventArgs : EventArgs {

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="defaultValue">Default property value.</param>
        public StateReadEventArgs(string name, int defaultValue) {
            if (name == null) { throw new ArgumentNullException(nameof(name), "Name cannot be null."); }
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentOutOfRangeException(nameof(name), "Name cannot be empty."); }

            this.Name = name;
            this.DefaultValue = defaultValue;
            this.Value = defaultValue;
        }


        /// <summary>
        /// Gets property name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets default  property value.
        /// </summary>
        public int DefaultValue { get; }

        /// <summary>
        /// Gets/sets property value.
        /// </summary>
        public int Value { get; set; }

    }



    /// <summary>
    /// State write event arguments.
    /// </summary>
    public class StateWriteEventArgs : EventArgs {

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        public StateWriteEventArgs(string name, int value) {
            if (name == null) { throw new ArgumentNullException(nameof(name), "Name cannot be null."); }
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentOutOfRangeException(nameof(name), "Name cannot be empty."); }

            this.Name = name;
            this.Value = value;
        }


        /// <summary>
        /// Gets property name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets property value.
        /// </summary>
        public int Value { get; }

    }

}
