namespace Maze
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
            this.btnLoad = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cmbAlgorithm = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblTimer = new System.Windows.Forms.Label();
            this.lblNodeCount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbOutputPath = new System.Windows.Forms.CheckBox();
            this.pbMaze = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbMaze)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Location = new System.Drawing.Point(335, 489);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(89, 23);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(70, 69);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(47, 13);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "lblStatus";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmbAlgorithm
            // 
            this.cmbAlgorithm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAlgorithm.FormattingEnabled = true;
            this.cmbAlgorithm.Items.AddRange(new object[] {
            "BreadthFirst",
            "DepthFirst",
            "DeadEndFill"});
            this.cmbAlgorithm.Location = new System.Drawing.Point(175, 489);
            this.cmbAlgorithm.Name = "cmbAlgorithm";
            this.cmbAlgorithm.Size = new System.Drawing.Size(152, 21);
            this.cmbAlgorithm.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Timer:";
            // 
            // lblTimer
            // 
            this.lblTimer.AutoSize = true;
            this.lblTimer.Location = new System.Drawing.Point(70, 20);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(43, 13);
            this.lblTimer.TabIndex = 4;
            this.lblTimer.Text = "lblTimer";
            // 
            // lblNodeCount
            // 
            this.lblNodeCount.AutoSize = true;
            this.lblNodeCount.Location = new System.Drawing.Point(70, 44);
            this.lblNodeCount.Name = "lblNodeCount";
            this.lblNodeCount.Size = new System.Drawing.Size(71, 13);
            this.lblNodeCount.TabIndex = 5;
            this.lblNodeCount.Text = "lblNodeCount";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Nodes :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Status:";
            // 
            // cbOutputPath
            // 
            this.cbOutputPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbOutputPath.AutoSize = true;
            this.cbOutputPath.Checked = true;
            this.cbOutputPath.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOutputPath.Location = new System.Drawing.Point(175, 464);
            this.cbOutputPath.Name = "cbOutputPath";
            this.cbOutputPath.Size = new System.Drawing.Size(140, 17);
            this.cbOutputPath.TabIndex = 9;
            this.cbOutputPath.Text = "Output path (vs flood-fill)";
            this.cbOutputPath.UseVisualStyleBackColor = true;
            // 
            // pbMaze
            // 
            this.pbMaze.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbMaze.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbMaze.Location = new System.Drawing.Point(12, 102);
            this.pbMaze.Name = "pbMaze";
            this.pbMaze.Size = new System.Drawing.Size(414, 313);
            this.pbMaze.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbMaze.TabIndex = 10;
            this.pbMaze.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(441, 523);
            this.Controls.Add(this.pbMaze);
            this.Controls.Add(this.cbOutputPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblNodeCount);
            this.Controls.Add(this.lblTimer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbAlgorithm);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnLoad);
            this.Name = "MainForm";
            this.Text = "Maze";
            ((System.ComponentModel.ISupportInitialize)(this.pbMaze)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cmbAlgorithm;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Label lblNodeCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbOutputPath;
        private System.Windows.Forms.PictureBox pbMaze;
    }
}

