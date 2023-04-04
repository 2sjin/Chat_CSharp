using Core;
using System.Net.Sockets;

namespace Client;

public partial class RoomListForm : Form {
    public RoomListForm() {
        InitializeComponent();

        // 방목록(폼)이 활성화 될 때마다 새로고침 해야함
        Activated += (sender, e) => {
            listBoxRooms.Items.Clear();
        };
    }

    // 채팅방 만들기 버튼 클릭
    private void btnCreate_Click(object sender, EventArgs e) {
        string roomName = tbRoomName.Text;

        // 텍스트박스 빈칸 확인
        if (string.IsNullOrEmpty(roomName)) {
            MessageBox.Show("방 이름을 입력하세요.", this.Text);
            return;
        }

        // 리스트박스에 방 추가
        listBoxRooms.Items.Add(roomName);
        tbRoomName.Text = null;

        // 채팅방 입장
        ChatRoomForm chatRoom = new ChatRoomForm();
        chatRoom.Text = roomName;
        chatRoom.ShowDialog();
    }

    // 채팅방 입장하기 버튼 클릭
    private void btnEnter_Click(object sender, EventArgs e) {
        // 목록에서 선택된 채팅방이 없으면 리턴
        if (listBoxRooms.SelectedItem == null)
            return;

        // 채팅방 입장
        ChatRoomForm chatRoom = new ChatRoomForm();
        chatRoom.Text = listBoxRooms.SelectedItem.ToString();
        chatRoom.ShowDialog();
    }
}
