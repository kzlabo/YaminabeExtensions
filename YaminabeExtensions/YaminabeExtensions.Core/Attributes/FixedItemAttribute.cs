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
using System.Runtime.CompilerServices;

namespace YaminabeExtensions.Core.Attributes
{
    /// <summary>
    /// 固定長項目モデル属性を提供します。
    /// </summary>
    /// <revisionHistory>
    ///     <revision date="2021/01/23" version="1.0.0.0" author="kzlabo">新規作成。</revision>
    /// </revisionHistory>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FixedItemAttribute : Attribute
    {
        #region -------------------- enum --------------------

        /// <summary>
        /// 余項目長埋めの方向を表します。
        /// </summary>
        public enum PaddingOptions
        {
            /// <summary>
            /// 左埋め。
            /// </summary>
            Left = 0,
            /// <summary>
            /// 右埋め。
            /// </summary>
            Right = 1
        }

        #endregion

        #region -------------------- property --------------------

        /// <summary>
        /// 項目番号を取得します。
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// 項目長を取得します。
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// 余項目長埋めの方向を取得します。
        /// </summary>
        public PaddingOptions PaddingOption { get; }

        /// <summary>
        /// 余項目長を埋める文字を取得します。
        /// </summary>
        public char Padding { get; }

        /// <summary>
        /// プロパティ名を取得します。
        /// </summary>
        public string PropertyName { get; }

        #endregion

        #region -------------------- constructor --------------------

        /// <summary>
        /// <see cref="FixedItemAttribute"/>
        /// </summary>
        /// <param name="order">項目番号。</param>
        /// <param name="length">項目長。</param>
        /// <param name="paddingOption">余項目長埋めの方向。</param>
        /// <param name="padding">余項目長文字。</param>
        /// <param name="propertyName">プロパティ名。</param>
        public FixedItemAttribute(
            int order,
            int length,
            PaddingOptions paddingOption = PaddingOptions.Right, 
            char padding = ' ',
            [CallerMemberName] string propertyName = ""
            )
        {
            this.Order = order;
            this.Length = length;
            this.PaddingOption = paddingOption;
            this.Padding = padding;
            this.PropertyName = propertyName;
        }

        #endregion
    }
}
