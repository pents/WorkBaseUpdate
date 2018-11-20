using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBConnectionLib;
using System.IO;
using System.Data;

namespace UpdateBazeKMZ
{

    public abstract class FileProcces
    {
        public delegate void ProgressChanged(LoadProgressArgs args);
        public delegate void ProgressNotify(string Msg);
        public delegate void ProgressCompleted();

        protected ConnectionHandler cHandle = ConnectionHandler.GetInstance();


        protected int totalLines(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                int i = 0;
                while (sr.ReadLine() != null) { ++i; }

                return i;
            }
        }


        public abstract void ReadFile(string filePath);

    }
}
