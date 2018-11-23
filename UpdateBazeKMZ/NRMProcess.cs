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
    public class NRMProcess : FileProcces
    {
        public NRMProcess(string filePath) : base(filePath) { }

        private DataTable getTable()
        {
            DataTable dt = new DataTable();

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

        public override void ReadFile()
        {
            DataTable dataTable = getTable();
            cHandle.ExecuteQuery("DELETE FROM TBNRM");
   
            int linesCount = totalLines(FilePath);
            int currentLineNumber = 0;

            using (StreamReader fileStream = new StreamReader(FilePath, Encoding.Default))
            {
                string currentLine = "";
                while ((currentLine = fileStream.ReadLine()).Length > 5)
                {
                    double NRM = Convert.ToDouble(currentLine.Substring(40, 12).Trim().Replace('.', ','));
                    double Mass = Convert.ToDouble(currentLine.Substring(58, 9).Trim().Replace('.', ','));
                    int Count = Convert.ToInt32(currentLine.Substring(69, 5).Trim());

                    dataTable.Rows.Add(
                            currentLine.Substring(3, 25).Trim(),      /*Detail*/
                            currentLine.Substring(28, 12).Trim(),     /*Material*/
                            NRM.ToString().Replace(',', '.'),         /*NRM*/
                            currentLine.Substring(52, 3).Trim(),      /*EIK*/
                            currentLine.Substring(55, 3).Trim(),      /*EIN*/
                            Mass.ToString().Replace(',', '.'),        /*Mass*/
                            Count,                                    /*Count*/
                            currentLine.Substring(74, 20).Trim(),     /*Prof*/
                            currentLine.Substring(94, 15).Trim()      /*Route*/
                    );
                    // берем по 70к строк
                    if ((currentLineNumber % 70000 == 0) || (currentLineNumber == linesCount - 1))
                    {
                        WriteAsync(dataTable, "TBNRM"); // очистка таблицы для ввода новых данных
                        dataTable.Clear();
                    }

                    if ((currentLineNumber % (linesCount / 100) == 0) || (currentLineNumber == linesCount - 1))
                    {
                        OnProgressChanged(new LoadProgressArgs(currentLineNumber, linesCount - 1)); // текущее состояние загрузки
                    }
                    currentLineNumber++;

                }
            }

            OnProgressCompleted();
        }

    }
}
