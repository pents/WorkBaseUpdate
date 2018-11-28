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
        public File_GRP(string filePath) : base(filePath)
        {
            dataTable = getTable();
            deleteRequired = false;
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
            string DetailID = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM TBDetailID WHERE Detail = '{0}'",
                              currentLine.Substring(3, 25).Trim()));
            if (DetailID != "0")
            {
                cHandle.ExecuteQuery(string.Format("DELETE FROM TBDetailID WHERE Detail = '{0}'",
                              currentLine.Substring(3, 25).Trim()));
            }
            dataTable.Rows.Add(currentLine.Substring(3, 25).Trim(),
                   currentLine.Substring(28, 1).Trim(),
                   currentLine.Substring(88).Trim(),
                   currentLine.Substring(46, 25).Trim(),
                   currentLine.Substring(73, 15).Trim()
                   );
            OnProgressAsyncWriteRequired(50000);
        }
    }
}
