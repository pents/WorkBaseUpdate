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
        public File_OSMT(string filePath) : base()
        {
            FilePath = filePath;
        }

        private DataTable getTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Dep", typeof(string));
            dt.Columns.Add("MOL", typeof(string));
            dt.Columns.Add("MaterialID", typeof(string));
            dt.Columns.Add("CountMaterial", typeof(float));

            return dt;
        }

        public override void ReadFile()
        {
            DataTable dataTable = getTable();
            cHandle.ExecuteQuery("DELETE FROM TBBalanceDep");
            int linesCount = totalLines(FilePath);
            int currentLineNumber = 0;
            using (StreamReader fileStream = new StreamReader(FilePath, Encoding.Default))
            {
                string currentLine = "";
                while ((currentLine = fileStream.ReadLine()).Length > 5)
                {
                    dataTable.Rows.Add(
                        currentLine.Substring(0, 3), currentLine.Substring(3, 2),
                        currentLine.Substring(5, 12), currentLine.Substring(113, 12)
                        );

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
