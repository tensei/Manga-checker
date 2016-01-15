using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manga_checker.Properties;

namespace Manga_checker.Handlers
{
    public class DebugText
    {
        public static void Write(string text)
        {
            //Read
            Settings.Default.Debug += text + "\n";
        }
    }
}
