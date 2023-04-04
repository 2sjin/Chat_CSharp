using Core;
using System.Net.Sockets;

namespace Client;

public partial class LoginForm : Form {
    public LoginForm() {
        InitializeComponent();                                  // Form 초기화
        Singleton.Instance.LoginResponsed += LoginResponsed;    // 로그인 응답 이벤트 추가
        FormClosing += (sender, e) => {
            Singleton.Instance.LoginResponsed -= LoginResponsed;    // 로그인 응답 이벤트 제거
        };
    }

    // [로그인] 버튼 클릭
    private async void btnLogin_Click(object sender, EventArgs e) {
        // 텍스트박스 빈칸 확인
        if (string.IsNullOrEmpty(tbID.Text) || string.IsNullOrEmpty(tbNick.Text)) {
            MessageBox.Show("ID와 닉네임을 모두 입력하세요.", this.Text);
            return;
        }

        // 비동기 방식(TAP)으로 서버에 연결 요청
        await Singleton.Instance.ConnectAsync();

        // 로그인 요청 패킷을 서버에 전송함
        LoginRequestPacket packet = new LoginRequestPacket(tbID.Text, tbNick.Text);         // 패킷 생성
        await Singleton.Instance.Socket.SendAsync(packet.Serialize(), SocketFlags.None);    // 패킷 직렬화 및 전송
    }

    // 로그인 응답 이벤트
    private void LoginResponsed(object? sender, EventArgs e) {
        LoginResponsePacket packet = (LoginResponsePacket)sender!;

        // 로그인 성공 패킷을 응답받은 경우
        if (packet.ResponseCode == 200) {
            MessageBox.Show("로그인 성공 (Code " + packet.ResponseCode.ToString() + ")", this.Text);

            // 싱글톤 개체의 ID와 닉네임 값 변경
            Singleton.Instance.Id = tbID.Text;
            Singleton.Instance.Nickname = tbNick.Text;

            // 채팅방 목록 Form 생성(비동기식)
            IAsyncResult ar = null;
            ar = BeginInvoke(() => {
                RoomListForm roomListForm = new RoomListForm();
                roomListForm.ShowDialog();
                EndInvoke(ar);
            });
        }
        
        // 로그인 실패 패킷을 응답받은 경우
        else {
            MessageBox.Show("로그인 실패 (Code " + packet.ResponseCode.ToString() + ")", this.Text);
            Singleton.Instance.Socket.Shutdown(SocketShutdown.Send);    // Send 스트림 연결 종료(Receive는 가능)
        }
    }
}