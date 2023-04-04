using System.Text;
using System.Net;

namespace Core;

// 인터페이션 구현(로그인 패킷)
internal class LoginRequestPacket : IPacket {
    // 프로퍼티
    public string Id { get; private set; }
    public string Nickname { get; private set; }

    // 생성자
    public LoginRequestPacket(string id, string nickname) {
        Id = id;
        Nickname = nickname;
    }

    // 직렬화 메소드(객체를 바이트 배열로 변환)
    public byte[] Serialize() {
        // 패킷 타입(2바이트)
        byte[] packetType = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)PacketType.LoginRequest));

        // ID 및 ID의 크기(2바이트)
        byte[] id = Encoding.UTF8.GetBytes(Id);
        byte[] idSize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)id.Length));

        // 닉네임 및 닉네임의 크기(2바이트)
        byte[] nickname = Encoding.UTF8.GetBytes(Nickname);
        byte[] nicknameSize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)nickname.Length));

        // 첫 2바이트를 제외한 패킷의 전체 크기(2바이트)
        short dataSize = (short)(packetType.Length + id.Length + idSize.Length + nickname.Length + nicknameSize.Length);
        byte[] header = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(dataSize));

        // 버퍼 생성
        byte[] buffer = new byte[2 + dataSize];

        // 바이트 배열의 정보를 버퍼에 복사
        int offset = 0;
        foreach (byte[] b in new byte[][] { header, packetType, idSize, id, nicknameSize, nickname }) {
            Array.Copy(header, 0, buffer, offset, b.Length);
            offset += b.Length;
        }

        return buffer;
    }
}
