namespace Client {
    partial class RoomListForm {
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
            this.RoomlistBox = new System.Windows.Forms.ListBox();
            this.btnEnter = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // RoomlistBox
            // 
            this.RoomlistBox.FormattingEnabled = true;
            this.RoomlistBox.ItemHeight = 20;
            this.RoomlistBox.Location = new System.Drawing.Point(28, 31);
            this.RoomlistBox.Name = "RoomlistBox";
            this.RoomlistBox.Size = new System.Drawing.Size(177, 324);
            this.RoomlistBox.TabIndex = 0;
            // 
            // btnEnter
            // 
            this.btnEnter.Location = new System.Drawing.Point(28, 372);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(177, 39);
            this.btnEnter.TabIndex = 1;
            this.btnEnter.Text = "입장하기";
            this.btnEnter.UseVisualStyleBackColor = true;
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(235, 76);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(257, 42);
            this.btnCreate.TabIndex = 2;
            this.btnCreate.Text = "채팅방 만들기";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(235, 31);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(257, 27);
            this.textBox1.TabIndex = 3;
            // 
            // RoomListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 450);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnEnter);
            this.Controls.Add(this.RoomlistBox);
            this.Name = "RoomListForm";
            this.Text = "RoomListForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListBox RoomlistBox;
        private Button btnEnter;
        private Button btnCreate;
        private TextBox textBox1;
    }
}