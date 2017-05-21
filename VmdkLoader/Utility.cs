using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VmdkLoader {
    class Utility {
        
        public static string GetSystemDir() {
            return Environment.SystemDirectory;
        }

        public static string readToEnd(StreamReader reader) {
            StringBuilder sb = new StringBuilder();
            while (reader.Peek() >= 0) {
                sb.Append((char)reader.Read());
            }
            return sb.ToString();
        }
    }
}
