namespace Tests;

using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public partial class DocumentTests {

    private static Stream GetResourceStream(string fileName) {
        if (fileName == null) { return null; }
        var helperType = typeof(DocumentTests).GetTypeInfo();
        var assembly = helperType.Assembly;
        return assembly.GetManifestResourceStream(helperType.Namespace + ".Assets." + fileName);
    }

}
