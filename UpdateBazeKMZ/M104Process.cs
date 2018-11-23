using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.IO;

namespace UpdateBazeKMZ
{
    public class File_M104 : FileProcces
    {
        public File_M104(string filePath) : base(filePath) { }

        private DataTable getTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Production",           typeof(string));
            dt.Columns.Add("Assemblyes",           typeof(string));
            dt.Columns.Add("DetailID",             typeof(int));
            dt.Columns.Add("CountAssembly",        typeof(float));
            dt.Columns.Add("CountProductions",     typeof(float));
            dt.Columns.Add("TypeDetais",           typeof(int));
            dt.Columns.Add("TypeAssembly",         typeof(int));
            dt.Columns.Add("Sign",                 typeof(int));
            dt.Columns.Add("PrimaryApplicability", typeof(string));

            return dt;
        }

        public override void ReadFile()
        {
            cHandle.ExecuteQuery("DELETE FROM TBM104");

            int linesCount = totalLines(FilePath);
            int currentLineNumber = 1;

            DataTable dataTable = getTable(); // создание таблицы для ввода данных

            using (StreamReader fileStream = new StreamReader(FilePath, Encoding.Default))
            {
                string currentLine = "";
                while ((currentLine = fileStream.ReadLine()).Length > 5)
                {
                    string DetailID = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM SGT_MMC.dbo.TBDetailID WHERE Detail = '{0}'",
                    currentLine.Substring(50, 25).Trim()));

                    if (DetailID == "0")
                    {
                        OnProgressNotify(string.Format("Для Detail = {0} не найден DetailID", currentLine.Substring(50, 25).Trim()));
                        currentLineNumber++;
                        continue;
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
                    if ((currentLineNumber % 150000 == 0) || (currentLineNumber == linesCount-1))
                    {
                        WriteAsync(dataTable, "TBM104"); 
                        dataTable.Clear();  // очистка таблицы для ввода новых данных
                    }
                    if ((currentLineNumber % (linesCount / 100) == 0) || (currentLineNumber == linesCount-1))
                    {
                        OnProgressChanged(new LoadProgressArgs(currentLineNumber, linesCount-1)); // текущее состояние загрузки
                    }
                        
                    currentLineNumber++;
                }
            }
            OnProgressCompleted();
        }

    }
}
