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
using System.Text;

namespace YaminabeExtensions.Core.Attributes
{
    /// <summary>
    /// 固定長レコードモデル属性を提供します。
    /// </summary>
    /// <example>
    ///     固定長レコードモデル用クラス定義。
    ///     <code language="C#" numberLines="true">
    ///         [FixedRecord(100, "Shift_JIS")]
    ///         public class FixedRecord
    ///         {
    ///             [FixedItem(1, 10)]
    ///             public string Item1 { get; set; }
    ///             
    ///             [FixedItem(2, 50)]
    ///             public string Item2 { get; set; }
    ///             
    ///             [FixedItem(3, 5)]
    ///             public string Item3 { get; set; }
    ///             
    ///             [FixedItem(4, 35)]
    ///             public string Space { get; set; }
    ///             
    ///             public string Dummy { get; set; } = "dummy";
    ///         }
    ///     </code>
    ///     固定長レコードモデルから固定長文字列への変換例を下記に示します。
    ///     <code language="C#" numberLines="true">
    ///         var record = new FixedRecord();
    ///         record.Item1 = "0123456789";
    ///         record.Item2 = "abcdeABCDEａｂｃｄｅＡＢＣＤＥあいうえお０１２３４";
    ///         record.Item3 = "10";
    ///         record.Space = "";
    ///         record.Dummy = "dummy";
    ///         
    ///         // 変換結果
    ///         // 「0123456789abcdeABCDEａｂｃｄｅＡＢＣＤＥあいうえお０１２３４   10                                   」
    ///         _ = FixedRecordConverter.ToFixedString(record);
    ///     </code>
    ///     固定長文字列から固定長レコードモデルの生成例を下記に示します。。
    ///     <code language="C#" numberLines="true">
    ///         var source = "0123456789abcdeABCDEａｂｃｄｅＡＢＣＤＥあいうえお０１２３４   10                                   ";
    ///         
    ///         // 生成結果
    ///         // record.Item1 = "0123456789";
    ///         // record.Item2 = "abcdeABCDEａｂｃｄｅＡＢＣＤＥあいうえお０１２３４";
    ///         // record.Item3 = "   10";
    ///         // record.Space = "                                   ";
    ///         // record.Dummy = "dummy";
    ///         var record = FixedRecordFactory.CreateFromRecord&lt;FixedSjisSampleRecord&gt;(source);
    ///     </code>
    /// </example>
    /// <revisionHistory>
    ///     <revision date="2021/01/23" version="1.0.0.0" author="kzlabo">新規作成。</revision>
    /// </revisionHistory>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FixedRecordAttribute : Attribute
    {
        #region -------------------- field --------------------

        private string _encodingName;

        #endregion

        #region -------------------- property --------------------

        /// <summary>
        /// レコード長を取得します。
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// エンコードを取得します。
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                return Encoding.GetEncoding(this._encodingName);
            }
        }

        #endregion

        #region -------------------- constructor --------------------

        /// <summary>
        /// <see cref="FixedRecordAttribute"/> クラスの新しいインスタンスを作成します。
        /// </summary>
        /// <param name="length">レコード長。</param>
        /// <param name="encodingName">エンコード指定。</param>
        public FixedRecordAttribute(
            int length,
            string encodingName
            )
        {
            this.Length = length;
            this._encodingName = encodingName;
        }

        #endregion
    }
}
