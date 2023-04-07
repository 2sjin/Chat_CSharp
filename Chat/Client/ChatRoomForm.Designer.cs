namespace Client {
    partial class ChatRoomForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tbMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.listBoxUsers = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbMessages = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbMessage
            // 
            this.tbMessage.Location = new System.Drawing.Point(23, 370);
            this.tbMessage.Name = "tbMessage";
            this.tbMessage.Size = new System.Drawing.Size(373, 27);
            this.tbMessage.TabIndex = 0;
            this.tbMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbMessage_KeyDown);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(402, 370);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(67, 27);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "전송";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // listBoxUsers
            // 
            this.listBoxUsers.FormattingEnabled = true;
            this.listBoxUsers.ItemHeight = 20;
            this.listBoxUsers.Location = new System.Drawing.Point(489, 50);
            this.listBoxUsers.Name = "listBoxUsers";
            this.listBoxUsers.Size = new System.Drawing.Size(149, 164);
            this.listBoxUsers.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(520, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "대화 참여자";
            // 
            // tbMessages
            // 
            this.tbMessages.BackColor = System.Drawing.SystemColors.Window;
            this.tbMessages.Location = new System.Drawing.Point(23, 27);
            this.tbMessages.Multiline = true;
            this.tbMessages.Name = "tbMessages";
            this.tbMessages.ReadOnly = true;
            this.tbMessages.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbMessages.Size = new System.Drawing.Size(446, 323);
            this.tbMessages.TabIndex = 5;
            // 
            // ChatRoomForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 425);
            this.Controls.Add(this.tbMessages);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBoxUsers);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.tbMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ChatRoomForm";
            this.Text = "ChatRoomForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox tbMessage;
        private Button btnSend;
        private ListBox listBoxUsers;
        private Label label1;
        private TextBox tbMessages;
    }
}