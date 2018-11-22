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
        public File_Marsh(string filePath) : base(filePath) { }


        private DataTable getTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("DetailID", typeof(int));
            dt.Columns.Add("Route", typeof(string));

            return dt;
        }

        public override void ReadFile()
        {
            DataTable dataTable = getTable();
            cHandle.ExecuteQuery("DELETE FROM TBRoutes");

            int linesCount = totalLines(FilePath);
            int currentLineNumber = 0;
            using (StreamReader fileStream = new StreamReader(FilePath, Encoding.Default))
            {
                string currentLine = "";
                while ((currentLine = fileStream.ReadLine()).Length > 5)
                {
                    string DetailID = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM TBDetailsID WHERE Detail = {0}", currentLine.Substring(0, 25)));
                    if (DetailID == null) continue;

                    dataTable.Rows.Add(DetailID, currentLine.Substring(25).Trim());

                    if ((currentLineNumber % (linesCount / 100) == 0) || (currentLineNumber == linesCount - 1))
                    {
                        OnProgressChanged(new LoadProgressArgs(currentLineNumber, linesCount - 1)); // текущее состояние загрузки
                    }
                    currentLineNumber++;
                }
            }

            cHandle.InsertBulkQuery(dataTable, "TBBalanceDep");
            OnProgressCompleted();
        }
    }
}
