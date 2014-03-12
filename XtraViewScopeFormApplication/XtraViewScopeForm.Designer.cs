using System;
using System.IO;
namespace XtraViewScopeFormApplication
{
    partial class XtraViewScopeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XtraViewScopeForm));
            this.startButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.configFilePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.outputDirectory = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.fileNameFormat = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.fileNameFormatSuffix = new System.Windows.Forms.Label();
            this.stopButton = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.progressReportLinkLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(574, 141);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // configFilePath
            // 
            this.configFilePath.Location = new System.Drawing.Point(142, 23);
            this.configFilePath.Multiline = true;
            this.configFilePath.Name = "configFilePath";
            this.configFilePath.ReadOnly = true;
            this.configFilePath.Size = new System.Drawing.Size(507, 36);
            this.configFilePath.TabIndex = 1;
            this.configFilePath.Text = "C:\\Projects\\XtraViewScopeFormApplication\\XtraViewScopeFormApplication\\Resources\\X" +
    "MLFile1.xml";
            this.configFilePath.Click += new System.EventHandler(this.configFilePath_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Config File Path";
            // 
            // outputDirectory
            // 
            this.outputDirectory.Location = new System.Drawing.Point(143, 65);
            this.outputDirectory.Multiline = true;
            this.outputDirectory.Name = "outputDirectory";
            this.outputDirectory.ReadOnly = true;
            this.outputDirectory.Size = new System.Drawing.Size(506, 42);
            this.outputDirectory.TabIndex = 3;
            this.outputDirectory.Text = "C:\\waveform";
            this.outputDirectory.Click += new System.EventHandler(this.configFilePath_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Output Directory";
            // 
            // fileNameFormat
            // 
            this.fileNameFormat.Location = new System.Drawing.Point(142, 113);
            this.fileNameFormat.Name = "fileNameFormat";
            this.fileNameFormat.Size = new System.Drawing.Size(205, 20);
            this.fileNameFormat.TabIndex = 5;
            this.fileNameFormat.Text = "Scope_Analysis";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "File Name Format";
            // 
            // fileNameFormatSuffix
            // 
            this.fileNameFormatSuffix.AutoSize = true;
            this.fileNameFormatSuffix.Location = new System.Drawing.Point(350, 116);
            this.fileNameFormatSuffix.Name = "fileNameFormatSuffix";
            this.fileNameFormatSuffix.Size = new System.Drawing.Size(0, 13);
            this.fileNameFormatSuffix.TabIndex = 7;
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(493, 141);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 8;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // progressReportLinkLabel
            // 
            this.progressReportLinkLabel.AutoSize = true;
            this.progressReportLinkLabel.Location = new System.Drawing.Point(12, 146);
            this.progressReportLinkLabel.Name = "progressReportLinkLabel";
            this.progressReportLinkLabel.Size = new System.Drawing.Size(0, 13);
            this.progressReportLinkLabel.TabIndex = 10;
            this.progressReportLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.progressReportLinkLabel_LinkClicked);
            // 
            // XtraViewScopeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 172);
            this.Controls.Add(this.progressReportLinkLabel);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.fileNameFormatSuffix);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.fileNameFormat);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.outputDirectory);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.configFilePath);
            this.Controls.Add(this.startButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "XtraViewScopeForm";
            this.Text = "XtraViewScope";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox configFilePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox outputDirectory;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox fileNameFormat;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label fileNameFormatSuffix;
        private System.Windows.Forms.Button stopButton;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.LinkLabel progressReportLinkLabel;
    }
}

