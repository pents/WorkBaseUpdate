using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace UpdateBazeKMZ
{
    class ExtBackGroundWorker:BackgroundWorker
    {
        //----------------------------------------------------------------------------------------------------------------
        #region Свойства класса

        public string FileName { get; set; } //Имя файла        
        //public ActionType Type { get; set; } //Вид операции

        #endregion

        //----------------------------------------------------------------------------------------------------------------
        #region Конструктор класса

        //Конструктор по умолчанию
        public ExtBackGroundWorker()
        {
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        //Конструктор с параметрами (Имя файла и заголовок описания)
        public ExtBackGroundWorker(string _fName, string _note)
        {
            FileName = _fName;            
        }
        
        
        #endregion
                
    }
}
