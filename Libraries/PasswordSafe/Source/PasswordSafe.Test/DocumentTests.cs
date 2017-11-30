using System;
using System.IO;
using System.Reflection;
using Xunit;
using PwSafe = Medo.Security.Cryptography.PasswordSafe;

namespace PasswordSafe.Test {
    public class DocumentTests {

        [Fact(DisplayName = "PasswordSafe: Document: Load Empty.psafe3")]
        public void Document_Empty() {
            using (var doc = PwSafe.Document.Load(GetResourceStream("Empty.psafe3"), "123")) {
                Assert.Equal(7, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Headers[PwSafe.HeaderType.Version].Version);
                Assert.Equal(new Guid("3b872b47-dee9-4c4f-ba63-4d93a86dfa4c"), doc.Headers[PwSafe.HeaderType.Uuid].Uuid);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                Assert.Equal(new DateTime(2015, 12, 28, 5, 57, 23, DateTimeKind.Utc), doc.Headers[PwSafe.HeaderType.TimestampOfLastSave].Time);
                Assert.Equal("Josip", doc.Headers[PwSafe.HeaderType.LastSavedByUser].Text);
                Assert.Equal("GANDALF", doc.Headers[PwSafe.HeaderType.LastSavedOnHost].Text);
                Assert.Equal("Password Safe V3.37", doc.Headers[PwSafe.HeaderType.WhatPerformedLastSave].Text);

                Assert.Equal(0, doc.Entries.Count);
            }
        }

        [Fact(DisplayName = "PasswordSafe: Document: Load Empty.psafe3 (password mismatch)")]
        public void Document_Empty_PasswordMismatch() {
            Assert.Throws<FormatException>(() => {
                using (var doc = PwSafe.Document.Load(GetResourceStream("Empty.psafe3"), "XXX")) {
                }
            });
        }


