namespace UpdateBazeKMZ
{
    partial class FrmMain
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.tsDate = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsLabInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.gbImportInfo = new System.Windows.Forms.GroupBox();
            this.M104Load_Click = new System.Windows.Forms.Button();
            this.cmdOSTM = new System.Windows.Forms.Button();
            this.cmdPer300 = new System.Windows.Forms.Button();
            this.cmdLoadRoutes = new System.Windows.Forms.Button();
            this.cmdLoadNRM = new System.Windows.Forms.Button();
            this.cmdLoadPTN = new System.Windows.Forms.Button();
            this.cmdLoadM106 = new System.Windows.Forms.Button();
            this.cmdLoadCSMR = new System.Windows.Forms.Button();
            this.cmdImportM101 = new System.Windows.Forms.Button();
            this.cmdImportGP = new System.Windows.Forms.Button();
            this.dsBaseKMZ = new UpdateBazeKMZ.dsBaseKMZ();
            this.tbgpTableAdapter = new UpdateBazeKMZ.dsBaseKMZTableAdapters.TBGPTableAdapter();
            this.tbCodeProductsTableAdapter = new UpdateBazeKMZ.dsBaseKMZTableAdapters.TbCodeProductsTableAdapter();
            this.tBDetailIDTableAdapter = new UpdateBazeKMZ.dsBaseKMZTableAdapters.TBDetailIDTableAdapter();
            this.statusBar.SuspendLayout();
            this.gbImportInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsBaseKMZ)).BeginInit();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsDate,
            this.tsLabInfo,
            this.toolStripStatusLabel1,
            this.tsProgressBar});
            this.statusBar.Location = new System.Drawing.Point(0, 436);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(583, 22);
            this.statusBar.TabIndex = 0;
            this.statusBar.Text = "statusStrip1";
            // 
            // tsDate
            // 
            this.tsDate.Name = "tsDate";
            this.tsDate.Size = new System.Drawing.Size(0, 17);
            // 
            // tsLabInfo
            // 
            this.tsLabInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsLabInfo.Name = "tsLabInfo";
            this.tsLabInfo.Size = new System.Drawing.Size(0, 17);
            this.tsLabInfo.Visible = false;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            this.toolStripStatusLabel1.Click += new System.EventHandler(this.toolStripStatusLabel1_Click);
            // 
            // tsProgressBar
            // 
            this.tsProgressBar.Name = "tsProgressBar";
            this.tsProgressBar.Size = new System.Drawing.Size(100, 16);
            this.tsProgressBar.Visible = false;
            // 
            // timer
            // 
            this.timer.Interval = 10;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // gbImportInfo
            // 
            this.gbImportInfo.Controls.Add(this.M104Load_Click);
            this.gbImportInfo.Controls.Add(this.cmdOSTM);
            this.gbImportInfo.Controls.Add(this.cmdPer300);
            this.gbImportInfo.Controls.Add(this.cmdLoadRoutes);
            this.gbImportInfo.Controls.Add(this.cmdLoadNRM);
            this.gbImportInfo.Controls.Add(this.cmdLoadPTN);
            this.gbImportInfo.Controls.Add(this.cmdLoadM106);
            this.gbImportInfo.Controls.Add(this.cmdLoadCSMR);
            this.gbImportInfo.Controls.Add(this.cmdImportM101);
            this.gbImportInfo.Controls.Add(this.cmdImportGP);
            this.gbImportInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gbImportInfo.Location = new System.Drawing.Point(12, 12);
            this.gbImportInfo.Name = "gbImportInfo";
            this.gbImportInfo.Size = new System.Drawing.Size(562, 411);
            this.gbImportInfo.TabIndex = 1;
            this.gbImportInfo.TabStop = false;
            this.gbImportInfo.Text = "Комманды корректировки";
            // 
            // M104Load_Click
            // 
            this.M104Load_Click.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.M104Load_Click.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.M104Load_Click.ForeColor = System.Drawing.SystemColors.ControlText;
            this.M104Load_Click.Location = new System.Drawing.Point(257, 121);
            this.M104Load_Click.Name = "M104Load_Click";
            this.M104Load_Click.Size = new System.Drawing.Size(295, 39);
            this.M104Load_Click.TabIndex = 9;
            this.M104Load_Click.Tag = "\\\\192.168.16.50\\bazaotd\\M104.TXT";
            this.M104Load_Click.Text = "Загрузка файла M104";
            this.M104Load_Click.UseVisualStyleBackColor = false;
            this.M104Load_Click.Click += new System.EventHandler(this.LoadM104_Click);
            // 
            // cmdOSTM
            // 
            this.cmdOSTM.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.cmdOSTM.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdOSTM.Location = new System.Drawing.Point(257, 71);
            this.cmdOSTM.Name = "cmdOSTM";
            this.cmdOSTM.Size = new System.Drawing.Size(295, 44);
            this.cmdOSTM.TabIndex = 8;
            this.cmdOSTM.Tag = "\\\\192.168.16.50\\BazaOtd\\OSTM.txt";
            this.cmdOSTM.Text = "Импорт файла OSTM.txt (Остатки материалов в подразделениях)";
            this.cmdOSTM.UseVisualStyleBackColor = false;
            this.cmdOSTM.Click += new System.EventHandler(this.cmdOSTM_Click);
            // 
            // cmdPer300
            // 
            this.cmdPer300.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.cmdPer300.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdPer300.Location = new System.Drawing.Point(257, 21);
            this.cmdPer300.Name = "cmdPer300";
            this.cmdPer300.Size = new System.Drawing.Size(295, 35);
            this.cmdPer300.TabIndex = 7;
            this.cmdPer300.Tag = "\\\\192.168.16.50\\BazaOtd\\PER300.txt";
            this.cmdPer300.Text = "Остатки материалов на складах (PER300)";
            this.cmdPer300.UseVisualStyleBackColor = false;
            this.cmdPer300.Click += new System.EventHandler(this.cmdPer300_Click);
            // 
            // cmdLoadRoutes
            // 
            this.cmdLoadRoutes.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.cmdLoadRoutes.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdLoadRoutes.Location = new System.Drawing.Point(6, 353);
            this.cmdLoadRoutes.Name = "cmdLoadRoutes";
            this.cmdLoadRoutes.Size = new System.Drawing.Size(245, 52);
            this.cmdLoadRoutes.TabIndex = 6;
            this.cmdLoadRoutes.Tag = "\\\\192.168.16.50\\BazaOtd\\BAZEKMZ\\Files\\MARSH.txt";
            this.cmdLoadRoutes.Text = "Импорт файла MARSH (Маршруты деталей)";
            this.cmdLoadRoutes.UseVisualStyleBackColor = false;
            this.cmdLoadRoutes.Click += new System.EventHandler(this.cmdLoadRoutes_Click);
            // 
            // cmdLoadNRM
            // 
            this.cmdLoadNRM.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.cmdLoadNRM.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdLoadNRM.Location = new System.Drawing.Point(6, 241);
            this.cmdLoadNRM.Name = "cmdLoadNRM";
            this.cmdLoadNRM.Size = new System.Drawing.Size(245, 47);
            this.cmdLoadNRM.TabIndex = 5;
            this.cmdLoadNRM.Tag = "\\\\192.168.16.50\\BazaOtd\\BAZEKMZ\\Files\\NRM.txt";
            this.cmdLoadNRM.Text = "Импорт файла NRM (Норма расхода материала)";
            this.cmdLoadNRM.UseVisualStyleBackColor = false;
            this.cmdLoadNRM.Click += new System.EventHandler(this.cmdLoadNRM_Click);
            // 
            // cmdLoadPTN
            // 
            this.cmdLoadPTN.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.cmdLoadPTN.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdLoadPTN.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdLoadPTN.Location = new System.Drawing.Point(6, 303);
            this.cmdLoadPTN.Name = "cmdLoadPTN";
            this.cmdLoadPTN.Size = new System.Drawing.Size(245, 44);
            this.cmdLoadPTN.TabIndex = 4;
            this.cmdLoadPTN.Tag = "\\\\192.168.16.50\\BazaOtd\\BAZEKMZ\\Files\\PTN.txt";
            this.cmdLoadPTN.Text = "Импорт файла ПТН";
            this.cmdLoadPTN.UseVisualStyleBackColor = false;
            this.cmdLoadPTN.Click += new System.EventHandler(this.cmdLoadPTN_Click);
            // 
            // cmdLoadM106
            // 
            this.cmdLoadM106.BackColor = System.Drawing.SystemColors.Control;
            this.cmdLoadM106.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdLoadM106.Location = new System.Drawing.Point(6, 120);
            this.cmdLoadM106.Name = "cmdLoadM106";
            this.cmdLoadM106.Size = new System.Drawing.Size(245, 40);
            this.cmdLoadM106.TabIndex = 3;
            this.cmdLoadM106.Tag = "\\\\192.168.16.50\\BazaOtd\\BAZEKMZ\\Files\\M106.txt";
            this.cmdLoadM106.Text = "Импорт файла М106 (Состав изделий/сборок)";
            this.cmdLoadM106.UseVisualStyleBackColor = false;
            this.cmdLoadM106.Click += new System.EventHandler(this.cmdLoadM106_Click);
            // 
            // cmdLoadCSMR
            // 
            this.cmdLoadCSMR.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.cmdLoadCSMR.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdLoadCSMR.Location = new System.Drawing.Point(6, 176);
            this.cmdLoadCSMR.Name = "cmdLoadCSMR";
            this.cmdLoadCSMR.Size = new System.Drawing.Size(245, 49);
            this.cmdLoadCSMR.TabIndex = 2;
            this.cmdLoadCSMR.Tag = "\\\\192.168.16.50\\BazaOtd\\BAZEKMZ\\Files\\CSMR.txt";
            this.cmdLoadCSMR.Text = "Импотр файла CSMR (Загрузка материалов)";
            this.cmdLoadCSMR.UseVisualStyleBackColor = false;
            this.cmdLoadCSMR.Click += new System.EventHandler(this.cmdLoadCSMR_Click);
            // 
            // cmdImportM101
            // 
            this.cmdImportM101.Location = new System.Drawing.Point(6, 21);
            this.cmdImportM101.Name = "cmdImportM101";
            this.cmdImportM101.Size = new System.Drawing.Size(245, 35);
            this.cmdImportM101.TabIndex = 1;
            this.cmdImportM101.Tag = "\\\\192.168.16.50\\BazaOtd\\BAZEKMZ\\Files\\SPRAVN.txt";
            this.cmdImportM101.Text = "Импорт М101 (Шифры изделий)";
            this.cmdImportM101.UseVisualStyleBackColor = true;
            this.cmdImportM101.Click += new System.EventHandler(this.cmdImportM101_Click);
            // 
            // cmdImportGP
            // 
            this.cmdImportGP.Location = new System.Drawing.Point(6, 71);
            this.cmdImportGP.Name = "cmdImportGP";
            this.cmdImportGP.Size = new System.Drawing.Size(245, 33);
            this.cmdImportGP.TabIndex = 0;
            this.cmdImportGP.Tag = "\\\\192.168.16.50\\BazaOtd\\BAZEKMZ\\Files\\Gprn.txt";
            this.cmdImportGP.Text = "Импорт файла ГП (Детали)";
            this.cmdImportGP.UseVisualStyleBackColor = true;
            this.cmdImportGP.Click += new System.EventHandler(this.cmdImportGP_Click);
            // 
            // dsBaseKMZ
            // 
            this.dsBaseKMZ.DataSetName = "dsBaseKMZ";
            this.dsBaseKMZ.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // tbgpTableAdapter
            // 
            this.tbgpTableAdapter.ClearBeforeFill = true;
            // 
            // tbCodeProductsTableAdapter
            // 
            this.tbCodeProductsTableAdapter.ClearBeforeFill = true;
            // 
            // tBDetailIDTableAdapter
            // 
            this.tBDetailIDTableAdapter.ClearBeforeFill = true;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 458);
            this.Controls.Add(this.gbImportInfo);
            this.Controls.Add(this.statusBar);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Корректировка базы ММС";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.gbImportInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dsBaseKMZ)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel tsDate;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.GroupBox gbImportInfo;
        private System.Windows.Forms.Button cmdImportGP;
        private System.Windows.Forms.ToolStripStatusLabel tsLabInfo;
        private dsBaseKMZ dsBaseKMZ;
        private dsBaseKMZTableAdapters.TBGPTableAdapter tbgpTableAdapter;
        private System.Windows.Forms.Button cmdImportM101;
        private dsBaseKMZTableAdapters.TBDetailIDTableAdapter tBDetailIDTableAdapter;
        private System.Windows.Forms.Button cmdLoadCSMR;
        private System.Windows.Forms.Button cmdLoadM106;
        private System.Windows.Forms.Button cmdLoadPTN;
        private System.Windows.Forms.Button cmdLoadNRM;
        private System.Windows.Forms.Button cmdLoadRoutes;
        private System.Windows.Forms.Button cmdPer300;
        private System.Windows.Forms.Button cmdOSTM;
        private dsBaseKMZTableAdapters.TbCodeProductsTableAdapter tbCodeProductsTableAdapter;
        public System.Windows.Forms.ToolStripProgressBar tsProgressBar;
        public System.Windows.Forms.Button M104Load_Click;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}

