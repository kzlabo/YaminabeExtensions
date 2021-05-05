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
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using YaminabeExtensions.Sql;

namespace YaminabeExtensions.Test.Tests.Sql
{
    /// <summary>
    /// SQLコネクション拡張メソッドの検証を提供します。
    /// </summary>
    [TestClass]
    public class SqlConnectionExtensionsUnitTest
    {
        #region -------------------- test stub --------------------

        /// <summary>
        /// BulkCopy拡張メソッドのデータモデルクラスを検証するためのスタブクラス。
        /// </summary>
        private class BulkCopyWorkRow
        {
            public int Id { get; set; }

            // 別名指定の検証用に項目名を変更
            [BulkCopy( ColumnName = "Name" )]
            public string Namae { get; set; }
            public int? Age { get; set; }

            // 除外対象指定の検証用にマーク
            [BulkCopy( Ignore = true )]
            public System.DateTime ApplyDateTime { get; set; }
        }

        /// <summary>
        /// BulkCopy拡張メソッドのデータ型マッピングを検証するためのスタブクラス。
        /// </summary>
        private class BulkCopyMapTestRow
        {
            public enum EnumValueOption
            {
                Zero = 0,
                First = 1,
                Second = 2
            }

            public int Id { get; set; }

            public string StringValue { get; set; }

            public short ShortValue { get; set; }

            public short? ShortWithNullable { get; set; }

            public int IntValue { get; set; }

            public int? IntWithNullable { get; set; }

            public long LongValue { get; set; }

            public long? LongWithNullable { get; set; }

            public float FloatValue { get; set; }

            public float? FloatWithNullable { get; set; }

            public double DoubleValue { get; set; }

            public double? DoubleWithNullable { get; set; }

            public decimal DecimalValue { get; set; }

            public decimal? DecimalWithNullable { get; set; }

            public DateTime DateTimeValue { get; set; }

            public DateTime? DateTimeWithNullable { get; set; }

            public byte[] ByteValues { get; set; }

            public bool BoolValue { get; set; }

            public bool? BoolWithNullable { get; set; }

            public Guid GuidValue { get; set; }

            public Guid? GuidWithNullable { get; set; }

            public EnumValueOption EnumValue { get; set; }
        }

        /// <summary>
        /// Bulk系拡張メソッドのデータモデルクラスを検証するためのスタブクラス。
        /// </summary>
        private class BulkSqlWorkRow
        {
            [BulkCopy(PrimaryKey = true)]
            public int Id { get; set; }

            [BulkCopy(PrimaryKey = true)]
            public int SubId { get; set; }

            public string Name { get; set; }

            public int? Age { get; set; }
        }

        #endregion

        #region -------------------- test method --------------------

        #region -------------------- BulkCopy --------------------

        /// <summary>
        /// <see cref="SqlConnectionExtensions.BulkCopy{T}(SqlConnection, string, IEnumerable{T}, bool, SqlBulkCopyOptions, SqlTransaction)"/> の正常系検証を実施します。
        /// </summary>
        /// <remarks>
        /// 匿名型パターン。
        /// </remarks>
        [TestCategory("SqlConnection:BulkCopy")]
        [TestMethod]
        public void BulkCopyDynamicSuccessTest()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"(localdb)\ProjectsV13";
            builder.InitialCatalog = "YaminabeExtensions.Db";

            using var connection = new SqlConnection(builder.ToString());
            connection.BulkCopy(
                "BulkCopyWorkTable",
                new[] {
                    new { Id = 1, Name = "山田" },
                    new { Id = 2, Name = "田中" },
                    new { Id = 3, Name = "佐藤" }
                    },
                true,
                SqlBulkCopyOptions.Default,
                null
                );
        }

