using System.Text;
using System.Net;

namespace Core;

// 인터페이션 구현(방 목록 요청 패킷)
public class DuplicatePacket : IPacket {
    // 직렬화 메소드(객체를 바이트 배열로 변환)
    public byte[] Serialize() {
        // 직렬화(패킷 타입(2바이트), 방 이름, 방 이름의 크기(2바이트)
        byte[] packetType = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)PacketType.Duplicate));

        // 첫 2바이트를 제외한 패킷의 전체 크기(2바이트)
        short dataSize = (short)(packetType.Length);
        byte[] header = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(dataSize));

        // 버퍼 생성
        byte[] buffer = new byte[2 + dataSize];

        // 바이트 배열의 정보를 버퍼에 복사
        int offset = 0;
        foreach (byte[] b in new byte[][] { header, packetType }) {
            Array.Copy(b, 0, buffer, offset, b.Length);
            offset += b.Length;
        }

        // 버퍼 반환
        return buffer;
    }
}
