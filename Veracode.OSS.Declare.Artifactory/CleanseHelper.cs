using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Veracode.OSS.Declare.Artifactory
{
    public static class CleanseHelper
    {
        public static string Cleanse(string strIn)
        {
            try
            {
                return Regex.Replace(strIn, @"[^\w\.\-:/\\*]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }
    }
}
