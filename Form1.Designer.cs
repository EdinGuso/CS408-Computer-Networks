namespace project_client
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_IP = new System.Windows.Forms.TextBox();
            this.textBox_Port = new System.Windows.Forms.TextBox();
            this.textBox_Username = new System.Windows.Forms.TextBox();
            this.logs = new System.Windows.Forms.RichTextBox();
            this.button_Connect = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.button_Browse = new System.Windows.Forms.Button();
            this.button_Send = new System.Windows.Forms.Button();
            this.button_GetFileList = new System.Windows.Forms.Button();
            this.textBox_Filename = new System.Windows.Forms.TextBox();
            this.button_Download = new System.Windows.Forms.Button();
            this.button_Copy = new System.Windows.Forms.Button();
            this.button_Delete = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.button_BrowseFolder = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.button_GetPublicFileList = new System.Windows.Forms.Button();
            this.textBox_ClientName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.button_MakePublic = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_publicFilename = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.button_publicBrowseFolder = new System.Windows.Forms.Button();
            this.button_publicDownload = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(72, 52);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(72, 88);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(72, 122);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Username";
            // 
            // textBox_IP
            // 
            this.textBox_IP.Location = new System.Drawing.Point(149, 49);
            this.textBox_IP.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.textBox_IP.Name = "textBox_IP";
            this.textBox_IP.Size = new System.Drawing.Size(103, 22);
            this.textBox_IP.TabIndex = 3;
            // 
            // textBox_Port
            // 
            this.textBox_Port.Location = new System.Drawing.Point(149, 84);
            this.textBox_Port.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.textBox_Port.Name = "textBox_Port";
            this.textBox_Port.Size = new System.Drawing.Size(103, 22);
            this.textBox_Port.TabIndex = 4;
            // 
            // textBox_Username
            // 
            this.textBox_Username.Location = new System.Drawing.Point(149, 122);
            this.textBox_Username.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.textBox_Username.Name = "textBox_Username";
            this.textBox_Username.Size = new System.Drawing.Size(103, 22);
            this.textBox_Username.TabIndex = 5;
            // 
            // logs
            // 
            this.logs.Location = new System.Drawing.Point(277, 37);
            this.logs.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.logs.Name = "logs";
            this.logs.ReadOnly = true;
            this.logs.Size = new System.Drawing.Size(474, 353);
            this.logs.TabIndex = 6;
            this.logs.Text = "";
            // 
            // button_Connect
            // 
            this.button_Connect.Location = new System.Drawing.Point(149, 154);
            this.button_Connect.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.button_Connect.Name = "button_Connect";
            this.button_Connect.Size = new System.Drawing.Size(103, 37);
            this.button_Connect.TabIndex = 7;
            this.button_Connect.Text = "Connect";
            this.button_Connect.UseVisualStyleBackColor = true;
            this.button_Connect.Click += new System.EventHandler(this.button_Connect_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(72, 298);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "Select File";
            // 
            // button_Browse
            // 
            this.button_Browse.Enabled = false;
            this.button_Browse.Location = new System.Drawing.Point(149, 286);
            this.button_Browse.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.button_Browse.Name = "button_Browse";
            this.button_Browse.Size = new System.Drawing.Size(103, 37);
            this.button_Browse.TabIndex = 10;
            this.button_Browse.Text = "Browse";
            this.button_Browse.UseVisualStyleBackColor = true;
            this.button_Browse.Click += new System.EventHandler(this.button_Browse_Click);
            // 
            // button_Send
            // 
            this.button_Send.Enabled = false;
            this.button_Send.Location = new System.Drawing.Point(149, 335);
            this.button_Send.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.button_Send.Name = "button_Send";
            this.button_Send.Size = new System.Drawing.Size(103, 37);
            this.button_Send.TabIndex = 11;
            this.button_Send.Text = "Send";
            this.button_Send.UseVisualStyleBackColor = true;
            this.button_Send.Click += new System.EventHandler(this.button_Send_Click);
            // 
            // button_GetFileList
            // 
            this.button_GetFileList.Enabled = false;
            this.button_GetFileList.Location = new System.Drawing.Point(772, 37);
            this.button_GetFileList.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.button_GetFileList.Name = "button_GetFileList";
            this.button_GetFileList.Size = new System.Drawing.Size(103, 37);
            this.button_GetFileList.TabIndex = 12;
            this.button_GetFileList.Text = "Get File List";
            this.button_GetFileList.UseVisualStyleBackColor = true;
            this.button_GetFileList.Click += new System.EventHandler(this.button_GetFileList_Click);
            // 
            // textBox_Filename
            // 
            this.textBox_Filename.Enabled = false;
            this.textBox_Filename.Location = new System.Drawing.Point(772, 104);
            this.textBox_Filename.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.textBox_Filename.Name = "textBox_Filename";
            this.textBox_Filename.Size = new System.Drawing.Size(103, 22);
            this.textBox_Filename.TabIndex = 13;
            // 
            // button_Download
            // 
            this.button_Download.Enabled = false;
            this.button_Download.Location = new System.Drawing.Point(772, 211);
            this.button_Download.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.button_Download.Name = "button_Download";
            this.button_Download.Size = new System.Drawing.Size(103, 37);
            this.button_Download.TabIndex = 14;
            this.button_Download.Text = "Download";
            this.button_Download.UseVisualStyleBackColor = true;
            this.button_Download.Click += new System.EventHandler(this.button_Download_Click);
            // 
            // button_Copy
            // 
            this.button_Copy.Enabled = false;
            this.button_Copy.Location = new System.Drawing.Point(772, 258);
            this.button_Copy.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.button_Copy.Name = "button_Copy";
            this.button_Copy.Size = new System.Drawing.Size(103, 37);
            this.button_Copy.TabIndex = 15;
            this.button_Copy.Text = "Copy";
            this.button_Copy.UseVisualStyleBackColor = true;
            this.button_Copy.Click += new System.EventHandler(this.button_Copy_Click);
            // 
            // button_Delete
            // 
            this.button_Delete.Enabled = false;
            this.button_Delete.Location = new System.Drawing.Point(772, 355);
            this.button_Delete.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.button_Delete.Name = "button_Delete";
            this.button_Delete.Size = new System.Drawing.Size(103, 37);
            this.button_Delete.TabIndex = 16;
            this.button_Delete.Text = "Delete";
            this.button_Delete.UseVisualStyleBackColor = true;
            this.button_Delete.Click += new System.EventHandler(this.button_Delete_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(769, 87);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 17);
            this.label5.TabIndex = 17;
            this.label5.Text = "Filename";
            // 
            // button_BrowseFolder
            // 
            this.button_BrowseFolder.Enabled = false;
            this.button_BrowseFolder.Location = new System.Drawing.Point(772, 164);
            this.button_BrowseFolder.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.button_BrowseFolder.Name = "button_BrowseFolder";
            this.button_BrowseFolder.Size = new System.Drawing.Size(103, 37);
            this.button_BrowseFolder.TabIndex = 18;
            this.button_BrowseFolder.Text = "Browse";
            this.button_BrowseFolder.UseVisualStyleBackColor = true;
            this.button_BrowseFolder.Click += new System.EventHandler(this.button_BrowseFolder_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(769, 146);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 17);
            this.label6.TabIndex = 19;
            this.label6.Text = "Select Folder";
            // 
            // button_GetPublicFileList
            // 
            this.button_GetPublicFileList.Enabled = false;
            this.button_GetPublicFileList.Location = new System.Drawing.Point(917, 37);
            this.button_GetPublicFileList.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.button_GetPublicFileList.Name = "button_GetPublicFileList";
            this.button_GetPublicFileList.Size = new System.Drawing.Size(143, 37);
            this.button_GetPublicFileList.TabIndex = 20;
            this.button_GetPublicFileList.Text = "Get Public File List";
            this.button_GetPublicFileList.UseVisualStyleBackColor = true;
            this.button_GetPublicFileList.Click += new System.EventHandler(this.button_GetPublicFileList_Click);
            // 
            // textBox_ClientName
            // 
            this.textBox_ClientName.Enabled = false;
            this.textBox_ClientName.Location = new System.Drawing.Point(917, 161);
            this.textBox_ClientName.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.textBox_ClientName.Name = "textBox_ClientName";
            this.textBox_ClientName.Size = new System.Drawing.Size(103, 22);
            this.textBox_ClientName.TabIndex = 21;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(914, 143);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 17);
            this.label7.TabIndex = 22;
            this.label7.Text = "Client name";
            // 
            // button_MakePublic
            // 
            this.button_MakePublic.Enabled = false;
            this.button_MakePublic.Location = new System.Drawing.Point(772, 307);
            this.button_MakePublic.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.button_MakePublic.Name = "button_MakePublic";
            this.button_MakePublic.Size = new System.Drawing.Size(103, 37);
            this.button_MakePublic.TabIndex = 23;
            this.button_MakePublic.Text = "Make Public";
            this.button_MakePublic.UseVisualStyleBackColor = true;
            this.button_MakePublic.Click += new System.EventHandler(this.button_MakePublic_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(914, 89);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 17);
            this.label8.TabIndex = 25;
            this.label8.Text = "Filename";
            // 
            // textBox_publicFilename
            // 
            this.textBox_publicFilename.Enabled = false;
            this.textBox_publicFilename.Location = new System.Drawing.Point(917, 106);
            this.textBox_publicFilename.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.textBox_publicFilename.Name = "textBox_publicFilename";
            this.textBox_publicFilename.Size = new System.Drawing.Size(103, 22);
            this.textBox_publicFilename.TabIndex = 24;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(914, 193);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(91, 17);
            this.label9.TabIndex = 28;
            this.label9.Text = "Select Folder";
            // 
            // button_publicBrowseFolder
            // 
            this.button_publicBrowseFolder.Enabled = false;
            this.button_publicBrowseFolder.Location = new System.Drawing.Point(917, 211);
            this.button_publicBrowseFolder.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.button_publicBrowseFolder.Name = "button_publicBrowseFolder";
            this.button_publicBrowseFolder.Size = new System.Drawing.Size(103, 37);
            this.button_publicBrowseFolder.TabIndex = 27;
            this.button_publicBrowseFolder.Text = "Browse";
            this.button_publicBrowseFolder.UseVisualStyleBackColor = true;
            this.button_publicBrowseFolder.Click += new System.EventHandler(this.button_publicBrowseFolder_Click);
            // 
            // button_publicDownload
            // 
            this.button_publicDownload.Enabled = false;
            this.button_publicDownload.Location = new System.Drawing.Point(917, 258);
            this.button_publicDownload.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.button_publicDownload.Name = "button_publicDownload";
            this.button_publicDownload.Size = new System.Drawing.Size(103, 37);
            this.button_publicDownload.TabIndex = 26;
            this.button_publicDownload.Text = "Download";
            this.button_publicDownload.UseVisualStyleBackColor = true;
            this.button_publicDownload.Click += new System.EventHandler(this.button_publicDownload_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1089, 431);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.button_publicBrowseFolder);
            this.Controls.Add(this.button_publicDownload);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBox_publicFilename);
            this.Controls.Add(this.button_MakePublic);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBox_ClientName);
            this.Controls.Add(this.button_GetPublicFileList);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.button_BrowseFolder);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button_Delete);
            this.Controls.Add(this.button_Copy);
            this.Controls.Add(this.button_Download);
            this.Controls.Add(this.textBox_Filename);
            this.Controls.Add(this.button_GetFileList);
            this.Controls.Add(this.button_Send);
            this.Controls.Add(this.button_Browse);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button_Connect);
            this.Controls.Add(this.logs);
            this.Controls.Add(this.textBox_Username);
            this.Controls.Add(this.textBox_Port);
            this.Controls.Add(this.textBox_IP);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.Name = "Form1";
            this.Text = "Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_IP;
        private System.Windows.Forms.TextBox textBox_Port;
        private System.Windows.Forms.TextBox textBox_Username;
        private System.Windows.Forms.RichTextBox logs;
        private System.Windows.Forms.Button button_Connect;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button_Browse;
        private System.Windows.Forms.Button button_Send;
        private System.Windows.Forms.Button button_GetFileList;
        private System.Windows.Forms.TextBox textBox_Filename;
        private System.Windows.Forms.Button button_Download;
        private System.Windows.Forms.Button button_Copy;
        private System.Windows.Forms.Button button_Delete;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_BrowseFolder;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button_GetPublicFileList;
        private System.Windows.Forms.TextBox textBox_ClientName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button_MakePublic;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox_publicFilename;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button button_publicBrowseFolder;
        private System.Windows.Forms.Button button_publicDownload;
    }
}

