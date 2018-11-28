using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateBazeKMZ
{
    public class File_Marsh : FileProcces
    {
        public File_Marsh(string filePath) : base(filePath) { dataTable = getTable(); deleteRequired = true; }


        private DataTable getTable()
        {
            DataTable dt = new DataTable();

            dt.TableName = "TBRoutes";

            dt.Columns.Add("DetailID", typeof(int));
            dt.Columns.Add("Route", typeof(string));

            return dt;
        }

        protected override void processFile(string currentLine)
        {

            string DetailID = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM TBDetailsID WHERE Detail = {0}", currentLine.Substring(0, 25)));

            if (DetailID == "0")
            {
                OnProgressNotify(string.Format("Для Detail = {0} не найден DetailID", currentLine.Substring(0, 25)));
            }else
            {
                dataTable.Rows.Add(int.Parse(DetailID), currentLine.Substring(25).Trim());
            }
        }
    }
}
