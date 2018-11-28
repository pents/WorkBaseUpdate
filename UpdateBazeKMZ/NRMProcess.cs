using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UpdateBazeKMZ
{
    public class File_NRM : FileProcces
    {
        public File_NRM(string filePath) : base(filePath) { dataTable = getTable(); deleteRequired = true; }

        private DataTable getTable()
        {
            DataTable dt = new DataTable();

            dt.TableName = "TBNRM";

            dt.Columns.Add("Detail", typeof(string));
            dt.Columns.Add("Material", typeof(string));
            dt.Columns.Add("NRM", typeof(float));
            dt.Columns.Add("EIK", typeof(string));
            dt.Columns.Add("EIN", typeof(string));
            dt.Columns.Add("Mass", typeof(float));
            dt.Columns.Add("Count", typeof(int));
            dt.Columns.Add("Prof", typeof(string));
            dt.Columns.Add("Route", typeof(string));

            return dt;
        }

        protected override void processFile(string currentLine)
        {

            double NRM = Convert.ToDouble(currentLine.Substring(40, 12).Trim().Replace('.', ','));
            double Mass = Convert.ToDouble(currentLine.Substring(58, 9).Trim().Replace('.', ','));
            int Count = Convert.ToInt32(currentLine.Substring(69, 5).Trim());

            dataTable.Rows.Add(
                    currentLine.Substring(3, 25).Trim(),      /*Detail*/
                    currentLine.Substring(28, 12).Trim(),     /*Material*/
                    (float)NRM,                               /*NRM*/
                    currentLine.Substring(52, 3).Trim(),      /*EIK*/
                    currentLine.Substring(55, 3).Trim(),      /*EIN*/
                    (float)Mass,                              /*Mass*/
                    Count,                                    /*Count*/
                    currentLine.Substring(74, 20).Trim(),     /*Prof*/
                    currentLine.Substring(94, 15).Trim()      /*Route*/
            );
            // берем по 70к строк
            OnProgressAsyncWriteRequired(70000);

        }

    }
}
