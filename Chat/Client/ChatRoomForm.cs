using Core;
using System.Net.Sockets;

namespace Client {
    public partial class ChatRoomForm : Form {
        public ChatRoomForm() {
            InitializeComponent();

            listBox2.Items.Add(Singleton.Instance.Nickname);

            // 이벤트 핸들러에 메소드 추가
            Singleton.Instance.UserEnterResponsed += UserEnterResponsed;  // 유저 입장 메소드 추가

            // Form 종료 시, 이벤트 핸들러에 메소드 제거
            FormClosing += (sender, e) => {
                Singleton.Instance.UserEnterResponsed -= UserEnterResponsed;      // 유저 입장 메소드 제거
                Singleton.Instance.Socket.Shutdown(SocketShutdown.Send);          // Send 스트림 연결 종료(Receive는 가능)
            };
        }

        // 유저 입장 이벤트
        private void UserEnterResponsed(object? sender, EventArgs e) {
            UserEnterPacket packet = (UserEnterPacket)sender!;
            listBox2.Items.Add(packet.Nickname);
        }
    }
}