        [Fact(DisplayName = "PasswordSafe: Document: Load/Save Simple.psafe3")]
        public void Document_Simple() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("Simple.psafe3"), "123")) {
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(8, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Headers[PwSafe.HeaderType.Version].Version);
                Assert.Equal(new Guid("0b073824-a406-2f4b-87b2-48656a6b5011"), doc.Headers[PwSafe.HeaderType.Uuid].Uuid);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 59, DateTimeKind.Utc), doc.Headers[PwSafe.HeaderType.TimestampOfLastSave].Time);
                Assert.Equal("Josip", doc.Headers[PwSafe.HeaderType.LastSavedByUser].Text);
                Assert.Equal("GANDALF", doc.Headers[PwSafe.HeaderType.LastSavedOnHost].Text);
                Assert.Equal("Password Safe V3.37", doc.Headers[PwSafe.HeaderType.WhatPerformedLastSave].Text);
                Assert.Equal("01a93b6ef7c5af4a5990bd5c20064cc62e", doc.Headers[PwSafe.HeaderType.RecentlyUsedEntries].Text);

                Assert.Equal(2, doc.Entries.Count);

                Assert.Equal(4, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("f76e3ba9-afc5-594a-90bd-5c20064cc62e"), doc.Entries[0].Records[PwSafe.RecordType.Uuid].Uuid);
                Assert.Equal("A", doc.Entries[0].Records[PwSafe.RecordType.Title].Text);
                Assert.Equal("A123", doc.Entries[0].Records[PwSafe.RecordType.Password].Text);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 47, DateTimeKind.Utc), doc.Entries[0].Records[PwSafe.RecordType.CreationTime].Time);

                Assert.Equal(4, doc.Entries[1].Records.Count);
                Assert.Equal(new Guid("fb40f24e-68ec-c74e-8e87-293dd274d10c"), doc.Entries[1].Records[PwSafe.RecordType.Uuid].Uuid);
                Assert.Equal("B", doc.Entries[1].Records[PwSafe.RecordType.Title].Text);
                Assert.Equal("B123", doc.Entries[1].Records[PwSafe.RecordType.Password].Text);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 59, DateTimeKind.Utc), doc.Entries[1].Records[PwSafe.RecordType.CreationTime].Time);

                Assert.False(doc.HasChanged);
                doc.Save(msSave);
                Assert.False(doc.HasChanged);
            }

            msSave.Position = 0;

            using (var doc = PwSafe.Document.Load(msSave, "123")) { //reload to verify
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(8, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Headers[PwSafe.HeaderType.Version].Version);
                Assert.Equal(new Guid("0b073824-a406-2f4b-87b2-48656a6b5011"), doc.Headers[PwSafe.HeaderType.Uuid].Uuid);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 59, DateTimeKind.Utc), doc.Headers[PwSafe.HeaderType.TimestampOfLastSave].Time);
                Assert.Equal("Josip", doc.Headers[PwSafe.HeaderType.LastSavedByUser].Text);
                Assert.Equal("GANDALF", doc.Headers[PwSafe.HeaderType.LastSavedOnHost].Text);
                Assert.Equal("Password Safe V3.37", doc.Headers[PwSafe.HeaderType.WhatPerformedLastSave].Text);
                Assert.Equal("01a93b6ef7c5af4a5990bd5c20064cc62e", doc.Headers[PwSafe.HeaderType.RecentlyUsedEntries].Text);

                Assert.Equal(2, doc.Entries.Count);

                Assert.Equal(4, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("f76e3ba9-afc5-594a-90bd-5c20064cc62e"), doc.Entries[0].Records[PwSafe.RecordType.Uuid].Uuid);
                Assert.Equal("A", doc.Entries[0].Records[PwSafe.RecordType.Title].Text);
                Assert.Equal("A123", doc.Entries[0].Records[PwSafe.RecordType.Password].Text);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 47, DateTimeKind.Utc), doc.Entries[0].Records[PwSafe.RecordType.CreationTime].Time);

                Assert.Equal(4, doc.Entries[1].Records.Count);
                Assert.Equal(new Guid("fb40f24e-68ec-c74e-8e87-293dd274d10c"), doc.Entries[1].Records[PwSafe.RecordType.Uuid].Uuid);
                Assert.Equal("B", doc.Entries[1].Records[PwSafe.RecordType.Title].Text);
                Assert.Equal("B123", doc.Entries[1].Records[PwSafe.RecordType.Password].Text);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 59, DateTimeKind.Utc), doc.Entries[1].Records[PwSafe.RecordType.CreationTime].Time);

                Assert.False(doc.HasChanged);
            }
        }

        [Fact(DisplayName = "PasswordSafe: Document: Load/Save Simple.psafe3 (track modify)")]
        public void Document_Simple_TrackModify() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("Simple.psafe3"), "123")) {
                doc.TrackAccess = false;

                doc.Headers[PwSafe.HeaderType.NonDefaultPreferences] = null;
                doc.Headers[PwSafe.HeaderType.RecentlyUsedEntries] = null;
                var x = doc.Entries[0].Password; //just access
                doc.Entries["B"].Notes = "Notes";

                Assert.True(doc.HasChanged);
                doc.Save(msSave);
                Assert.False(doc.HasChanged);
            }

            msSave.Position = 0;

            using (var doc = PwSafe.Document.Load(msSave, "123")) { //reload to verify
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(6, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Version);
                Assert.Equal(new Guid("0b073824-a406-2f4b-87b2-48656a6b5011"), doc.Uuid);
                Assert.True((DateTime.UtcNow >= doc.LastSaveTime) && (doc.LastSaveTime > DateTime.MinValue));
                Assert.True(doc.LastSaveUser.Length > 0);
                Assert.True(doc.LastSaveHost.Length > 0);
                Assert.NotEqual("Password Safe V3.37", doc.LastSaveApplication);

                Assert.Equal(2, doc.Entries.Count);

                Assert.Equal(4, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("f76e3ba9-afc5-594a-90bd-5c20064cc62e"), doc.Entries[0].Uuid);
                Assert.Equal("A", doc.Entries[0].Title);
                Assert.Equal("A123", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 47, DateTimeKind.Utc), doc.Entries[0].CreationTime);

                Assert.Equal(6, doc.Entries[1].Records.Count);
                Assert.Equal(new Guid("fb40f24e-68ec-c74e-8e87-293dd274d10c"), doc.Entries[1].Uuid);
                Assert.Equal("B", doc.Entries[1].Title);
                Assert.Equal("B123", doc.Entries[1].Password);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 59, DateTimeKind.Utc), doc.Entries[1].CreationTime);
                Assert.True((DateTime.UtcNow >= doc.Entries[1].LastModificationTime) && (doc.Entries[1].LastModificationTime > DateTime.MinValue));
                Assert.Equal("Notes", doc.Entries[1].Notes);

                Assert.False(doc.HasChanged);
            }
        }


        [Fact(DisplayName = "PasswordSafe: Document: Load/Save Simple.psafe3 (track read bytes)")]
        public void Document_Simple_TrackAccess_ReadBytes() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("Simple.psafe3"), "123")) {
                doc.TrackModify = false;

                doc.Headers[PwSafe.HeaderType.NonDefaultPreferences] = null;
                doc.Headers[PwSafe.HeaderType.RecentlyUsedEntries] = null;
                var x = doc.Entries[0].Records[PwSafe.RecordType.Password].GetBytes(); //just access

                Assert.True(doc.HasChanged);
                doc.Save(msSave);
                Assert.False(doc.HasChanged);
            }

            msSave.Position = 0;

            using (var doc = PwSafe.Document.Load(msSave, "123")) { //reload to verify
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(6, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Version);
                Assert.Equal(new Guid("0b073824-a406-2f4b-87b2-48656a6b5011"), doc.Uuid);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 59, DateTimeKind.Utc), doc.LastSaveTime);
                Assert.Equal("Josip", doc.LastSaveUser);
                Assert.Equal("GANDALF", doc.LastSaveHost);
                Assert.Equal("Password Safe V3.37", doc.LastSaveApplication);

                Assert.Equal(2, doc.Entries.Count);

                Assert.Equal(5, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("f76e3ba9-afc5-594a-90bd-5c20064cc62e"), doc.Entries[0].Uuid);
                Assert.Equal("A", doc.Entries[0].Title);
                Assert.Equal("A123", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 47, DateTimeKind.Utc), doc.Entries[0].CreationTime);
                Assert.True((DateTime.UtcNow >= doc.Entries[0].LastAccessTime) && (doc.Entries[0].Records.Contains(PwSafe.RecordType.LastAccessTime)));

                Assert.Equal(4, doc.Entries[1].Records.Count);
                Assert.Equal(new Guid("fb40f24e-68ec-c74e-8e87-293dd274d10c"), doc.Entries[1].Uuid);
                Assert.Equal("B", doc.Entries[1].Title);
                Assert.Equal("B123", doc.Entries[1].Password);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 59, DateTimeKind.Utc), doc.Entries[1].CreationTime);

                Assert.False(doc.HasChanged);
            }
        }

        [Fact(DisplayName = "PasswordSafe: Document: Load/Save Simple.psafe3 (don't track silently read bytes)")]
        public void Document_Simple_TrackAccess_ReadBytesSilently() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("Simple.psafe3"), "123")) {
                doc.TrackModify = false;

                doc.Headers[PwSafe.HeaderType.NonDefaultPreferences] = null;
                doc.Headers[PwSafe.HeaderType.RecentlyUsedEntries] = null;
                var x = doc.Entries[0].Records[PwSafe.RecordType.Password].GetBytesSilently(); //just access

                Assert.True(doc.HasChanged);
                doc.Save(msSave);
                Assert.False(doc.HasChanged);
            }

            msSave.Position = 0;

            using (var doc = PwSafe.Document.Load(msSave, "123")) { //reload to verify
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(6, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Version);
                Assert.Equal(new Guid("0b073824-a406-2f4b-87b2-48656a6b5011"), doc.Uuid);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 59, DateTimeKind.Utc), doc.LastSaveTime);
                Assert.Equal("Josip", doc.LastSaveUser);
                Assert.Equal("GANDALF", doc.LastSaveHost);
                Assert.Equal("Password Safe V3.37", doc.LastSaveApplication);

                Assert.Equal(2, doc.Entries.Count);

                Assert.Equal(4, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("f76e3ba9-afc5-594a-90bd-5c20064cc62e"), doc.Entries[0].Uuid);
                Assert.Equal("A", doc.Entries[0].Title);
                Assert.Equal("A123", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 47, DateTimeKind.Utc), doc.Entries[0].CreationTime);

                Assert.Equal(4, doc.Entries[1].Records.Count);
                Assert.Equal(new Guid("fb40f24e-68ec-c74e-8e87-293dd274d10c"), doc.Entries[1].Uuid);
                Assert.Equal("B", doc.Entries[1].Title);
                Assert.Equal("B123", doc.Entries[1].Password);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 59, DateTimeKind.Utc), doc.Entries[1].CreationTime);

                Assert.False(doc.HasChanged);
            }
        }

        [Fact(DisplayName = "PasswordSafe: Document: Load/Save Simple.psafe3 (track access)")]
        public void Document_Simple_TrackAccess() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("Simple.psafe3"), "123")) {
                doc.TrackModify = false;

                doc.Headers[PwSafe.HeaderType.NonDefaultPreferences] = null;
                doc.Headers[PwSafe.HeaderType.RecentlyUsedEntries] = null;
                var x = doc.Entries[0].Password; //just access
                doc.Entries["B"].Notes = "Notes";

                Assert.True(doc.HasChanged);
                doc.Save(msSave);
                Assert.False(doc.HasChanged);
            }

            msSave.Position = 0;

            using (var doc = PwSafe.Document.Load(msSave, "123")) { //reload to verify
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(6, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Version);
                Assert.Equal(new Guid("0b073824-a406-2f4b-87b2-48656a6b5011"), doc.Uuid);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 59, DateTimeKind.Utc), doc.LastSaveTime);
                Assert.Equal("Josip", doc.LastSaveUser);
                Assert.Equal("GANDALF", doc.LastSaveHost);
                Assert.Equal("Password Safe V3.37", doc.LastSaveApplication);

                Assert.Equal(2, doc.Entries.Count);

                Assert.Equal(5, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("f76e3ba9-afc5-594a-90bd-5c20064cc62e"), doc.Entries[0].Uuid);
                Assert.Equal("A", doc.Entries[0].Title);
                Assert.Equal("A123", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 47, DateTimeKind.Utc), doc.Entries[0].CreationTime);
                Assert.True((DateTime.UtcNow >= doc.Entries[0].LastAccessTime) && (doc.Entries[0].Records.Contains(PwSafe.RecordType.LastAccessTime)));

                Assert.Equal(5, doc.Entries[1].Records.Count);
                Assert.Equal(new Guid("fb40f24e-68ec-c74e-8e87-293dd274d10c"), doc.Entries[1].Uuid);
                Assert.Equal("B", doc.Entries[1].Title);
                Assert.Equal("B123", doc.Entries[1].Password);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 59, DateTimeKind.Utc), doc.Entries[1].CreationTime);
                Assert.Equal("Notes", doc.Entries[1].Notes);

                Assert.False(doc.HasChanged);
            }
        }

        [Fact(DisplayName = "PasswordSafe: Document: Load/Save Simple.psafe3 (track access and modify)")]
        public void Document_Simple_TrackAccessAndModify() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("Simple.psafe3"), "123")) {
                doc.Headers[PwSafe.HeaderType.NonDefaultPreferences] = null;
                doc.Headers[PwSafe.HeaderType.RecentlyUsedEntries] = null;
                var x = doc.Entries[0].Password; //just access
                doc.Entries["B"].Notes = "Notes";

                Assert.True(doc.HasChanged);
                doc.Save(msSave);
                Assert.False(doc.HasChanged);
            }

            msSave.Position = 0;

            using (var doc = PwSafe.Document.Load(msSave, "123")) { //reload to verify
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(6, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Version);
                Assert.Equal(new Guid("0b073824-a406-2f4b-87b2-48656a6b5011"), doc.Uuid);
                Assert.True(DateTime.UtcNow >= doc.LastSaveTime);
                Assert.True(doc.LastSaveUser.Length > 0);
                Assert.True(doc.LastSaveHost.Length > 0);
                Assert.NotEqual("Password Safe V3.37", doc.LastSaveApplication);

                Assert.Equal(2, doc.Entries.Count);

                Assert.Equal(5, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("f76e3ba9-afc5-594a-90bd-5c20064cc62e"), doc.Entries[0].Uuid);
                Assert.Equal("A", doc.Entries[0].Title);
                Assert.Equal("A123", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 47, DateTimeKind.Utc), doc.Entries[0].CreationTime);
                Assert.True((DateTime.UtcNow >= doc.Entries[0].LastAccessTime) && (doc.Entries[0].Records.Contains(PwSafe.RecordType.LastAccessTime)));

                Assert.Equal(6, doc.Entries[1].Records.Count);
                Assert.Equal(new Guid("fb40f24e-68ec-c74e-8e87-293dd274d10c"), doc.Entries[1].Uuid);
                Assert.Equal("B", doc.Entries[1].Title);
                Assert.Equal("B123", doc.Entries[1].Password);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 59, DateTimeKind.Utc), doc.Entries[1].CreationTime);
                Assert.True((DateTime.UtcNow >= doc.Entries[1].LastModificationTime) && (doc.Entries[1].Records.Contains(PwSafe.RecordType.LastModificationTime)));
                Assert.Equal("Notes", doc.Entries[1].Notes);

                Assert.False(doc.HasChanged);
            }
        }

        [Fact(DisplayName = "PasswordSafe: Document: Load/Save Simple.psafe3 read-only (track access and modify)")]
        public void Document_Simple_TrackAccessAndModify_Readonly() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("Simple.psafe3"), "123")) {
                doc.IsReadOnly = true;

                doc.Save(msSave);
                Assert.False(doc.HasChanged);
            }

            msSave.Position = 0;

            using (var doc = PwSafe.Document.Load(msSave, "123")) { //reload to verify
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(8, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Headers[PwSafe.HeaderType.Version].Version);
                Assert.Equal(new Guid("0b073824-a406-2f4b-87b2-48656a6b5011"), doc.Headers[PwSafe.HeaderType.Uuid].Uuid);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 59, DateTimeKind.Utc), doc.Headers[PwSafe.HeaderType.TimestampOfLastSave].Time);
                Assert.Equal("Josip", doc.Headers[PwSafe.HeaderType.LastSavedByUser].Text);
                Assert.Equal("GANDALF", doc.Headers[PwSafe.HeaderType.LastSavedOnHost].Text);
                Assert.Equal("Password Safe V3.37", doc.Headers[PwSafe.HeaderType.WhatPerformedLastSave].Text);
                Assert.Equal("01a93b6ef7c5af4a5990bd5c20064cc62e", doc.Headers[PwSafe.HeaderType.RecentlyUsedEntries].Text);

                Assert.Equal(2, doc.Entries.Count);

                Assert.Equal(4, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("f76e3ba9-afc5-594a-90bd-5c20064cc62e"), doc.Entries[0].Records[PwSafe.RecordType.Uuid].Uuid);
                Assert.Equal("A", doc.Entries[0].Records[PwSafe.RecordType.Title].Text);
                Assert.Equal("A123", doc.Entries[0].Records[PwSafe.RecordType.Password].Text);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 47, DateTimeKind.Utc), doc.Entries[0].Records[PwSafe.RecordType.CreationTime].Time);

                Assert.Equal(4, doc.Entries[1].Records.Count);
                Assert.Equal(new Guid("fb40f24e-68ec-c74e-8e87-293dd274d10c"), doc.Entries[1].Records[PwSafe.RecordType.Uuid].Uuid);
                Assert.Equal("B", doc.Entries[1].Records[PwSafe.RecordType.Title].Text);
                Assert.Equal("B123", doc.Entries[1].Records[PwSafe.RecordType.Password].Text);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 59, DateTimeKind.Utc), doc.Entries[1].Records[PwSafe.RecordType.CreationTime].Time);

                Assert.False(doc.HasChanged);
            }
        }

        [Fact(DisplayName = "PasswordSafe: Document: Load/Save Simple.psafe3 read-only (try modify)")]
        public void Document_Simple_Readonly_TryModify() {
            Assert.Throws<NotSupportedException>(() => {
                var msSave = new MemoryStream();
                using (var doc = PwSafe.Document.Load(GetResourceStream("Simple.psafe3"), "123")) {
                    doc.IsReadOnly = true;
                    doc.Uuid = Guid.NewGuid();
                }
            });
        }


        [Fact(DisplayName = "PasswordSafe: Document: Load/Save SimpleTree.psafe3")]
        public void Document_SimpleTree() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("SimpleTree.psafe3"), "123")) {
                doc.TrackAccess = false;
                doc.TrackModify = false;
                doc.IsReadOnly = true;

                Assert.Equal(7, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Version);
                Assert.Equal(new Guid("5f46a9f5-1b9e-f743-8d58-228d8c99b87f"), doc.Uuid);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                Assert.Equal(new DateTime(2016, 01, 02, 07, 41, 25, DateTimeKind.Utc), doc.LastSaveTime);
                Assert.Equal("Josip", doc.LastSaveUser);
                Assert.Equal("GANDALF", doc.LastSaveHost);
                Assert.Equal("Password Safe V3.37", doc.LastSaveApplication);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.RecentlyUsedEntries].Text);

                Assert.Equal(2, doc.Entries.Count);

                Assert.Equal(6, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("f76e3ba9-afc5-594a-90bd-5c20064cc62e"), doc.Entries[0].Uuid);
                Assert.True(string.Equals("X.Y", doc.Entries[0].Group));
                Assert.Equal("A", doc.Entries[0].Title);
                Assert.Equal("A123", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2015, 12, 28, 08, 36, 47, DateTimeKind.Utc), doc.Entries[0].CreationTime);
                Assert.Equal(new DateTime(2016, 01, 02, 07, 41, 25, DateTimeKind.Utc), doc.Entries[0].LastModificationTime);

                Assert.Equal(6, doc.Entries[1].Records.Count);
                Assert.Equal(new Guid("fb40f24e-68ec-c74e-8e87-293dd274d10c"), doc.Entries[1].Uuid);
                Assert.True(string.Equals("Z", doc.Entries[1].Group));
                Assert.Equal("B", doc.Entries[1].Title);
                Assert.Equal("B123", doc.Entries[1].Password);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 59, DateTimeKind.Utc), doc.Entries[1].CreationTime);
                Assert.Equal(new DateTime(2016, 01, 02, 07, 40, 06, DateTimeKind.Utc), doc.Entries[1].LastModificationTime);

                Assert.False(doc.HasChanged);
                doc.Save(msSave);
                Assert.False(doc.HasChanged);
            }

            msSave.Position = 0;

            using (var doc = PwSafe.Document.Load(msSave, "123")) { //reload to verify
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(7, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Version);
                Assert.Equal(new Guid("5f46a9f5-1b9e-f743-8d58-228d8c99b87f"), doc.Uuid);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                Assert.Equal(new DateTime(2016, 01, 02, 07, 41, 25, DateTimeKind.Utc), doc.LastSaveTime);
                Assert.Equal("Josip", doc.LastSaveUser);
                Assert.Equal("GANDALF", doc.LastSaveHost);
                Assert.Equal("Password Safe V3.37", doc.LastSaveApplication);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.RecentlyUsedEntries].Text);

                Assert.Equal(2, doc.Entries.Count);

                Assert.Equal(6, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("f76e3ba9-afc5-594a-90bd-5c20064cc62e"), doc.Entries[0].Uuid);
                Assert.True(string.Equals("X.Y", doc.Entries[0].Group));
                Assert.Equal("A", doc.Entries[0].Title);
                Assert.Equal("A123", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2015, 12, 28, 08, 36, 47, DateTimeKind.Utc), doc.Entries[0].CreationTime);
                Assert.Equal(new DateTime(2016, 01, 02, 07, 41, 25, DateTimeKind.Utc), doc.Entries[0].LastModificationTime);

                Assert.Equal(6, doc.Entries[1].Records.Count);
                Assert.Equal(new Guid("fb40f24e-68ec-c74e-8e87-293dd274d10c"), doc.Entries[1].Uuid);
                Assert.True(string.Equals("Z", doc.Entries[1].Group));
                Assert.Equal("B", doc.Entries[1].Title);
                Assert.Equal("B123", doc.Entries[1].Password);
                Assert.Equal(new DateTime(2015, 12, 28, 8, 36, 59, DateTimeKind.Utc), doc.Entries[1].CreationTime);
                Assert.Equal(new DateTime(2016, 01, 02, 07, 40, 06, DateTimeKind.Utc), doc.Entries[1].LastModificationTime);

                Assert.False(doc.HasChanged);
            }
        }


        [Fact(DisplayName = "PasswordSafe: Document: Load/Save Test10.psafe3")]
        public void Document_Test10() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("Test10.psafe3"), "Test")) {
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(8, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Headers[PwSafe.HeaderType.Version].Version);
                Assert.Equal(new Guid("f80256c7-3aef-7447-8d2c-65c54981c2ff"), doc.Uuid);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                Assert.Equal(new DateTime(2016, 01, 11, 07, 39, 31, DateTimeKind.Utc), doc.LastSaveTime);
                Assert.Equal("Josip", doc.LastSaveUser);
                Assert.Equal("GANDALF", doc.LastSaveHost);
                Assert.Equal("Password Safe V3.37", doc.LastSaveApplication);
                Assert.Equal("1", doc.Headers[PwSafe.HeaderType.TreeDisplayStatus].Text);

                Assert.Equal(1, doc.Entries.Count);

                Assert.Equal(6, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("74c96b2d-950a-4643-b202-b7967947f781"), doc.Entries[0].Uuid);
                Assert.Equal("1234567890", (string)doc.Entries[0].Group);
                Assert.Equal("1234567890", doc.Entries[0].Title);
                Assert.Equal("1234567890", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2016, 01, 11, 07, 35, 01, DateTimeKind.Utc), doc.Entries[0].CreationTime);
                Assert.Equal(new DateTime(2016, 01, 11, 07, 39, 10, DateTimeKind.Utc), doc.Entries[0].PasswordModificationTime);

                Assert.False(doc.HasChanged);
                doc.Save(msSave);
                Assert.False(doc.HasChanged);
            }

            msSave.Position = 0;

            using (var doc = PwSafe.Document.Load(msSave, "Test")) { //reload to verify
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(8, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Headers[PwSafe.HeaderType.Version].Version);
                Assert.Equal(new Guid("f80256c7-3aef-7447-8d2c-65c54981c2ff"), doc.Uuid);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                Assert.Equal(new DateTime(2016, 01, 11, 07, 39, 31, DateTimeKind.Utc), doc.LastSaveTime);
                Assert.Equal("Josip", doc.LastSaveUser);
                Assert.Equal("GANDALF", doc.LastSaveHost);
                Assert.Equal("Password Safe V3.37", doc.LastSaveApplication);
                Assert.Equal("1", doc.Headers[PwSafe.HeaderType.TreeDisplayStatus].Text);

                Assert.Equal(1, doc.Entries.Count);

                Assert.Equal(6, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("74c96b2d-950a-4643-b202-b7967947f781"), doc.Entries[0].Uuid);
                Assert.Equal("1234567890", (string)doc.Entries[0].Group);
                Assert.Equal("1234567890", doc.Entries[0].Title);
                Assert.Equal("1234567890", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2016, 01, 11, 07, 35, 01, DateTimeKind.Utc), doc.Entries[0].CreationTime);
                Assert.Equal(new DateTime(2016, 01, 11, 07, 39, 10, DateTimeKind.Utc), doc.Entries[0].PasswordModificationTime);

                Assert.False(doc.HasChanged);
            }
        }

        [Fact(DisplayName = "PasswordSafe: Document: Load/Save Test11.psafe3")]
        public void Document_Test11() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("Test11.psafe3"), "Test")) {
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(7, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Headers[PwSafe.HeaderType.Version].Version);
                Assert.Equal(new Guid("f80256c7-3aef-7447-8d2c-65c54981c2ff"), doc.Uuid);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                Assert.Equal(new DateTime(2016, 01, 11, 07, 35, 01, DateTimeKind.Utc), doc.LastSaveTime);
                Assert.Equal("Josip", doc.LastSaveUser);
                Assert.Equal("GANDALF", doc.LastSaveHost);
                Assert.Equal("Password Safe V3.37", doc.LastSaveApplication);

                Assert.Equal(1, doc.Entries.Count);

                Assert.Equal(5, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("74c96b2d-950a-4643-b202-b7967947f781"), doc.Entries[0].Uuid);
                Assert.Equal("12345678901", (string)doc.Entries[0].Group);
                Assert.Equal("12345678901", doc.Entries[0].Title);
                Assert.Equal("12345678901", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2016, 01, 11, 07, 35, 01, DateTimeKind.Utc), doc.Entries[0].CreationTime);

                Assert.False(doc.HasChanged);
                doc.Save(msSave);
                Assert.False(doc.HasChanged);
            }

            msSave.Position = 0;

            using (var doc = PwSafe.Document.Load(msSave, "Test")) { //reload to verify
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(7, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Headers[PwSafe.HeaderType.Version].Version);
                Assert.Equal(new Guid("f80256c7-3aef-7447-8d2c-65c54981c2ff"), doc.Uuid);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                Assert.Equal(new DateTime(2016, 01, 11, 07, 35, 01, DateTimeKind.Utc), doc.LastSaveTime);
                Assert.Equal("Josip", doc.LastSaveUser);
                Assert.Equal("GANDALF", doc.LastSaveHost);
                Assert.Equal("Password Safe V3.37", doc.LastSaveApplication);

                Assert.Equal(1, doc.Entries.Count);

                Assert.Equal(5, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("74c96b2d-950a-4643-b202-b7967947f781"), doc.Entries[0].Uuid);
                Assert.Equal("12345678901", (string)doc.Entries[0].Group);
                Assert.Equal("12345678901", doc.Entries[0].Title);
                Assert.Equal("12345678901", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2016, 01, 11, 07, 35, 01, DateTimeKind.Utc), doc.Entries[0].CreationTime);

                Assert.False(doc.HasChanged);
            }
        }

        [Fact(DisplayName = "PasswordSafe: Document: Load/Save PasswordHistory.psafe3 (history enabled)")]
        public void Document_TestPasswordHistoryEnabled() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("PasswordHistory.psafe3"), "123")) {
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(8, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Headers[PwSafe.HeaderType.Version].Version);
                Assert.Equal(new Guid("257a6fba-9816-2a43-9aa7-9bbea870e713"), doc.Uuid);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 47, 40, DateTimeKind.Utc), doc.LastSaveTime);
                Assert.Equal("Josip", doc.LastSaveUser);
                Assert.Equal("GANDALF", doc.LastSaveHost);
                Assert.Equal("Password Safe V3.38", doc.LastSaveApplication);

                Assert.Equal(1, doc.Entries.Count);

                Assert.Equal(7, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("e857fe9c-091e-b44c-8574-e435549e1cc7"), doc.Entries[0].Uuid);
                Assert.Equal("", (string)doc.Entries[0].Group);
                Assert.Equal("Test", doc.Entries[0].Title);
                Assert.Equal("3", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].CreationTime);

                Assert.Equal(true, doc.Entries[0].PasswordHistory.Enabled);
                Assert.Equal(2, doc.Entries[0].PasswordHistory.MaximumCount);
                Assert.Equal(2, doc.Entries[0].PasswordHistory.Count);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[0].TimeFirstUsed);
                Assert.Equal("1", doc.Entries[0].PasswordHistory[0].HistoricalPassword);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 27, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[1].TimeFirstUsed);
                Assert.Equal("2", doc.Entries[0].PasswordHistory[1].HistoricalPassword);

                Assert.False(doc.HasChanged);

                doc.Entries[0].Password = "4";
                Assert.True(doc.HasChanged);

                Assert.Equal(7, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("e857fe9c-091e-b44c-8574-e435549e1cc7"), doc.Entries[0].Uuid);
                Assert.Equal("", (string)doc.Entries[0].Group);
                Assert.Equal("Test", doc.Entries[0].Title);
                Assert.Equal("4", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].CreationTime);

                Assert.Equal(true, doc.Entries[0].PasswordHistory.Enabled);
                Assert.Equal(2, doc.Entries[0].PasswordHistory.MaximumCount);
                Assert.Equal(2, doc.Entries[0].PasswordHistory.Count);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 27, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[0].TimeFirstUsed);
                Assert.Equal("2", doc.Entries[0].PasswordHistory[0].HistoricalPassword);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 44, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[1].TimeFirstUsed);
                Assert.Equal("3", doc.Entries[0].PasswordHistory[1].HistoricalPassword);

                doc.Save(msSave);
                Assert.False(doc.HasChanged);
            }

            msSave.Position = 0;

            using (var doc = PwSafe.Document.Load(msSave, "123")) { //reload to verify
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(1, doc.Entries.Count);

                Assert.Equal(7, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("e857fe9c-091e-b44c-8574-e435549e1cc7"), doc.Entries[0].Uuid);
                Assert.Equal("", (string)doc.Entries[0].Group);
                Assert.Equal("Test", doc.Entries[0].Title);
                Assert.Equal("4", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].CreationTime);

                Assert.Equal(true, doc.Entries[0].PasswordHistory.Enabled);
                Assert.Equal(2, doc.Entries[0].PasswordHistory.MaximumCount);
                Assert.Equal(2, doc.Entries[0].PasswordHistory.Count);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 27, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[0].TimeFirstUsed);
                Assert.Equal("2", doc.Entries[0].PasswordHistory[0].HistoricalPassword);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 44, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[1].TimeFirstUsed);
                Assert.Equal("3", doc.Entries[0].PasswordHistory[1].HistoricalPassword);

                Assert.False(doc.HasChanged);
            }
        }

        [Fact(DisplayName = "PasswordSafe: Document: Load/Save PasswordHistory.psafe3 (history enabled, more entries)")]
        public void Document_TestPasswordHistoryEnabledMore() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("PasswordHistory.psafe3"), "123")) {
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(8, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Headers[PwSafe.HeaderType.Version].Version);
                Assert.Equal(new Guid("257a6fba-9816-2a43-9aa7-9bbea870e713"), doc.Uuid);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 47, 40, DateTimeKind.Utc), doc.LastSaveTime);
                Assert.Equal("Josip", doc.LastSaveUser);
                Assert.Equal("GANDALF", doc.LastSaveHost);
                Assert.Equal("Password Safe V3.38", doc.LastSaveApplication);

                Assert.Equal(1, doc.Entries.Count);

                Assert.Equal(7, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("e857fe9c-091e-b44c-8574-e435549e1cc7"), doc.Entries[0].Uuid);
                Assert.Equal("", (string)doc.Entries[0].Group);
                Assert.Equal("Test", doc.Entries[0].Title);
                Assert.Equal("3", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].CreationTime);

                Assert.Equal(true, doc.Entries[0].PasswordHistory.Enabled);
                Assert.Equal(2, doc.Entries[0].PasswordHistory.MaximumCount);
                Assert.Equal(2, doc.Entries[0].PasswordHistory.Count);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[0].TimeFirstUsed);
                Assert.Equal("1", doc.Entries[0].PasswordHistory[0].HistoricalPassword);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 27, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[1].TimeFirstUsed);
                Assert.Equal("2", doc.Entries[0].PasswordHistory[1].HistoricalPassword);

                Assert.False(doc.HasChanged);

                doc.Entries[0].PasswordHistory.MaximumCount = 3;
                doc.Entries[0].Password = "4";
                Assert.True(doc.HasChanged);

                Assert.Equal(7, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("e857fe9c-091e-b44c-8574-e435549e1cc7"), doc.Entries[0].Uuid);
                Assert.Equal("", (string)doc.Entries[0].Group);
                Assert.Equal("Test", doc.Entries[0].Title);
                Assert.Equal("4", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].CreationTime);

                Assert.Equal(true, doc.Entries[0].PasswordHistory.Enabled);
                Assert.Equal(3, doc.Entries[0].PasswordHistory.MaximumCount);
                Assert.Equal(3, doc.Entries[0].PasswordHistory.Count);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[0].TimeFirstUsed);
                Assert.Equal("1", doc.Entries[0].PasswordHistory[0].HistoricalPassword);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 27, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[1].TimeFirstUsed);
                Assert.Equal("2", doc.Entries[0].PasswordHistory[1].HistoricalPassword);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 44, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[2].TimeFirstUsed);
                Assert.Equal("3", doc.Entries[0].PasswordHistory[2].HistoricalPassword);

                doc.Save(msSave);
                Assert.False(doc.HasChanged);
            }

            msSave.Position = 0;

            using (var doc = PwSafe.Document.Load(msSave, "123")) { //reload to verify
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(1, doc.Entries.Count);

                Assert.Equal(7, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("e857fe9c-091e-b44c-8574-e435549e1cc7"), doc.Entries[0].Uuid);
                Assert.Equal("", (string)doc.Entries[0].Group);
                Assert.Equal("Test", doc.Entries[0].Title);
                Assert.Equal("4", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].CreationTime);

                Assert.Equal(true, doc.Entries[0].PasswordHistory.Enabled);
                Assert.Equal(3, doc.Entries[0].PasswordHistory.MaximumCount);
                Assert.Equal(3, doc.Entries[0].PasswordHistory.Count);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[0].TimeFirstUsed);
                Assert.Equal("1", doc.Entries[0].PasswordHistory[0].HistoricalPassword);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 27, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[1].TimeFirstUsed);
                Assert.Equal("2", doc.Entries[0].PasswordHistory[1].HistoricalPassword);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 44, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[2].TimeFirstUsed);
                Assert.Equal("3", doc.Entries[0].PasswordHistory[2].HistoricalPassword);

                Assert.False(doc.HasChanged);
            }
        }

        [Fact(DisplayName = "PasswordSafe: Document: Load/Save PasswordHistory.psafe3 (history enabled, indirect change)")]
        public void Document_TestPasswordHistoryIndirectChange() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("PasswordHistory.psafe3"), "123")) {
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(8, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Headers[PwSafe.HeaderType.Version].Version);
                Assert.Equal(new Guid("257a6fba-9816-2a43-9aa7-9bbea870e713"), doc.Uuid);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 47, 40, DateTimeKind.Utc), doc.LastSaveTime);
                Assert.Equal("Josip", doc.LastSaveUser);
                Assert.Equal("GANDALF", doc.LastSaveHost);
                Assert.Equal("Password Safe V3.38", doc.LastSaveApplication);

                Assert.Equal(1, doc.Entries.Count);

                Assert.Equal(7, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("e857fe9c-091e-b44c-8574-e435549e1cc7"), doc.Entries[0].Uuid);
                Assert.Equal("", (string)doc.Entries[0].Group);
                Assert.Equal("Test", doc.Entries[0].Title);
                Assert.Equal("3", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].CreationTime);

                Assert.Equal(true, doc.Entries[0].PasswordHistory.Enabled);
                Assert.Equal(2, doc.Entries[0].PasswordHistory.MaximumCount);
                Assert.Equal(2, doc.Entries[0].PasswordHistory.Count);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[0].TimeFirstUsed);
                Assert.Equal("1", doc.Entries[0].PasswordHistory[0].HistoricalPassword);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 27, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[1].TimeFirstUsed);
                Assert.Equal("2", doc.Entries[0].PasswordHistory[1].HistoricalPassword);

                Assert.False(doc.HasChanged);

                doc.Entries[0].PasswordHistory.MaximumCount = 3;
                doc.Entries[0][PwSafe.RecordType.Password].Text = "4";
                Assert.True(doc.HasChanged);

                Assert.Equal(7, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("e857fe9c-091e-b44c-8574-e435549e1cc7"), doc.Entries[0].Uuid);
                Assert.Equal("", (string)doc.Entries[0].Group);
                Assert.Equal("Test", doc.Entries[0].Title);
                Assert.Equal("4", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].CreationTime);

                Assert.Equal(true, doc.Entries[0].PasswordHistory.Enabled);
                Assert.Equal(3, doc.Entries[0].PasswordHistory.MaximumCount);
                Assert.Equal(3, doc.Entries[0].PasswordHistory.Count);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[0].TimeFirstUsed);
                Assert.Equal("1", doc.Entries[0].PasswordHistory[0].HistoricalPassword);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 27, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[1].TimeFirstUsed);
                Assert.Equal("2", doc.Entries[0].PasswordHistory[1].HistoricalPassword);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 44, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[2].TimeFirstUsed);
                Assert.Equal("3", doc.Entries[0].PasswordHistory[2].HistoricalPassword);

                doc.Save(msSave);
                Assert.False(doc.HasChanged);
            }

            msSave.Position = 0;

            using (var doc = PwSafe.Document.Load(msSave, "123")) { //reload to verify
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(1, doc.Entries.Count);

                Assert.Equal(7, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("e857fe9c-091e-b44c-8574-e435549e1cc7"), doc.Entries[0].Uuid);
                Assert.Equal("", (string)doc.Entries[0].Group);
                Assert.Equal("Test", doc.Entries[0].Title);
                Assert.Equal("4", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].CreationTime);

                Assert.Equal(true, doc.Entries[0].PasswordHistory.Enabled);
                Assert.Equal(3, doc.Entries[0].PasswordHistory.MaximumCount);
                Assert.Equal(3, doc.Entries[0].PasswordHistory.Count);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[0].TimeFirstUsed);
                Assert.Equal("1", doc.Entries[0].PasswordHistory[0].HistoricalPassword);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 27, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[1].TimeFirstUsed);
                Assert.Equal("2", doc.Entries[0].PasswordHistory[1].HistoricalPassword);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 44, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[2].TimeFirstUsed);
                Assert.Equal("3", doc.Entries[0].PasswordHistory[2].HistoricalPassword);

                Assert.False(doc.HasChanged);
            }
        }

        [Fact(DisplayName = "PasswordSafe: Document: Load/Save PasswordHistory.psafe3 (history disabled)")]
        public void Document_TestPasswordHistoryDisabled() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("PasswordHistory.psafe3"), "123")) {
                doc.TrackAccess = false;
                doc.TrackModify = false;
                doc.Entries[0].PasswordHistory.Enabled = false;

                doc.Entries[0].Password = "4";
                Assert.True(doc.HasChanged);

                doc.Save(msSave);
                Assert.False(doc.HasChanged);
            }

            msSave.Position = 0;

            using (var doc = PwSafe.Document.Load(msSave, "123")) { //reload to verify
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(1, doc.Entries.Count);

                Assert.Equal(7, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("e857fe9c-091e-b44c-8574-e435549e1cc7"), doc.Entries[0].Uuid);
                Assert.Equal("", (string)doc.Entries[0].Group);
                Assert.Equal("Test", doc.Entries[0].Title);
                Assert.Equal("4", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].CreationTime);

                Assert.Equal(false, doc.Entries[0].PasswordHistory.Enabled);
                Assert.Equal(2, doc.Entries[0].PasswordHistory.MaximumCount);
                Assert.Equal(0, doc.Entries[0].PasswordHistory.Count);

                Assert.False(doc.HasChanged);
            }
        }

        [Fact(DisplayName = "PasswordSafe: Document: Load/Save PasswordHistory.psafe3 (history enabled, clear)")]
        public void Document_TestPasswordHistoryClear() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("PasswordHistory.psafe3"), "123")) {
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(8, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Headers[PwSafe.HeaderType.Version].Version);
                Assert.Equal(new Guid("257a6fba-9816-2a43-9aa7-9bbea870e713"), doc.Uuid);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 47, 40, DateTimeKind.Utc), doc.LastSaveTime);
                Assert.Equal("Josip", doc.LastSaveUser);
                Assert.Equal("GANDALF", doc.LastSaveHost);
                Assert.Equal("Password Safe V3.38", doc.LastSaveApplication);

                Assert.Equal(1, doc.Entries.Count);

                Assert.Equal(7, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("e857fe9c-091e-b44c-8574-e435549e1cc7"), doc.Entries[0].Uuid);
                Assert.Equal("", (string)doc.Entries[0].Group);
                Assert.Equal("Test", doc.Entries[0].Title);
                Assert.Equal("3", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].CreationTime);

                Assert.Equal(true, doc.Entries[0].PasswordHistory.Enabled);
                Assert.Equal(2, doc.Entries[0].PasswordHistory.MaximumCount);
                Assert.Equal(2, doc.Entries[0].PasswordHistory.Count);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[0].TimeFirstUsed);
                Assert.Equal("1", doc.Entries[0].PasswordHistory[0].HistoricalPassword);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 27, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[1].TimeFirstUsed);
                Assert.Equal("2", doc.Entries[0].PasswordHistory[1].HistoricalPassword);

                Assert.False(doc.HasChanged);

                doc.Entries[0].Password = "4";
                Assert.True(doc.HasChanged);

                Assert.Equal(7, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("e857fe9c-091e-b44c-8574-e435549e1cc7"), doc.Entries[0].Uuid);
                Assert.Equal("", (string)doc.Entries[0].Group);
                Assert.Equal("Test", doc.Entries[0].Title);
                Assert.Equal("4", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].CreationTime);

                Assert.Equal(true, doc.Entries[0].PasswordHistory.Enabled);
                Assert.Equal(2, doc.Entries[0].PasswordHistory.MaximumCount);
                Assert.Equal(2, doc.Entries[0].PasswordHistory.Count);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 27, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[0].TimeFirstUsed);
                Assert.Equal("2", doc.Entries[0].PasswordHistory[0].HistoricalPassword);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 44, DateTimeKind.Utc), doc.Entries[0].PasswordHistory[1].TimeFirstUsed);
                Assert.Equal("3", doc.Entries[0].PasswordHistory[1].HistoricalPassword);

                doc.Entries[0].PasswordHistory.Clear();

                Assert.Equal(true, doc.Entries[0].PasswordHistory.Enabled);
                Assert.Equal(2, doc.Entries[0].PasswordHistory.MaximumCount);
                Assert.Equal(0, doc.Entries[0].PasswordHistory.Count);

                doc.Save(msSave);
                Assert.False(doc.HasChanged);
            }

            msSave.Position = 0;

            using (var doc = PwSafe.Document.Load(msSave, "123")) { //reload to verify
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(1, doc.Entries.Count);

                Assert.Equal(7, doc.Entries[0].Records.Count);
                Assert.Equal(new Guid("e857fe9c-091e-b44c-8574-e435549e1cc7"), doc.Entries[0].Uuid);
                Assert.Equal("", (string)doc.Entries[0].Group);
                Assert.Equal("Test", doc.Entries[0].Title);
                Assert.Equal("4", doc.Entries[0].Password);
                Assert.Equal(new DateTime(2016, 06, 25, 20, 32, 15, DateTimeKind.Utc), doc.Entries[0].CreationTime);

                Assert.Equal(true, doc.Entries[0].PasswordHistory.Enabled);
                Assert.Equal(2, doc.Entries[0].PasswordHistory.MaximumCount);
                Assert.Equal(0, doc.Entries[0].PasswordHistory.Count);

                Assert.False(doc.HasChanged);
            }
        }


        [Fact(DisplayName = "PasswordSafe: Document: Load/Save new file")]
        public void Document_NewSaveAndLoad() {
            using (var msFile = new MemoryStream()) {
                using (var doc = new PwSafe.Document("Password")) {
                    doc.Entries.Add(new PwSafe.Entry("Test"));
                    doc.Save(msFile);
                }

                msFile.Position = 0;

                using (var doc = PwSafe.Document.Load(msFile, "Password")) {
                    Assert.Equal(1, doc.Entries.Count);
                    Assert.Equal("Test", doc.Entries[0].Title);
                }
            }
        }


        [Fact(DisplayName = "PasswordSafe: Document: Change password")]
        public void Document_ChangePassword() {
            using (var msFile = new MemoryStream()) {
                using (var doc = new PwSafe.Document("Password")) {
                    doc.Entries.Add(new PwSafe.Entry("Test"));
                    doc.Save(msFile);
                    Assert.False(doc.HasChanged);

                    msFile.SetLength(0); //clean previous save

                    doc.ChangePassphrase("Password2");
                    Assert.True(doc.HasChanged);

                    doc.Save(msFile);
                    Assert.False(doc.HasChanged);
                }

                msFile.Position = 0;

                using (var doc = PwSafe.Document.Load(msFile, "Password2")) {
                    Assert.Equal(1, doc.Entries.Count);
                    Assert.Equal("Test", doc.Entries[0].Title);
                }
            }
        }


        [Fact(DisplayName = "PasswordSafe: Document: Change password (verify old)")]
        public void Document_ChangeOldPassword() {
            using (var msFile = new MemoryStream()) {
                using (var doc = new PwSafe.Document("Password")) {
                    doc.Entries.Add(new PwSafe.Entry("Test"));
                    doc.Save(msFile);
                    Assert.False(doc.HasChanged);

                    var result = doc.TryChangePassphrase("Password", "Password2");
                    Assert.True(result);
                    Assert.True(doc.HasChanged);

                    msFile.SetLength(0); //clean previous save
                    doc.Save(msFile);
                    Assert.False(doc.HasChanged);
                }

                msFile.Position = 0;

                using (var doc = PwSafe.Document.Load(msFile, "Password2")) {
                    Assert.Equal(1, doc.Entries.Count);
                    Assert.Equal("Test", doc.Entries[0].Title);
                }
            }
        }

        [Fact(DisplayName = "PasswordSafe: Document: Change password (verify failed)")]
        public void Document_ChangeOldPasswordFailed() {
            using (var msFile = new MemoryStream()) {
                using (var doc = new PwSafe.Document("Password")) {
                    doc.Entries.Add(new PwSafe.Entry("Test"));
                    doc.Save(msFile);
                    Assert.False(doc.HasChanged);

                    var result = doc.TryChangePassphrase("Password1", "Password2");
                    Assert.False(result);
                    Assert.False(doc.HasChanged);

                    msFile.SetLength(0); //clean previous save
                    doc.Save(msFile);
                    Assert.False(doc.HasChanged);
                }

                msFile.Position = 0;

                using (var doc = PwSafe.Document.Load(msFile, "Password")) {
                    Assert.Equal(1, doc.Entries.Count);
                    Assert.Equal("Test", doc.Entries[0].Title);
                }
            }
        }


        [Fact(DisplayName = "PasswordSafe: Document: Validate password")]
        public void Document_ValidatePassword() {
            using (var msFile = new MemoryStream()) {
                using (var doc = new PwSafe.Document("Password")) {
                    doc.Entries.Add(new PwSafe.Entry("Test"));
                    doc.Save(msFile);
                    Assert.False(doc.HasChanged);

                    var result1 = doc.ValidatePassphrase("Password2");
                    Assert.False(result1);
                    Assert.False(doc.HasChanged);

                    var result2 = doc.ValidatePassphrase("Password");
                    Assert.True(result2);
                    Assert.False(doc.HasChanged);
                }
            }
        }


        [Fact(DisplayName = "PasswordSafe: Document: Load Empty.psafe3 (via file name)")]
        public void Document_Empty_FileName_Load() {
            var fileName = Path.GetTempFileName();
            try {

                using (var streamIn = GetResourceStream("Empty.psafe3"))
                using (var streamOut = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
                    var buffer = new byte[streamIn.Length];
                    streamIn.Read(buffer, 0, buffer.Length);
                    streamOut.Write(buffer, 0, buffer.Length);
                }

                using (var doc = PwSafe.Document.Load(fileName, "123")) {
                    Assert.Equal(7, doc.Headers.Count);
                    Assert.Equal(0x030D, doc.Headers[PwSafe.HeaderType.Version].Version);
                    Assert.Equal(new Guid("3b872b47-dee9-4c4f-ba63-4d93a86dfa4c"), doc.Headers[PwSafe.HeaderType.Uuid].Uuid);
                    Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                    Assert.Equal(new DateTime(2015, 12, 28, 5, 57, 23, DateTimeKind.Utc), doc.Headers[PwSafe.HeaderType.TimestampOfLastSave].Time);
                    Assert.Equal("Josip", doc.Headers[PwSafe.HeaderType.LastSavedByUser].Text);
                    Assert.Equal("GANDALF", doc.Headers[PwSafe.HeaderType.LastSavedOnHost].Text);
                    Assert.Equal("Password Safe V3.37", doc.Headers[PwSafe.HeaderType.WhatPerformedLastSave].Text);

                    Assert.Equal(0, doc.Entries.Count);
                }

            } finally {
                File.Delete(fileName);
            }
        }

        [Fact(DisplayName = "PasswordSafe: Document: Save Empty.psafe3 (via file name)")]
        public void Document_Empty_FileName_Save() {
            var fileName = Path.GetTempFileName();
            try {

                using (var doc = PwSafe.Document.Load(GetResourceStream("Empty.psafe3"), "123")) {
                    doc.IsReadOnly = true;
                    doc.Save(fileName);
                }

                using (var doc = PwSafe.Document.Load(fileName, "123")) {
                    Assert.Equal(7, doc.Headers.Count);
                    Assert.Equal(0x030D, doc.Headers[PwSafe.HeaderType.Version].Version);
                    Assert.Equal(new Guid("3b872b47-dee9-4c4f-ba63-4d93a86dfa4c"), doc.Headers[PwSafe.HeaderType.Uuid].Uuid);
                    Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                    Assert.Equal(new DateTime(2015, 12, 28, 5, 57, 23, DateTimeKind.Utc), doc.Headers[PwSafe.HeaderType.TimestampOfLastSave].Time);
                    Assert.Equal("Josip", doc.Headers[PwSafe.HeaderType.LastSavedByUser].Text);
                    Assert.Equal("GANDALF", doc.Headers[PwSafe.HeaderType.LastSavedOnHost].Text);
                    Assert.Equal("Password Safe V3.37", doc.Headers[PwSafe.HeaderType.WhatPerformedLastSave].Text);

                    Assert.Equal(0, doc.Entries.Count);
                }

            } finally {
                File.Delete(fileName);
            }
        }


        [Fact(DisplayName = "PasswordSafe: Document: Change to same (track modify)")]
        public void Document_NoModify_ChangeToSame() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("Simple.psafe3"), "123")) {
                doc.TrackAccess = false;
                Assert.False(doc.HasChanged);

                doc.Entries[0].Title = "A";
                doc.Entries[1].Title = "B";
                Assert.False(doc.HasChanged);

                doc.Entries[0].Title = "a";
                Assert.True(doc.HasChanged);
            }
        }


        #region Utils

        private static MemoryStream GetResourceStream(string fileName) {
            var resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PasswordSafe.Test.Resources." + fileName);
            var buffer = new byte[(int)resStream.Length];
            resStream.Read(buffer, 0, buffer.Length);
            return new MemoryStream(buffer) { Position = 0 };
        }

        #endregion

    }

}
