namespace AtoK
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.outputList = new System.Windows.Forms.ListBox();
            this.SaveExtractedDocs = new System.Windows.Forms.CheckBox();
            this.LibraryGen = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.button2 = new System.Windows.Forms.Button();
            this.Verbose = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.busy = new System.Windows.Forms.Button();
            this.CleanUp = new System.Windows.Forms.Button();
            this.LaunchPCBNew = new System.Windows.Forms.Button();
            this.FileHistory = new System.Windows.Forms.ComboBox();
            this.ClearHistory = new System.Windows.Forms.Button();
            this.Edit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // outputList
            // 
            this.outputList.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.outputList.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputList.FormattingEnabled = true;
            this.outputList.HorizontalScrollbar = true;
            this.outputList.ItemHeight = 16;
            this.outputList.Location = new System.Drawing.Point(12, 103);
            this.outputList.Name = "outputList";
            this.outputList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.outputList.Size = new System.Drawing.Size(668, 132);
            this.outputList.TabIndex = 97;
            this.outputList.SelectedIndexChanged += new System.EventHandler(this.outputList_SelectedIndexChanged);
            // 
            // SaveExtractedDocs
            // 
            this.SaveExtractedDocs.AutoSize = true;
            this.SaveExtractedDocs.Location = new System.Drawing.Point(25, 72);
            this.SaveExtractedDocs.Name = "SaveExtractedDocs";
            this.SaveExtractedDocs.Size = new System.Drawing.Size(159, 18);
            this.SaveExtractedDocs.TabIndex = 98;
            this.SaveExtractedDocs.Text = "Save Extracted Docs";
            this.SaveExtractedDocs.UseVisualStyleBackColor = true;
            // 
            // LibraryGen
            // 
            this.LibraryGen.AutoSize = true;
            this.LibraryGen.Location = new System.Drawing.Point(228, 72);
            this.LibraryGen.Name = "LibraryGen";
            this.LibraryGen.Size = new System.Drawing.Size(138, 18);
            this.LibraryGen.TabIndex = 99;
            this.LibraryGen.Text = "Generate Library";
            this.LibraryGen.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(149, 11);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(118, 22);
            this.button1.TabIndex = 100;
            this.button1.Text = "Convert";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFile
            // 
            this.openFile.DefaultExt = "pcbdoc";
            this.openFile.FileName = "openFileDialog1";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 11);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(118, 22);
            this.button2.TabIndex = 102;
            this.button2.Text = "Select Source PCB";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Verbose
            // 
            this.Verbose.AutoSize = true;
            this.Verbose.Checked = true;
            this.Verbose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Verbose.Location = new System.Drawing.Point(410, 72);
            this.Verbose.Name = "Verbose";
            this.Verbose.Size = new System.Drawing.Size(75, 18);
            this.Verbose.TabIndex = 103;
            this.Verbose.Text = "Verbose";
            this.Verbose.UseVisualStyleBackColor = true;
            this.Verbose.Click += new System.EventHandler(this.Verbose_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // busy
            // 
            this.busy.Location = new System.Drawing.Point(542, 69);
            this.busy.Name = "busy";
            this.busy.Size = new System.Drawing.Size(31, 23);
            this.busy.TabIndex = 104;
            this.busy.UseVisualStyleBackColor = true;
            this.busy.Visible = false;
            // 
            // CleanUp
            // 
            this.CleanUp.Location = new System.Drawing.Point(286, 10);
            this.CleanUp.Name = "CleanUp";
            this.CleanUp.Size = new System.Drawing.Size(118, 23);
            this.CleanUp.TabIndex = 105;
            this.CleanUp.Text = "Clean Up";
            this.CleanUp.UseVisualStyleBackColor = true;
            this.CleanUp.Click += new System.EventHandler(this.CleanUp_Click);
            // 
            // LaunchPCBNew
            // 
            this.LaunchPCBNew.Location = new System.Drawing.Point(423, 11);
            this.LaunchPCBNew.Name = "LaunchPCBNew";
            this.LaunchPCBNew.Size = new System.Drawing.Size(118, 23);
            this.LaunchPCBNew.TabIndex = 106;
            this.LaunchPCBNew.Text = "Launch PCBNew";
            this.LaunchPCBNew.UseVisualStyleBackColor = true;
            this.LaunchPCBNew.Click += new System.EventHandler(this.LaunchPCBNew_Click);
            // 
            // FileHistory
            // 
            this.FileHistory.FormattingEnabled = true;
            this.FileHistory.Location = new System.Drawing.Point(12, 39);
            this.FileHistory.MaxDropDownItems = 20;
            this.FileHistory.Name = "FileHistory";
            this.FileHistory.Size = new System.Drawing.Size(666, 22);
            this.FileHistory.TabIndex = 107;
            this.FileHistory.SelectedIndexChanged += new System.EventHandler(this.FileHistory_SelectedIndexChanged);
            // 
            // ClearHistory
            // 
            this.ClearHistory.Location = new System.Drawing.Point(560, 10);
            this.ClearHistory.Name = "ClearHistory";
            this.ClearHistory.Size = new System.Drawing.Size(118, 23);
            this.ClearHistory.TabIndex = 108;
            this.ClearHistory.Text = "Clear History";
            this.ClearHistory.UseVisualStyleBackColor = true;
            this.ClearHistory.Click += new System.EventHandler(this.ClearHistory_Click);
            // 
            // Edit
            // 
            this.Edit.Location = new System.Drawing.Point(603, 69);
            this.Edit.Name = "Edit";
            this.Edit.Size = new System.Drawing.Size(75, 23);
            this.Edit.TabIndex = 109;
            this.Edit.Text = "Edit";
            this.Edit.UseVisualStyleBackColor = true;
            this.Edit.Click += new System.EventHandler(this.Edit_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(690, 247);
            this.Controls.Add(this.Edit);
            this.Controls.Add(this.ClearHistory);
            this.Controls.Add(this.FileHistory);
            this.Controls.Add(this.LaunchPCBNew);
            this.Controls.Add(this.CleanUp);
            this.Controls.Add(this.busy);
            this.Controls.Add(this.Verbose);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.LibraryGen);
            this.Controls.Add(this.SaveExtractedDocs);
            this.Controls.Add(this.outputList);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(470, 100);
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "AtoK";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_Closing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox outputList;
        private System.Windows.Forms.CheckBox SaveExtractedDocs;
        private System.Windows.Forms.CheckBox LibraryGen;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFile;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox Verbose;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button busy;
        private System.Windows.Forms.Button CleanUp;
        private System.Windows.Forms.Button LaunchPCBNew;
        private System.Windows.Forms.ComboBox FileHistory;
        private System.Windows.Forms.Button ClearHistory;
        private System.Windows.Forms.Button Edit;
    }
}

