using Core;
using System.Net.Sockets;

namespace Client;

public partial class LoginForm : Form {
    public LoginForm() {
        InitializeComponent();                                  // Form �ʱ�ȭ
        Singleton.Instance.LoginResponsed += LoginResponsed;    // �α��� ���� �̺�Ʈ �߰�
        FormClosing += (sender, e) => {
            Singleton.Instance.LoginResponsed -= LoginResponsed;    // �α��� ���� �̺�Ʈ ����
        };
    }

    // [�α���] ��ư Ŭ��
    private async void btnLogin_Click(object sender, EventArgs e) {
        // �ؽ�Ʈ�ڽ� ��ĭ Ȯ��
        if (string.IsNullOrEmpty(tbID.Text) || string.IsNullOrEmpty(tbNick.Text)) {
            MessageBox.Show("ID�� �г����� ��� �Է��ϼ���.", this.Text);
            return;
        }

        // �񵿱� ���(TAP)���� ������ ���� ��û
        await Singleton.Instance.ConnectAsync();

        // �α��� ��û ��Ŷ�� ������ ������
        LoginRequestPacket packet = new LoginRequestPacket(tbID.Text, tbNick.Text);         // ��Ŷ ����
        await Singleton.Instance.Socket.SendAsync(packet.Serialize(), SocketFlags.None);    // ��Ŷ ����ȭ �� ����
    }

    // �α��� ���� �̺�Ʈ
    private void LoginResponsed(object? sender, EventArgs e) {
        LoginResponsePacket packet = (LoginResponsePacket)sender!;

        // �α��� ���� ��Ŷ�� ������� ���
        if (packet.ResponseCode == 200) {
            MessageBox.Show("�α��� ���� (Code " + packet.ResponseCode.ToString() + ")", this.Text);

            // �̱��� ��ü�� ID�� �г��� �� ����
            Singleton.Instance.Id = tbID.Text;
            Singleton.Instance.Nickname = tbNick.Text;

            // ä�ù� ��� Form ����(�񵿱��)
            IAsyncResult ar = null;
            ar = BeginInvoke(() => {
                RoomListForm roomListForm = new RoomListForm();
                roomListForm.ShowDialog();
                EndInvoke(ar);
            });
        }
        
        // �α��� ���� ��Ŷ�� ������� ���
        else {
            MessageBox.Show("�α��� ���� (Code " + packet.ResponseCode.ToString() + ")", this.Text);
            Singleton.Instance.Socket.Shutdown(SocketShutdown.Send);    // Send ��Ʈ�� ���� ����(Receive�� ����)
        }
    }
}