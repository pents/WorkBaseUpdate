using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateBazeKMZ
{
    public class CSMRProcess : FileProcces
    {
        public CSMRProcess(string filePath) : base(filePath) { }



        private DataTable getTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("DetailID", typeof(int));
            dt.Columns.Add("Operation", typeof(string));
            dt.Columns.Add("DepID", typeof(int));
            dt.Columns.Add("EquipmentID", typeof(int));
            dt.Columns.Add("Rank", typeof(int));
            dt.Columns.Add("NMin", typeof(float));
            dt.Columns.Add("Price", typeof(float));
            dt.Columns.Add("StMin", typeof(float));
            dt.Columns.Add("Procent", typeof(int));
            dt.Columns.Add("Mex", typeof(int));
            dt.Columns.Add("Kvn", typeof(int));
            dt.Columns.Add("PrUch", typeof(string));

            return dt;
        }


        public override void ReadFile()
        {
            OnProgressNotify("Инициализация...");
            int linesCount = totalLines(FilePath);
            int currentLineNumber = 1;

            DataTable dataTable = getTable(); // создание таблицы для ввода данных



        }
    }
}
