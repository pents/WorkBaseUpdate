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
        public File_PER300(string filePath) : base(filePath) { }

        private DataTable getTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("MaterialID", typeof(string));
            dt.Columns.Add("Storage", typeof(string));
            dt.Columns.Add("BalanceDay", typeof(float));
            dt.Columns.Add("BalanceMonth", typeof(float));
            return dt;
        }

        public override void ReadFile()
        {
            DataTable dataTable = getTable();
            cHandle.ExecuteQuery("DELETE FROM TBStorageBalance");
            int linesCount = totalLines(FilePath);
            int currentLineNumber = 0;
            using (StreamReader fileStream = new StreamReader(FilePath, Encoding.Default))
            {
                string currentLine = "";
                while ((currentLine = fileStream.ReadLine()).Length > 5)
                {
                    dataTable.Rows.Add(
                        currentLine.Substring(3,12), currentLine.Substring(0,3), 
                        currentLine.Substring(15,11), currentLine.Substring(26,11)
                        );

                    if ((currentLineNumber % (linesCount / 100) == 0) || (currentLineNumber == linesCount - 1))
                    {
                        OnProgressChanged(new LoadProgressArgs(currentLineNumber, linesCount - 1)); // текущее состояние загрузки
                    }
                    currentLineNumber++;
                }
            }

            cHandle.InsertBulkQuery(dataTable, "TBStorageBalance");
            OnProgressCompleted();
        }
    }
}
