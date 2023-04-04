using Core;
using System.Net.Sockets;

namespace Client;

public partial class Form1 : Form {
    public Form1() {
        InitializeComponent();
    }

    // [�α���] ��ư Ŭ��
    private async void btnLogin_Click(object sender, EventArgs e) {
        // �ؽ�Ʈ�ڽ� ��ĭ Ȯ��
        if (string.IsNullOrEmpty(tbID.Text) || string.IsNullOrEmpty(tbNick.Text)) {
            MessageBox.Show("ID�� �г����� ��� �Է��ϼ���.", this.Text);
            return;
        }

        // �α���
        await Singleton.Instance.ConnectAsync();
        LoginRequestPacket packet = new LoginRequestPacket(tbID.Text, tbNick.Text);
        await Singleton.Instance.Socket.SendAsync(packet.Serialize(), SocketFlags.None);

        Singleton.Instance.Id = tbID.Text;
        Singleton.Instance.Nickname = tbNick.Text;
    }
}