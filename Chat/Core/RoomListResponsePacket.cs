using System.Text;
using System.Net;

namespace Core;

// 인터페이션 구현(방 목록 응답 패킷)
public class RoomListResponsePacket : IPacket {
    // 방 이름 리스트
    public List<string> RoomNames { get; }

    // 생성자
    public RoomListResponsePacket(ICollection<string> roomNames) {
        RoomNames = new List<string>(roomNames);
    }

    // 생성자(바이트 배열을 역직렬화하여 리스트 저장)
    public RoomListResponsePacket(byte[] buffer) {
        RoomNames = new List<string>();

        for (int i = 2; i < buffer.Length;) {
            short roomNameSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, i));
            i += sizeof(short);

            RoomNames.Add(Encoding.UTF8.GetString(buffer, i, roomNameSize));
            i += roomNameSize;
        }
    }

    // 직렬화 메소드(객체를 바이트 배열로 변환)
    public byte[] Serialize() {
        // 직렬화(패킷 타입(2바이트), 응답코드)
        byte[] packetType = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)PacketType.RoomListResponse));

        // 첫 2바이트를 제외한 패킷의 전체 크기(2바이트)
        short dataSize = (short)(packetType.Length);

        // 임시 변수
        List<byte[]> temp = new List<byte[]>();

        // 
        foreach(var item in RoomNames) {
            byte[] nameBuffer = Encoding.UTF8.GetBytes(item);
            byte[] nameSize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)nameBuffer.Length));
            dataSize += (short)(nameBuffer.Length + nameSize.Length);
            temp.Add(nameSize);
            temp.Add(nameBuffer);
        }

        // 첫 2바이트를 제외한 패킷의 전체 크기(2바이트)
        byte[] header = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(dataSize));

        // 버퍼 생성
        byte[] buffer = new byte[2 + dataSize];

        // 바이트 배열의 정보를 버퍼에 복사
        int offset = 0;
        foreach (byte[] b in new byte[][] { header, packetType }) {
            Array.Copy(b, 0, buffer, offset, b.Length);
            offset += b.Length;
        }
        foreach (var item in temp) {
            Array.Copy(item, 0, buffer, offset, item.Length);
            offset += item.Length;
        }

        // 버퍼 반환
        return buffer;
    }
}
