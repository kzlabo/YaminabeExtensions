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
    ///     <revision date="2020/05/24" version="1.0.3.0" author="kzlabo">列挙型に対応。</revision>
    ///     <revision date="2020/06/11" version="1.0.4.0" author="kzlabo">メモリ効率化の為に、一括登録をIDataReaderに対応。</revision>
    ///     <revision date="2021/05/05" version="1.1.0.0" author="kzlabo">更新・削除処理の追加。</revision>
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
                return attribute?.ColumnName ?? property.Name.Replace("_ExPK", string.Empty);
            }

            // SQLServerのデータ型と.NETのデータ型のマッピング
            var columns = new List<string>();
            foreach (var property in properties.Where(p => isTarget(p) == true))
            {
                var mapType = property.PropertyType;
                if (property.PropertyType.IsEnum == true)
                {
                    mapType = typeof(int);
                }

                if (_sqlServerTypeMap.ContainsKey(mapType) == false)
                {
                    throw new NotImplementedException($"{property.PropertyType.FullName} は未定義のデータ型です。");
                }
                columns.Add($"[{getColumnName(property)}] {_sqlServerTypeMap[mapType]}");
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
        /// <see cref="BulkCopyDataReader{TModel}"/> として取得します。
        /// </summary>
        /// <typeparam name="TModel">モデルの型。</typeparam>
        /// <param name="models">モデルリスト。</param>
        /// <returns>
        /// <see cref="BulkCopyDataReader{TModel}"/> を返却します。
        /// </returns>
        public static BulkCopyDataReader<TModel> AsDBulkCopyDataReader<TModel>(
            this IEnumerable<TModel> models
            )
        {
            return new BulkCopyDataReader<TModel>(models);
        }

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
        /// <typeparam name="T">モデルの型。</typeparam>
        /// <param name="connection">コネクション。</param>
        /// <param name="destinationTableName">宛先テーブル名。</param>
        /// <param name="dataReader">データリーダ。</param>
        /// <param name="truncate">宛先テーブルのデータ削有無。 <c>true</c> の場合は登録前に削除を行う。</param>
        /// <param name="copyOptions">コピーオプション。</param>
        /// <param name="externalTransaction">トランザクション。</param>
        public static void BulkCopy<T>(
            this SqlConnection connection,
            string destinationTableName,
            BulkCopyDataReader<T> dataReader,
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
                // マッピング
                dataReader.SetColumnMappings(sqlBulkCopy.ColumnMappings);
                // 一括登録実行
                sqlBulkCopy.DestinationTableName = destinationTableName;
                sqlBulkCopy.WriteToServer(dataReader);
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
        ///     匿名型を用いた一括登録例を下記に示します。
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

            // 一括登録実行
            connection.BulkCopy<T>(
                destinationTableName,
                data.AsDBulkCopyDataReader<T>(),
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
        ///     匿名型を用いた一括登録例を下記に示します。
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

        #endregion

        #region -------------------- BulkDelete --------------------

        /// <summary>
        /// 宛先テーブルに対して一括でデータ削除を行います。
        /// </summary>
        /// <typeparam name="T">モデルの型。</typeparam>
        /// <param name="connection">コネクション。</param>
        /// <param name="destinationTableName">宛先テーブル名。</param>
        /// <param name="dataReader">データリーダ。</param>
        /// <param name="externalTransaction">トランザクション。</param>
        public static void BulkDelete<T>(
            this SqlConnection connection,
            string destinationTableName,
            BulkCopyDataReader<T> dataReader,
            SqlTransaction externalTransaction
            )
        {
            // 一時テーブル作成
            var temporaryTableName = $"#_tmp{Guid.NewGuid().ToString("N")}";
            CreateTemporaryTable<T>(
                connection,
                temporaryTableName,
                externalTransaction
                );

            using (var sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, externalTransaction))
            {
                // マッピング
                dataReader.SetColumnMappings(sqlBulkCopy.ColumnMappings);
                // 一括登録実行
                sqlBulkCopy.DestinationTableName = temporaryTableName;
                sqlBulkCopy.WriteToServer(dataReader);
            }

            // 削除コマンド生成
            // モデルに設定された主キーを元にJOINを生成
            // 生成されるSQLの例）
            //
            // DELETE [T1]
            // FROM [宛先テーブル名] AS [T1]
            //     INNER JOIN [一時テーブル名] AS [T2]
            //         ON [T1].[主キー1] = [T2].[主キー2] AND [T1].[主キー2] = [T2].[主キー2] …
            using (var deleteCommand = new SqlCommand($@"
DELETE [T1]
FROM [{destinationTableName}] AS [T1]
    INNER JOIN [{temporaryTableName}] AS [T2]
        ON {dataReader.GetPrimaryColumns().Select(column => $"[T1].[{column}] = [T2].[{column}]" ).Aggregate((a, b) => $"{a} AND {b}")}
", connection, externalTransaction))
            {
                deleteCommand.ExecuteNonQuery();
            }

            // 一時テーブルは削除
            using (var dropCommand = new SqlCommand($"DROP TABLE [{temporaryTableName}]", connection, externalTransaction))
            {
                dropCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 宛先テーブルに対して一括でデータ削除を行います。
        /// </summary>
        /// <typeparam name="T">モデルの型。</typeparam>
        /// <param name="connection">コネクション。</param>
        /// <param name="destinationTableName">宛先テーブル名。</param>
        /// <param name="data">データ列挙。</param>
        /// <param name="externalTransaction">トランザクション。</param>
        /// <example>
        ///     データモデル用クラス定義。
        ///     <code language="C#" numberLines="true">
        ///         public class BulkSqlWorkRow
        ///         {
        ///             // 主キー指定
        ///             [BulkCopy(PrimaryKey = true)]
        ///             public int Id { get; set; }
        ///             
        ///             // 主キー指定
        ///             [BulkCopy(PrimaryKey = true)]
        ///             public int SubId { get; set; }
        ///
        ///             public string Name { get; set; }
        ///
        ///             public int? Age { get; set; }
        ///         }
        ///     </code>
        ///     データモデル用クラスを用いた一括削除例を下記に示します。
        ///     <code language="C#" numberLines="true">
        ///         var rows = new List&lt;BulkSqlWorkRow&gt;();
        ///         rows.Add(new BulkSqlWorkRow() { Id = 1, SubId = 1, Name = "山田", Age = null });
        ///         rows.Add(new BulkSqlWorkRow() { Id = 2, SubId = 1, Name = "田中", Age = 16 });
        ///         rows.Add(new BulkSqlWorkRow() { Id = 3, SubId = 3, Name = "佐藤", Age = 20l });
        ///         
        ///         var builder = new SqlConnectionStringBuilder();
        ///         builder.DataSource = @"(localdb)\ProjectsV13";
        ///         builder.InitialCatalog = "YaminabeExtensions.Db";
        ///         
        ///         using var connection = new SqlConnection(builder.ToString());
        ///         connection.BulkDelete&lt;BulkSqlWorkRow&gt;(
        ///             "BulkSqlWorkTable",
        ///             rows,
        ///             null
        ///             );
        ///     </code>
        ///     匿名型を用いた一括削除例を下記に示します。
        ///     <code language="C#" numberLines="true">
        ///         var builder = new SqlConnectionStringBuilder();
        ///         builder.DataSource = @"(localdb)\ProjectsV13";
        ///         builder.InitialCatalog = "YaminabeExtensions.Db";
        ///         
        ///         // 匿名型は主キー項目を識別する為に項目名の末尾に _ExPK を指定
        ///         using var connection = new SqlConnection(builder.ToString());
        ///         connection.BulkDelete(
        ///             "BulkSqlWorkTable",
        ///             new[] {
        ///                 new { Id_ExPK = 1, SubId_ExPK = 1, Name = "山田", Age = null },
        ///                 new { Id_ExPK = 2, SubId_ExPK = 1, Name = "田中", Age = 16 },
        ///                 new { Id_ExPK = 3, SubId_ExPK = 3, Name = "佐藤", Age = 201 }
        ///                 },
        ///             null
        ///             );
        ///             
        ///         // また削除処理には主キー以外は不要の為、主キー項目のみを定義した場合は _ExPK の指定は不要
        ///         connection.BulkDelete(
        ///             "BulkSqlWorkTable",
        ///             new[] {
        ///                 new { Id = 1, SubId = 1 },
        ///                 new { Id = 2, SubId = 1 },
        ///                 new { Id = 3, SubId = 3 }
        ///                 },
        ///             null
        ///             );
        ///     </code>
        /// </example>
        public static void BulkDelete<T>(
            this SqlConnection connection,
            string destinationTableName,
            IEnumerable<T> data,
            SqlTransaction externalTransaction
            )
        {
            // 一括削除データがない場合は処理無効
            if (data == null || data.Count() == 0 || data.First() == null)
            {
                return;
            }

            // コネクションが開かれていない場合はオープンし、一括削除後にクローズ
            var connectionState = ConnectionState.Open;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                connectionState = ConnectionState.Closed;
            }

            // 一括削除実行
            connection.BulkDelete<T>(
                destinationTableName,
                data.AsDBulkCopyDataReader<T>(),
                externalTransaction
                );

            // 実行前コネクションが閉じられていればクローズで終了
            if (connectionState == System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        #endregion

        #region -------------------- BulkUpdate --------------------

        /// <summary>
        /// 宛先テーブルに対して一括でデータ更新を行います。
        /// </summary>
        /// <typeparam name="T">モデルの型。</typeparam>
        /// <param name="connection">コネクション。</param>
        /// <param name="destinationTableName">宛先テーブル名。</param>
        /// <param name="dataReader">データリーダ。</param>
        /// <param name="externalTransaction">トランザクション。</param>
        public static void BulkUpdate<T>(
            this SqlConnection connection,
            string destinationTableName,
            BulkCopyDataReader<T> dataReader,
            SqlTransaction externalTransaction
            )
        {
            // 一時テーブル作成
            var temporaryTableName = $"#_tmp{Guid.NewGuid().ToString("N")}";
            CreateTemporaryTable<T>(
                connection,
                temporaryTableName,
                externalTransaction
                );

            using (var sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, externalTransaction))
            {
                // マッピング
                dataReader.SetColumnMappings(sqlBulkCopy.ColumnMappings);
                // 一括登録実行
                sqlBulkCopy.DestinationTableName = temporaryTableName;
                sqlBulkCopy.WriteToServer(dataReader);
            }

            // 更新コマンド生成
            // モデルに設定された主キーを元にJOINを生成
            // 生成されるSQLの例）
            //
            // UPDATE [T1]
            // SET
            //     [T1].[更新対象1] = [T2].[更新対象1] , [T1].[更新対象2] = [T2].[更新対象2] …
            // FROM [宛先テーブル名] AS [T1]
            //     INNER JOIN [一時テーブル名] AS [T2]
            //         ON [T1].[主キー1] = [T2].[主キー2] AND [T1].[主キー2] = [T2].[主キー2] …
            using (var updateCommand = new SqlCommand($@"
UPDATE [T1]
SET
    {dataReader.GetUpdateColumns().Select(column => $"[T1].[{column}] = [T2].[{column}]").Aggregate((a, b) => $"{a} , {b}")}
FROM [{destinationTableName}] AS [T1]
    INNER JOIN [{temporaryTableName}] AS [T2]
        ON {dataReader.GetPrimaryColumns().Select(column => $"[T1].[{column}] = [T2].[{column}]").Aggregate((a, b) => $"{a} AND {b}")}
", connection, externalTransaction))
            {
                updateCommand.ExecuteNonQuery();
            }

            // 一時テーブルは削除
            using (var dropCommand = new SqlCommand($"DROP TABLE [{temporaryTableName}]", connection, externalTransaction))
            {
                dropCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 宛先テーブルに対して一括でデータ更新を行います。
        /// </summary>
        /// <typeparam name="T">モデルの型。</typeparam>
        /// <param name="connection">コネクション。</param>
        /// <param name="destinationTableName">宛先テーブル名。</param>
        /// <param name="data">データ列挙。</param>
        /// <param name="externalTransaction">トランザクション。</param>
        /// <example>
        ///     データモデル用クラス定義。
        ///     <code language="C#" numberLines="true">
        ///         public class BulkSqlWorkRow
        ///         {
        ///             // 主キー指定
        ///             [BulkCopy(PrimaryKey = true)]
        ///             public int Id { get; set; }
        ///             
        ///             // 主キー指定
        ///             [BulkCopy(PrimaryKey = true)]
        ///             public int SubId { get; set; }
        ///
        ///             public string Name { get; set; }
        ///
        ///             public int? Age { get; set; }
        ///         }
        ///     </code>
        ///     データモデル用クラスを用いた一括更新例を下記に示します。
        ///     <code language="C#" numberLines="true">
        ///         var rows = new List&lt;BulkSqlWorkRow&gt;();
        ///         rows.Add(new BulkSqlWorkRow() { Id = 1, SubId = 1, Name = "山田", Age = null });
        ///         rows.Add(new BulkSqlWorkRow() { Id = 2, SubId = 1, Name = "田中", Age = 16 });
        ///         rows.Add(new BulkSqlWorkRow() { Id = 3, SubId = 3, Name = "佐藤", Age = 20l });
        ///         
        ///         var builder = new SqlConnectionStringBuilder();
        ///         builder.DataSource = @"(localdb)\ProjectsV13";
        ///         builder.InitialCatalog = "YaminabeExtensions.Db";
        ///         
        ///         using var connection = new SqlConnection(builder.ToString());
        ///         connection.BulkUpdate&lt;BulkSqlWorkRow&gt;(
        ///             "BulkSqlWorkTable",
        ///             rows,
        ///             null
        ///             );
        ///     </code>
        ///     匿名型を用いた一括更新例を下記に示します。
        ///     <code language="C#" numberLines="true">
        ///         var builder = new SqlConnectionStringBuilder();
        ///         builder.DataSource = @"(localdb)\ProjectsV13";
        ///         builder.InitialCatalog = "YaminabeExtensions.Db";
        ///         
        ///         // 匿名型は主キー項目を識別する為に項目名の末尾に _ExPK を指定
        ///         using var connection = new SqlConnection(builder.ToString());
        ///         connection.BulkUpdate(
        ///             "BulkSqlWorkTable",
        ///             new[] {
        ///                 new { Id_ExPK = 1, SubId_ExPK = 1, Name = "山田", Age = null },
        ///                 new { Id_ExPK = 2, SubId_ExPK = 1, Name = "田中", Age = 16 },
        ///                 new { Id_ExPK = 3, SubId_ExPK = 3, Name = "佐藤", Age = 201 }
        ///                 },
        ///             null
        ///             );
        ///     </code>
        /// </example>
        public static void BulkUpdate<T>(
            this SqlConnection connection,
            string destinationTableName,
            IEnumerable<T> data,
            SqlTransaction externalTransaction
            )
        {
            // 一括削除データがない場合は処理無効
            if (data == null || data.Count() == 0 || data.First() == null)
            {
                return;
            }

            // コネクションが開かれていない場合はオープンし、一括削除後にクローズ
            var connectionState = ConnectionState.Open;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                connectionState = ConnectionState.Closed;
            }

            // 一括更新実行
            connection.BulkUpdate<T>(
                destinationTableName,
                data.AsDBulkCopyDataReader<T>(),
                externalTransaction
                );

            // 実行前コネクションが閉じられていればクローズで終了
            if (connectionState == System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        #endregion

        #region -------------------- BulkMerge --------------------

        /// <summary>
        /// 宛先テーブルに対して一括でデータ登録・更新・削除を行います。
        /// </summary>
        /// <typeparam name="T">モデルの型。</typeparam>
        /// <param name="connection">コネクション。</param>
        /// <param name="destinationTableName">宛先テーブル名。</param>
        /// <param name="dataReader">データリーダ。</param>
        /// <param name="externalTransaction">トランザクション。</param>
        /// <param name="notMatchedDelete">宛先テーブルのみに存在するデータの削除要否。<c>true</c> の場合は削除を行う。 <c>false</c>の場合は削除を行わない。</param>
        public static void BulkMerge<T>(
            this SqlConnection connection,
            string destinationTableName,
            BulkCopyDataReader<T> dataReader,
            SqlTransaction externalTransaction,
            bool notMatchedDelete = true
            )
        {
            // 一時テーブル作成
            var temporaryTableName = $"#_tmp{Guid.NewGuid().ToString("N")}";
            CreateTemporaryTable<T>(
                connection,
                temporaryTableName,
                externalTransaction
                );

            using (var sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, externalTransaction))
            {
                // マッピング
                dataReader.SetColumnMappings(sqlBulkCopy.ColumnMappings);
                // 一括登録実行
                sqlBulkCopy.DestinationTableName = temporaryTableName;
                sqlBulkCopy.WriteToServer(dataReader);
            }

            // 更新コマンド生成
            // モデルに設定された主キーを元にJOINを生成
            // 生成されるSQLの例）
            //
            // MERGE [宛先テーブル名] AS [T1]
            // USING [一時テーブル名] AS [T2]
            //     ON [T1].[主キー1] = [T2].[主キー2] AND [T1].[主キー2] = [T2].[主キー2] …
            // WHEN MATCHED
            //     THEN UPDATE
            //         SET
            //             [T1].[更新対象1] = [T2].[更新対象1] , [T1].[更新対象2] = [T2].[更新対象2] …
            // WHEN NOT MATCHED BY TARGET
            //     THEN INSERT ([項目1] , [項目2] …)
            //         VALUES ([T2].[項目1], [T2].[項目2] …)
            // WHEN NOT MATCHED BY SOURCE
            //     DELETE
            using (var mergeCommand = new SqlCommand($@"
MERGE[{ destinationTableName }] AS[T1]
USING[{ temporaryTableName}] AS[T2]
    ON { dataReader.GetPrimaryColumns().Select(column => $"[T1].[{column}] = [T2].[{column}]").Aggregate((a, b) => $"{a} AND {b}")}
            WHEN MATCHED
    THEN UPDATE
        SET
            { dataReader.GetUpdateColumns().Select(column => $"[T1].[{column}] = [T2].[{column}]").Aggregate((a, b) => $"{a} , {b}")}
            WHEN NOT MATCHED BY TARGET
                THEN INSERT({ dataReader.GetColumns().Select(column => $"[{column}]").Aggregate((a, b) => $"{a} , {b}")})
                    VALUES({ dataReader.GetColumns().Select(column => $"[T2].[{column}]").Aggregate((a, b) => $"{a} , {b}")})
{ (notMatchedDelete ? $@"
            WHEN NOT MATCHED BY SOURCE
                THEN DELETE
" : string.Empty)}
;
", connection, externalTransaction))
            {
                mergeCommand.ExecuteNonQuery();
            }

            // 一時テーブルは削除
            using (var dropCommand = new SqlCommand($"DROP TABLE [{temporaryTableName}]", connection, externalTransaction))
            {
                dropCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 宛先テーブルに対して一括でデータ登録・更新・削除を行います。
        /// </summary>
        /// <typeparam name="T">モデルの型。</typeparam>
        /// <param name="connection">コネクション。</param>
        /// <param name="destinationTableName">宛先テーブル名。</param>
        /// <param name="data">データ列挙。</param>
        /// <param name="externalTransaction">トランザクション。</param>
        /// <param name="notMatchedDelete">宛先テーブルのみに存在するデータの削除要否。<c>true</c> の場合は削除を行う。 <c>false</c>の場合は削除を行わない。</param>
        /// <example>
        ///     データモデル用クラス定義。
        ///     <code language="C#" numberLines="true">
        ///         public class BulkSqlWorkRow
        ///         {
        ///             // 主キー指定
        ///             [BulkCopy(PrimaryKey = true)]
        ///             public int Id { get; set; }
        ///             
        ///             // 主キー指定
        ///             [BulkCopy(PrimaryKey = true)]
        ///             public int SubId { get; set; }
        ///
        ///             public string Name { get; set; }
        ///
        ///             public int? Age { get; set; }
        ///         }
        ///     </code>
        ///     データモデル用クラスを用いた一括登録・更新・削除例を下記に示します。
        ///     <code language="C#" numberLines="true">
        ///         var rows = new List&lt;BulkSqlWorkRow&gt;();
        ///         rows.Add(new BulkSqlWorkRow() { Id = 1, SubId = 1, Name = "山田", Age = null });
        ///         rows.Add(new BulkSqlWorkRow() { Id = 2, SubId = 1, Name = "田中", Age = 16 });
        ///         rows.Add(new BulkSqlWorkRow() { Id = 3, SubId = 3, Name = "佐藤", Age = 20l });
        ///         
        ///         var builder = new SqlConnectionStringBuilder();
        ///         builder.DataSource = @"(localdb)\ProjectsV13";
        ///         builder.InitialCatalog = "YaminabeExtensions.Db";
        ///         
        ///         using var connection = new SqlConnection(builder.ToString());
        ///         connection.BulkMerge&lt;BulkSqlWorkRow&gt;(
        ///             "BulkSqlWorkTable",
        ///             rows,
        ///             null,
        ///             true
        ///             );
        ///         
        ///         // 宛先テーブルにしか存在しないデータの削除を行いわない場合は
        ///         // notMatchedDelete に false を指定することで 登録 更新 のみを実行可能
        ///         connection.BulkMerge&lt;BulkSqlWorkRow&gt;(
        ///             "BulkSqlWorkTable",
        ///             rows,
        ///             null,
        ///             false
        ///             );
        ///     </code>
        ///     匿名型を用いた一括登録・更新・削除例を下記に示します。
        ///     <code language="C#" numberLines="true">
        ///         var builder = new SqlConnectionStringBuilder();
        ///         builder.DataSource = @"(localdb)\ProjectsV13";
        ///         builder.InitialCatalog = "YaminabeExtensions.Db";
        ///         
        ///         // 匿名型は主キー項目を識別する為に項目名の末尾に _ExPK を指定
        ///         using var connection = new SqlConnection(builder.ToString());
        ///         connection.BulkMerge(
        ///             "BulkSqlWorkTable",
        ///             new[] {
        ///                 new { Id_ExPK = 1, SubId_ExPK = 1, Name = "山田", Age = null },
        ///                 new { Id_ExPK = 2, SubId_ExPK = 1, Name = "田中", Age = 16 },
        ///                 new { Id_ExPK = 3, SubId_ExPK = 3, Name = "佐藤", Age = 201 }
        ///                 },
        ///             null,
        ///             true
        ///             );
        ///     </code>
        /// </example>
        public static void BulkMerge<T>(
            this SqlConnection connection,
            string destinationTableName,
            IEnumerable<T> data,
            SqlTransaction externalTransaction,
            bool notMatchedDelete = true
            )
        {
            // 一括削除データがない場合は処理無効
            if (data == null || data.Count() == 0 || data.First() == null)
            {
                return;
            }

            // コネクションが開かれていない場合はオープンし、一括削除後にクローズ
            var connectionState = ConnectionState.Open;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                connectionState = ConnectionState.Closed;
            }

            // 一括登録・更新・削除実行
            connection.BulkMerge<T>(
                destinationTableName,
                data.AsDBulkCopyDataReader<T>(),
                externalTransaction,
                notMatchedDelete
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
