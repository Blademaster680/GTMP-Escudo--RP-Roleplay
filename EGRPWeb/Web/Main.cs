using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace Web
{
    public class Main
    {
        public static string GetSha256FromString(string strData)
        {
            var message = Encoding.ASCII.GetBytes(strData);
            var hashString = new SHA256Managed();
            var hex = "";

            var hashValue = hashString.ComputeHash(message);
            foreach (var x in hashValue)
                hex += string.Format("{0:x2}", x);
            return hex;
        }
    }
}