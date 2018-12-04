using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateBazeKMZ
{
    public class File_M106 : FileProcces
    {
        public File_M106(string filePath) : base(filePath) { dataTable = getTable(); deleteRequired = false; updateRequired = true; }

        private string _checkRecord = "";

        private DataTable getTable()
        {
            DataTable dt = new DataTable();

            dt.TableName = "TBAssemblyes";

            dt.Columns.Add("DetailWhereTo", typeof(string));
            dt.Columns.Add("DetailWhat",    typeof(string));
            dt.Columns.Add("Count",         typeof(string));

            return dt;
        }

        protected override void processFile(string currentLine)
        {

            dataTable.Rows.Add(currentLine.Substring(0, 25).Trim(),
                                currentLine.Substring(25, 25).Trim(),
                                currentLine.Substring(50).Trim());
            OnProgressAsyncWriteRequired(70000);
       
        }
    }
}
