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

    public class File_M104 : File
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

        public new void ReadFile(string filePath)
        {

            var FileD = base.ReadFile(filePath, Encoding.Default);
            var FileLen = FileD.Count();
            ConnectionHandler cHandle = ConnectionHandler.GetInstance();

            DataTable dataTable = getTable();

            cHandle.ExecuteQuery("DELETE FROM TBM104");
            for (int i = 0; i < FileLen; ++i)
            {
                if (FileD[i].Length < 5) continue;
                FileM104Data data = new FileM104Data();
                try
                {
                    string DetailID = cHandle.ExecuteOneElemQuery(string.Format("SELECT ID FROM SGT_MMC.dbo.TBDetailID WHERE Detail = '{0}'", 
                        FileD[i].Substring(50, 25).Trim()));

                    if (DetailID == "0")
                    {
                        Log.Add(string.Format("Для Detail = {0} не найден DetailID", FileD[i].Substring(50, 25).Trim()));
                        continue;
                    }
                    else
                    {
                        dataTable.Rows.Add(FileD[i].Substring(136, 10).Trim(),
                                           FileD[i].Substring(25, 25).Trim(),
                                           int.Parse(DetailID),
                                           float.Parse(FileD[i].Substring(75, 9).Trim().Replace('.', ',')),
                                           float.Parse(FileD[i].Substring(84, 9).Trim().Replace('.', ',')),
                                           int.Parse(FileD[i].Substring(93, 1)),
                                           int.Parse(FileD[i].Substring(94, 1)),
                                           int.Parse(FileD[i].Substring(95, 1)),
                                           FileD[i].Substring(96, 25).Trim()
                                           );
                    }

                }
                catch (Exception e)
                {
                    throw new ReadFileErrorException(string.Format("Ошибка чтения файла ReadFile(M104Process.cs) Итерация {0} Сообщение:{1}", i, e.Message));
                }
            }

            cHandle.InsertBulkQuery(dataTable, "TBM104"); // Запись в базу

        }


    internal class ReadFileErrorException : Exception
    {
        public ReadFileErrorException() {  }
        public ReadFileErrorException(string message) : base(message)  {  }
    }

    internal class WrongFileGivenException : Exception
    {
        public WrongFileGivenException() {  }
        public WrongFileGivenException(string message) : base(message) {  }
    }

    internal class WriteToDBErrorException : Exception
    {
        public WriteToDBErrorException()  {  }
        public WriteToDBErrorException(string message) : base(message) {  }
    }
}
