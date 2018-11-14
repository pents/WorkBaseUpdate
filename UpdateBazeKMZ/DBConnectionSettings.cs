using DBConnectionLib;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateBazeKMZ
{
    class DBConnectionSettings
    {
        
        public string DataSource { get; private set; }
        public string InitialCatalog { get; private set; }
        public string UserID { get; private set; }
        public string Password { get; private set; }
        public bool PersistSecurityInfo { get; private set; }

        public DBConnectionSettings(string dataSource, string initCatalog, string userID, string passWord, bool SecInfo)
        {
            DataSource = dataSource;
            InitialCatalog = initCatalog;
            UserID = userID;
            Password = passWord;
            PersistSecurityInfo = SecInfo;

            EstablishConnection();
        }

        private void EstablishConnection()
        {
            SqlConnectionStringBuilder conStr = new SqlConnectionStringBuilder();

            conStr.DataSource = DataSource;
            conStr.InitialCatalog = InitialCatalog;
            conStr.UserID = UserID;
            conStr.Password = Password;
            conStr.PersistSecurityInfo = PersistSecurityInfo;

            ConnectionHandler.conStr = conStr;
        }

    }
}
