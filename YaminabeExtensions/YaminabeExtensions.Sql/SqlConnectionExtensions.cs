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

namespace YaminabeExtensions.Sql
{
    /// <summary>
    /// SQLコネクション拡張メソッドを提供します。
    /// </summary>
    /// <revisionHistory>
    ///     <revision date="2020/02/11" version="1.0.0.0" author="kzlabo">新規作成。</revision>
    ///     <revision date="2020/05/24" version="1.0.1.0" author="kzlabo">トランザクション漏れ修正。</revision>
    ///     <revision date="2020/05/24" version="1.0.2.0" author="kzlabo">データ有無判定の修正。</revision>
    /// </revisionHistory>
    public static class SqlConnectionExtensions
    {
        #region -------------------- static --------------------

        /// <summary>
        /// BulkCopyのタイムアウト値を取得または設定します。
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static int GlobalBulkCopyTimeout { get; set; } = 30;

        #endregion

        #region -------------------- method --------------------

        #region -------------------- CreateTable --------------------

        /// <summary>
        /// SQLServerのデータ型とC#のデータ型のマッピングを提供します。
        /// </summary>
        private static Dictionary<System.Type, string> _sqlServerTypeMap = new Dictionary<System.Type, string>()
        {
            { typeof(string), "nvarchar(2000)" },
            { typeof(short), "smallint" },
            { typeof(short?), "smallint" },
            { typeof(int), "int" },
            { typeof(int?), "int" },
            { typeof(long), "bigint" },
            { typeof(long?), "bigint" },
            { typeof(float), "float" },
            { typeof(float?), "float" },
            { typeof(double), "float" },
            { typeof(double?), "float" },
            { typeof(decimal), "decimal(30, 4)" },
            { typeof(decimal?), "decimal(30, 4)" },
            { typeof(DateTime), "datetime" },
            { typeof(DateTime?), "datetime" },
            { typeof(byte[]), "varbinary(max)" },
            { typeof(bool), "bit" },
            { typeof(bool?), "bit" },
            { typeof(Guid), "uniqueidentifier" },
            { typeof(Guid?), "uniqueidentifier" },
        };

        /// <summary>
        /// データモデルからSQLServerのデータベースにテーブルの作成を行います。
        /// </summary>
        /// <typeparam name="T">データモデルの型。</typeparam>
        /// <param name="connection">コネクション。</param>
        /// <param name="temporaryTableName">作成テーブル名。</param>
        /// <param name="externalTransaction">トランザクション。</param>
        private static void CreateTemporaryTable<T>(
            this SqlConnection connection,
            string temporaryTableName,
            SqlTransaction externalTransaction
            )
        {
            var properties = (typeof(T).GetProperties() as System.Reflection.PropertyInfo[]);
            CreateTemporaryTable(
                connection,
                temporaryTableName,
                properties,
                externalTransaction
                );
        }

        /// <summary>
        /// プロパティコレクションからSQLServerのデータベースにテーブルの作成を行います。
        /// </summary>
        /// <param name="connection">コネクション。</param>
        /// <param name="temporaryTableName">作成テーブル名。</param>
        /// <param name="properties">プロパティコレクション。</param>
        /// <param name="externalTransaction">トランザクション。</param>
        private static void CreateTemporaryTable(
            this SqlConnection connection,
            string temporaryTableName,
            System.Reflection.PropertyInfo[] properties,
            SqlTransaction externalTransaction
            )
        {
            // 一括登録対象プロパティ判定メソッド
            bool isTarget(System.Reflection.PropertyInfo property)
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

            // 一括登録対象プロパティの宛先項目名取得メソッド
            string getColumnName(System.Reflection.PropertyInfo property)
            {
                // 属性指定されていない場合はプロパティ名
                // カラム名指定がされている場合は指定名
                var attribute = Attribute.GetCustomAttribute(property, typeof(BulkCopyAttribute)) as BulkCopyAttribute;
                return attribute?.ColumnName ?? property.Name;
            }

            // SQLServerのデータ型と.NETのデータ型のマッピング
            var columns = new List<string>();
            foreach (var property in properties.Where(p => isTarget(p) == true))
            {
                if (_sqlServerTypeMap.ContainsKey(property.PropertyType) == false)
                {
                    throw new NotImplementedException($"{property.PropertyType.FullName} は未定義のデータ型です。");
                }
                columns.Add($"[{getColumnName(property)}] {_sqlServerTypeMap[property.PropertyType]}");
            }

            // テーブル作成
            var createSql = new System.Text.StringBuilder();
            createSql.AppendLine($"CREATE TABLE [{temporaryTableName}]");
            createSql.AppendLine($"(");
            createSql.AppendLine(string.Join(",\r\n", columns.ToArray()));
            createSql.AppendLine($")");
            using (var command = new SqlCommand(createSql.ToString(), connection, externalTransaction))
            {
                command.ExecuteNonQuery();
            }
        }