        /// <summary>
        /// <see cref="SqlConnectionExtensions.BulkCopy{T}(SqlConnection, string, IEnumerable{T}, bool, SqlBulkCopyOptions, SqlTransaction)"/> の正常系検証を実施します。
        /// </summary>
        /// <remarks>
        /// モデルパターン。
        /// </remarks>
        [TestCategory("SqlConnection:BulkCopy")]
        [TestMethod]
        public void BulkCopyGenericSuccessTest()
        {
            var rows = new List<BulkCopyWorkRow>();
            rows.Add(new BulkCopyWorkRow() { Id = 1, Namae = "山田", Age = null, ApplyDateTime = DateTime.Now });
            rows.Add(new BulkCopyWorkRow() { Id = 2, Namae = "田中", Age = 16, ApplyDateTime = DateTime.Now });
            rows.Add(new BulkCopyWorkRow() { Id = 3, Namae = "佐藤", Age = 20, ApplyDateTime = DateTime.Now });

            var connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.DataSource = @"(localdb)\ProjectsV13";
            connectionStringBuilder.InitialCatalog = "YaminabeExtensions.Db";

            var connection = new SqlConnection(connectionStringBuilder.ToString());
            connection.BulkCopy<BulkCopyWorkRow>(
                "BulkCopyWorkTable",
                rows,
                true,
                SqlBulkCopyOptions.Default,
                null
                );
        }

        /// <summary>
        /// <see cref="SqlConnectionExtensions.BulkCopyToTemporaryTable{T}(SqlConnection, string, IEnumerable{T}, bool, SqlBulkCopyOptions, SqlTransaction)"/> の正常系検証を実施します。
        /// </summary>
        /// <remarks>
        /// 匿名型パターン。
        /// </remarks>
        [TestCategory("SqlConnection:BulkCopy")]
        [TestMethod]
        public void BulkCopyToTemporaryTableDynamicSuccessTest()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"(localdb)\ProjectsV13";
            builder.InitialCatalog = "YaminabeExtensions.Db";


            using var connection = new SqlConnection(builder.ToString());
            using var command = new SqlCommand(string.Empty, connection);
            connection.Open();

            // 宛先テーブルに更新対象データセット
            var initialSql = @"
TRUNCATE TABLE [BulkCopyWorkTable];
INSERT INTO [BulkCopyWorkTable] VALUES(1, N'川田', 0);
INSERT INTO [BulkCopyWorkTable] VALUES(2, N'田下', 0);
INSERT INTO [BulkCopyWorkTable] VALUES(3, N'伊藤', 0);
";
            command.CommandText = initialSql;
            command.ExecuteNonQuery();

            // 一時テーブルに更新情報を一括登録
            connection.BulkCopyToTemporaryTable(
                "#BulkCopyWorkTable",
                    new[] {
                            new { Id = 1, Name = "山田", Age = null as int? },
                            new { Id = 2, Name = "田中", Age = 16 as int? },
                            new { Id = 3, Name = "佐藤", Age = 20 as int? }
                    },
                    true,
                    SqlBulkCopyOptions.Default,
                    null
                );

            // 一時テーブルを使用して宛先テーブルを更新
            var temporaryToTable = @"
UPDATE [A]
SET
    [A].[Name] = [B].[Name],
    [A].[Age] = [B].[Age]
FROM [BulkCopyWorkTable] AS [A]
    INNER JOIN [#BulkCopyWorkTable] AS [B]
        ON [A].[Id] = [B].[Id]
";
            command.CommandText = temporaryToTable;
            command.ExecuteNonQuery();

        }

