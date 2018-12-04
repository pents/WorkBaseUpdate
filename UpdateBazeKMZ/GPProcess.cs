using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateBazeKMZ
{
    public class File_GRP : FileProcces
    {
        public File_GRP(string filePath) : base(filePath, requiredOperation.UPDATE)
        {
            dataTable = getTable();
        }

        private DataTable getTable()
        {
            DataTable dt = new DataTable();

            dt.TableName = "TBDetailID";

            dt.Columns.Add("Detail",   typeof(string));
            dt.Columns.Add("Type",     typeof(string));
            dt.Columns.Add("Name",     typeof(string));
            dt.Columns.Add("FUse",     typeof(string));
            dt.Columns.Add("BaseProd", typeof(string));

            return dt;
        }

        protected override void processFile(string currentLine)
        {
            
            dataTable.Rows.Add(currentLine.Substring(3, 25).Trim(),
                   currentLine.Substring(28, 1).Trim(),
                   currentLine.Substring(88).Trim(),
                   currentLine.Substring(45, 25).Trim(),
                   currentLine.Substring(73, 15).Trim()
                   );
            OnProgressAsyncWriteRequired(50000);
        }
    }
}