        #endregion

        #region -------------------- BulkCopy --------------------

        /// <summary>
        /// 宛先テーブルに対して一括でデータ登録を行います。
        /// </summary>
        /// <param name="connection">コネクション。</param>
        /// <param name="destinationTableName">宛先テーブル名。</param>
        /// <param name="table">データテーブル。</param>
        /// <param name="truncate">宛先テーブルのデータ削有無。 <c>true</c> の場合は登録前に削除を行う。</param>
        /// <param name="copyOptions">コピーオプション。</param>
        /// <param name="externalTransaction">トランザクション。</param>
        public static void BulkCopy(
            this SqlConnection connection,
            string destinationTableName,
            DataTable table,
            bool truncate,
            SqlBulkCopyOptions copyOptions,
            SqlTransaction externalTransaction
            )
        {
            using (var sqlBulkCopy = new SqlBulkCopy(connection, copyOptions, externalTransaction))
            {
                // 一括登録前にデータを削除
                if (truncate)
                {
                    using (var truncateCommand = new SqlCommand($"TRUNCATE TABLE [{destinationTableName}]", connection, externalTransaction))
                    {
                        truncateCommand.ExecuteNonQuery();
                    }
                }
                // 一括登録実行
                sqlBulkCopy.DestinationTableName = destinationTableName;
                sqlBulkCopy.WriteToServer(table);
            }
        }

        /// <summary>
        /// 宛先テーブルに対して一括でデータ登録を行います。
        /// </summary>
        /// <typeparam name="T">データ列挙の型。</typeparam>
        /// <param name="connection">コネクション。</param>
        /// <param name="destinationTableName">宛先テーブル名。</param>
        /// <param name="data">データ列挙。</param>
        /// <param name="truncate">宛先テーブルのデータ削有無。 <c>true</c> の場合は登録前に削除を行う。</param>
        /// <param name="copyOptions">コピーオプション。</param>
        /// <param name="externalTransaction">トランザクション。</param>
        /// <example>
        ///     データモデル用クラス定義。
        ///     <code language="C#" numberLines="true">
        ///         public class BulkCopyWorkRow
        ///         {
        ///             public int Id { get; set; }
        ///
        ///             // 宛先テーブルのカラム名と異なるプロパティをマッピング
        ///             [BulkCopy(ColumnName = "Name")]
        ///             public string Namae { get; set; }
        ///
        ///             // 宛先テーブルに存在しない項目は除外対象としてマーク
        ///             [BulkCopy(Ignore = true)]
        ///             public System.DateTime ApplyDateTime { get; set; }
        ///         }
        ///     </code>
        ///     データモデル用クラスを用いた一括登録例を下記に示します。
        ///     <code language="C#" numberLines="true">
        ///         var rows = new List&lt;BulkCopyWorkRow&gt;();
        ///         rows.Add(new BulkCopyWorkRow() { Id = 1, Namae = "山田", ApplyDateTime = DateTime.Now });
        ///         rows.Add(new BulkCopyWorkRow() { Id = 2, Namae = "田中", ApplyDateTime = DateTime.Now });
        ///         rows.Add(new BulkCopyWorkRow() { Id = 3, Namae = "佐藤", ApplyDateTime = DateTime.Now });
        ///         
        ///         var builder = new SqlConnectionStringBuilder();
        ///         builder.DataSource = @"(localdb)\ProjectsV13";
        ///         builder.InitialCatalog = "YaminabeExtensions.Db";
        ///         
        ///         using var connection = new SqlConnection(builder.ToString());
        ///         connection.BulkCopy&lt;BulkCopyWorkTable&gt;(
        ///             "BulkCopyWorkTable",
        ///             rows,
        ///             true,
        ///             SqlBulkCopyOptions.Default,
        ///             null
        ///             );
        ///     </code>
        /// </example>
        public static void BulkCopy<T>(
            this SqlConnection connection,
            string destinationTableName,
            IEnumerable<T> data,
            bool truncate,
            SqlBulkCopyOptions copyOptions,
            SqlTransaction externalTransaction
            )
        {
            // 一括登録データがない場合は処理無効
            if (data == null || data.Count() == 0 || data.First() == null)
            {
                return;
            }

            // コネクションが開かれていない場合はオープンし、一括登録後にクローズ
            var connectionState = ConnectionState.Open;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                connectionState = ConnectionState.Closed;
            }

