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

namespace YaminabeExtensions.Core
{
    /// <summary>
    /// 浮動小数点操作拡張メソッドを提供します。
    /// </summary>
    /// <revisionHistory>
    ///     <revision date="2020/02/08" version="1.0.0.0" author="kzlabo">新規作成。</revision>
    /// </revisionHistory>
    public static class DecimalExtensions
    {
        #region -------------------- method --------------------

        #region -------------------- Round --------------------

        /// <summary>
        /// <see cref="System.Math.Round(decimal, int, System.MidpointRounding)"/> のラップメソッドを提供します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <param name="digits">小数部桁数。</param>
        /// <param name="mode">丸めの方法。</param>
        /// <returns>
        /// 丸めた結果を返却します。
        /// </returns>
        public static decimal Round(this decimal value, int digits = 0, System.MidpointRounding mode = System.MidpointRounding.ToEven)
        {
            return System.Math.Round(value, digits, mode);
        }

        #endregion

        #endregion
    }
}
