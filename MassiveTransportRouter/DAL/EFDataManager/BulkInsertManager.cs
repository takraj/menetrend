using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.DataAccess.EFDataManager
{
    public class BulkInsertManager
    {
        protected string tableName;
        protected List<string> columns = new List<string>();
        protected List<object[]> data = new List<object[]>();

        public BulkInsertManager(string tableName)
        {
            this.tableName = tableName;
        }

        public void AddColumn(string columnName)
        {
            this.columns.Add(columnName);
        }

        public void AddRow(params object[] data)
        {
            this.data.Add(data);
        }

        public void DoBulkInsert()
        {
            if (tableName.Length < 1 || columns.Count < 1 || data.Count < 1)
            {
                throw new Exception("Invalid table.");
            }

            DataTable dt = new DataTable();
            columns.ForEach(c => dt.Columns.Add(new DataColumn(c)));
            data.ForEach(d => dt.Rows.Add(d));

            using (SqlConnection cn = new SqlConnection(EF_GtfsDbContext.ConnectionString))
            {
                cn.Open();
                using (SqlBulkCopy copy = new SqlBulkCopy(cn))
                {
                    columns.ForEach(c => copy.ColumnMappings.Add(c, c));
                    copy.DestinationTableName = tableName;
                    copy.WriteToServer(dt);
                }
            }
        }
    }
}
