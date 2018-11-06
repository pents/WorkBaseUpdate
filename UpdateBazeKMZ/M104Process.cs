using DBConnectionLib;
using FileHandler;
using System;
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
    public delegate void ProgressChanged(LoadProgressArgs args);
    public delegate void ProgressNotify(string Msg);
    public delegate void ProgressCompleted();

    public struct LoadProgressArgs
    {
        public int currentProgress;
        public int fullProgress;
        public byte percentage;
        public string message;

        public LoadProgressArgs(int Current, int Full, string Msg)
        {

            currentProgress = Current;
            fullProgress = Full;
            percentage = (byte)(((float)currentProgress / (float)fullProgress) * 100);
            message = Msg;
        }

        public LoadProgressArgs(int Current, int Full) : this(Current, Full, "")
        {
        }
    }

    public class DBConnection
    {
        public DBConnection() { }
        public void EstablishConnection ()
        {
            SqlConnectionStringBuilder conStr = new SqlConnectionStringBuilder();

            conStr.DataSource = "10.255.7.203";
            conStr.InitialCatalog = "SGT_MMC";
            conStr.UserID = "UsersSGT";
            conStr.Password = "123QWEasd";
            conStr.PersistSecurityInfo = true;

            ConnectionHandler.conStr = conStr;
        }

    }

    public class File_M104
    {
        
        public event ProgressChanged progressChanged;
        public event ProgressNotify progressNotify;
        public event ProgressCompleted progressCompleted;

        public File_M104()
        {
        }

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

        private int totalLines(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                int i = 0;
                while (sr.ReadLine() != null) { ++i; }

                return i;
            }
        }

        public void ReadFile(string filePath)
        {
            ConnectionHandler cHandle = ConnectionHandler.GetInstance();
            cHandle.ExecuteQuery("DELETE FROM TBM104");

            int linesCount = totalLines(filePath);
            int currentLineNumber = 1;

            DataTable dataTable = getTable(); // создание таблицы
            using (StreamReader fileStream = new StreamReader(filePath, Encoding.Default))
            {
                string currentLine = "";
                while((currentLine = fileStream.ReadLine()).Length > 5)
                {
                    string DetailID = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM SGT_MMC.dbo.TBDetailID WHERE Detail = '{0}'",
                    currentLine.Substring(50, 25).Trim()));

                    if (DetailID == "0")
                    {
                        Log.Add(string.Format("Для Detail = {0} не найден DetailID", currentLine.Substring(50, 25).Trim()));
                        progressNotify("Обнаружена ошибка - лог-файл обновлен");
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
                    if (currentLineNumber % (linesCount/100) == 0) // обновление загрузки процесса каждые N операций
                        progressChanged(new LoadProgressArgs(currentLineNumber, linesCount)); // текущее состояние загрузки

                    currentLineNumber++;
                }
            }
            progressNotify("Формирование завершено загрузка в БД");
            cHandle.InsertBulkQuery(dataTable, "TBM104"); // Запись в базу
            progressCompleted();
        }

    }
}
