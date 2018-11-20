using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ProcessLog;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Threading;
using System.Data;
using System.IO;

namespace UpdateBazeKMZ
{

    public class File_M104 : FileProcces
    {
        public File_M104(string filePath) : base()
        {
            FilePath = filePath;
        }

        //private ConnectionHandler cHandle = ConnectionHandler.GetInstance();
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

            ThreadPool.QueueUserWorkItem(poolWriter); // запуск потока записи данных

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
                    // берем по 150к строк
                    if ((currentLineNumber % 150000 == 0) || (currentLineNumber == linesCount-1))
                    {
                        _dataPool.Enqueue(dataTable.Copy()); // очистка таблицы для ввода новых данных
                        dataTable.Clear();
                    }
                    if ((currentLineNumber % (linesCount / 100) == 0) || (currentLineNumber == linesCount-1))
                    {
                        OnProgressChanged(new LoadProgressArgs(currentLineNumber, linesCount-1)); // текущее состояние загрузки
                    }
                        
                    currentLineNumber++;
                }
            }
            _inProgress = false;
            OnProgressCompleted();
        }

        private void poolWriter(object state)
        {
            while(_inProgress || _dataPool.Count != 0)
            {
                if (_dataPool.Count != 0)
                {
                    cHandle.InsertBulkQuery(_dataPool.Dequeue(), "TBM104");
                }
            }
        }

    }
}
