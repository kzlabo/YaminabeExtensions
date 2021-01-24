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
using YaminabeExtensions.Core.Converters;

namespace YaminabeExtensions.Test.Tests.Core.Converters
{
    /// <summary>
    /// 固定長レコードモデルの変換処理の検証を提供します。
    /// </summary>
    [TestClass]
    public class FixedRecordConverterUnitTest
    {
        #region -------------------- test stub --------------------

        [FixedRecord(100, "Shift_JIS")]
        private class FixedSjisSampleRecord
        {
            [FixedItem(1, 10)]
            public string Item1 { get; set; }

            [FixedItem(2, 50)]
            public string Item2 { get; set; }

            [FixedItem(3, 5, FixedItemAttribute.PaddingOptions.Left)]
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

            [FixedItem(3, 5, FixedItemAttribute.PaddingOptions.Left)]
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

        [TestCategory("FixedRecordConverter:ToFixedString")]
        [TestMethod]
        public void ToFixedStringSuccessTest()
        {
            var source = "0123456789abcdeABCDEａｂｃｄｅＡＢＣＤＥあいうえお０１２３４   10                                   ";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Shift_JIS
            var shiftjisRecord = new FixedSjisSampleRecord();
            shiftjisRecord.Item1 = "0123456789";
            shiftjisRecord.Item2 = "abcdeABCDEａｂｃｄｅＡＢＣＤＥあいうえお０１２３４";
            shiftjisRecord.Item3 = "10";
            shiftjisRecord.Space = "";
            shiftjisRecord.Dummy = "dummy";

            var utf8Record = new FixedUtf8SampleRecord();
            utf8Record.Item1 = "0123456789";
            utf8Record.Item2 = "abcdeABCDEａｂｃｄｅＡＢＣＤＥあいうえお０１２３４";
            utf8Record.Item3 = "10";
            utf8Record.Space = "";
            utf8Record.Dummy = "dummy";

            Assert.AreEqual(source, FixedRecordConverter.ToFixedString(shiftjisRecord));
            Assert.AreEqual(source, FixedRecordConverter.ToFixedString(utf8Record));
        }

        [TestCategory("FixedRecordConverter:ToFixedString")]
        [TestMethod]
        public void ToFixedStringResponseTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var record = new FixedSjisSampleRecord();
            record.Item1 = "0123456789";
            record.Item2 = "abcdeABCDEａｂｃｄｅＡＢＣＤＥあいうえお０１２３４";
            record.Item3 = "10";
            record.Space = "";
            record.Dummy = "dummy";

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var i = 0; i < 100000; i++)
            {
                _ = FixedRecordConverter.ToFixedString(record);
            }
            stopWatch.Stop();
            if (stopWatch.ElapsedMilliseconds > 3000)
            {
                Assert.Fail("TimeOver");
            }
        }

        [TestCategory("FixedRecordConverter:ToFixedString")]
        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void ToFixedStringOverValueFaileTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var record = new FixedSjisSampleRecord();
            record.Item1 = "0123456789over";

            _ = FixedRecordConverter.ToFixedString(record);
        }

        [TestCategory("FixedRecordConverter:ToFixedString")]
        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void ToFixedStringFixedLengthUnderUnmatchFaileTest()
        {
            var record = new FixedLengthUnderUnmatchErrorSampleRecord();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            _ = FixedRecordConverter.ToFixedString(record);
        }

        [TestCategory("FixedRecordConverter:ToFixedString")]
        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void ToFixedStringFixedLengthOverUnmatchFaileTest()
        {
            var record = new FixedLengthOverUnmatchErrorSampleRecord();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            _ = FixedRecordConverter.ToFixedString(record);
        }

        #endregion

    }
}
