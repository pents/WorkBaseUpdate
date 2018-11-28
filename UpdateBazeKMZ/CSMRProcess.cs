using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateBazeKMZ
{
    public class File_CSMR : FileProcces
    {

        public File_CSMR(string filePath) : base(filePath) { dataTable = getTable(); deleteRequired = false; }

        private DataTable getTable()
        {
            DataTable dt = new DataTable();

            dt.TableName = "TBMaterial";

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
                currentLine.Substring(0, 12).Trim(), // MaterialNumber
                currentLine.Substring(12, 1).Trim(), // ItemType
                currentLine.Substring(13, 80).Trim(),// MaterialName 
                currentLine.Substring(93, 1).Trim(), // MainNomenclature
                currentLine.Substring(94, 2).Trim(),   // GLCode
                currentLine.Substring(96, 3).Trim(), // KEI
                acPrice, // AccountPrice
                currentLine.Substring(113, 15).Trim(), // JAccountPrice
                currentLine.Substring(128, 3).Trim(), // ReserveRate
                currentLine.Substring(131, 5).Trim(), // TransitRate
                currentLine.Substring(136, 3).Trim(), // WNumber
                pPrice, // PromPrice
                currentLine.Substring(153, 15).Trim(), // JPromPrice
                date, // Date
                currentLine.Substring(174, 1).Trim() // TypeOfAcceptance
            );
        }

        protected override void processFile(string currentLine)
        {

            string semiResult = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM TBStorage WHERE Number = '{0}' AND GroupLeader = '{1}'",
                                                        currentLine.Substring(136, 3).Trim(), currentLine.Substring(94, 2).Trim()));
            if (semiResult == "0")
            {
                cHandle.ExecuteQuery(string.Format("INSERT INTO TBStorage (Number, GroupLeader) VALUES ('{0}','{1}')", 
                                        currentLine.Substring(136,3).Trim(), currentLine.Substring(94,2).Trim()));
            }

            semiResult = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM TBMaterial WHERE MaterialNumber = '{0}'", 
                                                        currentLine.Substring(0,12).Trim()));

            if (semiResult != "0")
            {
                cHandle.ExecuteQuery(string.Format("DELETE FROM TBMaterial WHERE MaterialNumber = '{0}'", currentLine.Substring(0, 12).Trim()));
            }
            updateTable(currentLine);

            // каждые 50k строк запускаем поток записи и сбрасываем в него накопившиеся данные
            OnProgressAsyncWriteRequired(50000);                   

            
        }
    }
}
