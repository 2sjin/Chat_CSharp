using System.Text;
using System.Net;

namespace Core;

// 인터페이션 구현(로그인 요청 패킷)
public class ChatPacket : IPacket {
    // 프로퍼티
    public string Nickname { get; private set; }
    public string Message { get; private set; }

    // 생성자
    public ChatPacket(string nickname, string message) {
        Nickname = nickname;
        Message = message;
    }

    // 생성자(바이트 배열을 역직렬화하여 메시지와 닉네임 저장)
    public ChatPacket(byte[] buffer) {
        int offset = 2;

        short nicknameSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, offset));
        offset += sizeof(short);
        Nickname = Encoding.UTF8.GetString(buffer, offset, nicknameSize);
        offset += nicknameSize;

        short messageSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, offset));
        offset += sizeof(short);
        Message = Encoding.UTF8.GetString(buffer, offset, messageSize);

    }

    // 직렬화 메소드(객체를 바이트 배열로 변환)
    public byte[] Serialize() {
        // 직렬화(패킷 타입(2바이트), 닉네임, 닉네임 크기(2바이트), 메시지, 메시지 크기(2바이트))
        byte[] packetType = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)PacketType.Chat));
        byte[] nickname = Encoding.UTF8.GetBytes(Nickname);
        byte[] nicknameSize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)nickname.Length));
        byte[] message = Encoding.UTF8.GetBytes(Message);
        byte[] messageSize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)message.Length));


        // 첫 2바이트를 제외한 패킷의 전체 크기(2바이트)
        short dataSize = (short)(packetType.Length + nickname.Length + nicknameSize.Length + message.Length + messageSize.Length);
        byte[] header = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(dataSize));

        // 버퍼 생성
        byte[] buffer = new byte[2 + dataSize];

        // 바이트 배열의 정보를 버퍼에 복사
        int offset = 0;
        foreach (byte[] b in new byte[][] { header, packetType, nicknameSize, nickname, messageSize, message }) {
            Array.Copy(b, 0, buffer, offset, b.Length);
            offset += b.Length;
        }

        // 버퍼 반환
        return buffer;
    }
}
