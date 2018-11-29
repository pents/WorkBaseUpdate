using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using ProcessLog;
using System.IO;
using DBConnectionLib;

namespace UpdateBazeKMZ
{

    public partial class FrmMain : Form
    {
       
        //----------------------------------------------------------------------------------------------------------
        #region Конструкторы класса формы

        public FrmMain()
        {
            InitializeComponent();

            timer.Start(); //Запустить таймер

            // Initialization of database connection
            DBConnectionSettings dbSettings = new DBConnectionSettings("10.255.7.203", "SGT_MMC", "UsersSGT", "123QWEasd", false);

        }

        #endregion

        //----------------------------------------------------------------------------------------------------------
        #region Обработчики событий формы

        //Обработчик события загрузки формы и заполение таблиц
        private void FrmMain_Load(object sender, EventArgs e)
        {

        }

        //Изменнение временного интервала таймера
        private void timer_Tick(object sender, EventArgs e)
        {
            tsDate.Text = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
        }

        //Обработчик события закрытия формы
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        #endregion

        //-----------------------------------------------------------------------------------------------------------------------------------------

        private void LoadFile_Click(object sender, EventArgs e)
        {
            Button clickedBtn = (Button)sender;
            clickedBtn.Enabled = false;
            ThreadPool.QueueUserWorkItem(
            new WaitCallback(delegate(object state) 
            {
                exec_FileLoad(new File_M104(clickedBtn.Tag.ToString())); // @"\\192.168.16.50\BazaOtd\PER300.txt"
            }), 
            null);
        }
        // Only need to call threadPool.QueueUserWorkItem or any other async starting operation with this


        private void exec_FileLoad(FileProcces file)
        {
            Invoke((MethodInvoker)delegate
            {
                tsProgressBar.Visible = true;
                tsProgressBar.ProgressBar.Visible = true;
                tsLabInfo.Visible = true;
            });
            file.progressNotify += File_progressNotify;
            file.progressChanged += File_progressChanged;
            file.progressCompleted += File_progressCompleted;
            // Initialization of log file
            Log.Init(string.Format("{0}\\Logs\\Load{2}_LOG{1}.txt", Directory.GetCurrentDirectory(), DateTime.Now.ToShortDateString(), file.FileName));
            file.ReadFile();
        }

        private void File_progressNotify(LoadProgressArgs args)
        {
            Invoke((MethodInvoker)delegate
            {
                toolStripStatusLabel1.Text = args.message;
                Log.Add(args.message);
            });
        }

        private void File_progressChanged(LoadProgressArgs args)
        {
            Invoke((MethodInvoker)delegate
            {
                tsProgressBar.Maximum = args.fullProgress;
                tsProgressBar.Value = args.currentProgress;
                tsLabInfo.Text = string.Format("{0}/{1}", args.currentProgress, args.fullProgress);
                toolStripStatusLabel1.Text = string.Format("{0}%   {1}", args.percentage, args.message);
            });
        }

        private void File_progressCompleted(LoadProgressArgs args)
        {
            MessageBox.Show("Загрузка завершена");
            Invoke((MethodInvoker)delegate
            {
                tsProgressBar.Visible = false;
                tsProgressBar.ProgressBar.Visible = false;
                tsLabInfo.Visible = false;
                toolStripStatusLabel1.Text = "";
                Log.Add(string.Format("Загрузка {0} завершена", args.message));
            });
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }
    }
}