        /// <summary>
        /// <see cref="SqlConnectionExtensions.BulkCopyToTemporaryTable{T}(SqlConnection, string, IEnumerable{T}, bool, SqlBulkCopyOptions, SqlTransaction)"/> の正常系検証を実施します。
        /// </summary>
        /// <remarks>
        /// モデルパターン。
        /// </remarks>
        [TestCategory("SqlConnection:BulkCopy")]
        [TestMethod]
        public void BulkCopyToTemporaryTableGenericSuccessTest()
        {
            var rows = new List<BulkCopyWorkRow>();
            rows.Add(new BulkCopyWorkRow() { Id = 1, Namae = "山田", Age = null, ApplyDateTime = DateTime.Now });
            rows.Add(new BulkCopyWorkRow() { Id = 2, Namae = "田中", Age = 16, ApplyDateTime = DateTime.Now });
            rows.Add(new BulkCopyWorkRow() { Id = 3, Namae = "佐藤", Age = 20, ApplyDateTime = DateTime.Now });

            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"(localdb)\ProjectsV13";
            builder.InitialCatalog = "YaminabeExtensions.Db";

            using var connection = new SqlConnection(builder.ToString());
            using var command = new SqlCommand(string.Empty, connection);
            connection.Open();

            // 宛先テーブルに更新対象データセット
            var initialSql = @"
TRUNCATE TABLE [BulkCopyWorkTable];
INSERT INTO [BulkCopyWorkTable] VALUES(1, N'川田', 0);
INSERT INTO [BulkCopyWorkTable] VALUES(2, N'田下', 0);
INSERT INTO [BulkCopyWorkTable] VALUES(3, N'伊藤', 0);
";
            command.CommandText = initialSql;
            command.ExecuteNonQuery();

            // 一時テーブルに更新情報を一括登録
            connection.BulkCopyToTemporaryTable<BulkCopyWorkRow>(
                "#BulkCopyWorkTable",
                rows,
                true,
                SqlBulkCopyOptions.Default,
                null
                );

            // 一時テーブルを使用して宛先テーブルを更新
            var temporaryToTable = @"
UPDATE [A]
SET
    [A].[Name] = [B].[Name],
    [A].[Age] = [B].[Age]
FROM [BulkCopyWorkTable] AS [A]
    INNER JOIN [#BulkCopyWorkTable] AS [B]
        ON [A].[Id] = [B].[Id]
";
            command.CommandText = temporaryToTable.ToString();
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// SQLServerと.netのデータ型のマップ検証を実施します。
        /// </summary>
        [TestCategory("SqlConnection:BulkCopy")]
        [TestMethod]
        public void BulkCopyMapSuccessTest()
        {
            var rows = new List<BulkCopyMapTestRow>();
            rows.Add(new BulkCopyMapTestRow() { Id = 1, StringValue = "a", ShortValue = 1, ShortWithNullable = 1, IntValue = 1, IntWithNullable = 1, LongValue = 1L, LongWithNullable = 1L, FloatValue = 1.0F, FloatWithNullable = 1.0F, DoubleValue = 1.0D, DoubleWithNullable = 1.0D, DecimalValue = 1.0M, DecimalWithNullable = 1.0M, DateTimeValue = DateTime.Parse("2000/01/02"), DateTimeWithNullable = DateTime.Parse("2000/01/02"), ByteValues = new byte[] { 0xff, 0xff }, BoolValue = true, BoolWithNullable = true, GuidValue = Guid.Parse("00000000-0000-0000-0000-000000000000"), GuidWithNullable = Guid.Parse("00000000-0000-0000-0000-000000000000"), EnumValue = BulkCopyMapTestRow.EnumValueOption.First });
            rows.Add(new BulkCopyMapTestRow() { Id = 2, StringValue = "a", ShortValue = 1, ShortWithNullable = null, IntValue = 1, IntWithNullable = null, LongValue = 1L, LongWithNullable = null, FloatValue = 1.0F, FloatWithNullable = null, DoubleValue = 1.0D, DoubleWithNullable = null, DecimalValue = 1.0M, DecimalWithNullable = null, DateTimeValue = DateTime.Parse("2000/01/02"), DateTimeWithNullable = null, ByteValues = new byte[] { 0xff, 0xff }, BoolValue = true, BoolWithNullable = null, GuidValue = Guid.Parse("00000000-0000-0000-0000-000000000000"), GuidWithNullable = null, EnumValue = BulkCopyMapTestRow.EnumValueOption.First });

            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"(localdb)\ProjectsV13";
            builder.InitialCatalog = "YaminabeExtensions.Db";

            using var connection = new SqlConnection(builder.ToString());
            connection.Open();

            connection.BulkCopy<BulkCopyMapTestRow>(
                "BulkCopyMapTestTable",
                rows,
                true,
                SqlBulkCopyOptions.Default,
                null
                );

            connection.BulkCopyToTemporaryTable<BulkCopyMapTestRow>(
                "#BulkCopyMapTestTable",
                rows,
                true,
                SqlBulkCopyOptions.Default,
                null
                );
        }

        #endregion

        #region -------------------- BulkDelete --------------------

        /// <summary>
        /// <see cref="SqlConnectionExtensions.BulkDelete{T}(SqlConnection, string, IEnumerable{T}, SqlTransaction)"/> の正常系検証を実施します。
        /// </summary>
        /// <remarks>
        /// 匿名型に主キーマーカーを指定しないパターン。
        /// </remarks>
        [TestCategory("SqlConnection:BulkDelete")]
        [TestMethod]
        public void BulkDeleteDynamic1SuccessTest()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"(localdb)\ProjectsV13";
            builder.InitialCatalog = "YaminabeExtensions.Db";

            using var connection = new SqlConnection(builder.ToString());
            using var command = new SqlCommand(string.Empty, connection);
            connection.Open();

            // 宛先テーブルに更新対象データセット
            var initialSql = @"
TRUNCATE TABLE [BulkSqlWorkTable];
INSERT INTO [BulkSqlWorkTable]
VALUES
(1, 1, '山田', NULL),
(2, 1, '田中', 16),
(3, 1, '佐藤', 20)
";
            command.CommandText = initialSql;
            command.ExecuteNonQuery();

            // 一括削除
            connection.BulkDelete(
                "BulkSqlWorkTable", 
                new[] { 
                    new { Id = 1, SubId = 1 }, 
                    new { Id = 2, SubId = 1 }, 
                    new { Id = 3, SubId = 3 } 
                    }, 
                null
                );
        }

        /// <summary>
        /// <see cref="SqlConnectionExtensions.BulkDelete{T}(SqlConnection, string, IEnumerable{T}, SqlTransaction)"/> の正常系検証を実施します。
        /// </summary>
        /// <remarks>
        /// 匿名型に主キーマーカーを指定するパターン。
        /// </remarks>
        [TestCategory("SqlConnection:BulkDelete")]
        [TestMethod]
        public void BulkDeleteDynamic2SuccessTest()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"(localdb)\ProjectsV13";
            builder.InitialCatalog = "YaminabeExtensions.Db";

            using var connection = new SqlConnection(builder.ToString());
            using var command = new SqlCommand(string.Empty, connection);
            connection.Open();

            // 宛先テーブルに更新対象データセット
            var initialSql = @"
TRUNCATE TABLE [BulkSqlWorkTable];
INSERT INTO [BulkSqlWorkTable]
VALUES
(1, 1, '山田', NULL),
(2, 1, '田中', 16),
(3, 1, '佐藤', 20)
";
            command.CommandText = initialSql;
            command.ExecuteNonQuery();

            // 一括削除
            connection.BulkDelete(
                "BulkSqlWorkTable",
                new[] {
                    new { Id_ExPK = 1, SubId_ExPK = 1, Name = "山田", Age = null as int? },
                    new { Id_ExPK = 2, SubId_ExPK = 1, Name = "田中", Age = 16 as int? },
                    new { Id_ExPK = 3, SubId_ExPK = 3, Name = "佐藤", Age = 20 as int? }
                    },
                null
                );
        }

