using PwSafe = Medo.Security.Cryptography.PasswordSafe;
using System;
using System.IO;

namespace Example {
    internal class App {
        private static void Main() {
            var existingFile = @"Resources\Simple.psafe3";
            var newFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), @"NewExample.psafe3");

            //Load
            var doc = PwSafe.Document.Load(existingFile, "123");
            Show(doc, ConsoleColor.Gray);

            //modify
            doc.Entries["A"].Title = "Ax";
            doc.Entries["Ax"].Password = "A123x";

            //remove
            doc.Entries.Remove("B");

            //create new
            doc.Entries["C"].UserName = "Cuser";
            doc.Entries["C"].Password = "C123";
            doc.Entries["C"].Group = "Test";
            doc.Entries.Remove("C", PwSafe.RecordType.Group);

            Show(doc, ConsoleColor.White);

            //save
            doc.Save(newFile);

            //load again
            var doc2 = PwSafe.Document.Load(newFile, "123");
            Show(doc2, ConsoleColor.Yellow);

            Console.ReadKey();
        }


        private static void Show(PwSafe.Document doc, ConsoleColor color) {
            Console.ForegroundColor = color;

            Console.WriteLine("Headers");
            foreach (var field in doc.Headers) {
                Console.WriteLine("    {0}: {1}", field.HeaderType, field.ToString());
            }
            Console.WriteLine();

            Console.WriteLine("Entries");
            foreach (var entry in doc.Entries) {
                Console.WriteLine("    {0}:", entry.ToString());
                foreach (var field in entry.Records) {
                    Console.WriteLine("        {0}: {1}", field.RecordType, field.ToString());
                }
                Console.WriteLine();
            }

            Console.ResetColor();
        }
    }
}
