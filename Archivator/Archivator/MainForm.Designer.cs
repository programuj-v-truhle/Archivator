namespace Archivator
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.souborToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutBox = new System.Windows.Forms.ToolStripMenuItem();
            this.openArchieve = new System.Windows.Forms.ToolStripMenuItem();
            this.newArchieve = new System.Windows.Forms.ToolStripMenuItem();
            this.saveArchieve = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeApp = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.listFiles = new System.Windows.Forms.ListBox();
            this.buttonExtract = new System.Windows.Forms.Button();
            this.saveDialog = new System.Windows.Forms.SaveFileDialog();
            this.openDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listFiles);
            this.groupBox1.Location = new System.Drawing.Point(0, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(406, 164);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Soubory";
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.souborToolStripMenuItem,
            this.aboutBox});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(409, 24);
            this.mainMenu.TabIndex = 1;
            this.mainMenu.Text = "menu";
            // 
            // souborToolStripMenuItem
            // 
            this.souborToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newArchieve,
            this.openArchieve,
            this.saveArchieve,
            this.toolStripMenuItem1,
            this.closeApp});
            this.souborToolStripMenuItem.Name = "souborToolStripMenuItem";
            this.souborToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.souborToolStripMenuItem.Text = "&Soubor";
            // 
            // aboutBox
            // 
            this.aboutBox.Name = "aboutBox";
            this.aboutBox.Size = new System.Drawing.Size(76, 20);
            this.aboutBox.Text = "&O programu";
            this.aboutBox.Click += new System.EventHandler(this.aboutBox_Click);
            // 
            // openArchieve
            // 
            this.openArchieve.Name = "openArchieve";
            this.openArchieve.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openArchieve.Size = new System.Drawing.Size(191, 22);
            this.openArchieve.Text = "&Otevřít archív";
            this.openArchieve.Click += new System.EventHandler(this.openArchieve_Click);
            // 
            // newArchieve
            // 
            this.newArchieve.Name = "newArchieve";
            this.newArchieve.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newArchieve.Size = new System.Drawing.Size(191, 22);
            this.newArchieve.Text = "&Nový archív";
            this.newArchieve.Click += new System.EventHandler(this.newArchieve_Click);
            // 
            // saveArchieve
            // 
            this.saveArchieve.Name = "saveArchieve";
            this.saveArchieve.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveArchieve.Size = new System.Drawing.Size(191, 22);
            this.saveArchieve.Text = "&Uložit";
            this.saveArchieve.Click += new System.EventHandler(this.saveArchieve_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(188, 6);
            // 
            // closeApp
            // 
            this.closeApp.Name = "closeApp";
            this.closeApp.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.closeApp.Size = new System.Drawing.Size(191, 22);
            this.closeApp.Text = "&Ukončit";
            this.closeApp.Click += new System.EventHandler(this.closeApp_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Enabled = false;
            this.buttonAdd.Location = new System.Drawing.Point(0, 197);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(101, 23);
            this.buttonAdd.TabIndex = 2;
            this.buttonAdd.Text = "Přidat soubor";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Enabled = false;
            this.buttonRemove.Location = new System.Drawing.Point(107, 197);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(101, 23);
            this.buttonRemove.TabIndex = 3;
            this.buttonRemove.Text = "Odebrat soubor";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // listFiles
            // 
            this.listFiles.FormattingEnabled = true;
            this.listFiles.Location = new System.Drawing.Point(6, 19);
            this.listFiles.MultiColumn = true;
            this.listFiles.Name = "listFiles";
            this.listFiles.Size = new System.Drawing.Size(391, 134);
            this.listFiles.TabIndex = 0;
            // 
            // buttonExtract
            // 
            this.buttonExtract.Enabled = false;
            this.buttonExtract.Location = new System.Drawing.Point(305, 197);
            this.buttonExtract.Name = "buttonExtract";
            this.buttonExtract.Size = new System.Drawing.Size(101, 23);
            this.buttonExtract.TabIndex = 4;
            this.buttonExtract.Text = "Extrahovat soubor";
            this.buttonExtract.UseVisualStyleBackColor = true;
            this.buttonExtract.Click += new System.EventHandler(this.buttonExtract_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 225);
            this.Controls.Add(this.buttonExtract);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Vlastní archivátor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox listFiles;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem souborToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newArchieve;
        private System.Windows.Forms.ToolStripMenuItem openArchieve;
        private System.Windows.Forms.ToolStripMenuItem saveArchieve;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem closeApp;
        private System.Windows.Forms.ToolStripMenuItem aboutBox;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonExtract;
        private System.Windows.Forms.SaveFileDialog saveDialog;
        private System.Windows.Forms.OpenFileDialog openDialog;
    }
}

