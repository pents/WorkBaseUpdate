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
    public class PTNProccess : FileProcces
    {
        public PTNProccess(string filePath) : base(filePath) { dataTable = getTable(); deleteRequired = true; }

        private string _depID = "";
        private string _equipID = "";
        private string _detailID = "";

        private DataTable getTable()
        {
            DataTable dt = new DataTable();
            dt.TableName = "TBPTN";

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

        protected override void processFile(string currentLine)
        {


            _depID = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM TBDeps WHERE Dep + Sector = '{0}'", currentLine.Substring(34, 5).Trim()));
            if (_depID == "0")
            {
                cHandle.ExecuteQuery(string.Format("INSERT INTO TBDeps(Dep, Sector) VALUES ('{0}','{1}')",
                                                    currentLine.Substring(34,3).Trim(),
                                                    currentLine.Substring(37,2).Trim()
                                                    ));
                _depID = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM TBDeps WHERE Dep + Sector = '{0}'", currentLine.Substring(34, 5).Trim()));
            }

            _equipID = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM TBEquipmets WHERE DepID = {0} AND Equipment = '{1}'",
                                                                _depID,
                                                                currentLine.Substring(39,10).Trim()
                                                                ));

            if (_equipID == "0")
            {
                cHandle.ExecuteQuery(string.Format("INSERT INTO TBEquipmets(DepID, Equipment) VALUES ({0},'{1}')",
                                                    _depID,
                                                    currentLine.Substring(39, 10).Trim()
                                                    ));
                _equipID = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM TBEquipmets WHERE DepID = {0} AND Equipment = {1}",
                                                                    _depID,
                                                                    currentLine.Substring(39, 10)
                                                                    ));
            }

            _detailID = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM TBDetailsID WHERE Detail = {0}", 
                                                    currentLine.Substring(3,25)));


            double nrm = Convert.ToDouble(currentLine.Substring(50, 10).Trim().Replace('.', ',')); //Норма расхода
            double ras = Convert.ToDouble(currentLine.Substring(60, 14).Trim().Replace('.', ',')); //Расценка
            double stm = Convert.ToDouble(currentLine.Substring(74, 10).Trim().Replace('.', ',')); //Станкоминуты
            int proc = Convert.ToInt32(currentLine.Substring(84, 3).Trim()); //Процент возврата


            dataTable.Rows.Add( int.Parse(_detailID),
                                currentLine.Substring(28, 6).Trim(),
                                int.Parse(_depID),
                                int.Parse(_equipID),
                                int.Parse(currentLine.Substring(49, 1).Trim()),
                                (float)nrm,
                                (float)ras,
                                (float)stm,
                                proc,
                                int.Parse(currentLine.Substring(87, 1).Trim()),
                                int.Parse(currentLine.Substring(88, 1).Trim()),
                                currentLine.Substring(89, 1).Trim()
                                );


            // каждые 150к строк запускаем поток записи и сбрасываем в него накопившиеся данные
            OnProgressAsyncWriteRequired(150000);
        }
    }
}
