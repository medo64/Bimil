//Copyright (c) 2010 Josip Medved <jmedved@jmedved.com>

//2010-08-29: Initial version.


using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Diagnostics;

namespace Medo.Diagnostics {

    /// <summary>
    /// Timer that fires upon creation of object and stops with it's disposal.
    /// </summary>
    public class LifetimeWatch : IDisposable {

        private readonly string Name;
        private readonly Stopwatch Stopwatch;

        /// <summary>
        /// Initializes new instance.
        /// </summary>
        public LifetimeWatch()
            : this(null) {
        }

        /// <summary>
        /// Initializes new instance.
        /// </summary>
        /// <param name="name">Custom name for action.</param>
        public LifetimeWatch(string name) {
            this.Stopwatch = Stopwatch.StartNew();
            this.Name = name;
        }

        /// <summary>
        /// Gets the total elapsed time measured by the current instance.
        /// </summary>
        public TimeSpan Elapsed {
            get { return this.Stopwatch.Elapsed; }
        }

        /// <summary>
        /// Gets the total elapsed time measured by the current instance, in milliseconds.
        /// </summary>
        public long ElapsedMilliseconds {
            get { return this.Stopwatch.ElapsedMilliseconds; }
        }

        /// <summary>
        /// Gets the total elapsed time measured by the current instance, in timer ticks.
        /// </summary>
        public long ElapsedTicks {
            get { return this.Stopwatch.ElapsedTicks; }
        }


        #region IDisposable Members

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                this.Stopwatch.Stop();
                if (this.Name != null) {
                    Debug.WriteLine(string.Format("{1} completed in {0} milliseconds.", this.Stopwatch.ElapsedMilliseconds, this.Name));
                } else {
                    Debug.WriteLine(string.Format("Completed in {0} milliseconds.", this.Stopwatch.ElapsedMilliseconds));
                }
            }
        }

        #endregion

    }

}
