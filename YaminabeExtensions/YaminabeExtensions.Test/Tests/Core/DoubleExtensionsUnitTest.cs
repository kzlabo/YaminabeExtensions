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
    public class DoubleExtensionsUnitTest
    {
        #region -------------------- test method --------------------

        #region -------------------- Round --------------------

        /// <summary>
        /// <see cref="DoubleExtensions.Round(double, int, System.MidpointRounding)"/> の正常系検証を実施します。
        /// </summary>
        [TestCategory("double:Round")]
        [TestMethod]
        public void RoundSccessTest()
        {
            Assert.AreEqual(0.2d, (0.15d).Round(1));
            Assert.AreEqual(0.2d, (0.25d).Round(1));
            Assert.AreEqual(0.4d, (0.35d).Round(1));
            Assert.AreEqual(0.4d, (0.45d).Round(1));
            Assert.AreEqual(0.2d, (0.15d).Round(1, System.MidpointRounding.AwayFromZero));
            Assert.AreEqual(0.3d, (0.25d).Round(1, System.MidpointRounding.AwayFromZero));
            Assert.AreEqual(0.4d, (0.35d).Round(1, System.MidpointRounding.AwayFromZero));
            Assert.AreEqual(0.5d, (0.45d).Round(1, System.MidpointRounding.AwayFromZero));
        }

        #endregion

        #endregion
    }
}
