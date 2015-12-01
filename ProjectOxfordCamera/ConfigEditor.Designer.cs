namespace ProjectOxfordCamera
{
    partial class ConfigEditor
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
            this.textBoxApiKey = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.linkLabelApiKey = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // textBoxApiKey
            // 
            this.textBoxApiKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxApiKey.Location = new System.Drawing.Point(65, 12);
            this.textBoxApiKey.Name = "textBoxApiKey";
            this.textBoxApiKey.Size = new System.Drawing.Size(255, 20);
            this.textBoxApiKey.TabIndex = 0;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOK.Location = new System.Drawing.Point(12, 48);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(93, 48);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // linkLabelApiKey
            // 
            this.linkLabelApiKey.AutoSize = true;
            this.linkLabelApiKey.Location = new System.Drawing.Point(12, 15);
            this.linkLabelApiKey.Name = "linkLabelApiKey";
            this.linkLabelApiKey.Size = new System.Drawing.Size(48, 13);
            this.linkLabelApiKey.TabIndex = 4;
            this.linkLabelApiKey.Text = "API Key:";
            this.linkLabelApiKey.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelApiKey_LinkClicked);
            // 
            // ConfigEditor
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(332, 83);
            this.Controls.Add(this.linkLabelApiKey);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxApiKey);
            this.Name = "ConfigEditor";
            this.Text = "ConfigEditor";
            this.Load += new System.EventHandler(this.ConfigEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxApiKey;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.LinkLabel linkLabelApiKey;
    }
}