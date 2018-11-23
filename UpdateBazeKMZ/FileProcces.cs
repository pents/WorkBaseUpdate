using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBConnectionLib;
using System.IO;
using System.Data;
using System.Threading;

namespace UpdateBazeKMZ
{

    public abstract class FileProcces
    {
        public delegate void ProgressEvent(LoadProgressArgs args);

        public event ProgressEvent progressChanged;
        public event ProgressEvent progressNotify;
        public event ProgressEvent progressCompleted;

        public string FilePath { get; private set; } 
        public string FileName { get; private set; }

        protected ConnectionHandler cHandle = ConnectionHandler.GetInstance();

        private bool _inProgress = false;

        protected FileProcces(string filePath)
        {
            FilePath = filePath;
            FileName = "";
            for (int i = FilePath.Length-1; FilePath[i] != '\\'; --i)
            {
                FileName += FilePath[i];
            }
            FileName = new string(FileName.Reverse().ToArray());

            _inProgress = true;
        }
        
        protected void OnProgressChanged(LoadProgressArgs args)
        {
            // NOTE: below sintax is the same as progressChanged(args) -- it's just a compiler sortage 
            progressChanged?.Invoke(args);
        }

        protected void OnProgressNotify(string message)
        {
            LoadProgressArgs args = new LoadProgressArgs(0,0,message);

            progressNotify?.Invoke(args);
        }

        protected void OnProgressCompleted()
        {
            _inProgress = false;
            progressCompleted?.Invoke(new LoadProgressArgs(0,0));
        }

        protected int totalLines(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                int i = 0;
                while (sr.ReadLine() != null) { ++i; }

                return i;
            }
        }

        protected void WriteAsync(DataTable dataTable, string tableName)
        {
            DataTable dt = dataTable.Copy();
            ThreadPool.QueueUserWorkItem(new WaitCallback(
                delegate(object state) 
                {
                    Write(dt, tableName);
                }
                ), null); // writer thread start
        }

        protected void Write(DataTable dt, string tableName)
        {
            cHandle.InsertBulkQuery(dt, tableName);
        }

        public abstract void ReadFile();

    }
}
