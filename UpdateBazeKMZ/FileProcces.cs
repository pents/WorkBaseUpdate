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
        public delegate void ProgressEvent(LoadProgressArgs args);
        public string FilePath { get; private set; } 
        public string FileName { get; private set; }

        public event ProgressEvent progressChanged;
        public event ProgressEvent progressNotify;
        public event ProgressEvent progressCompleted;

       
        protected ConnectionHandler cHandle = ConnectionHandler.GetInstance();

        protected FileProcces(string filePath)
        {
            FilePath = filePath;
            FileName = "";
            for (int i = FilePath.Length-1; FilePath[i] != '\\'; --i)
            {
                FileName += FilePath[i];
            }
            FileName = new string(FileName.Reverse().ToArray());
        }
        
        protected void OnProgressChanged(LoadProgressArgs args)
        {
            // NOTE: below is the same as progressChanged(args) -- it's just a compiler sortage 
            progressChanged?.Invoke(args);
        }

        protected void OnProgressNotify(string message)
        {
            LoadProgressArgs args = new LoadProgressArgs(0,0,message);

            progressNotify?.Invoke(args);
        }

        protected void OnProgressCompleted()
        {
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

        public abstract void ReadFile();

    }
}
