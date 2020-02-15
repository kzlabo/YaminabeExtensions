/*
 * Copyright 2020 kzlabo
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using YaminabeExtensions.Core;

namespace YaminabeExtensions.Test.Tests.Core
{
    /// <summary>
    /// 文字列操作拡張メソッドの検証を提供します。
    /// </summary>
    [TestClass]
    public class StringExtensionsUnitTest
    {
        #region -------------------- test method --------------------

        #region -------------------- EndsWithAdd --------------------

        /// <summary>
        /// <see cref="StringExtensions.EndsWithAdd(string, string)"/> の正常系検証を実施します。
        /// </summary>
        [TestCategory("String:EndsWithAdd")]
        [TestMethod]
        public void EndsWithAddSuccessTest()
        {
            string data1 = @"c:\test";
            string data2 = @"c:\test\";

            Assert.AreEqual(@"c:\test\", data1.EndsWithAdd(@"\"));
            Assert.AreEqual(@"c:\test\", data2.EndsWithAdd(@"\"));
        }

        /// <summary>
        /// <see cref="StringExtensions.EndsWithAdd(string, string)"/> の大文字小文字比較検証を実施します。
        /// </summary>
        [TestCategory("String:EndsWithAdd")]
        [TestMethod]
        public void EndsWithAddComparisonTest()
        {
            string data1 = @"c:\test\a";
            string data2 = @"c:\test\A";

            Assert.AreEqual(@"c:\test\a", data1.EndsWithAdd(@"a"));
            Assert.AreEqual(@"c:\test\aA", data1.EndsWithAdd(@"A"));
            Assert.AreEqual(@"c:\test\Aa", data2.EndsWithAdd(@"a"));
            Assert.AreEqual(@"c:\test\A", data2.EndsWithAdd(@"A"));
        }


        /// <summary>
        /// <see cref="StringExtensions.EndsWithAdd(string, string)"/> の <see cref="null"/ 指定検証を実施します。
        /// </summary>
        [TestCategory("String:EndsWithAdd")]
        [TestMethod]
        public void EndsWithAddNullTest()
        {
            string data = null;

            Assert.IsNull(data.EndsWithAdd(@"\"));
        }

        /// <summary>
        /// <see cref="StringExtensions.EndsWithAdd(string, string)"/> の <see cref="string.Empty"/> 指定検証を実施します。
        /// </summary>
        [TestCategory("String:EndsWithAdd")]
        [TestMethod]
        public void EndsWithAddEmptyTest()
        {
            string data = string.Empty;

            Assert.AreEqual(string.Empty, data.EndsWithAdd(@"\"));
        }

        #endregion

        #region -------------------- Path --------------------

        /// <summary>
        /// <see cref="StringExtensions.GetPathDirectoryName(string)"/> の正常系検証を実施します。
        /// </summary>
        [TestCategory("String:Path")]
        [TestMethod]
        public void GetPathDirectoryNameSuccessTest()
        {
            Assert.AreEqual(@"C:\Tests", @"C:\Tests\UnitTest.dll".GetPathDirectoryName());
        }

        /// <summary>
        /// <see cref="StringExtensions.GetPathExtension(string)"/> の正常系検証を実施します。
        /// </summary>
        [TestCategory("String:Path")]
        [TestMethod]
        public void GetPathExtensionSuccessTest()
        {
            Assert.AreEqual(@".dll", @"C:\Tests\UnitTest.dll".GetPathExtension());
        }

        /// <summary>
        /// <see cref="StringExtensions.GetPathFileName(string)"/> の正常系検証を実施します。
        /// </summary>
        [TestCategory("String:Path")]
        [TestMethod]
        public void GetPathFileNameSuccessTest()
        {
            Assert.AreEqual(@"UnitTest.dll", @"C:\Tests\UnitTest.dll".GetPathFileName());
        }

        /// <summary>
        /// <see cref="StringExtensions.GetPathFileNameWithoutExtension(string)"/> の正常系検証を実施します。
        /// </summary>
        [TestCategory("String:Path")]
        [TestMethod]
        public void GetPathFileNameWithoutExtensionSuccessTest()
        {
            Assert.AreEqual(@"UnitTest", @"C:\Tests\UnitTest.dll".GetPathFileNameWithoutExtension());
        }

        #endregion

        #region -------------------- Regex --------------------

        /// <summary>
        /// <see cref="StringExtensions.RegReplace(string, string, System.Text.RegularExpressions.MatchEvaluator, System.Text.RegularExpressions.RegexOptions, System.TimeSpan)"/> の正常系検証を実施します。
        /// </summary>
        [TestCategory("String:Regex")]
        [TestMethod]
        public void RegReplaceSuccessTest()
        {

            Assert.AreEqual(@"aabbccABCaabbcc", @"abcABCabc".RegReplace("[a-z]", m => { return $"{m.Value}{m.Value}"; }, System.Text.RegularExpressions.RegexOptions.None, System.Text.RegularExpressions.Regex.InfiniteMatchTimeout));
        }


        #endregion

        #region -------------------- Convert --------------------

        #region -------------------- Generic --------------------

        /// <summary>
        /// <see cref="StringExtensions.Convert{T}(string)"/> の正常系検証を実施します。
        /// </summary>
        [TestCategory("String:Convert")]
        [TestMethod]
        public void ConvertSuccessTest()
        {
            Assert.AreEqual(1000, "1000".Convert<int>());
            Assert.AreEqual(new System.DateTime(2000, 1, 2, 12, 23, 34), "2000/01/02 12:23:34".Convert<System.DateTime>());
        }

        #endregion

        #region -------------------- Int --------------------

        /// <summary>
        /// <see cref="StringExtensions.ConvertToInt(string)"/> の正常系検証を実施します。
        /// </summary>
        [TestCategory("String:ConvertToInt")]
        [TestMethod]
        public void ConvertToIntSuccessTest()
        {
            Assert.AreEqual(1000, "1000".ConvertToInt());
        }

        /// <summary>
        /// <see cref="StringExtensions.ConvertToInt(string)"/> の異常系検証を実施します。
        /// </summary>
        [TestCategory("String:ConvertToInt")]
        [TestMethod]
        public void ConvertToIntFailTest()
        {
            Assert.AreEqual(0, "aaa".ConvertToInt());
        }

        /// <summary>
        /// <see cref="StringExtensions.ConvertToInt(string, int)"/> の初期値指定時の正常系検証を実施します。
        /// </summary>
        [TestCategory("String:ConvertToInt")]
        [TestMethod]
        public void ConvertToIntAndDefSuccessTest()
        {
            Assert.AreEqual(1000, "1000".ConvertToInt(-1));
        }

        /// <summary>
        /// <see cref="StringExtensions.ConvertToInt(string, int)"/> の初期値指定時の異常系検証を実施します。
        /// </summary>
        [TestCategory("String:ConvertToInt")]
        [TestMethod]
        public void ConvertToIntAndDefFailTest()
        {
            Assert.AreEqual(-1, "aaa".ConvertToInt(-1));
        }

        /// <summary>
        /// <see cref="StringExtensions.ConvertToInt(string, int)"/> のオーバーフロー検証を実施します。
        /// </summary>
        [TestCategory("String:ConvertToInt")]
        [TestMethod]
        public void ConvertToIntNonOverflowTest()
        {
            Assert.AreEqual(2147483647, "2147483647".ConvertToInt());
        }

        /// <summary>
        /// <see cref="StringExtensions.ConvertToInt(string, int)"/> のオーバーフロー検証を実施します。
        /// </summary>
        [TestCategory("String:ConvertToInt")]
        [TestMethod]
        [ExpectedException(typeof(System.OverflowException))]
        public void ConvertToIntOverflowTest()
        {
            "2147483648".ConvertToInt();
        }

        #endregion

        #region -------------------- DateTime --------------------

        /// <summary>
        /// <see cref="StringExtensions.ConvertToDateTime(string, System.DateTime?)"/> の正常系検証を実施します。
        /// </summary>
        [TestCategory("String:ConvertToDateTime")]
        [TestMethod]
        public void ConvertToDateTimeSuccessTest()
        {
            Assert.AreEqual(new System.DateTime(2000, 1, 2, 12, 23, 34), "2000/01/02 12:23:34".ConvertToDateTime());
        }

        /// <summary>
        /// <see cref="StringExtensions.ConvertToDateTime(string, System.DateTime?)"/> の異常系検証を実施します。
        /// </summary>
        [TestCategory("String:ConvertToDateTime")]
        [TestMethod]
        public void ConvertToDateTimeFailTest()
        {
            Assert.AreEqual(System.DateTime.MinValue, "aaa".ConvertToDateTime());
        }

        /// <summary>
        /// <see cref="StringExtensions.ConvertToDateTime(string, System.DateTime?)"/> の初期値指定時の正常系検証を実施します。
        /// </summary>
        [TestCategory("String:ConvertToDateTime")]
        [TestMethod]
        public void ConvertToDateTimeAndDefSuccessTest()
        {
            Assert.AreEqual(new System.DateTime(2000, 1, 2, 12, 23, 34), "2000/01/02 12:23:34".ConvertToDateTime(new System.DateTime(1999,1,2,12,23,34)));
        }

        /// <summary>
        /// <see cref="StringExtensions.ConvertToDateTime(string, System.DateTime?)"/> の初期値指定時の異常系検証を実施します。
        /// </summary>
        [TestCategory("String:ConvertToDateTime")]
        [TestMethod]
        public void ConvertToDateTimeAndDefFailTest()
        {
            Assert.AreEqual(new System.DateTime(1999, 1, 2, 12, 23, 34), "aaa".ConvertToDateTime(new System.DateTime(1999, 1, 2, 12, 23, 34)));
        }

        /// <summary>
        /// <see cref="StringExtensions.ConvertToDateTime(string, System.DateTime?)"/> のオーバーフロー検証を実施します。
        /// </summary>
        [TestCategory("String:ConvertToDateTime")]
        [TestMethod]
        public void ConvertToDateTimeNonOverflowTest()
        {
            Assert.AreEqual(new System.DateTime(9999, 12, 31, 23, 59, 59), "9999/12/31 23:59:59".ConvertToDateTime());
        }

        /// <summary>
        /// <see cref="StringExtensions.ConvertToDateTime(string, System.DateTime?)"/> のオーバーフロー検証を実施します。
        /// </summary>
        [TestCategory("String:ConvertToDateTime")]
        [TestMethod]
        public void ConvertToDateTimeOverflowTest()
        {
            Assert.AreEqual(System.DateTime.MinValue, "10000/01/01 00:00:00".ConvertToDateTime());
        }

        #endregion

        #endregion

        #endregion
    }
}