        /// <summary>
        /// <see cref="SqlConnectionExtensions.BulkDelete{T}(SqlConnection, string, IEnumerable{T}, SqlTransaction)"/> の正常系検証を実施します。
        /// </summary>
        /// <remarks>
        /// モデルパターン。
        /// </remarks>
        [TestCategory("SqlConnection:BulkDelete")]
        [TestMethod]
        public void BulkDeleteGenericSuccessTest()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"(localdb)\ProjectsV13";
            builder.InitialCatalog = "YaminabeExtensions.Db";

            using var connection = new SqlConnection(builder.ToString());
            using var command = new SqlCommand(string.Empty, connection);
            connection.Open();

            // 宛先テーブルに更新対象データセット
            var initialSql = @"
TRUNCATE TABLE [BulkSqlWorkTable];
INSERT INTO [BulkSqlWorkTable]
VALUES
(1, 1, '山田', NULL),
(2, 1, '田中', 16),
(3, 1, '佐藤', 20)
";
            command.CommandText = initialSql;
            command.ExecuteNonQuery();

            // 一括削除データ作成
            var rows = new List<BulkSqlWorkRow>();
            rows.Add(new BulkSqlWorkRow() { Id = 1, SubId = 1, Name = "山田", Age = null });
            rows.Add(new BulkSqlWorkRow() { Id = 2, SubId = 1, Name = "田中", Age = 16 });
            rows.Add(new BulkSqlWorkRow() { Id = 3, SubId = 3, Name = "佐藤", Age = 20 });

