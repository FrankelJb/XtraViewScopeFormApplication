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
            this.uiBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.fileFormatInformationLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.counterLabel = new System.Windows.Forms.Label();
            this.averageLabel = new System.Windows.Forms.Label();
            this.shortestLabel = new System.Windows.Forms.Label();
            this.longestLabel = new System.Windows.Forms.Label();
            this.irKeyPresses = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.shouldSaveKeyPresses = new System.Windows.Forms.CheckBox();
            this.clearKeyPresses = new System.Windows.Forms.Button();
            this.graphHeartbeats = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(586, 141);
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
            this.configFilePath.Size = new System.Drawing.Size(519, 36);
            this.configFilePath.TabIndex = 1;
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
            this.outputDirectory.Size = new System.Drawing.Size(518, 42);
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
            this.stopButton.Location = new System.Drawing.Point(505, 141);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 8;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // uiBackgroundWorker
            // 
            this.uiBackgroundWorker.WorkerReportsProgress = true;
            this.uiBackgroundWorker.WorkerSupportsCancellation = true;
            this.uiBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.uiBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.uiBackgroundWorker_ProgressChanged);
            this.uiBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.uiBackgroundWorker_RunWorkerCompleted);
            // 
            // fileFormatInformationLabel
            // 
            this.fileFormatInformationLabel.AutoSize = true;
            this.fileFormatInformationLabel.Location = new System.Drawing.Point(347, 119);
            this.fileFormatInformationLabel.Name = "fileFormatInformationLabel";
            this.fileFormatInformationLabel.Size = new System.Drawing.Size(303, 13);
            this.fileFormatInformationLabel.TabIndex = 11;
            this.fileFormatInformationLabel.Text = "\"_yyyyMMddHHmmssfff.format\" will automatically be appended";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(361, 141);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 23);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // counterLabel
            // 
            this.counterLabel.AutoSize = true;
            this.counterLabel.Location = new System.Drawing.Point(358, 146);
            this.counterLabel.Name = "counterLabel";
            this.counterLabel.Size = new System.Drawing.Size(0, 13);
            this.counterLabel.TabIndex = 13;
            // 
            // averageLabel
            // 
            this.averageLabel.AutoSize = true;
            this.averageLabel.Location = new System.Drawing.Point(358, 200);
            this.averageLabel.Name = "averageLabel";
            this.averageLabel.Size = new System.Drawing.Size(53, 13);
            this.averageLabel.TabIndex = 14;
            this.averageLabel.Text = "Average: ";
            // 
            // shortestLabel
            // 
            this.shortestLabel.AutoSize = true;
            this.shortestLabel.Location = new System.Drawing.Point(358, 224);
            this.shortestLabel.Name = "shortestLabel";
            this.shortestLabel.Size = new System.Drawing.Size(49, 13);
            this.shortestLabel.TabIndex = 15;
            this.shortestLabel.Text = "Shortest:";
            // 
            // longestLabel
            // 
            this.longestLabel.AutoSize = true;
            this.longestLabel.Location = new System.Drawing.Point(358, 246);
            this.longestLabel.Name = "longestLabel";
            this.longestLabel.Size = new System.Drawing.Size(45, 13);
            this.longestLabel.TabIndex = 16;
            this.longestLabel.Text = "Longest";
            // 
            // irKeyPresses
            // 
            this.irKeyPresses.Location = new System.Drawing.Point(12, 197);
            this.irKeyPresses.Multiline = true;
            this.irKeyPresses.Name = "irKeyPresses";
            this.irKeyPresses.ReadOnly = true;
            this.irKeyPresses.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.irKeyPresses.Size = new System.Drawing.Size(311, 278);
            this.irKeyPresses.TabIndex = 17;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(358, 178);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Heartbeat Timing";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(13, 178);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "Key Presses";
            // 
            // shouldSaveKeyPresses
            // 
            this.shouldSaveKeyPresses.AutoSize = true;
            this.shouldSaveKeyPresses.Location = new System.Drawing.Point(12, 147);
            this.shouldSaveKeyPresses.Name = "shouldSaveKeyPresses";
            this.shouldSaveKeyPresses.Size = new System.Drawing.Size(143, 17);
            this.shouldSaveKeyPresses.TabIndex = 20;
            this.shouldSaveKeyPresses.Text = "Save Key Presses to File";
            this.shouldSaveKeyPresses.UseVisualStyleBackColor = true;
            // 
            // clearKeyPresses
            // 
            this.clearKeyPresses.Location = new System.Drawing.Point(12, 484);
            this.clearKeyPresses.Name = "clearKeyPresses";
            this.clearKeyPresses.Size = new System.Drawing.Size(75, 23);
            this.clearKeyPresses.TabIndex = 21;
            this.clearKeyPresses.Text = "Clear";
            this.clearKeyPresses.UseVisualStyleBackColor = true;
            this.clearKeyPresses.Click += new System.EventHandler(this.clearKeyPresses_Click);
            // 
            // graphHeartbeats
            // 
            this.graphHeartbeats.Location = new System.Drawing.Point(360, 279);
            this.graphHeartbeats.Name = "graphHeartbeats";
            this.graphHeartbeats.Size = new System.Drawing.Size(75, 35);
            this.graphHeartbeats.TabIndex = 22;
            this.graphHeartbeats.Text = "Graph Heartbeats";
            this.graphHeartbeats.UseVisualStyleBackColor = true;
            this.graphHeartbeats.Click += new System.EventHandler(this.graphHeartbeats_Click);
            // 
            // XtraViewScopeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 519);
            this.Controls.Add(this.graphHeartbeats);
            this.Controls.Add(this.clearKeyPresses);
            this.Controls.Add(this.shouldSaveKeyPresses);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.irKeyPresses);
            this.Controls.Add(this.longestLabel);
            this.Controls.Add(this.shortestLabel);
            this.Controls.Add(this.averageLabel);
            this.Controls.Add(this.counterLabel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.fileFormatInformationLabel);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
        private System.Windows.Forms.Label fileFormatInformationLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label counterLabel;
        private System.Windows.Forms.Label averageLabel;
        private System.Windows.Forms.Label shortestLabel;
        private System.Windows.Forms.Label longestLabel;
        public System.ComponentModel.BackgroundWorker uiBackgroundWorker;
        private System.Windows.Forms.TextBox irKeyPresses;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.CheckBox shouldSaveKeyPresses;
        private System.Windows.Forms.Button clearKeyPresses;
        private System.Windows.Forms.Button graphHeartbeats;
    }
}

