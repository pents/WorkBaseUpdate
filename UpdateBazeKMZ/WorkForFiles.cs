using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UpdateBazeKMZ
{
    class TextFile
    {
        //--------------------------------------------------------------------------------------------------------------
        #region Поля класса

        private string fPath;        

        #endregion
                
        //--------------------------------------------------------------------------------------------------------------
        #region Конструкторы класса

        public TextFile(string _path)
        {
            if (!string.IsNullOrEmpty(_path))
            {
                fPath = _path;
            }
            else
                System.Windows.Forms.MessageBox.Show("Путь к файлу не указан", "Путь не указан",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        #region Методы класса

        //Открывает текстовый файл и возвращает его поток
        protected string[] OpenTextFile()
        {
            try
            {             
                return File.ReadAllLines(fPath, Encoding.Default);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Ошибка чтения", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }            
        }

        #endregion

    }
}
