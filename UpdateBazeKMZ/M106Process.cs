using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateBazeKMZ
{
    public class M106Process : FileProcces
    {
        public M106Process(string filePath) : base(filePath) { dataTable = getTable(); deleteRequired = false; }

        private DataTable getTable()
        {
            DataTable dt = new DataTable();

            dt.TableName = "TBAssemblyes";

            dt.Columns.Add("DetailWhereTo", typeof(string));
            dt.Columns.Add("DetailWhat",    typeof(string));
            dt.Columns.Add("Count",         typeof(string));

            return dt;
        }

        protected override void processFile(string currentLine)
        {

        }
    }
}
