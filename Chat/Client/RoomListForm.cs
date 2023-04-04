using Core;
using System.Net.Sockets;

namespace Client;

public partial class RoomListForm : Form {
    public RoomListForm() {
        InitializeComponent();

        // 방목록(폼)이 활성화 될 때마다 새로고침 해야함
        Activated += (sender, e) => {
            RoomlistBox.Items.Clear();
        };
    }

    // 채팅방 입장하기 버튼 클릭
    private void btnEnter_Click(object sender, EventArgs e) {
        
    }

    // 채팅방 만들기 버튼 클릭
    private void btnCreate_Click(object sender, EventArgs e) {

    }
}
