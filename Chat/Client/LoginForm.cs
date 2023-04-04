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

        // �񵿱� ���(TAP)���� ������ ���� ��û
        await Singleton.Instance.ConnectAsync();

        // �α��� ��û ��Ŷ�� ������ ������
        LoginRequestPacket packet = new LoginRequestPacket(tbID.Text, tbNick.Text);         // ��Ŷ ����
        await Singleton.Instance.Socket.SendAsync(packet.Serialize(), SocketFlags.None);    // ��Ŷ ����ȭ �� ����

        // �̱��� ��ü�� ID�� �г��� �� ����
        Singleton.Instance.Id = tbID.Text;
        Singleton.Instance.Nickname = tbNick.Text;
    }
}