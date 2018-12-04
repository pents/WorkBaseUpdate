using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.IO;
using System.Collections;

namespace UpdateBazeKMZ
{
    public class File_M104 : FileProcces
    {


        public File_M104(string filePath) : base(filePath)
        {
            dataTable = getTable();
            deleteRequired = true;
            loadTBDetail();
        }

        private Hashtable HTDetail = new Hashtable();

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

            dt.TableName = "TBM104";

            dt.Columns.Add("Production",           typeof(string));
            dt.Columns.Add("Assemblyes",           typeof(string));
            dt.Columns.Add("CodeDetailsID",        typeof(int));
            dt.Columns.Add("CountAssembly",        typeof(float));
            dt.Columns.Add("CountProductions",     typeof(float));
            dt.Columns.Add("TypeDetais",           typeof(int));
            dt.Columns.Add("TypeAssembly",         typeof(int));
            dt.Columns.Add("Sign",                 typeof(int));
            dt.Columns.Add("PrimaryApplicability", typeof(string));
            
            return dt;
        }

        protected override void processFile(string currentLine)
        {
            if (HTDetail[currentLine.Substring(50, 25).Trim()] == null)
            {
                OnProgressNotify(string.Format("Для Detail = {0} не найден DetailID", currentLine.Substring(50, 25).Trim()));
            }
            else
            {
                dataTable.Rows.Add(currentLine.Substring(136, 10).Trim(),
                                    currentLine.Substring(25, 25).Trim(),
                                    int.Parse(HTDetail[currentLine.Substring(50, 25).Trim()].ToString()),
                                    float.Parse(currentLine.Substring(75, 9).Trim().Replace('.', ',')),
                                    float.Parse(currentLine.Substring(84, 9).Trim().Replace('.', ',')),
                                    int.Parse(currentLine.Substring(93, 1)),
                                    int.Parse(currentLine.Substring(94, 1)),
                                    int.Parse(currentLine.Substring(95, 1)),
                                    currentLine.Substring(96, 25).Trim()
                                    );

            }
            // каждые по 150к строк запускаем поток записи и сбрасываем в него накопившиеся данные
            OnProgressAsyncWriteRequired(150000);
        }

    }
}