            // マッピング用テーブル作成
            var table = new DataTable();
            using (var command = new SqlCommand($"SELECT TOP 0 * FROM [{destinationTableName}]", connection, externalTransaction))
            {
                table.Load(command.ExecuteReader());
            }

            // 一括登録対象プロパティ判定メソッド
            bool isTarget(System.Reflection.PropertyInfo property)
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

            // 一括登録対象プロパティの宛先項目名取得メソッド
            string getColumnName(System.Reflection.PropertyInfo property)
            {
                // 属性指定されていない場合はプロパティ名
                // カラム名指定がされている場合は指定名
                var attribute = Attribute.GetCustomAttribute(property, typeof(BulkCopyAttribute)) as BulkCopyAttribute;
                return attribute?.ColumnName ?? property.Name;
            }

            // データテーブルにデータ列挙を登録
            var properties = (typeof(T).GetProperties() as System.Reflection.PropertyInfo[]).Where(prop => isTarget(prop) == true);
            foreach (var row in data)
            {
                var newRow = table.NewRow();
                foreach (var propery in properties)
                {
                    newRow[getColumnName(propery)] = propery.GetValue(row) ?? DBNull.Value;
                }
                table.Rows.Add(newRow);
            }

            // 一括登録実行
            connection.BulkCopy(
                destinationTableName,
                table,
                truncate,
                copyOptions,
                externalTransaction
                );

