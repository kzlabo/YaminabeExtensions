# YaminabeExtensions

# DEMO

```
var connectionStringBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder();
connectionStringBuilder.DataSource = @"(localdb)\ProjectsV13";
connectionStringBuilder.InitialCatalog = "YaminabeExtensions.Db";

var connection = new System.Data.SqlClient.SqlConnection(connectionStringBuilder.ToString());
connection.BulkCopy(
    "BulkCopyWorkTable",
    new[] {
        new { Id = 1, Name = "éRìc" },
        new { Id = 2, Name = "ìcíÜ" },
        new { Id = 3, Name = "ç≤ì°" }
        },
    true,
    System.Data.SqlClient.SqlBulkCopyOptions.Default,
    null
    );
```