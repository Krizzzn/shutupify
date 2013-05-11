using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Shutupify
{
    public static class ResourceLoader
    {

        public static string LoadEmbeddedFile(this System.Reflection.Assembly assembly, string fileNameInAssembly)
        {
            return LoadEmbeddedFile(assembly, fileNameInAssembly, Encoding.Default);
        }

        public static string LoadEmbeddedFile(this System.Reflection.Assembly assembly, string fileNameInAssembly, Encoding encoding)
        {
            var bytes = LoadEmbeddedAsBytes(assembly, fileNameInAssembly);
            return encoding.GetString(bytes);
        }

        public static Byte[] LoadEmbeddedAsBytes(this System.Reflection.Assembly assembly, string fileNameInAssembly)
        {
            Byte[] bytes;

            using (System.IO.Stream stream = assembly.LoadEmbeddedAsStream(fileNameInAssembly))
            {
                bytes = new Byte[stream.Length];
                stream.Read(bytes, 0, (int)(stream.Length));
            }
            return bytes;
        }

        public static Stream LoadEmbeddedAsStream(this System.Reflection.Assembly assembly, string fileNameInAssembly)
        {
            string fullResourceName = GetResourceName(assembly, fileNameInAssembly);

            if (fullResourceName == null)
                throw new System.IO.FileNotFoundException("Could not find file " + fileNameInAssembly + " in the resources of assembly " + assembly.FullName + ".");

            return assembly.GetManifestResourceStream(fullResourceName);
        }


        public static string GetResourceName(this System.Reflection.Assembly assembly, string fileNameInAssembly)
        {
            var resName = from name in assembly.GetManifestResourceNames()
                          where name.ToLower().EndsWith(fileNameInAssembly.ToLower())
                          select name;

            return resName.FirstOrDefault();
        }
    }
}