            // 実行前コネクションが閉じられていればクローズで終了
            if (connectionState == System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        /// <summary>
        /// 宛先テーブルに対して一括でデータ登録を行います。
        /// </summary>
        /// <param name="connection">コネクション。</param>
        /// <param name="destinationTableName">宛先テーブル名。</param>
        /// <param name="data">データ列挙。（匿名型）</param>
        /// <param name="truncate">宛先テーブルのデータ削有無。 <c>true</c> の場合は登録前に削除を行う。</param>
        /// <param name="copyOptions">コピーオプション。</param>
        /// <param name="externalTransaction">トランザクション。</param>
        /// <example>
        ///     <code language="C#" numberLines="true">
        ///         var builder = new SqlConnectionStringBuilder();
        ///         builder.DataSource = @"(localdb)\ProjectsV13";
        ///         builder.InitialCatalog = "YaminabeExtensions.Db";
        ///
        ///         using var connection = new SqlConnection(builder.ToString());
        ///         connection.BulkCopy(
        ///             "BulkCopyWorkTable",
        ///             new[] {
        ///                 new { Id = 1, Name = "山田" },
        ///                 new { Id = 2, Name = "田中" },
        ///                 new { Id = 3, Name = "佐藤" }
        ///                 },
        ///             true,
        ///             SqlBulkCopyOptions.Default,
        ///             null
        ///             );
        ///     </code>
        /// </example>
        public static void BulkCopy(
            this SqlConnection connection,
            string destinationTableName,
            IEnumerable<dynamic> data,
            bool truncate,
            SqlBulkCopyOptions copyOptions,
            SqlTransaction externalTransaction
            )
        {
            // 一括登録データがない場合は処理無効
            if (data == null || data.Count() == 0 || data.First() == null)
            {
                return;
            }

            // コネクションが開かれていない場合はオープンし、一括登録後にクローズ
            var connectionState = ConnectionState.Open;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                connectionState = ConnectionState.Closed;
            }

            // マッピング用テーブル作成
            var table = new DataTable();
            using (var command = new SqlCommand($"SELECT TOP 0 * FROM [{destinationTableName}]", connection, externalTransaction))
            {
                table.Load(command.ExecuteReader());
            }

            // データテーブルにデータ列挙を登録
            var properties = data.First().GetType().GetProperties() as System.Reflection.PropertyInfo[];
            foreach(var row in data)
            {
                var newRow = table.NewRow();
                foreach (var propery in properties)
                {
                    newRow[propery.Name] = propery.GetValue(row) ?? DBNull.Value;
                }
                table.Rows.Add(newRow);
            }

            // 一括登録実行
            connection.BulkCopy(
                destinationTableName,
                table,
                truncate,
                copyOptions,
                externalTransaction
                );

            // 実行前コネクションが閉じられていればクローズで終了
            if (connectionState == System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        /// <summary>
        /// テーブルを新規作成し一括でデータ登録を行います。
        /// </summary>
        /// <typeparam name="T">データ列挙の型。</typeparam>
        /// <param name="connection">コネクション。</param>
        /// <param name="temporaryTableName">作成テーブル名。</param>
        /// <param name="data">データ列挙。</param>
        /// <param name="truncate">宛先テーブルのデータ削有無。 <c>true</c> の場合は登録前に削除を行う。</param>
        /// <param name="copyOptions">コピーオプション。</param>
        /// <param name="externalTransaction">トランザクション。</param>
        /// <example>
        ///     データモデル用クラス定義。
        ///     <code language="C#" numberLines="true">
        ///         public class BulkCopyWorkRow
        ///         {
        ///             public int Id { get; set; }
        ///
        ///             // 宛先テーブルのカラム名と異なるプロパティをマッピング
        ///             [BulkCopy(ColumnName = "Name")]
        ///             public string Namae { get; set; }
        ///
        ///             // 宛先テーブルに存在しない項目は除外対象としてマーク
        ///             [BulkCopy(Ignore = true)]
        ///             public System.DateTime ApplyDateTime { get; set; }
        ///         }
        ///     </code>
        ///     データモデル用クラスを用いた一括登録例を下記に示します。
        ///     主にT-SQLを利用して効率的なSQLを発行するための手段です。
        ///     <code language="C#" numberLines="true">
        ///         var rows = new List&lt;BulkCopyWorkRow&gt;();
        ///         rows.Add(new BulkCopyWorkRow() { Id = 1, Namae = "山田", ApplyDateTime = DateTime.Now });
        ///         rows.Add(new BulkCopyWorkRow() { Id = 2, Namae = "田中", ApplyDateTime = DateTime.Now });
        ///         rows.Add(new BulkCopyWorkRow() { Id = 3, Namae = "佐藤", ApplyDateTime = DateTime.Now });
        ///         
        ///         var builder = new SqlConnectionStringBuilder();
        ///         builder.DataSource = @"(localdb)\ProjectsV13";
        ///         builder.InitialCatalog = "YaminabeExtensions.Db";
        ///         
        ///         using var connection = new SqlConnection(builder.ToString());
        ///         // 新規に一時テーブルを作成し一括登録
        ///         connection.BulkCopyToTemporaryTable&lt;BulkCopyWorkRow&gt;(
        ///             "#BulkCopyWorkTable",
        ///             rows,
        ///             true,
        ///             SqlBulkCopyOptions.Default,
        ///             null
        ///             );
        ///         
        ///         // 一時テーブルを利用して宛先テーブルに対して一括更新
        ///         using var command = new SqlCommand(string.Empty, connection);
        ///         command.CommandText = @"
        ///             UPDATE [A]
        ///             SET
        ///                 [A].[Name] = [B].[Name],
        ///             FROM [BulkCopyWorkTable] AS [A]
        ///                 INNER JOIN [#BulkCopyWorkTable] AS [B]
        ///                     ON [A].[Id] = [B].[Id];
        ///         ";
        ///         command.ExecuteNonQuery();
        ///     </code>
        /// </example>
        public static void BulkCopyToTemporaryTable<T>(
            this SqlConnection connection,
            string temporaryTableName,
            IEnumerable<T> data,
            bool truncate,
            SqlBulkCopyOptions copyOptions,
            SqlTransaction externalTransaction
            )
        {
            // コネクションが開かれていない場合はオープンし、一括登録後にクローズ
            var connectionState = ConnectionState.Open;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                connectionState = ConnectionState.Closed;
            }

            // テーブル作成
            CreateTemporaryTable<T>(
                connection,
                temporaryTableName,
                externalTransaction
                );

            // 一括登録実行
            BulkCopy<T>(
                connection,
                temporaryTableName,
                data,
                truncate,
                copyOptions,
                externalTransaction
                );

            // 実行前コネクションが閉じられていればクローズで終了
            if (connectionState == System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        /// <summary>
        /// テーブルを新規作成し一括でデータ登録を行います。
        /// </summary>
        /// <param name="connection">コネクション。</param>
        /// <param name="temporaryTableName">作成テーブル名。</param>
        /// <param name="data">データ列挙。（匿名型）</param>
        /// <param name="truncate">宛先テーブルのデータ削有無。 <c>true</c> の場合は登録前に削除を行う。</param>
        /// <param name="copyOptions">コピーオプション。</param>
        /// <param name="externalTransaction">トランザクション。</param>
        /// <example>
        ///     <code language="C#" numberLines="true">
        ///         var builder = new SqlConnectionStringBuilder();
        ///         builder.DataSource = @"(localdb)\ProjectsV13";
        ///         builder.InitialCatalog = "YaminabeExtensions.Db";
        ///
        ///         using var connection = new SqlConnection(builder.ToString());
        ///         // 新規に一時テーブルを作成し一括登録
        ///         connection.BulkCopyToTemporaryTable(
        ///             "#BulkCopyWorkTable",
        ///             new[] {
        ///                 new { Id = 1, Name = "山田" },
        ///                 new { Id = 2, Name = "田中" },
        ///                 new { Id = 3, Name = "佐藤" }
        ///                 },
        ///             true,
        ///             SqlBulkCopyOptions.Default,
        ///             null
        ///             );
        ///             
        ///         // 一時テーブルを利用して宛先テーブルに対して一括更新
        ///         using var command = new SqlCommand(string.Empty, connection);
        ///         command.CommandText = @"
        ///             UPDATE [A]
        ///             SET
        ///                 [A].[Name] = [B].[Name],
        ///             FROM [BulkCopyWorkTable] AS [A]
        ///                 INNER JOIN [#BulkCopyWorkTable] AS [B]
        ///                     ON [A].[Id] = [B].[Id];
        ///         ";
        ///         command.ExecuteNonQuery();
        ///     </code>
        /// </example>
        public static void BulkCopyToTemporaryTable(
            this SqlConnection connection,
            string temporaryTableName,
            IEnumerable<dynamic> data,
            bool truncate,
            SqlBulkCopyOptions copyOptions,
            SqlTransaction externalTransaction
            )
        {
            // コネクションが開かれていない場合はオープンし、一括登録後にクローズ
            var connectionState = ConnectionState.Open;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                connectionState = ConnectionState.Closed;
            }

            // テーブル作成
            CreateTemporaryTable(
                connection,
                temporaryTableName,
                data.First().GetType().GetProperties().
                externalTransaction
                );

            // 一括登録実行
            BulkCopy(
                connection,
                temporaryTableName,
                data,
                truncate,
                copyOptions,
                externalTransaction
                );

            // 実行前コネクションが閉じられていればクローズで終了
            if (connectionState == System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        #endregion

        #endregion
    }
}
