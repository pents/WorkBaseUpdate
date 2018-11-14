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
    //Перечисления
    public enum ActionType { Load, Sync }; //Перечисление видов операций
    public enum FilesNames { M101, Gp, CSMR, M106, PTN, NRM, Routes, Per300, OSTM }     //Перечисления названий файлов

    public partial class FrmMain : Form
    {
        //----------------------------------------------------------------------------------------------------------
        #region Поля класса

        TBGP sFile;
        TBM101 M101;
        TBCSMR cSMR;
        TBM106 M106;
        TBPTN PTN;
        TBNRM NRM;
        TBRoutes Routes;
        TBPer300 Per300;
        TBOSTM Ostm;

        //Справочники
        Dictionary<string, int> units = new Dictionary<string, int>(); //Единицы измерения
        Dictionary<int, string> stor = new Dictionary<int, string>();  //Склады

        //Адаптеры данных
        dsBaseKMZTableAdapters.TBStorageTableAdapter taStor;
        dsBaseKMZTableAdapters.TBUnitsTableAdapter taUnits;
        dsBaseKMZTableAdapters.TBMaterialTableAdapter taMaterial;
        dsBaseKMZTableAdapters.TbAssemblyesTableAdapter taAssemblyes;
        dsBaseKMZTableAdapters.TBDepsTableAdapter taDeps;
        dsBaseKMZTableAdapters.TBEquipmetsTableAdapter taEquip;
        dsBaseKMZTableAdapters.TBPTNTableAdapter taPTN;
        //dsBaseKMZTableAdapters.TBNRMTableAdapter taNRM;

        ExtBackGroundWorker bgWorker;

        System.Data.SqlClient.SqlCommand sComm;
        System.Data.SqlClient.SqlConnection sConn;
        System.Data.SqlClient.SqlTransaction sTr;
        //DataRow[] dtRow;

        #endregion

        #region Свойства класса

        public int DepID { get; private set; } //Уникальный номер подразделения
        public int EquipID { get; private set; } //Уникальный номер оборудования

        #endregion

        //----------------------------------------------------------------------------------------------------------
        #region Конструкторы класса формы

        public FrmMain()
        {
            InitializeComponent();

            //Создание экземпляров адаптеров данных
            taUnits = new dsBaseKMZTableAdapters.TBUnitsTableAdapter();
            taStor = new dsBaseKMZTableAdapters.TBStorageTableAdapter();
            taDeps = new dsBaseKMZTableAdapters.TBDepsTableAdapter();
            taEquip = new dsBaseKMZTableAdapters.TBEquipmetsTableAdapter();

            //Временно
            sComm = new System.Data.SqlClient.SqlCommand(); //Создание экземпляра объекта команды                        
            sConn = taStor.Connection; //Установка активного соединения
            sConn.Open(); //Открытие соединения                        
            sComm.Connection = sConn;

            //Заполнение таблиц
            taUnits.Fill(dsBaseKMZ.TBUnits);
            taStor.Fill(dsBaseKMZ.TBStorage);
            taDeps.Fill(dsBaseKMZ.TBDeps);
            taEquip.Fill(dsBaseKMZ.TBEquipmets);

            FillDictionary(); //Вызов метода заполения справочников

            timer.Start(); //Запустить таймер



            // Initialization of database connection
            DBConnectionSettings dbSettings = new DBConnectionSettings("10.255.7.203", "SGT_MMC", "UsersSGT", "123QWEasd", false);
            // Initialization of log file
            Log.Init(string.Format("{0}\\Logs\\UpdateBazeKMZ_LOG{1}.txt", Directory.GetCurrentDirectory(), DateTime.Now.ToShortDateString()));
        }

        #endregion

        //----------------------------------------------------------------------------------------------------------
        #region Обработчики событий формы

        //Обработчик события загрузки формы и заполение таблиц
        private void FrmMain_Load(object sender, EventArgs e)
        {
            //tbgpTableAdapter.Fill(dsBaseKMZ.TBGP);

            bgWorker = new ExtBackGroundWorker(); //Создание экземпляра фонового процесса

        }

        //Изменнение временного интервала таймера
        private void timer_Tick(object sender, EventArgs e)
        {
            tsDate.Text = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
        }


        //Кнопки обновления
        //Нажатие на кнопку импорт файла M101.txt (Изделия)
        private void cmdImportM101_Click(object sender, EventArgs e)
        {
            if (!bgWorker.IsBusy)
            {
                tbCodeProductsTableAdapter.Fill(this.dsBaseKMZ.TbCodeProducts); //Заполнить таблицу изделий

                bgWorker.WorkerReportsProgress = true;
                bgWorker.WorkerSupportsCancellation = true;
                bgWorker.DoWork += bwWork_DoWork;
                bgWorker.ProgressChanged += bwWork_ProgressChanged;
                bgWorker.RunWorkerCompleted += bwWork_RunWorkerCompleted;
                bgWorker.FileName = "SPRAVN.txt";

                M101 = new TBM101(@"\\192.168.16.50\BazaOtd\BAZEKMZ\Files\SPRAVN.txt"); //Создание экземпляра класса объекта данных
                M101.ReadStr += OnWriteTbGP; //Подписать обработчик события чтения строки
                M101.ReadComplited += OnEndWrite; //Подписать обработчик события удаления записей из таблицы, которых нет в файле

                //Проверка открыт ли файл
                if (M101.IsOpen)
                {
                    tsProgressBar.Visible = true;
                    tsProgressBar.ProgressBar.Visible = true;
                    tsLabInfo.Visible = true;
                    tsProgressBar.Maximum = M101.StrCount; //Максимальное значение полоы состояния                
                    bgWorker.RunWorkerAsync(FilesNames.M101); //Запусть процесс                    
                }
            }
        }

        //Нажатие на кнопку импорт файла GP.txt
        private void cmdImportGP_Click(object sender, EventArgs e)
        {
            if (!bgWorker.IsBusy)
            {
                dsBaseKMZ.EnforceConstraints = false;     
                tBDetailIDTableAdapter.Fill(this.dsBaseKMZ.TBDetailID); //Заполнить таблицу деталей
                //dsBaseKMZ.EnforceConstraints = true;
                bgWorker.WorkerReportsProgress = true;
                bgWorker.WorkerSupportsCancellation = true;
                bgWorker.DoWork += bwWork_DoWork;
                bgWorker.ProgressChanged += bwWork_ProgressChanged;
                bgWorker.RunWorkerCompleted += bwWork_RunWorkerCompleted;
                bgWorker.FileName = "Gpr.txt";

                sFile = new TBGP(@"\\192.168.16.50\BazaOtd\BAZEKMZ\Files\Gprn.txt"); //Создание экземпляра класса объекта данных
                sFile.ReadStr += OnWriteTbGP; //Подписать обработчик события чтения строки
                sFile.ReadComplited += OnEndWrite; //Подписать обработчик события удаления записей из таблицы, которых нет в файле

                //Проверка открыт ли файл
                if (sFile.IsOpen)
                {
                    tsProgressBar.Visible = true;
                    tsProgressBar.ProgressBar.Visible = true;
                    tsLabInfo.Visible = true;
                    tsProgressBar.Maximum = sFile.StrCount; //Максимальное значение полоы состояния                
                    bgWorker.RunWorkerAsync(FilesNames.Gp); //Запусть процесс                    
                }
            }
        }

        //Нажатие на кнопку импорт файла M106.txt (Состав изделия)
        private void cmdLoadM106_Click(object sender, EventArgs e)
        {
            if (!bgWorker.IsBusy)
            {
                taAssemblyes = new dsBaseKMZTableAdapters.TbAssemblyesTableAdapter();
                taAssemblyes.Fill(dsBaseKMZ.TbAssemblyes);

                bgWorker.WorkerReportsProgress = true;
                bgWorker.WorkerSupportsCancellation = true;
                bgWorker.DoWork += bwWork_DoWork;
                bgWorker.ProgressChanged += bwWork_ProgressChanged;
                bgWorker.RunWorkerCompleted += bwWork_RunWorkerCompleted;
                bgWorker.FileName = "M106.txt";

                M106 = new TBM106(@"\\192.168.16.50\BazaOtd\BAZEKMZ\Files\M106.txt"); //Создание экземпляра класса объекта данных
                M106.ReadStr += OnWriteTbGP; //Подписать обработчик события чтения строки
                M106.ReadComplited += OnEndWrite; //Подписать обработчик события удаления записей из таблицы, которых нет в файле

                //Проверка открыт ли файл
                if (M106.IsOpen)
                {
                    tsProgressBar.Visible = true;
                    tsProgressBar.ProgressBar.Visible = true;
                    tsLabInfo.Visible = true;
                    tsProgressBar.Maximum = M106.StrCount; //Максимальное значение полоы состояния                
                    bgWorker.RunWorkerAsync(FilesNames.M106); //Запусть процесс                    
                }
            }
        }

        //Нажатие на кнопку импорт файла CSMR.txt (Материалы)
        private void cmdLoadCSMR_Click(object sender, EventArgs e)
        {
            if (!bgWorker.IsBusy)
            {
                taMaterial = new dsBaseKMZTableAdapters.TBMaterialTableAdapter();
                taMaterial.Fill(this.dsBaseKMZ.TBMaterial); //Заполнить таблицу деталей
                bgWorker.WorkerReportsProgress = true;
                bgWorker.WorkerSupportsCancellation = true;
                bgWorker.DoWork += bwWork_DoWork;
                bgWorker.ProgressChanged += bwWork_ProgressChanged;
                bgWorker.RunWorkerCompleted += bwWork_RunWorkerCompleted;
                bgWorker.FileName = "CSMR.txt";

                cSMR = new TBCSMR(@"\\192.168.16.50\BazaOtd\BAZEKMZ\Files\CSMR.txt"); //Создание экземпляра класса объекта данных
                cSMR.ReadStr += OnWriteTbGP; //Подписать обработчик события чтения строки
                cSMR.ReadComplited += OnEndWrite; //Подписать обработчик события удаления записей из таблицы, которых нет в файле

                //Проверка открыт ли файл
                if (cSMR.IsOpen)
                {
                    tsProgressBar.Visible = true;
                    tsProgressBar.ProgressBar.Visible = true;
                    tsLabInfo.Visible = true;
                    tsProgressBar.Maximum = cSMR.StrCount; //Максимальное значение полоы состояния                
                    bgWorker.RunWorkerAsync(FilesNames.CSMR); //Запусть процесс                    
                }
            }
        }

        //Нажатие на кнопку импорт файла PTN.txt (ПТН)
        private void cmdLoadPTN_Click(object sender, EventArgs e)
        {
            if (!bgWorker.IsBusy)
            {
                taPTN = new dsBaseKMZTableAdapters.TBPTNTableAdapter();
                taPTN.Fill(this.dsBaseKMZ.TBPTN); //Заполнить таблицу деталей
                bgWorker.WorkerReportsProgress = true;
                bgWorker.WorkerSupportsCancellation = true;
                bgWorker.DoWork += bwWork_DoWork;
                bgWorker.ProgressChanged += bwWork_ProgressChanged;
                bgWorker.RunWorkerCompleted += bwWork_RunWorkerCompleted;
                bgWorker.FileName = "PTN.tex";

                PTN = new TBPTN(@"\\192.168.16.50\BazaOtd\BAZEKMZ\Files\PTN.txt"); //Создание экземпляра класса объекта данных
                PTN.ReadStr += OnWriteTbGP; //Подписать обработчик события чтения строки
                PTN.ReadComplited += OnEndWrite; //Подписать обработчик события удаления записей из таблицы, которых нет в файле

                //Проверка открыт ли файл
                if (PTN.IsOpen)
                {
                    tsProgressBar.Visible = true;
                    tsProgressBar.ProgressBar.Visible = true;
                    tsLabInfo.Visible = true;
                    tsProgressBar.Maximum = PTN.StrCount; //Максимальное значение полоы состояния      

                    if (DeleteTablePTN()) //Если очистка таблицы прошла успешно, то запуск процедуры добавления
                        bgWorker.RunWorkerAsync(FilesNames.PTN); //Запусть процесс                    

                }
            }
        }

        //Нажатие на кнопку импорт файла NRM.txt (Норма расхода материалов)
        private void cmdLoadNRM_Click(object sender, EventArgs e)
        {
            if (!bgWorker.IsBusy)
            {
                //taNRM = new dsBaseKMZTableAdapters.TBNRMTableAdapter();
                //taNRM.Fill(this.dsBaseKMZ.TBNRM); //Заполнить таблицу деталей
                bgWorker.WorkerReportsProgress = true;
                bgWorker.WorkerSupportsCancellation = true;
                bgWorker.DoWork += bwWork_DoWork;
                bgWorker.ProgressChanged += bwWork_ProgressChanged;
                bgWorker.RunWorkerCompleted += bwWork_RunWorkerCompleted;
                bgWorker.FileName = "NRM.txt";

                NRM = new TBNRM(@"\\192.168.16.50\BazaOtd\BAZEKMZ\Files\NRM.txt"); //Создание экземпляра класса объекта данных
                NRM.ReadStr += OnWriteTbGP; //Подписать обработчик события чтения строки
                NRM.ReadComplited += OnEndWrite; //Подписать обработчик события удаления записей из таблицы, которых нет в файле

                //Проверка открыт ли файл
                if (NRM.IsOpen)
                {
                    tsProgressBar.Visible = true;
                    tsProgressBar.ProgressBar.Visible = true;
                    tsLabInfo.Visible = true;
                    tsProgressBar.Maximum = NRM.StrCount; //Максимальное значение полоы состояния           

                    if (DeleteTableNRM()) //Если очистка таблицы прошла успешно, то запуск процедуры добавления
                        bgWorker.RunWorkerAsync(FilesNames.NRM); //Запусть процесс                    
                }
            }
        }

        //Нажатие на кнопку импорт файла MARSH.txt (Маршруты деталей)
        private void cmdLoadRoutes_Click(object sender, EventArgs e)
        {
            if (!bgWorker.IsBusy)
            {
                bgWorker.WorkerReportsProgress = true;
                bgWorker.WorkerSupportsCancellation = true;
                bgWorker.DoWork += bwWork_DoWork;
                bgWorker.ProgressChanged += bwWork_ProgressChanged;
                bgWorker.RunWorkerCompleted += bwWork_RunWorkerCompleted;
                bgWorker.FileName = "MARSH.txt";

                Routes = new TBRoutes(@"\\192.168.16.50\BazaOtd\BAZEKMZ\Files\MARSH.txt"); //Создание экземпляра класса объекта данных
                Routes.ReadStr += OnWriteTbGP; //Подписать обработчик события чтения строки
                Routes.ReadComplited += OnEndWrite; //Подписать обработчик события удаления записей из таблицы, которых нет в файле

                //Проверка открыт ли файл
                if (Routes.IsOpen)
                {
                    tsProgressBar.Visible = true;
                    tsProgressBar.ProgressBar.Visible = true;
                    tsLabInfo.Visible = true;
                    tsProgressBar.Maximum = Routes.StrCount; //Максимальное значение полоы состояния           

                    if (DeleteTableRoutes()) //Если очистка таблицы прошла успешно, то запуск процедуры добавления
                        bgWorker.RunWorkerAsync(FilesNames.Routes); //Запусть процесс                    
                }
            }
        }

        //Нажатие на кнопку импорт файла PER300.txt (Остатки материала на складах)
        private void cmdPer300_Click(object sender, EventArgs e)
        {
            if (!bgWorker.IsBusy)
            {
                bgWorker.WorkerReportsProgress = true;
                bgWorker.WorkerSupportsCancellation = true;
                bgWorker.DoWork += bwWork_DoWork;
                bgWorker.ProgressChanged += bwWork_ProgressChanged;
                bgWorker.RunWorkerCompleted += bwWork_RunWorkerCompleted;
                bgWorker.FileName = "PER300.txt";

                //Routes = new TBRoutes(@"\\192.168.16.50\BazaOtd\BAZEKMZ\Files\MARSH.txt"); //Создание экземпляра класса объекта данных
                Per300 = new TBPer300(@"\\192.168.16.50\BazaOtd\PER300.txt"); //Создание экземпляра класса объекта данных
                Per300.ReadStr += OnWriteTbGP; //Подписать обработчик события чтения строки
                Per300.ReadComplited += OnEndWrite; //Подписать обработчик события удаления записей из таблицы, которых нет в файле

                //Проверка открыт ли файл
                if (Per300.IsOpen)
                {
                    tsProgressBar.Visible = true;
                    tsProgressBar.ProgressBar.Visible = true;
                    tsLabInfo.Visible = true;
                    tsProgressBar.Maximum = Per300.StrCount; //Максимальное значение полоы состояния           

                    if (DeleteTablePer()) //Если очистка таблицы прошла успешно, то запуск процедуры добавления
                        bgWorker.RunWorkerAsync(FilesNames.Per300); //Запусть процесс                    
                }
            }
        }

        //Нажатие на кнопку импорт файла OSTM.txt (Остатки материала в подразделениях)
        private void cmdOSTM_Click(object sender, EventArgs e)
        {
            if (!bgWorker.IsBusy)
            {
                bgWorker.WorkerReportsProgress = true;
                bgWorker.WorkerSupportsCancellation = true;
                bgWorker.DoWork += bwWork_DoWork;
                bgWorker.ProgressChanged += bwWork_ProgressChanged;
                bgWorker.RunWorkerCompleted += bwWork_RunWorkerCompleted;
                bgWorker.FileName = "OSTM.txt";

                //Routes = new TBRoutes(@"\\192.168.16.50\BazaOtd\BAZEKMZ\Files\MARSH.txt"); //Создание экземпляра класса объекта данных
                Ostm = new TBOSTM(@"\\192.168.16.50\BazaOtd\OSTM.txt"); //Создание экземпляра класса объекта данных
                Ostm.ReadStr += OnWriteTbGP; //Подписать обработчик события чтения строки
                Ostm.ReadComplited += OnEndWrite; //Подписать обработчик события удаления записей из таблицы, которых нет в файле

                //Проверка открыт ли файл
                if (Ostm.IsOpen)
                {
                    tsProgressBar.Visible = true;
                    tsProgressBar.ProgressBar.Visible = true;
                    tsLabInfo.Visible = true;
                    tsProgressBar.Maximum = Ostm.StrCount; //Максимальное значение полоы состояния           

                    if (DeleteTableOSTM()) //Если очистка таблицы прошла успешно, то запуск процедуры добавления
                        bgWorker.RunWorkerAsync(FilesNames.OSTM); //Запусть процесс                    
                }
            }
        }

        //Запуск процесса чтения строк из файла
        private void bwWork_DoWork(object sender, DoWorkEventArgs e)
        {
            switch ((int)e.Argument)
            {
                case 0:
                    {
                        M101.LoadToTable();
                        e.Result = true;
                        break;
                    }
                case 1:
                    {
                        sFile.LoadToTable();
                        e.Result = true;
                        break;
                    }
                case 2:
                    {
                        cSMR.LoadToTable();
                        e.Result = true;
                        break;
                    }
                case 3:
                    {
                        M106.LoadToTable();
                        e.Result = true;
                        break;
                    }
                case 4:
                    {
                        PTN.LoadToTable();
                        e.Result = true;
                        break;
                    }
                case 5:
                    {
                        NRM.LoadToTable();
                        e.Result = true;
                        break;
                    }
                case 6:
                    {
                        Routes.LoadToTable();
                        e.Result = true;
                        break;
                    }
                case 7:
                    {
                        Per300.LoadToTable();
                        e.Result = true;
                        break;
                    }
                case 8:
                    {
                        Ostm.LoadToTable();
                        e.Result = true;
                        break;
                    }
                case 9:
                    {

                        e.Result = true;
                        break;
                    }
                default:
                    break;
            }
        }

        //Отображение хода процесса загрузки
        private void bwWork_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (bgWorker.CancellationPending)
                return;

            tsProgressBar.ProgressBar.Value = e.ProgressPercentage;
            switch ((int)e.UserState)
            {
                case 0:
                    tsLabInfo.Text = "Импорт файла " + bgWorker.FileName + " (строк " + tsProgressBar.ProgressBar.Value + " из " + tsProgressBar.Maximum.ToString() + ")";
                    break;
                case 1:
                    tsLabInfo.Text = "Синхронизация данных (" + tsProgressBar.ProgressBar.Value + " из " + tsProgressBar.Maximum.ToString() + ")";
                    break;
                default:
                    break;
            }

        }

        //Обработчик события завершения процесса
        private void bwWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            tsProgressBar.Visible = false;
            tsLabInfo.Visible = false;
            MessageBox.Show("Файл " + bgWorker.FileName + " загружен", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.DataSource = dsBaseKMZ.TBGP)); //Временно!!!!!
        }

        //Обработчик события закрытия формы
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bgWorker.IsBusy)
                bgWorker.CancelAsync();
        }

        #endregion

        //-----------------------------------------------------------------------------------------------------------------------------------------
        #region Методы класса

        //Метод заполения сарвочников
        private void FillDictionary()
        {
            DataRow[] dtRow; //Массив строк таблиц

            //Заполнение справочника складов
            dtRow = dsBaseKMZ.TBStorage.Select();
            stor.Clear(); //Очистка справочника

            foreach (var item in dtRow)
            {
                stor.Add((int)item["ID"], item["Number"].ToString());
            }

            //Заполнение справочника единиц измерений            
            dtRow = dsBaseKMZ.TBUnits.Select();
            units.Clear(); //Очистка справочника

            foreach (var item in dtRow)
            {
                units.Add(item["Number"].ToString(), (int)item["ID"]);
            }
        }


        //Методы записи файла Gpr.txt
        //-----------------------------------------------------------------------------------------------------------------------------------------
        //Обработчик события записи строки в таблицу TBGP
        private void OnWriteTbGP(object sender, TBGPEventsArgs e)
        {
            switch (e.FileInd)
            {
                case 0: //Импорт файла М101
                    {
                        if (dsBaseKMZ.TbCodeProducts.Select(e.Filter).Length == 0) //Номер детали отсутствует в БД
                        {
                            tbCodeProductsTableAdapter.Insert(e.FStr.Substring(0, 10).Trim(), e.FStr.Substring(10, 25).Trim(),
                                e.FStr.Substring(35, 15).Trim(), true);
                        }
                        else
                        {
                            tbCodeProductsTableAdapter.UpdateQuery(e.FStr.Substring(0, 10).Trim(), e.FStr.Substring(10, 25).Trim(),
                                e.FStr.Substring(35, 15).Trim(), true);
                        }
                        break;
                    }
                case 1: //Импорт файла GP
                    {
                        if (dsBaseKMZ.TBDetailID.Select(e.Filter).Length == 0) //Номер детали отсутствует в БД
                        {
                            try
                            {
                                tBDetailIDTableAdapter.Insert(e.FStr.Substring(3, 25).Trim(), e.FStr.Substring(28, 1).Trim(),
                                e.FStr.Substring(88).Trim(), e.FStr.Substring(46, 25).Trim(), e.FStr.Substring(73, 15).Trim());
                            }
                            catch(Exception ex)
                            {

                            }
                            
                        }
                        else
                        {
                            tBDetailIDTableAdapter.UpdateQuery(e.FStr.Substring(28, 1).Trim(), e.FStr.Substring(88).Trim(), e.FStr.Substring(46, 25).Trim(),
                                e.FStr.Substring(73, 15).Trim(), e.FStr.Substring(3, 25).Trim());
                        }
                        break;
                    }
                case 2: //Испорт CSMR (материалы)
                    {
                        //Проверка есть ли номер склада в таблице БД
                        if (dsBaseKMZ.TBStorage.Select(string.Format("Number = '{0}' AND GroupLeader = '{1}'", e.FStr.Substring(136, 3).Trim(), e.FStr.Substring(94, 2).Trim())).Length == 0)
                        {
                            UpdateStors(e.FStr.Substring(136, 3).Trim(), e.FStr.Substring(94, 2).Trim());
                            taStor.Fill(dsBaseKMZ.TBStorage);
                        }

                        decimal acPrice = Convert.ToDecimal(e.FStr.Substring(100, 13).Trim().Replace('.', ',')); //Учетная цена
                        decimal pPrice = Convert.ToDecimal(e.FStr.Substring(140, 13).Trim().Replace('.', ','));  //Перспективная цена

                        DateTime date;
                        DateTime.TryParse(e.FStr.Substring(168, 2).Trim() + "." + e.FStr.Substring(170, 2).Trim()
                                + "." + e.FStr.Substring(172, 2).Trim(), out date);

                        //Номер материала отсутствует в БД
                        if (dsBaseKMZ.TBMaterial.Select(e.Filter).Length == 0)
                        {
                            taMaterial.Insert(e.FStr.Substring(0, 12).Trim(), e.FStr.Substring(12, 1).Trim(),
                                e.FStr.Substring(13, 80).Trim(), e.FStr.Substring(93, 1).Trim(), e.FStr.Substring(94, 2).Trim(),
                                e.FStr.Substring(96, 3).Trim(), acPrice, e.FStr.Substring(113, 15).Trim(),
                                e.FStr.Substring(128, 3).Trim(), e.FStr.Substring(131, 5).Trim(), e.FStr.Substring(136, 3).Trim(),
                                pPrice, e.FStr.Substring(153, 15).Trim(), date, e.FStr.Substring(174, 1).Trim());
                        }
                        else
                        {
                            sTr = sConn.BeginTransaction();
                            sComm.Transaction = sTr;

                            try
                            {
                                sComm.CommandText = "UPDATE TBMaterial SET ItemType = '" + e.FStr.Substring(12, 1).Trim() + "', MaterialName = '" + e.FStr.Substring(13, 80).Trim().Replace("'", " ") +
                                    "', MainNomenclatureAttr = '" + e.FStr.Substring(93, 1).Trim() + "', GLCode = '" + e.FStr.Substring(94, 2).Trim() +
                                    "', KEI = '" + e.FStr.Substring(96, 3).Trim() + "', AccountingPrice = '" + acPrice + "', JAccountingPrice = '" + e.FStr.Substring(113, 15).Trim() +
                                    "', ReserveRate = '" + e.FStr.Substring(128, 3).Trim() + "', TransitRate = '" + e.FStr.Substring(131, 5).Trim() +
                                    "', WNumber = '" + e.FStr.Substring(136, 3).Trim() + "', PromPrice = '" + pPrice + "', JPromPrice = '" + e.FStr.Substring(153, 15).Trim() +
                                    "', Date = '" + date + "', TypeOfAcceptance = '" + e.FStr.Substring(174, 1).Trim() +
                                    "' WHERE MaterialNumber = '" + e.FStr.Substring(0, 12).Trim() + "'";
                                sComm.ExecuteNonQuery();
                                sTr.Commit();
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show(ex.Message, "Ошибка записи", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                sTr.Rollback();
                            }
                        }
                        break;
                    }
                case 3: //Импотр файла M106 (Состав изделия)
                    {
                        double count = Convert.ToDouble(e.FStr.Substring(50).Trim().Replace('.', ','));

                        if (dsBaseKMZ.TbAssemblyes.Select(e.Filter).Length == 0) //Номер детали отсутствует в БД
                        {
                            taAssemblyes.Insert(e.FStr.Substring(0, 25).Trim(), e.FStr.Substring(25, 25).Trim(), count);
                        }
                        else
                        {
                            sTr = sConn.BeginTransaction();
                            sComm.Transaction = sTr;
                            sComm.CommandText = "UPDATE TBAssemblyes SET Count = " + count.ToString().Replace(',', '.') + " WHERE DetailWhereTo = '"
                                + e.FStr.Substring(0, 25).Trim() + "' AND DetailWhat = '" + e.FStr.Substring(25, 25).Trim() + "'";

                            try
                            {
                                sComm.ExecuteNonQuery();
                                sTr.Commit();
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show(ex.Message, "Ошибка записи", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                sTr.Rollback();
                            }
                        }
                        break;
                    }
                case 4: //Импорт файла PTN.txt
                    {
                        DataRow[] mass;

                        //Определим ключ подразделения
                        mass = dsBaseKMZ.TBDeps.Select(string.Format("Dep + Sector = '{0}'",
                            e.FStr.Substring(34, 5).Trim()));

                        if (mass.Length != 0) //Оборудование найдено
                            DepID = (int)mass[0][0]; //Присвоить ID                        

                        //Если подразделение отсутствует, то создать его
                        if (DepID == 0)
                            UpdateDeps(e.FStr.Substring(34, 3).Trim(), e.FStr.Substring(37, 2).Trim());

                        //Определим ключ оборудования
                        mass = dsBaseKMZ.TBEquipmets.Select(string.Format("DepID = {0} AND Equipment = '{1}'",
                            DepID, e.FStr.Substring(39, 10).Trim()));

                        if (mass.Length != 0) //Оборудование найдено
                            EquipID = (int)mass[0][0]; //Присвоить ID

                        //Если оборудование отсутствует, то создать его
                        if (EquipID == 0)
                            UpdateEquip(DepID, e.FStr.Substring(39, 10).Trim());

                        double nrm = Convert.ToDouble(e.FStr.Substring(50, 10).Trim().Replace('.', ',')); //Норма расхода
                        double ras = Convert.ToDouble(e.FStr.Substring(60, 14).Trim().Replace('.', ',')); //Расценка
                        double stm = Convert.ToDouble(e.FStr.Substring(74, 10).Trim().Replace('.', ',')); //Станкоминуты
                        int proc = Convert.ToInt32(e.FStr.Substring(84, 3).Trim()); //Процент возврата

                        //Проверка есть ли запись в ПТН
                        //sComm.CommandText = "SELECT TBPTN.ID FROM TBPTN INNER JOIN TBDetailID ON TBPTN.DetailID = TBDetailID.id " +
                        //    "WHERE TBDetailID.Detail = '" + e.FStr.Substring(3, 25).Trim() + "' AND TBPTN.Operation = '" + e.FStr.Substring(28, 6).Trim() + "'";

                        //System.Data.SqlClient.SqlDataReader dtReader = sComm.ExecuteReader();
                        //int ptnID = 0;

                        //if (dtReader.Read())
                        //    ptnID = (int)dtReader.GetValue(0);

                        //dtReader.Close();

                        //if (ptnID < 1) //Номер ПТН отсутствует в БД
                        //{
                        sComm.CommandText = "INSERT INTO TBPTN (DetailID, Operation, DepID, EquipmentID, Rank, NMin, Price, StMin, " +
                            "Procent, Mex, KVN, PrUch) VALUES((SELECT TBDetailID.ID FROM TBDetailID WHERE TBDetailID.Detail = '" + e.FStr.Substring(3, 25).Trim() + "'), " +
                            "'" + e.FStr.Substring(28, 6).Trim() + "', " + DepID + ", " + EquipID + ", '" + e.FStr.Substring(49, 1).Trim() +
                            "', " + nrm.ToString().Replace(',', '.') + ", " + ras.ToString().Replace(',', '.') + ", " + stm.ToString().Replace(',', '.') +
                            ", " + proc + ", '" + e.FStr.Substring(87, 1).Trim() + "', '" + e.FStr.Substring(88, 1).Trim() + "', '" + e.FStr.Substring(89, 1).Trim() + "')";
                        try
                        {
                            sComm.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.Message, "Ошибка записи", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        //}
                        //else
                        //{
                        //    sTr = sConn.BeginTransaction();
                        //    sComm.Transaction = sTr;
                        //    sComm.CommandText = "UPDATE TBPTN SET DepID = " + DepID + ", EquipmentID = " + EquipID + ", Rank = '" + e.FStr.Substring(49, 1).Trim() +
                        //        "', Nmin = " + nrm.ToString().Replace(',', '.') + ", Price = " + ras.ToString().Replace(',', '.') +
                        //        ", StMin = " + stm.ToString().Replace(',', '.') + ", Procent = " + proc + ", Mex = '" +
                        //        e.FStr.Substring(87, 1).Trim() + "', KVN = '" + e.FStr.Substring(88, 1).Trim() + "', PrUch = '" +
                        //        e.FStr.Substring(89, 1).Trim() + "' WHERE ID = " + ptnID;
                        //    try
                        //    {
                        //        sComm.ExecuteNonQuery();
                        //        sTr.Commit();
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        MessageBox.Show(ex.Message, "Ошибка записи", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //        sTr.Rollback();
                        //    }
                        //}
                        break;
                    }
                case 5:
                    {
                        double nrm = Convert.ToDouble(e.FStr.Substring(40, 12).Trim().Replace('.', ','));
                        double mas = Convert.ToDouble(e.FStr.Substring(58, 9).Trim().Replace('.', ','));
                        int _count = Convert.ToInt32(e.FStr.Substring(69, 5).Trim());

                        sTr = sConn.BeginTransaction();
                        sComm.Transaction = sTr;
                        sComm.CommandText = "INSERT INTO TBNRM (Detail, Material, NRM, EIK, EIN, MASS, Count, Prof, Route) VALUES ('" + e.FStr.Substring(3, 25).Trim()
                            + "', '" + e.FStr.Substring(28, 12).Trim() + "', " + nrm.ToString().Replace(',', '.') + ", '" + e.FStr.Substring(52, 3).Trim()
                            + "', '" + e.FStr.Substring(55, 3).Trim() + "', " + mas.ToString().Replace(',', '.') + ", " + _count
                            + ", '" + e.FStr.Substring(74, 20).Trim() + "', '" + e.FStr.Substring(94, 15).Trim() + "')";

                        try
                        {
                            sComm.ExecuteNonQuery();
                            sTr.Commit();
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.Message, "Ошибка записи", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            sTr.Rollback();
                        }

                        break;
                    }
                case 6:
                    {
                        sTr = sConn.BeginTransaction();
                        sComm.Transaction = sTr;
                        sComm.CommandText = "INSERT INTO TBRoutes (DetailID, Route) VALUES ((SELECT ID FROM TBDetailID WHERE Detail = '"
                            + e.FStr.Substring(0, 25).Trim() + "'), '" + e.FStr.Substring(25).Trim() + "')";

                        try
                        {
                            sComm.ExecuteNonQuery();
                            sTr.Commit();
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.Message, "Ошибка записи", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            sTr.Rollback();
                        }

                        break;
                    }
                case 7:
                    {
                        sTr = sConn.BeginTransaction();
                        sComm.Transaction = sTr;
                        sComm.CommandText = "INSERT INTO TBStorageBalance (MaterialID, Storage, BalanceDay, BalanceMonth) "
                            + " VALUES ('" + e.FStr.Substring(3, 12).Trim() + "', '" + e.FStr.Substring(0, 3).Trim()
                            + "', '" + e.FStr.Substring(15, 11).Trim() + "', '" + e.FStr.Substring(26, 11).Trim() + "')";

                        try
                        {
                            sComm.ExecuteNonQuery();
                            sTr.Commit();
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.Message, "Ошибка записи", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            sTr.Rollback();
                        }

                        break;
                    }

                case 8:
                    {
                        sTr = sConn.BeginTransaction();
                        sComm.Transaction = sTr;
                        sComm.CommandText = "INSERT INTO TBBalanceDep (Dep, MOL, MaterialID, CountMaterial) "
                            + " VALUES ('" + e.FStr.Substring(0, 3).Trim() + "', '" + e.FStr.Substring(3, 2).Trim()
                            + "', '" + e.FStr.Substring(5, 12).Trim() + "', '" + e.FStr.Substring(113, 12).Trim() + "')";

                        try
                        {
                            sComm.ExecuteNonQuery();
                            sTr.Commit();
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.Message, "Ошибка записи", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            sTr.Rollback();
                        }

                        break;
                    }

                default:
                    break;
            }

            bgWorker.ReportProgress(e.NCount, ActionType.Load);
        }

        //Метод удаления из таблицы деталей которых нет в файле
        private void OnEndWrite(object sender, TBGPEventsArgs e)
        {
            switch (e.FileInd)
            {
                case 0:
                    {
                        string[] detailFromFile;
                        DataRow[] detailFromTable = dsBaseKMZ.TbCodeProducts.Select(); //Выбрать все детали из таблицы изделий            

                        foreach (var item in detailFromTable) //Переборка коллекции
                        {
                            if (bgWorker.CancellationPending)
                                return;

                            detailFromFile = e.DetailList.Where(p => p.Trim() == item["PartNumber"].ToString().Trim()).Select(p => p).ToArray(); //Поиск в списке деталей из файла

                            if (detailFromFile.Length == 0) //Если в файле такой детали нет
                            {
                                //Пометить деталь как неиспользуемую
                                tbCodeProductsTableAdapter.UpdateQuery(item["PartNumber"].ToString().Trim(), item["PartNumber"].ToString().ToString().Trim(),
                                    item["Name"].ToString().Trim(), false);
                            }

                            e.NCount++;

                            bgWorker.ReportProgress(e.NCount, ActionType.Sync);
                        }
                        break;
                    }
                //case 4:
                //    {
                //        string[] detailFromFile;
                //        DataRow[] detailFromTable = dsBaseKMZ.TBPTN.Select(); //Выбрать все детали из таблицы TBGP            

                //        foreach (var item in detailFromTable) //Переборка коллекции
                //        {
                //            if (bgWorker.CancellationPending)
                //                return;

                //            detailFromFile = e.DetailList.Where(p => p == item["Detail"].ToString()).Select(p => p).ToArray(); //Поиск в списке деталей из файла

                //            if (detailFromFile.Length == 0) //Если в файле такой детали нет
                //            {
                //                tbgpTableAdapter.DeleteQuery(item["Detail"].ToString()); //Удалить запись из БД
                //                item.Delete(); //Удалить запись из таблицы
                //                item.AcceptChanges(); //Принять изменения                    
                //            }

                //            e.NCount++;

                //            bgWorker.ReportProgress(e.NCount, ActionType.Sync);
                //        }
                //        break;
                //    }
                default:
                    break;
            }

        }

        //Метод обновления справочника складов
        private void UpdateStors(string _num, string _gr)
        {
            taStor.Insert(_num, _gr);
        }

        //Метод обновления справочника подразделений из ПТН
        private void UpdateDeps(string _dep, string _sector)
        {
            taDeps.Insert(_dep, _sector);
            taDeps.Fill(dsBaseKMZ.TBDeps); //Повторно заполнить таблицу виртуальную подразделений            
            DepID = (int)dsBaseKMZ.TBDeps.Select(string.Format("Dep + Sector = '{0}'",
                _dep + _sector))[0][0];
        }

        //Метод обновления справочника оборудования из ПТН
        private void UpdateEquip(int _depID, string _equip)
        {
            taEquip.Insert(_depID, _equip);
            taEquip.Fill(dsBaseKMZ.TBEquipmets); //Повторно заполнить таблицу виртуальную оборудования
            EquipID = (int)dsBaseKMZ.TBEquipmets.Select(string.Format("DepID = {0} AND Equipment = '{1}'",
                            _depID, _equip))[0][0];
        }

        //Очистка таблицы ПТН
        private bool DeleteTablePTN()
        {
            sTr = sConn.BeginTransaction();
            sComm.Transaction = sTr;
            sComm.CommandText = "DELETE FROM TBPTN";
            try
            {
                sComm.ExecuteNonQuery();
                sTr.Commit();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка удаления", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sTr.Rollback();
                return false;
            }
        }

        //Очистка таблицы ПТН
        private bool DeleteTableNRM()
        {
            sTr = sConn.BeginTransaction();
            sComm.Transaction = sTr;
            sComm.CommandText = "DELETE FROM TBNRM";
            try
            {
                sComm.ExecuteNonQuery();
                sTr.Commit();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка удаления", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sTr.Rollback();
                return false;
            }
        }

        //Очистка таблицы маршруты деталей
        private bool DeleteTableRoutes()
        {
            sTr = sConn.BeginTransaction();
            sComm.Transaction = sTr;
            sComm.CommandText = "DELETE FROM TBRoutes";
            try
            {
                sComm.ExecuteNonQuery();
                sTr.Commit();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка удаления", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sTr.Rollback();
                return false;
            }
        }

        //Очистка таблицы остатки материалов
        private bool DeleteTablePer()
        {
            sTr = sConn.BeginTransaction();
            sComm.Transaction = sTr;
            sComm.CommandText = "DELETE FROM TBStorageBalance";
            try
            {
                sComm.ExecuteNonQuery();
                sTr.Commit();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка удаления", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sTr.Rollback();
                return false;
            }
        }

        //Очистка таблицы остатки материалов в подразделениях
        private bool DeleteTableOSTM()
        {
            sTr = sConn.BeginTransaction();
            sComm.Transaction = sTr;
            sComm.CommandText = "DELETE FROM TBBalanceDep";
            try
            {
                sComm.ExecuteNonQuery();
                sTr.Commit();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка удаления", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sTr.Rollback();
                return false;
            }
        }



        #endregion

        private void button1_Click(object sender, EventArgs e)
        {


            tsProgressBar.Visible = true;
            tsProgressBar.ProgressBar.Visible = true;
            tsLabInfo.Visible = true;
            M104Load_Click.Enabled = false;
            ThreadPool.QueueUserWorkItem(exec_M104Load);


        }

        private void exec_M104Load(object state)
        {
           
            File_M104 file = new File_M104();

            file.progressNotify += File_progressNotify;
            file.progressChanged += File_progressChanged;
            file.progressCompleted += File_progressCompleted;
            Log.Add("Начало загрузки");
            file.ReadFile(@"\\192.168.16.50\bazaotd\M104.TXT");
        }

        private void File_progressNotify(string Msg)
        {
            Invoke((MethodInvoker)delegate
            {
                toolStripStatusLabel1.Text = Msg;
                Log.Add(Msg);
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

        private void File_progressCompleted()
        {
            MessageBox.Show("Загрузка завершена");
            Invoke((MethodInvoker)delegate
            {
                tsProgressBar.Visible = false;
                tsProgressBar.ProgressBar.Visible = false;
                tsLabInfo.Visible = false;
                toolStripStatusLabel1.Text = "";
                Log.Add("Загрузка завершена");
            });
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }
    }
}