            // 一括削除
            connection.BulkDelete<BulkSqlWorkRow>("BulkSqlWorkTable", rows, null);
        }

        #endregion

        #region -------------------- BulkUpdate --------------------

        /// <summary>
        /// <see cref="SqlConnectionExtensions.BulkUpdate{T}(SqlConnection, string, IEnumerable{T}, SqlTransaction)"/> の正常系検証を実施します。
        /// </summary>
        /// <remarks>
        /// 匿名型パターン。
        /// </remarks>
        [TestCategory("SqlConnection:BulkUpdate")]
        [TestMethod]
        public void BulkUpdateDynamicSuccessTest()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"(localdb)\ProjectsV13";
            builder.InitialCatalog = "YaminabeExtensions.Db";

            using var connection = new SqlConnection(builder.ToString());
            using var command = new SqlCommand(string.Empty, connection);
            connection.Open();

            // 宛先テーブルに更新対象データセット
            var initialSql = @"
TRUNCATE TABLE [BulkSqlWorkTable];
INSERT INTO [BulkSqlWorkTable]
VALUES
(1, 1, '山田', NULL),
(2, 1, '田中', 16),
(3, 1, '佐藤', 20)
";
            command.CommandText = initialSql;
            command.ExecuteNonQuery();

            // 一括更新
            connection.BulkUpdate(
                "BulkSqlWorkTable",
                new[] {
                    new { Id_ExPK = 1, SubId_ExPK = 1, Name = "山田更新", Age = 100 as int? },
                    new { Id_ExPK = 2, SubId_ExPK = 1, Name = "田中更新", Age = null as int? },
                    new { Id_ExPK = 3, SubId_ExPK = 3, Name = "佐藤更新", Age = null as int? }
                    },
                null
                );
        }

        /// <summary>
        /// <see cref="SqlConnectionExtensions.BulkUpdate{T}(SqlConnection, string, IEnumerable{T}, SqlTransaction)"/> の正常系検証を実施します。
        /// </summary>
        /// <remarks>
        /// モデルパターン。
        /// </remarks>
        [TestCategory("SqlConnection:BulkUpdate")]
        [TestMethod]
        public void BulkUpdateGenericSuccessTest()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"(localdb)\ProjectsV13";
            builder.InitialCatalog = "YaminabeExtensions.Db";

            using var connection = new SqlConnection(builder.ToString());
            using var command = new SqlCommand(string.Empty, connection);
            connection.Open();

            // 宛先テーブルに更新対象データセット
            var initialSql = @"
TRUNCATE TABLE [BulkSqlWorkTable];
INSERT INTO [BulkSqlWorkTable]
VALUES
(1, 1, '山田', NULL),
(2, 1, '田中', 16),
(3, 1, '佐藤', 20)
";
            command.CommandText = initialSql;
            command.ExecuteNonQuery();

            // 一括更新データ作成
            var rows = new List<BulkSqlWorkRow>();
            rows.Add(new BulkSqlWorkRow() { Id = 1, SubId = 1, Name = "山田更新", Age = 100 });
            rows.Add(new BulkSqlWorkRow() { Id = 2, SubId = 1, Name = "田中更新", Age = null });
            rows.Add(new BulkSqlWorkRow() { Id = 3, SubId = 3, Name = "佐藤更新", Age = null });

            // 一括削除
            connection.BulkUpdate<BulkSqlWorkRow>(
                "BulkSqlWorkTable", 
                rows, 
                null
                );
        }

        #endregion

        #region -------------------- BulkMerge --------------------

        /// <summary>
        /// <see cref="SqlConnectionExtensions.BulkMerge{T}(SqlConnection, string, IEnumerable{T}, SqlTransaction, bool)"/> の正常系検証を実施します。
        /// </summary>
        /// <remarks>
        /// 匿名型パターン。
        /// </remarks>
        [TestCategory("SqlConnection:BulkMerge")]
        [TestMethod]
        public void BulkMergeDynamicSuccessTest()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"(localdb)\ProjectsV13";
            builder.InitialCatalog = "YaminabeExtensions.Db";

            using var connection = new SqlConnection(builder.ToString());
            using var command = new SqlCommand(string.Empty, connection);
            connection.Open();

            // 宛先テーブルに更新対象データセット
            var initialSql = @"
