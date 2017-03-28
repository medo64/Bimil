using System;
using System.Collections.Generic;
using System.Text;

namespace Medo.Security.Cryptography.PasswordSafe {
    /// <summary>
    /// Group path.
    /// Separate components are separated by dot (.) character.
    /// Class is immutable.
    /// </summary>
    public class GroupPath {

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="segments">Path segments. Null or empty components are ignored.</param>
        public GroupPath(params string[] segments) {
            var sb = new StringBuilder();
            if (segments != null) {
                foreach (var component in segments) {
                    if (!string.IsNullOrEmpty(component)) {
                        if (sb.Length > 0) { sb.Append("."); }
                        sb.Append(component.Replace(".", @"\."));
                    }
                }
            }
            this.Group = sb.ToString();
        }

        private GroupPath(string group) {
            this.Group = group;
        }


        private readonly string Group;


        /// <summary>
        /// Returns array of all segments.
        /// </summary>
        public string[] GetSegments() {
            var escaped = false;
            var segmentList = new List<string>();
            var segment = new StringBuilder();
            foreach (var ch in this.Group) {
                if (ch == '.') {
                    if (escaped) {
                        segment[segment.Length - 1] = '.';
                        escaped = false;
                    } else {
                        segmentList.Add(segment.ToString());
                        segment.Length = 0;
                    }
                } else {
                    if (ch == '\\') { escaped = true; }
                    segment.Append(ch);
                }
            }
            segmentList.Add(segment.ToString());
            return segmentList.ToArray();
        }


        /// <summary>
        /// Appends a new segment.
        /// </summary>
        /// <param name="segment">Segment to append.</param>
        public GroupPath Append(string segment) {
            if (string.IsNullOrEmpty(segment)) { return this; } //ok since class is immutable;
            if (string.IsNullOrEmpty(this.Group)) { return new GroupPath(segment); }

            var segments = this.GetSegments();
            var newSegments = new string[segments.Length + 1];
            Array.Copy(segments, 0, newSegments, 0, segments.Length);
            newSegments[newSegments.Length - 1] = segment;
            return new GroupPath(newSegments);
        }

        /// <summary>
        /// Returns path one level up the hierarchy.
        /// </summary>
        public GroupPath Up() {
            var segments = this.GetSegments();
            if (segments.Length <= 1) { return new GroupPath(""); }

            var newSegments = new string[segments.Length - 1];
            Array.Copy(segments, 0, newSegments, 0, newSegments.Length);
            return new GroupPath(newSegments);
        }


        /// <summary>
        /// Gets a segment of the path or null if segment doesn't exist.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        public string this[int index] {
            get {
                var segements = this.GetSegments();
                if ((index < 0) || (index >= segements.Length)) { return null; }
                return segements[index];
            }
        }


        #region Overrides

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj) {
            if (obj is GroupPath path) { return this.Group.Equals(path.Group, StringComparison.OrdinalIgnoreCase); }

            var group = obj as string;
            return this.Group.Equals(group, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// A hash code for the current object.
        /// </summary>
        public override int GetHashCode() {
            return this.Group.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return this.Group;
        }

        #endregion


        #region Operators

        /// <summary>
        /// Returns group from path.
        /// If path is null, empty string is assumed.
        /// </summary>
        /// <param name="groupPath">Path expression.</param>
        public static implicit operator string(GroupPath groupPath) {
            return groupPath?.Group ?? "";
        }

        /// <summary>
        /// Returns path from group.
        /// If group is null, empty group is assumed.
        /// </summary>
        /// <param name="group">Group value.</param>
        public static implicit operator GroupPath(string group) {
            return new GroupPath(group ?? "");
        }

        #endregion

    }
}
