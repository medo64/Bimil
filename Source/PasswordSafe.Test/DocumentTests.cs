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

        [Fact(DisplayName = "PasswordSafe: Document: Load/Save Simple.bimil")]
        public void Document_SimpleBimil() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("Simple.bimil"), "123")) {
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(8, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Headers[PwSafe.HeaderType.Version].Version);
                Assert.Equal(new Guid("0b073824-a406-2f4b-87b2-48656a6b5011"), doc.Headers[PwSafe.HeaderType.Uuid].Uuid);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                Assert.Equal(new DateTime(2019, 03, 08, 04, 27, 29, DateTimeKind.Utc), doc.Headers[PwSafe.HeaderType.TimestampOfLastSave].Time);
                Assert.Equal("Josip", doc.Headers[PwSafe.HeaderType.LastSavedByUser].Text);
                Assert.Equal("GANDALF", doc.Headers[PwSafe.HeaderType.LastSavedOnHost].Text);
                Assert.Equal("Bimil V2.70", doc.Headers[PwSafe.HeaderType.WhatPerformedLastSave].Text);
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
                Assert.Equal(new DateTime(2019, 03, 08, 04, 27, 29, DateTimeKind.Utc), doc.Headers[PwSafe.HeaderType.TimestampOfLastSave].Time);
                Assert.Equal("Josip", doc.Headers[PwSafe.HeaderType.LastSavedByUser].Text);
                Assert.Equal("GANDALF", doc.Headers[PwSafe.HeaderType.LastSavedOnHost].Text);
                Assert.Equal("Bimil V2.70", doc.Headers[PwSafe.HeaderType.WhatPerformedLastSave].Text);
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


        [Fact(DisplayName = "PasswordSafe: Document: Load/Save Simple.bimil from key")]
        public void Document_SimpleBimilFromKey() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("Simple.bimil"), null, new byte[] { 0x98, 0x33, 0x9C, 0x6D, 0xE6, 0xCF, 0x5A, 0x35, 0x53, 0x36, 0x7D, 0xFE, 0xF2, 0xC9, 0xDB, 0x1A, 0xAC, 0x28, 0xBD, 0x60, 0xFB, 0xA3, 0x9C, 0x37, 0x38, 0x4C, 0x93, 0xE6, 0x63, 0x51, 0xFE, 0xF8, 0x75, 0x45, 0x5F, 0xCD, 0x8D, 0xC3, 0x93, 0xC2, 0x1C, 0xB9, 0x14, 0xF1, 0x8E, 0xAA, 0x70, 0x49, 0xBA, 0xDE, 0xEC, 0xFB, 0x50, 0xCA, 0x65, 0x35, 0x06, 0x3E, 0x09, 0x0A, 0xE4, 0xE0, 0xFC, 0xB9 })) {
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(8, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Headers[PwSafe.HeaderType.Version].Version);
                Assert.Equal(new Guid("0b073824-a406-2f4b-87b2-48656a6b5011"), doc.Headers[PwSafe.HeaderType.Uuid].Uuid);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                Assert.Equal(new DateTime(2019, 03, 08, 04, 27, 29, DateTimeKind.Utc), doc.Headers[PwSafe.HeaderType.TimestampOfLastSave].Time);
                Assert.Equal("Josip", doc.Headers[PwSafe.HeaderType.LastSavedByUser].Text);
                Assert.Equal("GANDALF", doc.Headers[PwSafe.HeaderType.LastSavedOnHost].Text);
                Assert.Equal("Bimil V2.70", doc.Headers[PwSafe.HeaderType.WhatPerformedLastSave].Text);
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
                Assert.Throws<NotSupportedException>(() => {
                    doc.Save(msSave);
                });

                doc.SetPassphrase("123");
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
                Assert.Equal(new DateTime(2019, 03, 08, 04, 27, 29, DateTimeKind.Utc), doc.Headers[PwSafe.HeaderType.TimestampOfLastSave].Time);
                Assert.Equal("Josip", doc.Headers[PwSafe.HeaderType.LastSavedByUser].Text);
                Assert.Equal("GANDALF", doc.Headers[PwSafe.HeaderType.LastSavedOnHost].Text);
                Assert.Equal("Bimil V2.70", doc.Headers[PwSafe.HeaderType.WhatPerformedLastSave].Text);
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

        [Fact(DisplayName = "PasswordSafe: Document: Load/Save Simple.bimil using key")]
        public void Document_SimpleBimilToKey() {
            var msSave = new MemoryStream();
            using (var doc = PwSafe.Document.Load(GetResourceStream("Simple.bimil"), "123")) {
                doc.TrackAccess = false;
                doc.TrackModify = false;
                doc.Save(msSave, null, new byte[] { 0x11, 0x22, 0x33, 0x6D, 0xE6, 0xCF, 0x5A, 0x35, 0x53, 0x36, 0x7D, 0xFE, 0xF2, 0xC9, 0xDB, 0x1A, 0xAC, 0x28, 0xBD, 0x60, 0xFB, 0xA3, 0x9C, 0x37, 0x38, 0x4C, 0x93, 0xE6, 0x63, 0x51, 0xFE, 0xF8, 0x75, 0x45, 0x5F, 0xCD, 0x8D, 0xC3, 0x93, 0xC2, 0x1C, 0xB9, 0x14, 0xF1, 0x8E, 0xAA, 0x70, 0x49, 0xBA, 0xDE, 0xEC, 0xFB, 0x50, 0xCA, 0x65, 0x35, 0x06, 0x3E, 0x09, 0x0A, 0xE4, 0x11, 0x22, 0x33 });
            }

            msSave.Position = 0;

            using (var doc = PwSafe.Document.Load(msSave, "123")) { //reload to verify
                doc.TrackAccess = false;
                doc.TrackModify = false;

                Assert.Equal(8, doc.Headers.Count);
                Assert.Equal(0x030D, doc.Headers[PwSafe.HeaderType.Version].Version);
                Assert.Equal(new Guid("0b073824-a406-2f4b-87b2-48656a6b5011"), doc.Headers[PwSafe.HeaderType.Uuid].Uuid);
                Assert.Equal("", doc.Headers[PwSafe.HeaderType.NonDefaultPreferences].Text);
                Assert.Equal(new DateTime(2019, 03, 08, 04, 27, 29, DateTimeKind.Utc), doc.Headers[PwSafe.HeaderType.TimestampOfLastSave].Time);
                Assert.Equal("Josip", doc.Headers[PwSafe.HeaderType.LastSavedByUser].Text);
                Assert.Equal("GANDALF", doc.Headers[PwSafe.HeaderType.LastSavedOnHost].Text);
                Assert.Equal("Bimil V2.70", doc.Headers[PwSafe.HeaderType.WhatPerformedLastSave].Text);
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

                    Assert.Equal("50-61-73-73-77-6F-72-64", BitConverter.ToString(doc.GetPassphrase()));

                    doc.ChangePassphrase("Password2");
                    Assert.True(doc.HasChanged);

                    Assert.Equal("50-61-73-73-77-6F-72-64-32", BitConverter.ToString(doc.GetPassphrase()));

                    doc.Save(msFile);
                    Assert.False(doc.HasChanged);

                    Assert.Equal("50-61-73-73-77-6F-72-64-32", BitConverter.ToString(doc.GetPassphrase()));
                }

                msFile.Position = 0;

                using (var doc = PwSafe.Document.Load(msFile, "Password2")) {
                    Assert.Equal("50-61-73-73-77-6F-72-64-32", BitConverter.ToString(doc.GetPassphrase()));
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

                    Assert.Equal("50-61-73-73-77-6F-72-64", BitConverter.ToString(doc.GetPassphrase()));

                    var result = doc.TryChangePassphrase("Password", "Password2");
                    Assert.True(result);
                    Assert.True(doc.HasChanged);

                    Assert.Equal("50-61-73-73-77-6F-72-64-32", BitConverter.ToString(doc.GetPassphrase()));

                    msFile.SetLength(0); //clean previous save
                    doc.Save(msFile);
                    Assert.False(doc.HasChanged);

                    Assert.Equal("50-61-73-73-77-6F-72-64-32", BitConverter.ToString(doc.GetPassphrase()));
                }

                msFile.Position = 0;

                using (var doc = PwSafe.Document.Load(msFile, "Password2")) {
                    Assert.Equal("50-61-73-73-77-6F-72-64-32", BitConverter.ToString(doc.GetPassphrase()));
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

                    Assert.Equal("50-61-73-73-77-6F-72-64", BitConverter.ToString(doc.GetPassphrase()));

                    var result = doc.TryChangePassphrase("Password1", "Password2");
                    Assert.False(result);
                    Assert.False(doc.HasChanged);

                    Assert.Equal("50-61-73-73-77-6F-72-64", BitConverter.ToString(doc.GetPassphrase()));

                    msFile.SetLength(0); //clean previous save
                    doc.Save(msFile);
                    Assert.False(doc.HasChanged);

                    Assert.Equal("50-61-73-73-77-6F-72-64", BitConverter.ToString(doc.GetPassphrase()));
                }

                msFile.Position = 0;

                using (var doc = PwSafe.Document.Load(msFile, "Password")) {
                    Assert.Equal("50-61-73-73-77-6F-72-64", BitConverter.ToString(doc.GetPassphrase()));
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
            using (var doc = PwSafe.Document.Load(GetResourceStream("Simple.psafe3"), "123")) {
                doc.TrackAccess = false;
                Assert.False(doc.HasChanged);

                doc.Entries[0].Title = "A";
                doc.Entries[1].Title = "B";
                doc.Entries[0].Password = "A123";
                doc.Entries[1].Password = "B123";
                Assert.False(doc.HasChanged);

                doc.Entries[0].Title = "a";
                Assert.True(doc.HasChanged);
            }
        }


        [Fact(DisplayName = "PasswordSafe: Document: Load/Save Policies.psafe3")]
        public void Document_Policies() {
            using (var msFile = new MemoryStream()) {
                using (var doc = PwSafe.Document.Load(GetResourceStream("Policies.psafe3"), "123")) {
                    Assert.False(doc.HasChanged);

                    Assert.Equal(3, doc.NamedPasswordPolicies.Count);

                    var policy1 = doc.NamedPasswordPolicies[0];
                    Assert.Equal("Even", policy1.Name);
                    Assert.Equal((int)(PwSafe.PasswordPolicyStyle.UseUppercase | PwSafe.PasswordPolicyStyle.UseSymbols | PwSafe.PasswordPolicyStyle.MakePronounceable), (int)policy1.Style);
                    Assert.Equal(12, policy1.TotalPasswordLength);
                    Assert.Equal(0, policy1.MinimumLowercaseCount);
                    Assert.Equal(0, policy1.MinimumUppercaseCount);
                    Assert.Equal(1, policy1.MinimumDigitCount);
                    Assert.Equal(3, policy1.MinimumSymbolCount);
                    Assert.Equal("!#$&(+@|", new string(policy1.GetSpecialSymbolSet()));

                    var policy2 = doc.NamedPasswordPolicies[1];
                    Assert.Equal("Hex", policy2.Name);
                    Assert.Equal((int)(PwSafe.PasswordPolicyStyle.UseHexDigits), (int)policy2.Style);
                    Assert.Equal(10, policy2.TotalPasswordLength);
                    Assert.Equal(0, policy2.MinimumLowercaseCount);
                    Assert.Equal(0, policy2.MinimumUppercaseCount);
                    Assert.Equal(0, policy2.MinimumDigitCount);
                    Assert.Equal(0, policy2.MinimumSymbolCount);
                    Assert.Equal("!#$%&()*+,-./:;<=>?@[\\]^_{|}~", new string(policy2.GetSpecialSymbolSet()));

                    var policy3 = doc.NamedPasswordPolicies[2];
                    Assert.Equal("Odd", policy3.Name);
                    Assert.Equal((int)(PwSafe.PasswordPolicyStyle.UseLowercase | PwSafe.PasswordPolicyStyle.UseDigits | PwSafe.PasswordPolicyStyle.UseEasyVision), (int)policy3.Style);
                    Assert.Equal(11, policy3.TotalPasswordLength);
                    Assert.Equal(2, policy3.MinimumLowercaseCount);
                    Assert.Equal(4, policy3.MinimumUppercaseCount);
                    Assert.Equal(1, policy3.MinimumDigitCount);
                    Assert.Equal(3, policy3.MinimumSymbolCount);
                    Assert.Equal("!#$%&()*+,-./:;<=>?@[\\]^_{|}~", new string(policy3.GetSpecialSymbolSet()));

                    Assert.Equal(1, doc.Entries.Count);

                    var policy = doc.Entries[0].PasswordPolicy;
                    Assert.Equal((int)(PwSafe.PasswordPolicyStyle.UseLowercase | PwSafe.PasswordPolicyStyle.UseUppercase | PwSafe.PasswordPolicyStyle.UseDigits | PwSafe.PasswordPolicyStyle.UseSymbols | PwSafe.PasswordPolicyStyle.UseEasyVision), (int)policy.Style);
                    Assert.Equal(80, policy.TotalPasswordLength);
                    Assert.Equal(7, policy.MinimumLowercaseCount);
                    Assert.Equal(5, policy.MinimumUppercaseCount);
                    Assert.Equal(8, policy.MinimumDigitCount);
                    Assert.Equal(6, policy.MinimumSymbolCount);
                    Assert.Equal(@"#$%&*+-/<=>?@\^_~", new string(policy.GetSpecialSymbolSet()));

                    doc.Save(msFile);
                }

                msFile.Position = 0;

                using (var doc = PwSafe.Document.Load(msFile, "123")) {
                    Assert.False(doc.HasChanged);

                    Assert.Equal(3, doc.NamedPasswordPolicies.Count);

                    var policy1 = doc.NamedPasswordPolicies[0];
                    Assert.Equal("Even", policy1.Name);
                    Assert.Equal((int)(PwSafe.PasswordPolicyStyle.UseUppercase | PwSafe.PasswordPolicyStyle.UseSymbols | PwSafe.PasswordPolicyStyle.MakePronounceable), (int)policy1.Style);
                    Assert.Equal(12, policy1.TotalPasswordLength);
                    Assert.Equal(0, policy1.MinimumLowercaseCount);
                    Assert.Equal(0, policy1.MinimumUppercaseCount);
                    Assert.Equal(1, policy1.MinimumDigitCount);
                    Assert.Equal(3, policy1.MinimumSymbolCount);
                    Assert.Equal("!#$&(+@|", new string(policy1.GetSpecialSymbolSet()));

                    var policy2 = doc.NamedPasswordPolicies[1];
                    Assert.Equal("Hex", policy2.Name);
                    Assert.Equal((int)(PwSafe.PasswordPolicyStyle.UseHexDigits), (int)policy2.Style);
                    Assert.Equal(10, policy2.TotalPasswordLength);
                    Assert.Equal(0, policy2.MinimumLowercaseCount);
                    Assert.Equal(0, policy2.MinimumUppercaseCount);
                    Assert.Equal(0, policy2.MinimumDigitCount);
                    Assert.Equal(0, policy2.MinimumSymbolCount);
                    Assert.Equal("!#$%&()*+,-./:;<=>?@[\\]^_{|}~", new string(policy2.GetSpecialSymbolSet()));

                    var policy3 = doc.NamedPasswordPolicies[2];
                    Assert.Equal("Odd", policy3.Name);
                    Assert.Equal((int)(PwSafe.PasswordPolicyStyle.UseLowercase | PwSafe.PasswordPolicyStyle.UseDigits | PwSafe.PasswordPolicyStyle.UseEasyVision), (int)policy3.Style);
                    Assert.Equal(11, policy3.TotalPasswordLength);
                    Assert.Equal(2, policy3.MinimumLowercaseCount);
                    Assert.Equal(4, policy3.MinimumUppercaseCount);
                    Assert.Equal(1, policy3.MinimumDigitCount);
                    Assert.Equal(3, policy3.MinimumSymbolCount);
                    Assert.Equal("!#$%&()*+,-./:;<=>?@[\\]^_{|}~", new string(policy3.GetSpecialSymbolSet()));

                    Assert.Equal(1, doc.Entries.Count);

                    var policy = doc.Entries[0].PasswordPolicy;
                    Assert.Equal((int)(PwSafe.PasswordPolicyStyle.UseLowercase | PwSafe.PasswordPolicyStyle.UseUppercase | PwSafe.PasswordPolicyStyle.UseDigits | PwSafe.PasswordPolicyStyle.UseSymbols | PwSafe.PasswordPolicyStyle.UseEasyVision), (int)policy.Style);
                    Assert.Equal(80, policy.TotalPasswordLength);
                    Assert.Equal(7, policy.MinimumLowercaseCount);
                    Assert.Equal(5, policy.MinimumUppercaseCount);
                    Assert.Equal(8, policy.MinimumDigitCount);
                    Assert.Equal(6, policy.MinimumSymbolCount);
                    Assert.Equal(@"#$%&*+-/<=>?@\^_~", new string(policy.GetSpecialSymbolSet()));
                }
            }
        }


        [Fact(DisplayName = "PasswordSafe: Document: Single named policy")]
        public void Document_NamedPolicies_Single() {
            using (var msFile = new MemoryStream()) {
                using (var doc = new PwSafe.Document("123")) {
                    doc.Headers[PwSafe.HeaderType.NamedPasswordPolicies].Text = "0104Test020000a00100200300400";
                    doc.Save(msFile);
                }

                msFile.Position = 0;

                using (var doc = PwSafe.Document.Load(msFile, "123")) {
                    Assert.Equal(1, doc.NamedPasswordPolicies.Count);
                    var policy = doc.NamedPasswordPolicies[0];
                    Assert.Equal("Test", policy.Name);
                    Assert.Equal(0x0200, (int)policy.Style);
                    Assert.Equal(10, policy.TotalPasswordLength);
                    Assert.Equal(1, policy.MinimumLowercaseCount);
                    Assert.Equal(2, policy.MinimumUppercaseCount);
                    Assert.Equal(3, policy.MinimumDigitCount);
                    Assert.Equal(4, policy.MinimumSymbolCount);
                    Assert.Equal("", new string(policy.GetSpecialSymbolSet()));
                }
            }
        }

        [Fact(DisplayName = "PasswordSafe: Document: Single named policy (too short)")]
        public void Document_NamedPolicies_Single_TooShort() {
            using (var msFile = new MemoryStream()) {
                using (var doc = new PwSafe.Document("123")) {
                    doc.Headers[PwSafe.HeaderType.NamedPasswordPolicies].Text = "0104Test020000a0010020030040";
                    doc.Save(msFile);
                }

                msFile.Position = 0;

                using (var doc = PwSafe.Document.Load(msFile, "123")) {
                    Assert.Equal(0, doc.NamedPasswordPolicies.Count);
                }
            }
        }

        [Fact(DisplayName = "PasswordSafe: Document: Single named policy (too long)")]
        public void Document_NamedPolicies_Single_TooLong() {
            using (var msFile = new MemoryStream()) {
                using (var doc = new PwSafe.Document("123")) {
                    doc.Headers[PwSafe.HeaderType.NamedPasswordPolicies].Text = "0104Test020000a00100200300400+";
                    doc.Save(msFile);
                }

                msFile.Position = 0;

                using (var doc = PwSafe.Document.Load(msFile, "123")) {
                    Assert.Equal(1, doc.NamedPasswordPolicies.Count);
                    var policy = doc.NamedPasswordPolicies[0];
                    Assert.Equal("Test", policy.Name);
                    Assert.Equal(0x0200, (int)policy.Style);
                    Assert.Equal(10, policy.TotalPasswordLength);
                    Assert.Equal(1, policy.MinimumLowercaseCount);
                    Assert.Equal(2, policy.MinimumUppercaseCount);
                    Assert.Equal(3, policy.MinimumDigitCount);
                    Assert.Equal(4, policy.MinimumSymbolCount);
                    Assert.Equal("", new string(policy.GetSpecialSymbolSet()));
                }
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
