using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veracode.OSS.Declare.Artifactory.Tests
{
    [TestFixture]
    public class CleanseHelperTests
    {
        [Test]
        public void Cleanse_ShouldNotStripValidCharacters()
        {
            var original = "abzcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ\\/.-*";
            var cleansed = CleanseHelper.Cleanse(original);
            Assert.AreEqual(original, cleansed);
        }

        [Test]
        public void Cleanse_ShouldStripInvalidCharacters()
        {
            var original = EveryAsciiCharacter();
            var cleansed = CleanseHelper.Cleanse(original);
            Assert.IsTrue(cleansed.Length == 68);
        }

        private string EveryAsciiCharacter()
        {
            string asciichar = "";

            for (var i = 0; i < 128; i++) {
                asciichar += (Convert.ToChar(i)).ToString();
            }

            return asciichar;
        }
    }
}
