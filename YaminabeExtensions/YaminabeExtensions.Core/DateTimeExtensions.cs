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

using System;

namespace YaminabeExtensions.Core
{
    /// <summary>
    /// 日付操作拡張メソッドを提供します。
    /// </summary>
    /// <revisionHistory>
    ///     <revision date="2020/02/23" version="1.0.0.0" author="kzlabo">新規作成。</revision>
    /// </revisionHistory>
    public static class DateTimeExtensions
    {
        #region -------------------- method --------------------

        /// <summary>
        /// 現在のインスタンスの月初を取得します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <returns>
        /// 月初のインスタンスを返却します。
        /// </returns>
        public static DateTime StartDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        /// <summary>
        /// 現在のインスタンスの月末を取得します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <returns>
        /// 月末のインスタンスを返却します。
        /// </returns>
        public static DateTime EndDayOfMonth(this DateTime value)
        {
            return value.StartDayOfMonth().AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// 半期・四半期等の期番号を取得します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <param name="baseMonth">基準月。</param>
        /// <param name="quarterCount">期内の月数。（半期：6、四半期：3）</param>
        /// <returns>
        /// 指定した基準月での期番号を返却します。
        /// </returns>
        /// <example>
        ///     <code language="C#" numberLines="true">
        ///         // 4月を基準月とした四半期番号を取得する例
        ///         var date202004 = new System.DateTime(2020, 4, 10);
        ///         var date202103 = new System.DateTime(2021, 3, 10);
        ///         
        ///         var quarter202004 = date202004.NumberOfQuarter(4, 3);
        ///         var quarter202103 = date202103.NumberOfQuarter(4, 3);
        ///         
        ///         // quarter202004: 1
        ///         // quarter202103: 4
        ///     </code>
        /// </example>
        public static int NumberOfQuarter(this DateTime value, int baseMonth, int quarterCount)
        {
            return (((value.Month - baseMonth + (value.Month < baseMonth ? 13 : 1)) - 1) / quarterCount + 1);
        }

        /// <summary>
        /// 半期での期番号を取得します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <param name="baseMonth">基準月。</param>
        /// <returns>
        /// 指定した基準月での期番号を返却します。
        /// </returns>
        /// <example>
        ///     <code language="C#" numberLines="true">
        ///         // 4月を基準月とした半期番号を取得する例
        ///         var date202004 = new System.DateTime(2020, 4, 10);
        ///         var date202103 = new System.DateTime(2021, 3, 10);
        ///         
        ///         var quarter202004 = date202004.NumberOf2Quarter(4);
        ///         var quarter202103 = date202103.NumberOf2Quarter(4);
        ///         
        ///         // quarter202004: 1
        ///         // quarter202103: 2
        ///     </code>
        /// </example>
        /// <remarks>
        /// <see cref="DateTimeExtensions.NumberOfQuarter(DateTime, int, int)"/> のラップメソッドです。
        /// </remarks>
        public static int NumberOf2Quarter(this DateTime value, int baseMonth = 4)
        {
            return value.NumberOfQuarter(baseMonth, 6);
        }

        /// <summary>
        /// 四半期での期番号を取得します。
        /// </summary>
        /// <param name="value">インスタンス。</param>
        /// <param name="baseMonth">基準月。</param>
        /// <returns>
        /// 指定した基準月での期番号を返却します。
        /// </returns>
        /// <example>
        ///     <code language="C#" numberLines="true">
        ///         // 4月を基準月とした四半期番号を取得する例
        ///         var date202004 = new System.DateTime(2020, 4, 10);
        ///         var date202103 = new System.DateTime(2021, 3, 10);
        ///         
        ///         var quarter202004 = date202004.NumberOf4Quarter(4);
        ///         var quarter202103 = date202103.NumberOf4Quarter(4);
        ///         
        ///         // quarter202004: 1
        ///         // quarter202103: 4
        ///     </code>
        /// </example>
        /// <remarks>
        /// <see cref="DateTimeExtensions.NumberOfQuarter(DateTime, int, int)"/> のラップメソッドです。
        /// </remarks>
        public static int NumberOf4Quarter(this DateTime value, int baseMonth = 4)
        {
            return value.NumberOfQuarter(baseMonth, 3);
        }

        #endregion
    }
}
