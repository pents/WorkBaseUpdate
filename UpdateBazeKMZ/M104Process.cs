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

    /// <summary>
    /// Структура файла M014
    /// </summary>
    public class FileM104Data : FileHandler.FileData
    {
        public string Production;
        public string Assemblyes;
        public int DetailID;
        public float CountAssembly;
        public float CountProductions;
        public int TypeDetais;
        public int TypeAssembly;
        public int Sign;
        public string PrimaryApplicability;
    }


    public class DBConnection
    {
        public DBConnection() { }
        public void EstablishConnection ()
        {
            SqlConnectionStringBuilder conStr = new SqlConnectionStringBuilder();

            conStr.DataSource = "10.255.7.203";
            conStr.InitialCatalog = "SGT_MMC";
            //conStr.IntegratedSecurity = true; // true for testing purposes
            conStr.UserID = "UsersSGT";
            conStr.Password = "123QWEasd";
            conStr.PersistSecurityInfo = true;

            ConnectionHandler.conStr = conStr;
        }

    }

    public class File_M104
    {
        /// <summary>
        /// Объект структуры данных 
        /// </summary>
        public List<FileM104Data> Data { get; private set; }
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

        public void ReadFile(string filePath)
        {
            ConnectionHandler cHandle = ConnectionHandler.GetInstance();
            cHandle.ExecuteQuery("DELETE FROM TBM104");
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
                }
            }

            cHandle.InsertBulkQuery(dataTable, "TBM104"); // Запись в базу

        }


        internal class ReadFileErrorException : Exception
        {
            public ReadFileErrorException() { }
            public ReadFileErrorException(string message) : base(message) { }
        }

        internal class WrongFileGivenException : Exception
        {
            public WrongFileGivenException() { }
            public WrongFileGivenException(string message) : base(message) { }
        }

        internal class WriteToDBErrorException : Exception
        {
            public WriteToDBErrorException() { }
            public WriteToDBErrorException(string message) : base(message) { }
        }
    }
}
