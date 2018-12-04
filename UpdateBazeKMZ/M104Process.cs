using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.IO;

namespace UpdateBazeKMZ
{
    public class File_M104 : FileProcces
    {
        public File_M104(string filePath) : base(filePath) { dataTable = getTable(); deleteRequired = true;}

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
            string DetailID = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM SGT_MMC.dbo.TBDetailID WHERE Detail = '{0}'",
            currentLine.Substring(50, 25).Trim()));

            if (DetailID == "0")
            {
                OnProgressNotify(string.Format("Для Detail = {0} не найден DetailID", currentLine.Substring(50, 25).Trim()));
            }
            else
            {
                dataTable.Rows.Add(currentLine.Substring(136, 10).Trim(),
                                    currentLine.Substring(25, 25).Trim(),
                                    int.Parse(DetailID),
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
