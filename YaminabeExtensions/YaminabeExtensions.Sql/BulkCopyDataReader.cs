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
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace YaminabeExtensions.Sql
{
    /// <summary>
    /// <see cref="SqlBulkCopy.WriteToServer(IDataReader)"/> におけるリードモデルを提供します。
    /// </summary>
    /// <typeparam name="TModel">モデルの型。</typeparam>
    /// <seealso cref="System.Data.IDataReader" />
    /// <revisionHistory>
    ///     <revision date="2020/06/11" version="1.0.0.0" author="kzlabo">新規作成。</revision>
    ///     <revision date="2021/05/05" version="1.1.0.0" author="kzlabo">更新・削除処理の為のプロパティ・メソッドを追加。</revision>
    /// </revisionHistory>
    public class BulkCopyDataReader<TModel> : IDataReader
    {
        #region -------------------- property --------------------

        /// <summary>
        /// モデルリストを取得します。
        /// </summary>
        private IEnumerator<TModel> Models { get; }

        /// <summary>
        /// プロパティリストを取得します。
        /// </summary>
        private List<PropertyInfo> Properties { get; }

        /// <summary>
        /// 主キープロパティリストを取得します。
        /// </summary>
        private List<PropertyInfo> PrimaryKeyProperties { get; }

        /// <summary>
        /// 項目数を取得します。
        /// </summary>
        public int FieldCount
        {
            get
            {
                return this.Properties.Count;
            }
        }

        /// <summary>
        /// 入れ子の深さを取得します。
        /// </summary>
        public int Depth
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// データリーダが閉じているかを判定します。
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return this.Models == null;
            }
        }

        /// <summary>
        /// 指定した要素番号の値を取得します。
        /// </summary>
        /// <param name="i">要素番号。</param>
        /// <returns>
        /// 指定した要素番号の値を返却します。
        /// </returns>
        public object this[int i]
        {
            get
            {
                return GetValue(i);
            }
        }

        /// <summary>
        /// 指定した宛先項目名の値を取得します。
        /// </summary>
        /// <param name="name">項目名。</param>
        /// <returns>
        /// 指定した宛先項目名の値を返却します。
        /// </returns>
        public object this[string name]
        {
            get
            {
                return GetValueByName(name);
            }
        }

        #endregion

        #region -------------------- constructor --------------------

        /// <summary>
        /// <see cref="BulkCopyDataReader{TModel}"/> クラスの新しいインスタンスを作成します。
        /// </summary>
        /// <param name="models">モデルリスト。</param>
        public BulkCopyDataReader(IEnumerable<TModel> models)
        {
            this.Models = models.GetEnumerator();
            this.Properties = new List<PropertyInfo>();
            this.PrimaryKeyProperties = new List<PropertyInfo>();
            // 一括登録対象プロパティ取得
            foreach (var property in typeof(TModel).GetProperties().Where(property => IsTarget(property)))
            {
                this.Properties.Add(property);
            }
            // 主キー対象プロパティ取得
            foreach (var property in typeof(TModel).GetProperties().Where(property => IsPrimary(property)))
            {
                this.PrimaryKeyProperties.Add(property);
            }
        }

        #endregion

        #region -------------------- method --------------------

        /// <summary>
        /// 一括登録対象プロパティ判定メソッド。
        /// </summary>
        /// <param name="property">プロパティ。</param>
        /// <returns>
        /// 一括登録対象プロパティの場合は <c>true</c> 。一括登録対象プロパティではない場合は <c>false</c> 。
        /// </returns>
        private bool IsTarget(PropertyInfo property)
        {
            var attribute = Attribute.GetCustomAttribute(property, typeof(BulkCopyAttribute)) as BulkCopyAttribute;
            // 属性指定されていない場合は対象
            if (attribute == null)
            {
                return true;
            }
            // 除外指定されている場合は対象としない
            if (attribute.Ignore == true)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 主キープロパティ判定メソッド。
        /// </summary>
        /// <param name="property">プロパティ。</param>
        /// <returns>
        /// 主キー対象プロパティの場合は <c>true</c> 。 主キー対象プロパティではない場合は <c>false</c> 。
        /// </returns>
        private bool IsPrimary(PropertyInfo property)
        {
            var attribute = Attribute.GetCustomAttribute(property, typeof(BulkCopyAttribute)) as BulkCopyAttribute;
            // 属性指定されていない場合
            // かつ
            // プロパティ名の末尾が主キー対象名ではない場合
            // は主キーと判定しない
            if(attribute == null)
            {
                if (property.Name.EndsWith("_ExPK"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // 除外指定されている場合は主キーと判定しない
            if (attribute.Ignore == true)
            {
                return false;
            }
            return attribute.PrimaryKey;
        }

        /// <summary>
        /// 一括登録対象プロパティの宛先項目名を取得します。
        /// </summary>
        /// <param name="property">プロパティ。</param>
        /// <returns>
        /// 宛先項目名を返却します。
        /// </returns>
        private string GetColumnName(PropertyInfo property)
        {
            // 属性指定されていない場合はプロパティ名（主キー判別用のマーカー名部を除く）
            //   例）Id_ExPK ⇒ ID として取得
            // カラム名指定がされている場合は指定名
            var attribute = Attribute.GetCustomAttribute(property, typeof(BulkCopyAttribute)) as BulkCopyAttribute;
            return attribute?.ColumnName ?? property.Name.Replace("_ExPK", string.Empty);
        }

        /// <summary>
        /// アイテムを読み込みます。
        /// </summary>
        /// <returns>
        /// アイテムが存在する場合は <c>true</c> 。アイテムが存在しない場合は <c>false</c> 。
        /// </returns>
        public bool Read()
        {
            return this.Models.MoveNext();
        }

        /// <summary>
        /// リードモデルを閉じます。
        /// </summary>
        public void Close()
        {
            this.Models.Dispose();
        }

        /// <summary>
        /// リードモデルを解放します。
        /// </summary>
        public void Dispose()
        {
            this.Models.Dispose();
        }

        /// <summary>
        /// 指定した要素番号の値を取得します。
        /// </summary>
        /// <param name="i">要素番号。</param>
        /// <returns>
        /// 指定した要素番号の値を返却します。
        /// </returns>
        public object GetValue(int i)
        {
            return this.Properties[i].GetValue(this.Models.Current);
        }

        /// <summary>
        /// 指定した項目名の値を取得します。
        /// </summary>
        /// <param name="name">宛先項目名。</param>
        /// <returns>
        /// 指定した項目名の値を返却します。
        /// </returns>
        public object GetValueByName(string name)
        {
            var targetProperty = this.Properties.FirstOrDefault(property => property.Name.Equals(name));
            if (targetProperty == null)
            {
                throw new NotImplementedException($"{name} は未定義の項目です。");
            }
            return targetProperty.GetValue(this.Models.Current);
        }

        /// <summary>
        /// 指定した要素番号の項目名を取得します。
        /// </summary>
        /// <param name="i">要素番号。</param>
        /// <returns>
        /// 指定した要素番号の項目名を返却します。
        /// </returns>
        public string GetName(int i)
        {
            return this.Properties[i].Name;
        }

        /// <summary>
        /// 指定した項目名の序数を取得します。
        /// </summary>
        /// <param name="name">項目名。</param>
        /// <returns>
        /// 指定した項目名の序数を返却します。
        /// </returns>
        public int GetOrdinal(string name)
        {
            for (var i = 0; i < this.Properties.Count; i++)
            {
                if (this.Properties[i].Name.Equals(name))
                {
                    return i;
                }
            }
            throw new NotImplementedException($"{name} は未定義の項目です。");
        }

        /// <summary>
        /// モデルのプロパティと宛先項目のマッピングを行います。
        /// </summary>
        /// <param name="columnMappings">マッピングコレクション。</param>
        public void SetColumnMappings(SqlBulkCopyColumnMappingCollection columnMappings)
        {
            foreach (var property in this.Properties)
            {
                columnMappings.Add(property.Name, GetColumnName(property));
            }
        }

        /// <summary>
        /// モデルの対象プロパティを取得します。
        /// </summary>
        /// <returns>
        /// 対象プロパティを順次返却します。
        /// </returns>
        public IEnumerable<string> GetColumns()
        {
            foreach (var property in this.Properties)
            {
                yield return GetColumnName(property);
            }
        }

        /// <summary>
        /// モデルの主キー対象プロパティを取得します。
        /// </summary>
        /// <returns>
        /// 主キー対象プロパティを順次返却します。
        /// </returns>
        /// <remarks>
        /// 主キー対象プロパティが設定されていない場合はすべてのプロパティをキーとして取得します。
        /// </remarks>
        public IEnumerable<string> GetPrimaryColumns()
        {
            foreach (var property in this.PrimaryKeyProperties.Count > 0 ? this.PrimaryKeyProperties : this.Properties)
            {
                yield return GetColumnName(property);
            }
        }

        /// <summary>
        /// モデルの更新対象プロパティを取得します。
        /// </summary>
        /// <returns>
        /// 主キー対象プロパティを除いたプロパティを順次返却します。
        /// </returns>
        public IEnumerable<string> GetUpdateColumns()
        {
            foreach (var property in this.Properties)
            {
                // 主キーに存在するプロパティは対象外
                if (this.PrimaryKeyProperties.Contains(property))
                {
                    continue;
                }
                yield return GetColumnName(property);
            }
        }

        #region -------------------- not implemented --------------------

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public int RecordsAffected => throw new NotImplementedException();

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SqlBulkCopyに不要なので未実装。
        /// </summary>
        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}
