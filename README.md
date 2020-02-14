# YaminabeExtensions

闇鍋がごとく思いもよらない便利機能がみつかる・・・かもしれない。  
そんなプロジェクトです。

## こんなことができる

`SqlConnection`の拡張メソッドに`SqlBulkCopy`をラップし、匿名型を利用して一括登録ができるようにしたサンプルです。

```tsql
CREATE TABLE [dbo].[BulkCopyWorkTable]
(
    [Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NULL
)
```

```c#
var connectionStringBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder();
connectionStringBuilder.DataSource = @"(localdb)\ProjectsV13";
connectionStringBuilder.InitialCatalog = "YaminabeExtensions.Db";

var connection = new System.Data.SqlClient.SqlConnection(connectionStringBuilder.ToString());
connection.BulkCopy(
    "BulkCopyWorkTable",
    new[] {
        new { Id = 1, Name = "山田" },
        new { Id = 2, Name = "田中" },
        new { Id = 3, Name = "佐藤" }
        },
    true,
    System.Data.SqlClient.SqlBulkCopyOptions.Default,
    null
    );
```

BulkCopyWorkTable
|Id|Name|
|:---|:---|
|1|山田|
|2|田中|
|3|佐藤|

同様にデータモデルを指定できるようにしたサンプルです。

```c#
public class BulkCopyWorkRow
{
    public int Id { get; set; }

    // 宛先テーブルのカラム名と異なるプロパティをマッピング
    [BulkCopy( ColumnName = "Name" )]
    public string Namae { get; set; }

    // 宛先テーブルに存在しない項目は除外対象としてマーク
    [BulkCopy( Ignore = true )]
    public System.DateTime ApplyDateTime { get; set; }
}
```
```c#
var bulkCopyWorkRows = new List<BulkCopyWorkRow>();
bulkCopyWorkRows.Add(new BulkCopyWorkRow() { Id = 1, Namae = "山田", ApplyDateTime = System.DateTime.Now });
bulkCopyWorkRows.Add(new BulkCopyWorkRow() { Id = 2, Namae = "田中", ApplyDateTime = System.DateTime.Now });
bulkCopyWorkRows.Add(new BulkCopyWorkRow() { Id = 3, Namae = "佐藤", ApplyDateTime = System.DateTime.Now });

var connectionStringBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder();
connectionStringBuilder.DataSource = @"(localdb)\ProjectsV13";
connectionStringBuilder.InitialCatalog = "YaminabeExtensions.Db";

var connection = new System.Data.SqlClient.SqlConnection(connectionStringBuilder.ToString());
connection.BulkCopy<BulkCopyWorkRow>(
    "BulkCopyWorkTable",
    bulkCopyWorkRows,
    true,
    System.Data.SqlClient.SqlBulkCopyOptions.Default,
    null
    );
```

BulkCopyWorkTable
|Id|Name|
|:---|:---|
|1|山田|
|2|田中|
|3|佐藤|
