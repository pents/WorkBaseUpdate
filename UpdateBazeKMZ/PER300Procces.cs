using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBConnectionLib;
using System.IO;
using System.Data;

namespace UpdateBazeKMZ
{
    
    public class File_PER300 : FileProcces
    {
        public File_PER300(string filePath) : base(filePath) { dataTable = getTable(); deleteRequired = true; }

        private DataTable getTable()
        {
            DataTable dt = new DataTable();

            dt.TableName = "TBStorageBalance";

            dt.Columns.Add("MaterialID", typeof(string));
            dt.Columns.Add("Storage", typeof(string));
            dt.Columns.Add("BalanceDay", typeof(float));
            dt.Columns.Add("BalanceMonth", typeof(float));
            return dt;
        }

        protected override void processFile(string currentLine)
        {
            dataTable.Rows.Add(
                currentLine.Substring(3,12).Trim(), 
                currentLine.Substring(0,3).Trim(), 
                float.Parse(currentLine.Substring(15,11).Trim().Replace('.',',')), 
                float.Parse(currentLine.Substring(26,11).Trim().Replace('.', ','))
                );

        }
    }
}
