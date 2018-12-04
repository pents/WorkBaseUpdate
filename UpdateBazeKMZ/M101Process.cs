using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateBazeKMZ
{
    public class File_M101 : FileProcces
    {
        public File_M101(string filePath) : base(filePath, requiredOperation.UPDATE)
        {
            dataTable = getTable();
        }

        private DataTable getTable()
        {
            cHandle.ExecuteQuery("UPDATE TBCodeProducts SET Used = 0");

            DataTable dt = new DataTable();

            dt.TableName = "TBCodeProducts";

            dt.Columns.Add("PartNumber", typeof(string));
            dt.Columns.Add("CodeDetails", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Used", typeof(bool));
            return dt;
        }

        protected override void processFile(string currentLine)
        {
            dataTable.Rows.Add(currentLine.Substring(0, 10).Trim(),
                    currentLine.Substring(10, 25).Trim(),
                    currentLine.Substring(35, 15).Trim(),
                    true
                    );

        }
    }
}
