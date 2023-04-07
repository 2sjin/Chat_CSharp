using Core;
using System.Net.Sockets;

namespace Client {
    public partial class ChatRoomForm : Form {
        public ChatRoomForm() {
            InitializeComponent();

            listBoxUsers.Items.Add(Singleton.Instance.Nickname);

            // 이벤트 핸들러에 메소드 추가
            Singleton.Instance.UserEnterResponsed += UserEnterResponsed;  // 유저 입장 메소드 추가
            Singleton.Instance.UserLeaveResponsed += UserLeaveResponsed;  // 유저 퇴장 메소드 추가
            Singleton.Instance.ChatResponsed += ChatResponsed;  // 채팅(메시지 보내기) 메소드 추가

            // Form 종료 시, 이벤트 핸들러에 메소드 제거
            FormClosing += async (sender, e) => {
                Singleton.Instance.UserEnterResponsed -= UserEnterResponsed;      // 유저 입장 메소드 제거
                Singleton.Instance.UserLeaveResponsed -= UserLeaveResponsed;      // 유저 퇴장 메소드 제거
                Singleton.Instance.ChatResponsed -= ChatResponsed;  // 채팅(메시지 보내기) 메소드 추가

                // 유저 퇴장 패킷을 서버에 전송함
                UserLeavePacket packet = new UserLeavePacket(Singleton.Instance.Nickname);          // 패킷 생성
                await Singleton.Instance.Socket.SendAsync(packet.Serialize(), SocketFlags.None);    // 패킷 직렬화 및 전송
            };
        }

        // 메시지 전송 버튼 클릭
        private async void btnSend_Click(object sender, EventArgs e) {
            // 텍스트박스에 메시지를 입력하지 않았으면 전송하지 않음
            if (string.IsNullOrEmpty(tbMessage.Text))
                return;

            // 채팅(메시지 보내기) 패킷을 서버에 전송함
            ChatPacket packet = new ChatPacket(Singleton.Instance.Nickname, tbMessage.Text);    // 패킷 생성
            await Singleton.Instance.Socket.SendAsync(packet.Serialize(), SocketFlags.None);    // 패킷 직렬화 및 전송
            tbMessage.Text = null;
        }

        // 유저 입장 이벤트
        private void UserEnterResponsed(object? sender, EventArgs e) {
            UserEnterPacket packet = (UserEnterPacket)sender!;

            // 리스트박스에 입장한 유저 추가(비동기식)
            IAsyncResult? ar = null;
            ar = BeginInvoke(() => {
                listBoxUsers.Items.Add(packet.Nickname);
                EndInvoke(ar);
            });
        }

        // 유저 퇴장 이벤트
        private void UserLeaveResponsed(object? sender, EventArgs e) {
            UserLeavePacket packet = (UserLeavePacket)sender!;

            // 리스트박스에 퇴장한 유저 삭제 새로고침(비동기식)
            IAsyncResult? ar = null;
            ar = BeginInvoke(() => {
                listBoxUsers.Items.Remove(packet.Nickname);
                EndInvoke(ar);
            });
        }

        // 채팅(메시지 보내기) 이벤트
        private void ChatResponsed(object? sender, EventArgs e) {
            ChatPacket packet = (ChatPacket)sender!;

            // 채팅 메시지 출력(비동기식)
            IAsyncResult? ar = null;
            ar = BeginInvoke(() => {
                tbMessages.Text += ($"{packet.Nickname}: {packet.Message}\r\n");
                ScrollToDown();
            });
        }

        // 맨 아래로 스크롤 내리기 메소드
        private void ScrollToDown() {

        }
    }
}
