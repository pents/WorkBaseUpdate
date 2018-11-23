using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UpdateBazeKMZ
{
    public class PTNProccess : FileProcces
    {
        public PTNProccess(string filePath) : base(filePath) { }

        private Queue<DataTable> _dataPool = new Queue<DataTable>();
        private bool _inProgress = false;


        private DataTable getTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Production", typeof(string));
            dt.Columns.Add("Assemblyes", typeof(string));
            dt.Columns.Add("DetailID", typeof(int));
            dt.Columns.Add("CountAssembly", typeof(float));
            dt.Columns.Add("CountProductions", typeof(float));
            dt.Columns.Add("TypeDetais", typeof(int));
            dt.Columns.Add("TypeAssembly", typeof(int));
            dt.Columns.Add("Sign", typeof(int));
            dt.Columns.Add("PrimaryApplicability", typeof(string));

            return dt;
        }

        public override void ReadFile()
        {
            cHandle.ExecuteQuery("DELETE FROM TBM104");
            _inProgress = true;
            int linesCount = totalLines(FilePath);
            int currentLineNumber = 1;

            DataTable dataTable = getTable(); // создание таблицы для ввода данных



        }
    }
}
