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
    /// 浮動小数点操作拡張メソッドを検証します。
    /// </summary>
    [TestClass]
    public class DecimalExtensionsUnitTest
    {
        #region -------------------- test method --------------------

        #region -------------------- Round --------------------

        /// <summary>
        /// <see cref="DecimalExtensions.Round(decimal, int, System.MidpointRounding)"/> の正常系検証を実施します。
        /// </summary>
        [TestCategory("decimal:Round")]
        [TestMethod]
        public void RoundSccessTest()
        {
            Assert.AreEqual(0.2m, (0.15m).Round(1));
            Assert.AreEqual(0.2m, (0.25m).Round(1));
            Assert.AreEqual(0.4m, (0.35m).Round(1));
            Assert.AreEqual(0.4m, (0.45m).Round(1));
            Assert.AreEqual(0.2m, (0.15m).Round(1, System.MidpointRounding.AwayFromZero));
            Assert.AreEqual(0.3m, (0.25m).Round(1, System.MidpointRounding.AwayFromZero));
            Assert.AreEqual(0.4m, (0.35m).Round(1, System.MidpointRounding.AwayFromZero));
            Assert.AreEqual(0.5m, (0.45m).Round(1, System.MidpointRounding.AwayFromZero));
        }

        #endregion

        #endregion
    }
}