TRUNCATE TABLE [BulkSqlWorkTable];
INSERT INTO [BulkSqlWorkTable]
VALUES
(1, 1, '山田', NULL),
(2, 1, '田中', 16),
(3, 1, '佐藤', 20)
";
            command.CommandText = initialSql;
            command.ExecuteNonQuery();

            // 一括更新
            connection.BulkMerge(
                "BulkSqlWorkTable",
                new[] {
                    new { Id_ExPK = 1, SubId_ExPK = 1, Name = "山田更新", Age = 100 as int? },
                    new { Id_ExPK = 2, SubId_ExPK = 1, Name = "田中更新", Age = null as int? },
                    new { Id_ExPK = 3, SubId_ExPK = 3, Name = "佐藤更新", Age = null as int? }
                    },
                null,
                true
                );
        }

        /// <summary>
        /// <see cref="SqlConnectionExtensions.BulkMerge{T}(SqlConnection, string, IEnumerable{T}, SqlTransaction, bool)"/> の正常系検証を実施します。
        /// </summary>
        /// <remarks>
        /// モデルパターン。
        /// </remarks>
        [TestCategory("SqlConnection:BulkMerge")]
        [TestMethod]
        public void BulkMergeGenericSuccessTest()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"(localdb)\ProjectsV13";
            builder.InitialCatalog = "YaminabeExtensions.Db";

            using var connection = new SqlConnection(builder.ToString());
            using var command = new SqlCommand(string.Empty, connection);
            connection.Open();

            // 宛先テーブルに更新対象データセット
            var initialSql = @"
TRUNCATE TABLE [BulkSqlWorkTable];
INSERT INTO [BulkSqlWorkTable]
VALUES
(1, 1, '山田', NULL),
(2, 1, '田中', 16),
(3, 1, '佐藤', 20)
";
            command.CommandText = initialSql;
            command.ExecuteNonQuery();

            // 一括更新データ作成
            var rows = new List<BulkSqlWorkRow>();
            rows.Add(new BulkSqlWorkRow() { Id = 1, SubId = 1, Name = "山田更新", Age = 100 });
            rows.Add(new BulkSqlWorkRow() { Id = 2, SubId = 1, Name = "田中更新", Age = null });
            rows.Add(new BulkSqlWorkRow() { Id = 3, SubId = 3, Name = "佐藤更新", Age = null });

            // 一括削除
            connection.BulkMerge<BulkSqlWorkRow>(
                "BulkSqlWorkTable",
                rows,
                null,
                true
                );
        }

        /// <summary>
        /// アンマッチ時の宛先テーブルのデータ削除機能の無効化の検証を実施します。
        /// </summary>
        [TestCategory("SqlConnection:BulkMerge")]
        [TestMethod]
        public void BulkMergeNotMatchedNoDeleteSuccessTest()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"(localdb)\ProjectsV13";
            builder.InitialCatalog = "YaminabeExtensions.Db";

            using var connection = new SqlConnection(builder.ToString());
            using var command = new SqlCommand(string.Empty, connection);
            connection.Open();

            // 宛先テーブルに更新対象データセット
            var initialSql = @"
TRUNCATE TABLE [BulkSqlWorkTable];
INSERT INTO [BulkSqlWorkTable]
VALUES
(1, 1, '山田', NULL),
(2, 1, '田中', 16),
(3, 1, '佐藤', 20)
";
            command.CommandText = initialSql;
            command.ExecuteNonQuery();

            // 一括更新データ作成
            var rows = new List<BulkSqlWorkRow>();
            rows.Add(new BulkSqlWorkRow() { Id = 1, SubId = 1, Name = "山田更新", Age = 100 });
            rows.Add(new BulkSqlWorkRow() { Id = 2, SubId = 1, Name = "田中更新", Age = null });
            rows.Add(new BulkSqlWorkRow() { Id = 3, SubId = 3, Name = "佐藤更新", Age = null });

            // 一括削除
            connection.BulkMerge<BulkSqlWorkRow>(
                "BulkSqlWorkTable",
                rows,
                null,
                false
                );
        }

        #endregion

        #endregion
    }
}
