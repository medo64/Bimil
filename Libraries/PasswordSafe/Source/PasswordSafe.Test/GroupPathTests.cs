using System;
using Xunit;
using PwSafe = Medo.Security.Cryptography.PasswordSafe;

namespace PasswordSafe.Test {
    public class GroupPathTests {

        [Fact(DisplayName = "PasswordSafe: GroupPath: New")]
        public void GroupPath_New() {
            PwSafe.GroupPath path = "A";
            Assert.Equal("A", path.ToString());

            var segments = path.GetSegments();
            Assert.Equal(1, segments.Length);
            Assert.Equal("A", segments[0]);
        }

        [Fact(DisplayName = "PasswordSafe: GroupPath: New (via components)")]
        public void GroupPath_NewViaComponents() {
            var path = new PwSafe.GroupPath("A", "B");
            Assert.Equal("A.B", path.ToString());

            var segments = path.GetSegments();
            Assert.Equal(2, segments.Length);
            Assert.Equal("A", segments[0]);
            Assert.Equal("B", segments[1]);
        }

        [Fact(DisplayName = "PasswordSafe: GroupPath: New (via escaped components)")]
        public void GroupPath_NewViaComponentsEscaped() {
            var path = new PwSafe.GroupPath("A", "B.com");
            Assert.Equal(@"A.B\.com", path.ToString());

            var segments = path.GetSegments();
            Assert.Equal(2, segments.Length);
            Assert.Equal("A", segments[0]);
            Assert.Equal("B.com", segments[1]);
        }

        [Fact(DisplayName = "PasswordSafe: GroupPath: New (via escaped components 1)")]
        public void GroupPath_NewViaComponentsEscaped2() {
            var path = new PwSafe.GroupPath("A", @"B\.com");
            Assert.Equal(@"A.B\\.com", path.ToString());

            var segments = path.GetSegments();
            Assert.Equal(2, segments.Length);
            Assert.Equal("A", segments[0]);
            Assert.Equal(@"B\.com", segments[1]);
        }

        [Fact(DisplayName = "PasswordSafe: GroupPath: New (null)")]
        public void GroupPath_NewNull() {
            PwSafe.GroupPath path = default(string);
            Assert.Equal("", path.ToString());

            var segments = path.GetSegments();
            Assert.Equal(1, segments.Length);
            Assert.Equal("", segments[0]);
        }

        [Fact(DisplayName = "PasswordSafe: GroupPath: New tree")]
        public void GroupPath_NewTree() {
            PwSafe.GroupPath path = "A.B";
            Assert.Equal("A.B", path.ToString());

            var segments = path.GetSegments();
            Assert.Equal(2, segments.Length);
            Assert.Equal("A", segments[0]);
            Assert.Equal("B", segments[1]);
        }


        [Fact(DisplayName = "PasswordSafe: GroupPath: Up")]
        public void GroupPath_Up() {
            PwSafe.GroupPath path = @"A.B.C\.d";

            Assert.Equal(@"A.B.C\.d", path.ToString());
            Assert.Equal("A.B", path.Up().ToString());
            Assert.Equal("A", path.Up().Up().ToString());
            Assert.Equal("", path.Up().Up().Up().ToString());
            Assert.Equal("", path.Up().Up().Up().Up().ToString());
        }

        [Fact(DisplayName = "PasswordSafe: GroupPath: Append")]
        public void GroupPath_Append() {
            PwSafe.GroupPath path = "";

            Assert.Equal("", path.ToString());
            Assert.Equal("", path.Append(null).ToString());
            Assert.Equal("", path.Append("").ToString());
            Assert.Equal("A", path.Append("A").ToString());
            Assert.Equal("A.B", path.Append("A").Append("B").ToString());
            Assert.Equal(@"A.B.C\.d", path.Append("A").Append("B").Append("C.d").ToString());
            Assert.Equal(@"A.B.C\.d", path.Append("A").Append("B").Append("").Append("C.d").Append("").ToString()); //Empty elements are not appended.
        }


        [Fact(DisplayName = "PasswordSafe: GroupPath: Indexer Get")]
        public void GroupPath_Indexed() {
            PwSafe.GroupPath path = @"A.B.C\.d";

            Assert.Equal(null, path[-1]);
            Assert.Equal("A", path[0]);
            Assert.Equal("B", path[1]);
            Assert.Equal("C.d", path[2]);
            Assert.Equal(null, path[3]);
        }

    }
}
