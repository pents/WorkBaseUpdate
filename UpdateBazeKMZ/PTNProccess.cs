using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UpdateBazeKMZ
{
    public class File_PTN : FileProcces
    {
        public File_PTN(string filePath) : base(filePath, requiredOperation.DELETE)
        {
            dataTable = getTable();
            loadTBDeps();
            loadTBDetail();
            loadTBEquip();
        }

        private string _depID = "";
        private string _equipID = "";
        private string _detailID = "";

        private Hashtable HTDeps = new Hashtable();
        private Hashtable HTEquip = new Hashtable();
        private Hashtable HTDetail = new Hashtable();


        private void loadTBDeps()
        {
            DataTable Data_TBDeps = cHandle.GetDataTable("SELECT ID, (Dep + Sector) as DEPSEC FROM TBDeps", "TBDeps");

            foreach(DataRow row in Data_TBDeps.Rows)
            {
                HTDeps.Add(row["DEPSEC"], row["ID"]);
            }

            Data_TBDeps.Clear();
        }

        private void loadTBEquip()
        {
            DataTable Data_TBDeps = cHandle.GetDataTable("SELECT ID, DepID, Equipment  FROM TBEquipmets", "TBEquipmets");

            foreach (DataRow row in Data_TBDeps.Rows)
            {
                string test = string.Format("{0}{1}", row["DepID"].ToString(), row["Equipment"].ToString());

                HTEquip.Add(test, row["ID"]);
            }
            Data_TBDeps.Clear();

        }

        private void loadTBDetail()
        {
            DataTable Data_TBDeps = cHandle.GetDataTable("SELECT ID, Detail FROM TBDetailID", "TBDetailID");

            foreach (DataRow row in Data_TBDeps.Rows)
            {
                HTDetail.Add(row["Detail"].ToString(), row["ID"]);
            }
            Data_TBDeps.Clear();
        }

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
            dt.Columns.Add("Pruch", typeof(string));

            return dt;
        }

        protected override void processFile(string currentLine)
        {
            
            if (HTDeps[currentLine.Substring(34, 5).Trim()] == null)
            {
                cHandle.ExecuteQuery(string.Format("INSERT INTO TBDeps(Dep, Sector) VALUES ('{0}','{1}')",
                                                    currentLine.Substring(34,3).Trim(),
                                                    currentLine.Substring(37,2).Trim()
                                                    ));
                _depID = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM TBDeps WHERE Dep + Sector = '{0}'", currentLine.Substring(34, 5).Trim()));
                HTDeps.Add(currentLine.Substring(34, 5).Trim(), _depID);
            }
            else
            {
                _depID = HTDeps[currentLine.Substring(34, 5).Trim()].ToString();
            }

            

            if (HTEquip[_depID + currentLine.Substring(39, 10).Trim().ToString()] == null)
            {
                
                cHandle.ExecuteQuery(string.Format("INSERT INTO TBEquipmets(DepID, Equipment) VALUES ({0},'{1}')",
                                                    _depID,
                                                    currentLine.Substring(39, 10).Trim()
                                                    ));
                _equipID = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM TBEquipmets WHERE DepID = {0} AND Equipment = '{1}'",
                                                                    _depID,
                                                                    currentLine.Substring(39, 10)
                                                                    ));
                HTEquip.Add(_depID + currentLine.Substring(39, 10).Trim().ToString(), _equipID);
            }
            else
            {
                _equipID = HTEquip[_depID + currentLine.Substring(39, 10).Trim().ToString()].ToString();
            }

            _detailID = HTDetail[currentLine.Substring(3, 25).Trim()].ToString();

            double nrm = Convert.ToDouble(currentLine.Substring(50, 10).Trim().Replace('.', ',')); //Норма расхода
            double ras = Convert.ToDouble(currentLine.Substring(60, 14).Trim().Replace('.', ',')); //Расценка
            double stm = Convert.ToDouble(currentLine.Substring(74, 10).Trim().Replace('.', ',')); //Станкоминуты
            int proc = Convert.ToInt32(currentLine.Substring(84, 3).Trim()); //Процент возврата

            int detID = int.Parse(_detailID);
            string operation = currentLine.Substring(28, 6).Trim();
            int depid = int.Parse(_depID);
            int eqID = int.Parse(_equipID);
            int rank = int.Parse(currentLine.Substring(49, 1).Trim() == "" || currentLine.Substring(49, 1).Trim() == "\0"  ? "0" : currentLine.Substring(49, 1).Trim());
            int mex = int.Parse(currentLine.Substring(87, 1).Trim() == "" ? "0" : currentLine.Substring(87, 1).Trim());
            int kvn = int.Parse(currentLine.Substring(88, 1).Trim() == "" ? "0" : currentLine.Substring(88, 1).Trim());
            string pruch = currentLine.Substring(89, 1).Trim();

            dataTable.Rows.Add( detID,
                                operation,
                                depid,
                                eqID,
                                rank,
                                (float)nrm,
                                (float)ras,
                                (float)stm,
                                proc,
                                mex,
                                kvn,
                                pruch
                                );


            // каждые 150к строк запускаем поток записи и сбрасываем в него накопившиеся данные
            OnProgressAsyncWriteRequired(150000);
        }
    }
}
