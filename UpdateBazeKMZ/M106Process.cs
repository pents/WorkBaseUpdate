using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateBazeKMZ
{
    public class M106Process : FileProcces
    {
        public M106Process(string filePath) : base(filePath) { dataTable = getTable(); deleteRequired = false; }

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
            _checkRecord = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM TBAssemblyes WHERE DetailWhereTo = {0} AND DetailWhat = {1}",
                                                       currentLine.Substring(0,25).Trim(),
                                                       currentLine.Substring(25,25).Trim()));

            if (_checkRecord == "0")
            {
                dataTable.Rows.Add(currentLine.Substring(0, 25).Trim(),
                                   currentLine.Substring(25, 25).Trim(),
                                   currentLine.Substring(50).Trim());
            }
        }
    }
}
