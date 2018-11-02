using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UpdateBazeKMZ
{
    static class Program
    {

        public static FrmMain mainForm;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mainForm = new FrmMain();
            Application.Run(mainForm);
        }
    }
}
