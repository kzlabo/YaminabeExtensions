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
using System;
using System.Diagnostics;
using System.Text;
using YaminabeExtensions.Core.Attributes;
using YaminabeExtensions.Core.Factories;

namespace YaminabeExtensions.Test.Tests.Core.Factories
{
    /// <summary>
    /// 固定長レコードモデルの生成処理の検証を提供します。
    /// </summary>
    [TestClass]
    public class FixedRecordFactoryUnitTest
    {
        #region -------------------- test stub --------------------

        [FixedRecord(100, "Shift_JIS")]
        private class FixedSjisSampleRecord
        {
            [FixedItem(1, 10)]
            public string Item1 { get; set; }

            [FixedItem(2, 50)]
            public string Item2 { get; set; }

            [FixedItem(3, 5)]
            public string Item3 { get; set; }

            [FixedItem(4, 35)]
            public string Space { get; set; }

            public string Dummy { get; set; } = "dummy";
        }

        [FixedRecord(120, "utf-8")]
        private class FixedUtf8SampleRecord
        {
            [FixedItem(1, 10)]
            public string Item1 { get; set; }

            [FixedItem(2, 70)]
            public string Item2 { get; set; }

            [FixedItem(3, 5)]
            public string Item3 { get; set; }

            [FixedItem(4, 35)]
            public string Space { get; set; }

            public string Dummy { get; set; } = "dummy";
        }

        [FixedRecord(100, "Shift_JIS")]
        private class FixedLengthUnderUnmatchErrorSampleRecord
        {
            [FixedItem(1, 10)]
            public string Item1 { get; set; }

            [FixedItem(2, 20)]
            public string Item2 { get; set; }

            [FixedItem(3, 69)]
            public string Item3 { get; set; }
        }

        [FixedRecord(100, "Shift_JIS")]
        private class FixedLengthOverUnmatchErrorSampleRecord
        {
            [FixedItem(1, 10)]
            public string Item1 { get; set; }

            [FixedItem(2, 20)]
            public string Item2 { get; set; }

            [FixedItem(3, 71)]
            public string Item3 { get; set; }
        }

        #endregion

        #region -------------------- test method --------------------

        [TestCategory("FixedRecordFactory:CreateFromRecord")]
        [TestMethod]
        public void CreateFromRecordSuccessTest()
        {
            var source = "0123456789abcdeABCDEａｂｃｄｅＡＢＣＤＥあいうえお０１２３４   10                                   ";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Shift_JIS
            var shiftjisRecord1 = FixedRecordFactory.CreateFromRecord<FixedSjisSampleRecord>(Encoding.GetEncoding("Shift_JIS").GetBytes(source));
            Assert.AreEqual("0123456789", shiftjisRecord1.Item1);
            Assert.AreEqual("abcdeABCDEａｂｃｄｅＡＢＣＤＥあいうえお０１２３４", shiftjisRecord1.Item2);
            Assert.AreEqual("   10", shiftjisRecord1.Item3);
            Assert.AreEqual("                                   ", shiftjisRecord1.Space);
            Assert.AreEqual("dummy", shiftjisRecord1.Dummy);

            var shiftjisRecord2 = FixedRecordFactory.CreateFromRecord<FixedSjisSampleRecord>(source);
            Assert.AreEqual("0123456789", shiftjisRecord2.Item1);
            Assert.AreEqual("abcdeABCDEａｂｃｄｅＡＢＣＤＥあいうえお０１２３４", shiftjisRecord2.Item2);
            Assert.AreEqual("   10", shiftjisRecord2.Item3);
            Assert.AreEqual("                                   ", shiftjisRecord2.Space);
            Assert.AreEqual("dummy", shiftjisRecord2.Dummy);

            // UTF-8
            var utf8Record2 = FixedRecordFactory.CreateFromRecord<FixedUtf8SampleRecord>(Encoding.GetEncoding("utf-8").GetBytes(source));
            Assert.AreEqual("0123456789", utf8Record2.Item1);
            Assert.AreEqual("abcdeABCDEａｂｃｄｅＡＢＣＤＥあいうえお０１２３４", utf8Record2.Item2);
            Assert.AreEqual("   10", utf8Record2.Item3);
            Assert.AreEqual("                                   ", utf8Record2.Space);
            Assert.AreEqual("dummy", utf8Record2.Dummy);

            var utf8Record1 = FixedRecordFactory.CreateFromRecord<FixedUtf8SampleRecord>(source);
            Assert.AreEqual("0123456789", utf8Record1.Item1);
            Assert.AreEqual("abcdeABCDEａｂｃｄｅＡＢＣＤＥあいうえお０１２３４", utf8Record1.Item2);
            Assert.AreEqual("   10", utf8Record1.Item3);
            Assert.AreEqual("                                   ", utf8Record1.Space);
            Assert.AreEqual("dummy", utf8Record1.Dummy);
        }

        [TestCategory("FixedRecordFactory:CreateFromRecord")]
        [TestMethod]
        public void CreateFromRecordResponseTest()
        {
            var source = "0123456789abcdeABCDEａｂｃｄｅＡＢＣＤＥあいうえお０１２３４   10                                   ";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var i = 0; i < 100000; i++)
            {
                _ = FixedRecordFactory.CreateFromRecord<FixedSjisSampleRecord>(source);
            }
            stopWatch.Stop();
            if (stopWatch.ElapsedMilliseconds > 3000)
            {
                Assert.Fail("TimeOver");
            }
        }

        [TestCategory("FixedRecordFactory:CreateFromRecord")]
        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void CreateFromRecordFixedLengthUnderUnmatchFaileTest()
        {
            var source = "0123456789abcdeABCDEａｂｃｄｅＡＢＣＤＥあいうえお０１２３４   10                                   ";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            _ = FixedRecordFactory.CreateFromRecord<FixedLengthUnderUnmatchErrorSampleRecord>(source);
        }

        [TestCategory("FixedRecordFactory:CreateFromRecord")]
        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void CreateFromRecordFixedLengthOverUnmatchFaileTest()
        {
            var source = "0123456789abcdeABCDEａｂｃｄｅＡＢＣＤＥあいうえお０１２３４   10                                   ";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            _ = FixedRecordFactory.CreateFromRecord<FixedLengthOverUnmatchErrorSampleRecord>(source);
        }

        #endregion
    }
}
