using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UpdateBazeKMZ
{
    //-------------------------------------------------------------------------------------------------------------------------------------------
    //Делегаты
    //-------------------------------------------------------------------------------------------------------------------------------------------

    public delegate void TBGPReadHandler(object sender, TBGPEventsArgs e); //Делегат для файла GP
    //public delegate void TBM101ReadHandler(object sender, TBGPEventsArgs e); //Делегат для файла M101

    //-------------------------------------------------------------------------------------------------------------------------------------------
    //Классы данных событий
    //-------------------------------------------------------------------------------------------------------------------------------------------

    //Общий класс данных события
    public class FilesEventsArgs
    {
        public string FStr { get; set; } //Строка массива
        public string Filter { get; set; } //Фильтр        
        public int NCount { get; set; } //Счетчик итераций
        public int FileInd { get; set; } //Индекс структуры файла
    }

    //Класс данных события для объекта
    public class TBGPEventsArgs : FilesEventsArgs
    {
        public List<string> DetailList { get; set; } //Список деталей        
    }

    //Класс данных события для объекта TBGP
    //public class TBM101EventsArgs : FilesEventsArgs
    //{
    //    public List<string> DetailList { get; set; } //Список деталей        
    //}


    //-------------------------------------------------------------------------------------------------------------------------------------------
    //Классы данных
    //-------------------------------------------------------------------------------------------------------------------------------------------

    //Общий класс данных файла
    class FileData : TextFile
    {
        //-------------------------------------------------------------------------------------------------------------------------------------------
        #region Поля класса

        protected string[] fReader; //Массив строк файла        

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------
        #region Свойства класса

        public int StrCount { get; set; } //Количество строк файла
        public bool IsOpen { get; set; }  //Флаг того, что файл открыт

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------
        #region Конструкторы класса
                        
        public FileData(string _path) : base(_path)
        {
            fReader = OpenTextFile(); //Открыть файл

            if (fReader == null) //Если ошибка открытия файла
            {
                IsOpen = false; //Ошибка открытия файла
                StrCount = 0; //Запись количества строк файла
            }
            else
            {
                IsOpen = true;  //Файл открыт
                StrCount = fReader.Length; //Запись количества строк файла                                
            }
        }

        #endregion

    }

    //Данные файла Gpv.txt
    class TBGP : FileData
    {
        #region события класса

        public event TBGPReadHandler ReadStr; //Событие чтения строки из массива
        public event TBGPReadHandler ReadComplited; //Событие завершения загрузки

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------

        #region Конструкторы класса

        public TBGP(string _path) : base(_path)
        {
            //if (!base.IsOpen)
            //    return;
            //fReader = OpenTextFile(); //Открыть файл

            //if (fReader == null) //Если ошибка открытия файла
            //{
            //    IsOpen = false; //Ошибка открытия файла
            //    StrCount = 0; //Запись количества строк файла
            //}
            //else
            //{
            //    IsOpen = true;  //Файл открыт
            //    StrCount = fReader.Length; //Запись количества строк файла                                
            //}
        }


        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------
        #region Методы класса

        //Метод запись данных из массива в таблицу
        public void LoadToTable()
        {            
            TBGPEventsArgs tbEa = new TBGPEventsArgs();
            tbEa.DetailList = new List<string>(); //Создать список для хранения деталей

            foreach (var item in fReader)
            {
                if (item.Length < 5) //Если найден конец файла
                    break;

                tbEa.DetailList.Add(item.Substring(3, 25).Trim()); //Запись детали в список, для последующего сравнения
                tbEa.Filter = string.Format("Detail = '{0}'", item.Substring(3, 25).Trim()); //Запись фильтра
                tbEa.FStr = item; //Запись строки массива
                tbEa.FileInd = (int)FilesNames.Gp; //Передача индекса структуры файла

                tbEa.NCount++; //Увеличение счетчика
                
                if (ReadStr != null)
                    ReadStr(this, tbEa); //Вызов обработчика события записи строки в таблицу

                //if (tbEa.NCount > 1000)
                //    break;
            }

            tbEa.NCount = 0;
            if (ReadComplited != null)
                ReadComplited(this, tbEa); //Вызов обработчика события удаления деталей из таблицы которых нет в файле
        }

        #endregion

    }

    //Данные файла M101.txt (SPRAVN)
    class TBM101 : FileData
    {
        #region события класса

        public event TBGPReadHandler ReadStr; //Событие чтения строки из массива
        public event TBGPReadHandler ReadComplited; //Событие завершения загрузки

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------
        #region Конструкторы класса

            
        public TBM101(string _path) : base(_path)
        {
        ////    fReader = OpenTextFile(); //Открыть файл

        ////    if (fReader == null) //Если ошибка открытия файла
        ////    {
        ////        IsOpen = false; //Ошибка открытия файла
        ////        StrCount = 0; //Запись количества строк файла
        ////    }
        ////    else
        ////    {
        ////        IsOpen = true;  //Файл открыт
        ////        StrCount = fReader.Length; //Запись количества строк файла                                
        ////    }
       }

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------
        #region Методы класса

        //Метод запись данных из массива в таблицу
        public void LoadToTable()
        {            
            TBGPEventsArgs tbEa = new TBGPEventsArgs();
            tbEa.DetailList = new List<string>(); //Создать список для хранения деталей

            foreach (var item in fReader)
            {
                if (item.Length < 5) //Если найден конец файла
                    break;

                tbEa.DetailList.Add(item.Substring(0, 10).Trim()); //Запись детали в список, для последующего сравнения
                tbEa.Filter = string.Format("PartNumber = '{0}'", item.Substring(0, 10).Trim()); //Запись фильтра
                tbEa.FStr = item; //Запись строки массива
                tbEa.FileInd = (int)FilesNames.M101; //Передача индекса структуры файла

                tbEa.NCount++; //Увеличение счетчика

                if (ReadStr != null)
                    ReadStr(this, tbEa); //Вызов обработчика события записи строки в таблицу

                //if (tbEa.NCount > 1000)
                //    break;
            }

            tbEa.NCount = 0;
            if (ReadComplited != null)
                ReadComplited(this, tbEa); //Вызов обработчика события удаления деталей из таблицы которых нет в файле
        }

        #endregion
    }

    //Данные файла CSMR.txt
    class TBCSMR : FileData
    {
        #region события класса

        public event TBGPReadHandler ReadStr; //Событие чтения строки из массива
        public event TBGPReadHandler ReadComplited; //Событие завершения загрузки

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------
        #region Конструкторы класса


        public TBCSMR(string _path) : base(_path){ }
        

        #endregion
        //-------------------------------------------------------------------------------------------------------------------------------------------

        #region Методы класса

        //Метод запись данных из массива в таблицу
        public void LoadToTable()
        {
            TBGPEventsArgs tbEa = new TBGPEventsArgs();
            tbEa.DetailList = new List<string>(); //Создать список для хранения строк файла

            foreach (var item in fReader)
            {
                if (item.Length < 5) //Если найден конец файла
                    break;

                tbEa.DetailList.Add(item.Substring(0, 12).Trim()); //Запись детали в список, для последующего сравнения
                tbEa.Filter = string.Format("MaterialNumber = '{0}'", item.Substring(0, 12).Trim()); //Запись фильтра
                tbEa.FStr = item; //Запись строки массива
                tbEa.FileInd = (int)FilesNames.CSMR; //Передача индекса структуры файла

                tbEa.NCount++; //Увеличение счетчика

                if (ReadStr != null)
                    ReadStr(this, tbEa); //Вызов обработчика события записи строки в таблицу

                //if (tbEa.NCount > 150)
                //    break;
            }

            tbEa.NCount = 0;
            if (ReadComplited != null)
                ReadComplited(this, tbEa); //Вызов обработчика события удаления деталей из таблицы которых нет в файле
        }

        #endregion
    }

    //Данные файла M106.txt
    class TBM106 : FileData
    {
        #region события класса

        public event TBGPReadHandler ReadStr; //Событие чтения строки из массива
        public event TBGPReadHandler ReadComplited; //Событие завершения загрузки

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------
        #region Конструкторы класса


        public TBM106(string _path) : base(_path) { }


        #endregion
        //-------------------------------------------------------------------------------------------------------------------------------------------

        #region Методы класса

        //Метод запись данных из массива в таблицу
        public void LoadToTable()
        {
            TBGPEventsArgs tbEa = new TBGPEventsArgs();
            tbEa.DetailList = new List<string>(); //Создать список для хранения строк файла

            foreach (var item in fReader)
            {
                if (item.Length < 5) //Если найден конец файла
                    break;

                tbEa.DetailList.Add(item.Substring(0, 25) + item.Substring(25, 25)); //Запись детали в список, для последующего сравнения

                tbEa.Filter = string.Format("DetailWhereTo = '{0}' AND DetailWhat = '{1}'",
                    item.Substring(0, 25).Trim(), item.Substring(25, 25).Trim()); //Запись фильтра

                tbEa.FStr = item; //Запись строки массива
                tbEa.FileInd = (int)FilesNames.M106; //Передача индекса структуры файла

                tbEa.NCount++; //Увеличение счетчика

                if (ReadStr != null)
                    ReadStr(this, tbEa); //Вызов обработчика события записи строки в таблицу

                //if (tbEa.NCount > 150)
                //    break;
            }

            tbEa.NCount = 0;
            if (ReadComplited != null)
                ReadComplited(this, tbEa); //Вызов обработчика события удаления деталей из таблицы которых нет в файле
        }

        #endregion
    }

    //Данные файла PTN.txt
    class TBPTN : FileData
    {
        #region события класса

        public event TBGPReadHandler ReadStr; //Событие чтения строки из массива
        public event TBGPReadHandler ReadComplited; //Событие завершения загрузки

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------
        #region Конструкторы класса


        public TBPTN(string _path) : base(_path) { }


        #endregion
        //-------------------------------------------------------------------------------------------------------------------------------------------

        #region Методы класса

        //Метод запись данных из массива в таблицу
        public void LoadToTable()
        {
            TBGPEventsArgs tbEa = new TBGPEventsArgs();
            tbEa.DetailList = new List<string>(); //Создать список для хранения строк файла

            foreach (var item in fReader)
            {
                if (item.Length < 5) //Если найден конец файла
                    break;

                tbEa.DetailList.Add(item.Substring(3, 31)); //Запись детали в список, для последующего сравнения

                //tbEa.Filter = string.Format("DetailID = '{0}' AND Operation = '{1}' AND ",
                //    item.Substring(0, 25).Trim(), item.Substring(25, 25).Trim()); //Запись фильтра

                tbEa.FStr = item; //Запись строки массива
                tbEa.FileInd = (int)FilesNames.PTN; //Передача индекса структуры файла

                tbEa.NCount++; //Увеличение счетчика

                if (ReadStr != null)
                    ReadStr(this, tbEa); //Вызов обработчика события записи строки в таблицу

                //if (tbEa.NCount > 150)
                //    break;
            }

            tbEa.NCount = 0;
            if (ReadComplited != null)
                ReadComplited(this, tbEa); //Вызов обработчика события удаления деталей из таблицы которых нет в файле
        }

        #endregion
    }

    //Данные файла PTN.txt
    class TBNRM : FileData
    {
        #region события класса

        public event TBGPReadHandler ReadStr; //Событие чтения строки из массива
        public event TBGPReadHandler ReadComplited; //Событие завершения загрузки

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------
        #region Конструкторы класса


        public TBNRM(string _path) : base(_path) { }


        #endregion
        //-------------------------------------------------------------------------------------------------------------------------------------------

        #region Методы класса

        //Метод запись данных из массива в таблицу
        public void LoadToTable()
        {
            TBGPEventsArgs tbEa = new TBGPEventsArgs();
            tbEa.DetailList = new List<string>(); //Создать список для хранения строк файла

            foreach (var item in fReader)
            {
                if (item.Length < 5) //Если найден конец файла
                    break; //Выход из цикла
                
                tbEa.FStr = item; //Запись строки массива
                tbEa.FileInd = (int)FilesNames.NRM; //Передача индекса структуры файла

                tbEa.NCount++; //Увеличение счетчика

                if (ReadStr != null)
                    ReadStr(this, tbEa); //Вызов обработчика события записи строки в таблицу

                //if (tbEa.NCount > 10)
                //    break;
            }

            tbEa.NCount = 0; //Обнуление счетчика
            if (ReadComplited != null)
                ReadComplited(this, tbEa); //Вызов обработчика события удаления деталей из таблицы которых нет в файле
        }

        #endregion
    }

    //Данные файла PTN.txt
    class TBRoutes : FileData
    {
        #region события класса

        public event TBGPReadHandler ReadStr; //Событие чтения строки из массива
        public event TBGPReadHandler ReadComplited; //Событие завершения загрузки

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------
        #region Конструкторы класса


        public TBRoutes(string _path) : base(_path) { }


        #endregion
        //-------------------------------------------------------------------------------------------------------------------------------------------

        #region Методы класса

        //Метод запись данных из массива в таблицу
        public void LoadToTable()
        {
            TBGPEventsArgs tbEa = new TBGPEventsArgs();
            tbEa.DetailList = new List<string>(); //Создать список для хранения строк файла

            foreach (var item in fReader)
            {
                if (item.Length < 5) //Если найден конец файла
                    break; //Выход из цикла

                tbEa.FStr = item; //Запись строки массива
                tbEa.FileInd = (int)FilesNames.Routes; //Передача индекса структуры файла

                tbEa.NCount++; //Увеличение счетчика

                if (ReadStr != null)
                    ReadStr(this, tbEa); //Вызов обработчика события записи строки в таблицу

                //if (tbEa.NCount > 10)
                //    break;
            }

            tbEa.NCount = 0; //Обнуление счетчика
            if (ReadComplited != null)
                ReadComplited(this, tbEa); //Вызов обработчика события удаления деталей из таблицы которых нет в файле
        }

        #endregion
    }

    //Данные файла Per300.txt
    class TBPer300 : FileData
    {
        #region события класса

        public event TBGPReadHandler ReadStr; //Событие чтения строки из массива
        public event TBGPReadHandler ReadComplited; //Событие завершения загрузки

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------
        #region Конструкторы класса


        public TBPer300(string _path) : base(_path) { }


        #endregion
        //-------------------------------------------------------------------------------------------------------------------------------------------

        #region Методы класса

        //Метод запись данных из массива в таблицу
        public void LoadToTable()
        {
            TBGPEventsArgs tbEa = new TBGPEventsArgs();
            tbEa.DetailList = new List<string>(); //Создать список для хранения строк файла

            foreach (var item in fReader)
            {
                if (item.Length < 5) //Если найден конец файла
                    break; //Выход из цикла

                tbEa.FStr = item; //Запись строки массива
                tbEa.FileInd = (int)FilesNames.Per300; //Передача индекса структуры файла

                tbEa.NCount++; //Увеличение счетчика

                if (ReadStr != null)
                    ReadStr(this, tbEa); //Вызов обработчика события записи строки в таблицу

                //if (tbEa.NCount > 10)
                //    break;
            }

            tbEa.NCount = 0; //Обнуление счетчика
            if (ReadComplited != null)
                ReadComplited(this, tbEa); //Вызов обработчика события удаления деталей из таблицы которых нет в файле
        }

        #endregion
    }

    //Данные файла OSTM.txt
    class TBOSTM : FileData
    {
        #region события класса

        public event TBGPReadHandler ReadStr; //Событие чтения строки из массива
        public event TBGPReadHandler ReadComplited; //Событие завершения загрузки

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------
        #region Конструкторы класса


        public TBOSTM(string _path) : base(_path) { }


        #endregion
        //-------------------------------------------------------------------------------------------------------------------------------------------

        #region Методы класса

        //Метод запись данных из массива в таблицу
        public void LoadToTable()
        {
            TBGPEventsArgs tbEa = new TBGPEventsArgs();
            tbEa.DetailList = new List<string>(); //Создать список для хранения строк файла
            
            foreach (var item in fReader)
            {
                if (item.Length < 5) //Если найден конец файла
                    break; //Выход из цикла

                tbEa.FStr = item; //Запись строки массива
                tbEa.FileInd = (int)FilesNames.OSTM; //Передача индекса структуры файла

                tbEa.NCount++; //Увеличение счетчика

                if (ReadStr != null)
                    ReadStr(this, tbEa); //Вызов обработчика события записи строки в таблицу

                //if (tbEa.NCount > 10)
                //    break;
            }

            tbEa.NCount = 0; //Обнуление счетчика
            if (ReadComplited != null)
                ReadComplited(this, tbEa); //Вызов обработчика события удаления деталей из таблицы которых нет в файле
        }

        #endregion
    }

}
