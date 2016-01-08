using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manga_checker.Handlers
{
    class Base64
    {
        public static DebugText debug = new DebugText();
        public string Base64Encode(string plainText)
        {
            try
            {
                var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                return Convert.ToBase64String(plainTextBytes);
            }
            catch (Exception e)
            {
                debug.Write(e.Message);
                return "null";
            }
            
        }

        public string Base64Decode(string base64EncodedData)
        {
            try
            {
                var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception e)
            {
                debug.Write(e.Message);
                return "null";
            }
        }
    }
}
