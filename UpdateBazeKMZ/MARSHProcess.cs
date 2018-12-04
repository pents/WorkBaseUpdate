using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateBazeKMZ
{
    public class File_Marsh : FileProcces
    {
        public File_Marsh(string filePath) : base(filePath)
        {
            dataTable = getTable();
            deleteRequired = true;
            loadDetails();
        }

        private Hashtable HTDetail = new Hashtable();

        private void loadDetails()
        {
            DataTable Data_TBDeps = cHandle.GetDataTable("SELECT ID, Detail FROM TBDetailID", "TBDetailID");

            foreach (DataRow row in Data_TBDeps.Rows)
            {
                HTDetail.Add(row["Detail"].ToString(), row["ID"]);
            }
            Data_TBDeps.Clear();
        }

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

            if (HTDetail[currentLine.Substring(0, 25).Trim()] == null)
            {
                OnProgressNotify(string.Format("Для Detail = {0} не найден DetailID", currentLine.Substring(0, 25).Trim()));
            }else
            {
               
               dataTable.Rows.Add(int.Parse(HTDetail[currentLine.Substring(0, 25).Trim()].ToString()), currentLine.Substring(25).Trim());
            }
        }
    }
}
