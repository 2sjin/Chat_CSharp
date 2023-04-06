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
            this.listBoxRooms = new System.Windows.Forms.ListBox();
            this.btnEnter = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.tbRoomName = new System.Windows.Forms.TextBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.groupBoxCreateRoom = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxUserInfo = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblID = new System.Windows.Forms.Label();
            this.lblNickname = new System.Windows.Forms.Label();
            this.groupBoxCreateRoom.SuspendLayout();
            this.groupBoxUserInfo.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxRooms
            // 
            this.listBoxRooms.FormattingEnabled = true;
            this.listBoxRooms.ItemHeight = 20;
            this.listBoxRooms.Location = new System.Drawing.Point(28, 31);
            this.listBoxRooms.Name = "listBoxRooms";
            this.listBoxRooms.Size = new System.Drawing.Size(177, 404);
            this.listBoxRooms.TabIndex = 0;
            // 
            // btnEnter
            // 
            this.btnEnter.Location = new System.Drawing.Point(232, 31);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(125, 61);
            this.btnEnter.TabIndex = 1;
            this.btnEnter.Text = "입장하기";
            this.btnEnter.UseVisualStyleBackColor = true;
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(79, 112);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(117, 42);
            this.btnCreate.TabIndex = 4;
            this.btnCreate.Text = "방 만들기";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // tbRoomName
            // 
            this.tbRoomName.Location = new System.Drawing.Point(20, 68);
            this.tbRoomName.Name = "tbRoomName";
            this.tbRoomName.Size = new System.Drawing.Size(234, 27);
            this.tbRoomName.TabIndex = 3;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(380, 31);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(125, 61);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "새로고침";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // groupBoxCreateRoom
            // 
            this.groupBoxCreateRoom.Controls.Add(this.label1);
            this.groupBoxCreateRoom.Controls.Add(this.btnCreate);
            this.groupBoxCreateRoom.Controls.Add(this.tbRoomName);
            this.groupBoxCreateRoom.Location = new System.Drawing.Point(232, 260);
            this.groupBoxCreateRoom.Name = "groupBoxCreateRoom";
            this.groupBoxCreateRoom.Size = new System.Drawing.Size(273, 175);
            this.groupBoxCreateRoom.TabIndex = 5;
            this.groupBoxCreateRoom.TabStop = false;
            this.groupBoxCreateRoom.Text = "새 채팅방 만들기";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "채팅방 이름";
            // 
            // groupBoxUserInfo
            // 
            this.groupBoxUserInfo.Controls.Add(this.tableLayoutPanel1);
            this.groupBoxUserInfo.Location = new System.Drawing.Point(232, 126);
            this.groupBoxUserInfo.Name = "groupBoxUserInfo";
            this.groupBoxUserInfo.Size = new System.Drawing.Size(273, 99);
            this.groupBoxUserInfo.TabIndex = 6;
            this.groupBoxUserInfo.TabStop = false;
            this.groupBoxUserInfo.Text = "로그인 정보";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.46442F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 71.53558F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblID, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblNickname, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 23);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(267, 73);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "ID:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 20);
            this.label3.TabIndex = 1;
            this.label3.Text = "닉네임:";
            // 
            // lblID
            // 
            this.lblID.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblID.AutoSize = true;
            this.lblID.Location = new System.Drawing.Point(79, 8);
            this.lblID.Name = "lblID";
            this.lblID.Size = new System.Drawing.Size(89, 20);
            this.lblID.TabIndex = 2;
            this.lblID.Text = "sample1234";
            // 
            // lblNickname
            // 
            this.lblNickname.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblNickname.AutoSize = true;
            this.lblNickname.Location = new System.Drawing.Point(79, 44);
            this.lblNickname.Name = "lblNickname";
            this.lblNickname.Size = new System.Drawing.Size(84, 20);
            this.lblNickname.TabIndex = 3;
            this.lblNickname.Text = "샘플닉네임";
            // 
            // RoomListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 473);
            this.Controls.Add(this.groupBoxUserInfo);
            this.Controls.Add(this.groupBoxCreateRoom);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnEnter);
            this.Controls.Add(this.listBoxRooms);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "RoomListForm";
            this.Text = "RoomListForm";
            this.groupBoxCreateRoom.ResumeLayout(false);
            this.groupBoxCreateRoom.PerformLayout();
            this.groupBoxUserInfo.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ListBox listBoxRooms;
        private Button btnEnter;
        private Button btnCreate;
        private TextBox tbRoomName;
        private Button btnRefresh;
        private GroupBox groupBoxCreateRoom;
        private Label label1;
        private GroupBox groupBoxUserInfo;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label2;
        private Label label3;
        private Label lblID;
        private Label lblNickname;
    }
}