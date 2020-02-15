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
var builder = new SqlConnectionStringBuilder();
builder.DataSource = @"(localdb)\ProjectsV13";
builder.InitialCatalog = "YaminabeExtensions.Db";

var connection = new SqlConnection(builder.ToString());
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
```

3件のレコードが`BulkCopyWorkTable`に一括挿入されます。
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
    public DateTime ApplyDateTime { get; set; }
}
```
```c#
var rows = new List<BulkCopyWorkRow>();
rows.Add(new BulkCopyWorkRow() { Id = 1, Namae = "山田", ApplyDateTime = DateTime.Now });
rows.Add(new BulkCopyWorkRow() { Id = 2, Namae = "田中", ApplyDateTime = DateTime.Now });
rows.Add(new BulkCopyWorkRow() { Id = 3, Namae = "佐藤", ApplyDateTime = DateTime.Now });

var builder = new SqlConnectionStringBuilder();
builder.DataSource = @"(localdb)\ProjectsV13";
builder.InitialCatalog = "YaminabeExtensions.Db";

var connection = new SqlConnection(builder.ToString());
connection.BulkCopy<BulkCopyWorkRow>(
    "BulkCopyWorkTable",
    rows,
    true,
    SqlBulkCopyOptions.Default,
    null
    );
```

3件のレコードが`BulkCopyWorkTable`に一括挿入されます。
|Id|Name|
|:---|:---|
|1|山田|
|2|田中|
|3|佐藤|
