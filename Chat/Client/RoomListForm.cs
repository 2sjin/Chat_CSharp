using Core;
using System.Net.Sockets;

namespace Client;

public partial class RoomListForm : Form {
    public RoomListForm() {
        InitializeComponent();
        Singleton.Instance.CreateRoomResponsed += CreateRoomResponsed;  // 방 생성 응답 이벤트 추가
        FormClosing += (sender, e) => {
            Singleton.Instance.LoginResponsed -= CreateRoomResponsed;   // 방 생성 응답 이벤트 제거
        };

        // 방목록(폼)이 활성화 될 때마다 새로고침 해야함
        Activated += (sender, e) => {
            listBoxRooms.Items.Clear();
        };
    }

    // 채팅방 만들기 버튼 클릭
    private async void btnCreate_Click(object sender, EventArgs e) {
        // 텍스트박스 빈칸 확인
        if (string.IsNullOrEmpty(tbRoomName.Text)) {
            MessageBox.Show("방 이름을 입력하세요.", this.Text);
            return;
        }

        CreateRoomRequestPacket packet = new CreateRoomRequestPacket(tbRoomName.Text);
        await Singleton.Instance.Socket.SendAsync(packet.Serialize(), SocketFlags.None);
    }

    // 채팅방 입장하기 버튼 클릭
    private async void btnEnter_Click(object sender, EventArgs e) {
        // 목록에서 선택된 채팅방이 없으면 리턴
        if (listBoxRooms.SelectedItem == null)
            return;

        // 채팅방 입장
        ChatRoomForm chatRoom = new ChatRoomForm();
        chatRoom.Text = listBoxRooms.SelectedItem.ToString();
        chatRoom.ShowDialog();
    }

    // 방 생성 응답 이벤트
    private void CreateRoomResponsed(object? sender, EventArgs e) {
        CreateRoomResponsePacket packet = (CreateRoomResponsePacket)sender!;

        // 로그인 성공 패킷을 응답받은 경우
        if (packet.ResponseCode == 200) {
            string roomName = tbRoomName.Text;

            // 리스트박스에 방 추가
            listBoxRooms.Items.Add(roomName);
            tbRoomName.Text = null;

            // 채팅방 입장
            ChatRoomForm chatRoom = new ChatRoomForm();
            chatRoom.Text = roomName;
            chatRoom.ShowDialog();
        }

        // 로그인 실패 패킷을 응답받은 경우
        else {
            MessageBox.Show("로그인 실패 (Code " + packet.ResponseCode.ToString() + ")", this.Text);
            Singleton.Instance.Socket.Shutdown(SocketShutdown.Send);    // Send 스트림 연결 종료(Receive는 가능)
        }
    }
}
