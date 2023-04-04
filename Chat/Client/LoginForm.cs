namespace Client {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        // [로그인] 버튼 클릭
        private async void btnLogin_Click(object sender, EventArgs e) {
            // 텍스트박스 빈칸 확인
            if (string.IsNullOrEmpty(tbID.Text) || string.IsNullOrEmpty(tbNick.Text)) {
                MessageBox.Show("ID와 닉네임을 모두 입력하세요.", this.Text);
                return;
            }

            
        }
    }
}