using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateBazeKMZ
{
    public class CSMRProcess : FileProcces
    {

        private DataTable dataTable;

        public CSMRProcess(string filePath) : base(filePath) { dataTable = getTable(); }

        private DataTable getTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("MaterialNumber",   typeof(string));
            dt.Columns.Add("ItemType",         typeof(string));
            dt.Columns.Add("MaterialName",     typeof(string));
            dt.Columns.Add("MainNomenclature", typeof(string));
            dt.Columns.Add("GLCode",           typeof(string));
            dt.Columns.Add("KEI",              typeof(string));
            dt.Columns.Add("AccountPrice",     typeof(decimal));
            dt.Columns.Add("JAccountPrice",    typeof(string));
            dt.Columns.Add("ReserveRate",      typeof(string));
            dt.Columns.Add("TransitRate",      typeof(string));
            dt.Columns.Add("WNumber",          typeof(string));
            dt.Columns.Add("PromPrice",        typeof(decimal));
            dt.Columns.Add("JPromPrice",       typeof(string));
            dt.Columns.Add("Date",             typeof(string));
            dt.Columns.Add("TypeOfAcceptance", typeof(string));
            return dt;
        }

        private void updateTable(string currentLine)
        {
            
            string date = string.Format("{0}.{1}.{2}",
            currentLine.Substring(168, 2),
            currentLine.Substring(170, 2),
            currentLine.Substring(172, 2));

            decimal acPrice = Convert.ToDecimal(currentLine.Substring(100, 13).Trim().Replace('.', ',')); //Учетная цена
            decimal pPrice = Convert.ToDecimal(currentLine.Substring(140, 13).Trim().Replace('.', ','));  //Перспективная цена

            dataTable.Rows.Add(
                currentLine.Substring(0, 12), // MaterialNumber
                currentLine.Substring(12, 1), // ItemType
                currentLine.Substring(13, 80),// MaterialName 
                currentLine.Substring(93, 1), // MainNomenclature
                currentLine.Substring(94, 2),   // GLCode
                currentLine.Substring(96, 3), // KEI
                acPrice, // AccountPrice
                currentLine.Substring(113, 15), // JAccountPrice
                currentLine.Substring(128, 3), // ReserveRate
                currentLine.Substring(131, 5), // TransitRate
                currentLine.Substring(136, 3), // WNumber
                pPrice, // PromPrice
                currentLine.Substring(153, 15), // JPromPrice
                date, // Date
                currentLine.Substring(174, 1) // TypeOfAcceptance
            );
        }

        public override void ReadFile()
        {
            OnProgressNotify("Инициализация...");
            int linesCount = totalLines(FilePath);
            int currentLineNumber = 1;

            string semiResult = "";

             

            using (StreamReader fileStream = new StreamReader(FilePath, Encoding.Default))
            {
                string currentLine = "";
                while ((currentLine = fileStream.ReadLine()).Length > 5)
                {
                    semiResult = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM TBStorage WHERE Number = '{0}' AND GroupLeader = '{1}'",
                                                             currentLine.Substring(136, 3), currentLine.Substring(94, 2)));
                    if (semiResult == "0")
                    {
                        cHandle.ExecuteQuery(string.Format("INSERT INTO TBStorage (Number, GroupLeader) VALUES ({0},{1})", 
                                             currentLine.Substring(136,3), currentLine.Substring(94,2)));
                    }

                    semiResult = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM TBMaterial WHERE MaterialNumber = {0}", 
                                                             currentLine.Substring(0,12)));

                    if (semiResult == "0")
                    {
                        cHandle.ExecuteQuery(string.Format("DELETE FROM TBMaterial WHERE MaterialNumber = {0}", currentLine.Substring(0, 12)));
                        updateTable(currentLine);
                    }
                    else
                    {
                        updateTable(currentLine);
                    }

                    // каждые 50k строк запускаем поток записи и сбрасываем в него накопившиеся данные
                    if ((currentLineNumber % 50000 == 0) || (currentLineNumber == linesCount - 1))
                    {
                        WriteAsync(dataTable, "TBMaterial");
                        dataTable.Clear();  // очистка таблицы для ввода новых данных
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
