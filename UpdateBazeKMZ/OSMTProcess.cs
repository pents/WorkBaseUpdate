using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateBazeKMZ
{
    public class File_OSMT : FileProcces
    {
        public File_OSMT(string filePath) : base(filePath, requiredOperation.DELETE)
        {
            dataTable = getTable(); 
        }

        private DataTable getTable()
        {
            DataTable dt = new DataTable();

            dt.TableName = "TBBalanceDep";

            dt.Columns.Add("Dep",           typeof(string));
            dt.Columns.Add("MOL",           typeof(string));
            dt.Columns.Add("MaterialID",    typeof(string));
            dt.Columns.Add("CountMaterial", typeof(float));

            return dt;
        }

        protected override void processFile(string currentLine)
        {

            dataTable.Rows.Add(
                currentLine.Substring(0, 3).Trim(), 
                currentLine.Substring(3, 2).Trim(),
                currentLine.Substring(5, 12).Trim(), 
                float.Parse(currentLine.Substring(113, 12).Trim().Replace('.',','))
                );

        }
    }
}
