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
        protected DataTable dataTable = null;
        protected bool deleteRequired;
        protected bool updateRequired = false;

        private int linesCount;
        private int currentLineNumber = 1;
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
        
        protected void OnProgressAsyncWriteRequired(int requiredCount)
        {
            if ((currentLineNumber % requiredCount == 0) || (currentLineNumber == linesCount - 1))
            {
                WriteAsync(dataTable, dataTable.TableName);
                dataTable.Clear();  // очистка таблицы для ввода новых данных
            }
        }

        protected void OnProgressChanged(LoadProgressArgs args)
        {
            // NOTE: below sintax is the same as progressChanged(args) -- it's just a compiler sortage 
            progressChanged?.Invoke(args);
        }

        protected void OnProgressNotify(string message)
        {
            progressNotify?.Invoke(new LoadProgressArgs(0, 0, message));
        }

        protected void OnProgressCompleted()
        {
            _inProgress = false;
            progressCompleted?.Invoke(new LoadProgressArgs(0,0,FileName));
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

        private void Write(DataTable dt, string tableName)
        {
            if (updateRequired)
            {
                cHandle.UpdateBulkQuery(dt, tableName);
            }
            else
            {
                cHandle.InsertBulkQuery(dt, tableName);
            }
        }

        private void WriteAsync(DataTable dataTable, string tableName)
        {
            DataTable dt = dataTable.Copy();
            ThreadPool.QueueUserWorkItem(new WaitCallback(
                delegate (object state)
                {
                    Write(dt, tableName);
                }
                ), null); // writer thread start
            dataTable.Clear();
        }

        private void progressStateUpdate()
        {
            if ((currentLineNumber % (linesCount / 100) == 0) || (currentLineNumber == linesCount - 1))
            {
                OnProgressChanged(new LoadProgressArgs(currentLineNumber, linesCount - 1)); // текущее состояние загрузки
            }
        }

        public void ReadFile()
        {
            if (deleteRequired) cHandle.ExecuteQuery(string.Format("DELETE FROM {0}", dataTable.TableName));
            OnProgressNotify(string.Format("Инициализация {0}...", FileName));
            linesCount = totalLines(FilePath);

            using (StreamReader fileStream = new StreamReader(FilePath, Encoding.Default))
            {
                string currentLine = "";
                while ((currentLine = fileStream.ReadLine()).Length > 5)
                {
                    processFile(currentLine);
                    currentLineNumber++;
                    progressStateUpdate();
                }
                Write(dataTable, dataTable.TableName);
            }
            OnProgressCompleted();
        }

        protected abstract void processFile(string currentLine);

    }
}
