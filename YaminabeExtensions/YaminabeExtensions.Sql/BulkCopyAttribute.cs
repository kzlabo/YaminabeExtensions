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

namespace YaminabeExtensions.Sql
{
    /// <summary>
    /// <see cref="System.Data.SqlClient.SqlBulkCopy"/> における拡張属性を提供します。
    /// </summary>
    /// <seealso cref="System.Attribute" />
    /// <revisionHistory>
    ///     <revision date="2020/02/11" version="1.0.0.0" author="kzlabo">新規作成。</revision>
    ///     <revision date="2021/05/05" version="1.1.0.0" author="kzlabo">更新・削除処理の為の主キー判定を追加。</revision>
    /// </revisionHistory>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BulkCopyAttribute : Attribute
    {
        /// <summary>
        /// プロパティをコピーの除外対象とするかどうかを指定します。
        /// </summary>
        /// <value>
        ///   除外対象とする場合は <c>true</c>。 除外しない場合は <c>false</c> を指定します。
        /// </value>
        public bool Ignore { get; set; } = false;

        /// <summary>
        /// プロパティ名とカラム名が一致しない場合に、カラム名を指定します。
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// プロパティが主キーであるかどうかを指定します。
        /// </summary>
        /// <value>
        ///   主キーとする場合は <c>true</c>。 主キーではない場合は <c>false</c> を指定します。
        /// </value>
        public bool PrimaryKey { get; set; } = false;
    }
}
