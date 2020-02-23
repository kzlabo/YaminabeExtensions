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
    /// 日付操作拡張メソッドの検証を提供します。
    /// </summary>
    [TestClass]
    public class DateTimeExtensionsUnitTest
    {
        #region -------------------- test method --------------------

        /// <summary>
        /// <see cref="DateTimeExtensions.StartDayOfMonth(System.DateTime)"/> の正常系検証を実施します。
        /// </summary>
        [TestCategory("DateTime:StartDayOfMonth")]
        [TestMethod]
        public void StartDayOfMonthSuccessTest()
        {
            var month202001 = new System.DateTime(2020, 1, 31);
            var month202002 = new System.DateTime(2020, 2, 29);
            var month202003 = new System.DateTime(2020, 3, 31);
            var month202004 = new System.DateTime(2020, 4, 30);
            var month202005 = new System.DateTime(2020, 5, 31);
            var month202006 = new System.DateTime(2020, 6, 30);
            var month202007 = new System.DateTime(2020, 7, 31);
            var month202008 = new System.DateTime(2020, 8, 31);
            var month202009 = new System.DateTime(2020, 9, 30);
            var month202010 = new System.DateTime(2020, 10, 31);
            var month202011 = new System.DateTime(2020, 11, 30);
            var month202012 = new System.DateTime(2020, 12, 31);
            var month202101 = new System.DateTime(2021, 1, 31);
            var month202102 = new System.DateTime(2021, 2, 28);
            var month202103 = new System.DateTime(2021, 3, 31);
            var month202104 = new System.DateTime(2021, 4, 30);
            var month202105 = new System.DateTime(2021, 5, 31);
            var month202106 = new System.DateTime(2021, 6, 30);
            var month202107 = new System.DateTime(2021, 7, 31);
            var month202108 = new System.DateTime(2021, 8, 31);
            var month202109 = new System.DateTime(2021, 9, 30);
            var month202110 = new System.DateTime(2021, 10, 31);
            var month202111 = new System.DateTime(2021, 11, 30);
            var month202112 = new System.DateTime(2021, 12, 31);

            Assert.AreEqual(new System.DateTime(2020, 1, 1), month202001.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 2, 1), month202002.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 3, 1), month202003.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 4, 1), month202004.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 5, 1), month202005.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 6, 1), month202006.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 7, 1), month202007.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 8, 1), month202008.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 9, 1), month202009.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 10, 1), month202010.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 11, 1), month202011.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 12, 1), month202012.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 1, 1), month202101.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 2, 1), month202102.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 3, 1), month202103.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 4, 1), month202104.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 5, 1), month202105.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 6, 1), month202106.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 7, 1), month202107.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 8, 1), month202108.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 9, 1), month202109.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 10, 1), month202110.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 11, 1), month202111.StartDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 12, 1), month202112.StartDayOfMonth());
        }

        /// <summary>
        /// <see cref="DateTimeExtensions.EndDayOfMonth(System.DateTime)"/> の正常系検証を実施します。
        /// </summary>
        [TestCategory("DateTime:EndDayOfMonth")]
        [TestMethod]
        public void EndDayOfMonthSuccessTest()
        {
            var month202001 = new System.DateTime(2020, 1, 10);
            var month202002 = new System.DateTime(2020, 2, 10);
            var month202003 = new System.DateTime(2020, 3, 10);
            var month202004 = new System.DateTime(2020, 4, 10);
            var month202005 = new System.DateTime(2020, 5, 10);
            var month202006 = new System.DateTime(2020, 6, 10);
            var month202007 = new System.DateTime(2020, 7, 10);
            var month202008 = new System.DateTime(2020, 8, 10);
            var month202009 = new System.DateTime(2020, 9, 10);
            var month202010 = new System.DateTime(2020, 10, 10);
            var month202011 = new System.DateTime(2020, 11, 10);
            var month202012 = new System.DateTime(2020, 12, 10);
            var month202101 = new System.DateTime(2021, 1, 10);
            var month202102 = new System.DateTime(2021, 2, 10);
            var month202103 = new System.DateTime(2021, 3, 10);
            var month202104 = new System.DateTime(2021, 4, 10);
            var month202105 = new System.DateTime(2021, 5, 10);
            var month202106 = new System.DateTime(2021, 6, 10);
            var month202107 = new System.DateTime(2021, 7, 10);
            var month202108 = new System.DateTime(2021, 8, 10);
            var month202109 = new System.DateTime(2021, 9, 10);
            var month202110 = new System.DateTime(2021, 10, 10);
            var month202111 = new System.DateTime(2021, 11, 10);
            var month202112 = new System.DateTime(2021, 12, 10);

            Assert.AreEqual(new System.DateTime(2020, 1, 31), month202001.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 2, 29), month202002.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 3, 31), month202003.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 4, 30), month202004.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 5, 31), month202005.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 6, 30), month202006.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 7, 31), month202007.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 8, 31), month202008.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 9, 30), month202009.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 10, 31), month202010.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 11, 30), month202011.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2020, 12, 31), month202012.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 1, 31), month202101.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 2, 28), month202102.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 3, 31), month202103.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 4, 30), month202104.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 5, 31), month202105.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 6, 30), month202106.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 7, 31), month202107.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 8, 31), month202108.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 9, 30), month202109.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 10, 31), month202110.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 11, 30), month202111.EndDayOfMonth());
            Assert.AreEqual(new System.DateTime(2021, 12, 31), month202112.EndDayOfMonth());
        }

        /// <summary>
        /// <see cref="DateTimeExtensions.NumberOfQuarter(System.DateTime, int, int)"/> の正常系検証を実施します。
        /// </summary>
        [TestCategory("DateTime:NumberOfQuarter")]
        [TestMethod]
        public void NumberOfQuarterSuccessTest()
        {
            var month202001 = new System.DateTime(2020, 1, 10);
            var month202002 = new System.DateTime(2020, 2, 10);
            var month202003 = new System.DateTime(2020, 3, 10);
            var month202004 = new System.DateTime(2020, 4, 10);
            var month202005 = new System.DateTime(2020, 5, 10);
            var month202006 = new System.DateTime(2020, 6, 10);
            var month202007 = new System.DateTime(2020, 7, 10);
            var month202008 = new System.DateTime(2020, 8, 10);
            var month202009 = new System.DateTime(2020, 9, 10);
            var month202010 = new System.DateTime(2020, 10, 10);
            var month202011 = new System.DateTime(2020, 11, 10);
            var month202012 = new System.DateTime(2020, 12, 10);

            // -------------------- 半期計算 --------------------
            // 1月基準
            Assert.AreEqual(1, month202001.NumberOfQuarter(1, 6));
            Assert.AreEqual(1, month202002.NumberOfQuarter(1, 6));
            Assert.AreEqual(1, month202003.NumberOfQuarter(1, 6));
            Assert.AreEqual(1, month202004.NumberOfQuarter(1, 6));
            Assert.AreEqual(1, month202005.NumberOfQuarter(1, 6));
            Assert.AreEqual(1, month202006.NumberOfQuarter(1, 6));
            Assert.AreEqual(2, month202007.NumberOfQuarter(1, 6));
            Assert.AreEqual(2, month202008.NumberOfQuarter(1, 6));
            Assert.AreEqual(2, month202009.NumberOfQuarter(1, 6));
            Assert.AreEqual(2, month202010.NumberOfQuarter(1, 6));
            Assert.AreEqual(2, month202011.NumberOfQuarter(1, 6));
            Assert.AreEqual(2, month202012.NumberOfQuarter(1, 6));

            // 4月基準
            Assert.AreEqual(2, month202001.NumberOfQuarter(4, 6));
            Assert.AreEqual(2, month202002.NumberOfQuarter(4, 6));
            Assert.AreEqual(2, month202003.NumberOfQuarter(4, 6));
            Assert.AreEqual(1, month202004.NumberOfQuarter(4, 6));
            Assert.AreEqual(1, month202005.NumberOfQuarter(4, 6));
            Assert.AreEqual(1, month202006.NumberOfQuarter(4, 6));
            Assert.AreEqual(1, month202007.NumberOfQuarter(4, 6));
            Assert.AreEqual(1, month202008.NumberOfQuarter(4, 6));
            Assert.AreEqual(1, month202009.NumberOfQuarter(4, 6));
            Assert.AreEqual(2, month202010.NumberOfQuarter(4, 6));
            Assert.AreEqual(2, month202011.NumberOfQuarter(4, 6));
            Assert.AreEqual(2, month202012.NumberOfQuarter(4, 6));

            // -------------------- 四半期計算 --------------------
            // 1月基準
            Assert.AreEqual(1, month202001.NumberOfQuarter(1, 3));
            Assert.AreEqual(1, month202002.NumberOfQuarter(1, 3));
            Assert.AreEqual(1, month202003.NumberOfQuarter(1, 3));
            Assert.AreEqual(2, month202004.NumberOfQuarter(1, 3));
            Assert.AreEqual(2, month202005.NumberOfQuarter(1, 3));
            Assert.AreEqual(2, month202006.NumberOfQuarter(1, 3));
            Assert.AreEqual(3, month202007.NumberOfQuarter(1, 3));
            Assert.AreEqual(3, month202008.NumberOfQuarter(1, 3));
            Assert.AreEqual(3, month202009.NumberOfQuarter(1, 3));
            Assert.AreEqual(4, month202010.NumberOfQuarter(1, 3));
            Assert.AreEqual(4, month202011.NumberOfQuarter(1, 3));
            Assert.AreEqual(4, month202012.NumberOfQuarter(1, 3));

            // 4月基準
            Assert.AreEqual(4, month202001.NumberOfQuarter(4, 3));
            Assert.AreEqual(4, month202002.NumberOfQuarter(4, 3));
            Assert.AreEqual(4, month202003.NumberOfQuarter(4, 3));
            Assert.AreEqual(1, month202004.NumberOfQuarter(4, 3));
            Assert.AreEqual(1, month202005.NumberOfQuarter(4, 3));
            Assert.AreEqual(1, month202006.NumberOfQuarter(4, 3));
            Assert.AreEqual(2, month202007.NumberOfQuarter(4, 3));
            Assert.AreEqual(2, month202008.NumberOfQuarter(4, 3));
            Assert.AreEqual(2, month202009.NumberOfQuarter(4, 3));
            Assert.AreEqual(3, month202010.NumberOfQuarter(4, 3));
            Assert.AreEqual(3, month202011.NumberOfQuarter(4, 3));
            Assert.AreEqual(3, month202012.NumberOfQuarter(4, 3));
        }

        /// <summary>
        /// <see cref="DateTimeExtensions.NumberOf2Quarter(System.DateTime, int)"/> の正常系検証を実施します。
        /// </summary>
        [TestCategory("DateTime:NumberOf2Quarter")]
        [TestMethod]
        public void NumberOf2QuarterSuccessTest()
        {
            var month202001 = new System.DateTime(2020, 1, 10);
            var month202002 = new System.DateTime(2020, 2, 10);
            var month202003 = new System.DateTime(2020, 3, 10);
            var month202004 = new System.DateTime(2020, 4, 10);
            var month202005 = new System.DateTime(2020, 5, 10);
            var month202006 = new System.DateTime(2020, 6, 10);
            var month202007 = new System.DateTime(2020, 7, 10);
            var month202008 = new System.DateTime(2020, 8, 10);
            var month202009 = new System.DateTime(2020, 9, 10);
            var month202010 = new System.DateTime(2020, 10, 10);
            var month202011 = new System.DateTime(2020, 11, 10);
            var month202012 = new System.DateTime(2020, 12, 10);

            // 指定なし
            Assert.AreEqual(2, month202001.NumberOf2Quarter());
            Assert.AreEqual(2, month202002.NumberOf2Quarter());
            Assert.AreEqual(2, month202003.NumberOf2Quarter());
            Assert.AreEqual(1, month202004.NumberOf2Quarter());
            Assert.AreEqual(1, month202005.NumberOf2Quarter());
            Assert.AreEqual(1, month202006.NumberOf2Quarter());
            Assert.AreEqual(1, month202007.NumberOf2Quarter());
            Assert.AreEqual(1, month202008.NumberOf2Quarter());
            Assert.AreEqual(1, month202009.NumberOf2Quarter());
            Assert.AreEqual(2, month202010.NumberOf2Quarter());
            Assert.AreEqual(2, month202011.NumberOf2Quarter());
            Assert.AreEqual(2, month202012.NumberOf2Quarter());

            // 1月基準
            Assert.AreEqual(1, month202001.NumberOf2Quarter(1));
            Assert.AreEqual(1, month202002.NumberOf2Quarter(1));
            Assert.AreEqual(1, month202003.NumberOf2Quarter(1));
            Assert.AreEqual(1, month202004.NumberOf2Quarter(1));
            Assert.AreEqual(1, month202005.NumberOf2Quarter(1));
            Assert.AreEqual(1, month202006.NumberOf2Quarter(1));
            Assert.AreEqual(2, month202007.NumberOf2Quarter(1));
            Assert.AreEqual(2, month202008.NumberOf2Quarter(1));
            Assert.AreEqual(2, month202009.NumberOf2Quarter(1));
            Assert.AreEqual(2, month202010.NumberOf2Quarter(1));
            Assert.AreEqual(2, month202011.NumberOf2Quarter(1));
            Assert.AreEqual(2, month202012.NumberOf2Quarter(1));
        }

        /// <summary>
        /// <see cref="DateTimeExtensions.NumberOf4Quarter(System.DateTime, int)"/> の正常系検証を実施します。
        /// </summary>
        [TestCategory("DateTime:NumberOf4Quarter")]
        [TestMethod]
        public void NumberOf4QuarterSuccessTest()
        {
            var month202001 = new System.DateTime(2020, 1, 10);
            var month202002 = new System.DateTime(2020, 2, 10);
            var month202003 = new System.DateTime(2020, 3, 10);
            var month202004 = new System.DateTime(2020, 4, 10);
            var month202005 = new System.DateTime(2020, 5, 10);
            var month202006 = new System.DateTime(2020, 6, 10);
            var month202007 = new System.DateTime(2020, 7, 10);
            var month202008 = new System.DateTime(2020, 8, 10);
            var month202009 = new System.DateTime(2020, 9, 10);
            var month202010 = new System.DateTime(2020, 10, 10);
            var month202011 = new System.DateTime(2020, 11, 10);
            var month202012 = new System.DateTime(2020, 12, 10);

            // 指定なし
            Assert.AreEqual(4, month202001.NumberOf4Quarter());
            Assert.AreEqual(4, month202002.NumberOf4Quarter());
            Assert.AreEqual(4, month202003.NumberOf4Quarter());
            Assert.AreEqual(1, month202004.NumberOf4Quarter());
            Assert.AreEqual(1, month202005.NumberOf4Quarter());
            Assert.AreEqual(1, month202006.NumberOf4Quarter());
            Assert.AreEqual(2, month202007.NumberOf4Quarter());
            Assert.AreEqual(2, month202008.NumberOf4Quarter());
            Assert.AreEqual(2, month202009.NumberOf4Quarter());
            Assert.AreEqual(3, month202010.NumberOf4Quarter());
            Assert.AreEqual(3, month202011.NumberOf4Quarter());
            Assert.AreEqual(3, month202012.NumberOf4Quarter());

            // 1月基準
            Assert.AreEqual(1, month202001.NumberOf4Quarter(1));
            Assert.AreEqual(1, month202002.NumberOf4Quarter(1));
            Assert.AreEqual(1, month202003.NumberOf4Quarter(1));
            Assert.AreEqual(2, month202004.NumberOf4Quarter(1));
            Assert.AreEqual(2, month202005.NumberOf4Quarter(1));
            Assert.AreEqual(2, month202006.NumberOf4Quarter(1));
            Assert.AreEqual(3, month202007.NumberOf4Quarter(1));
            Assert.AreEqual(3, month202008.NumberOf4Quarter(1));
            Assert.AreEqual(3, month202009.NumberOf4Quarter(1));
            Assert.AreEqual(4, month202010.NumberOf4Quarter(1));
            Assert.AreEqual(4, month202011.NumberOf4Quarter(1));
            Assert.AreEqual(4, month202012.NumberOf4Quarter(1));
        }

        #endregion
    }
}
