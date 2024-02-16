using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Tool_DB_TableCol_Compare
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionStringA = "Data Source=127.0.0.1;Initial Catalog=DB_Test1;Integrated Security=True";
            string connectionStringB = "Data Source=127.0.0.2;Initial Catalog=DB_Test2;Integrated Security=True";

            using (var connectionA = new SqlConnection(connectionStringA))
            using (var connectionB = new SqlConnection(connectionStringB))
            {
                connectionA.Open();
                connectionB.Open();

                // 獲取A資料庫中的所有表和字段
                DataTable tablesA = connectionA.GetSchema("Tables");
                foreach (DataRow tableRowA in tablesA.Rows)
                {
                    string tableNameA = (string)tableRowA["TABLE_NAME"];
                    DataTable columnsA = connectionA.GetSchema("Columns", new[] { null, null, tableNameA });

                    // 獲取B資料庫中對應表的所有字段
                    DataTable columnsB = connectionB.GetSchema("Columns", new[] { null, null, tableNameA });

                    // 比對字段數量是否一致
                    if (columnsA.Rows.Count != columnsB.Rows.Count)
                    {
                        Console.WriteLine($"表 {tableNameA} 的字段數量不一致。");
                        continue;
                    }

                    // 比對每個字段的名稱和類型是否一致
                    foreach (DataRow columnRowA in columnsA.Rows)
                    {
                        string columnNameA = (string)columnRowA["COLUMN_NAME"];
                        string dataTypeA = (string)columnRowA["DATA_TYPE"];

                        DataRow matchingColumnB = columnsB.AsEnumerable()
                            .FirstOrDefault(row =>
                                (string)row["COLUMN_NAME"] == columnNameA &&
                                (string)row["DATA_TYPE"] == dataTypeA);

                        if (matchingColumnB == null)
                        {
                            Console.WriteLine($"表 {tableNameA} 的字段 {columnNameA} 類型不一致。");
                        }
                    }
                }
            }

            Console.WriteLine("比對完成。");
            Console.ReadLine();

        }
    }
}
