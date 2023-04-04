using System.Text;
using System.Net;

namespace Core;

public class CreateRoomResponsePacket : IPacket {
    // 응답코드
    public int ResponseCode { get; private set; }

    // 생성자
    public CreateRoomResponsePacket(int responseCode) {
        ResponseCode = responseCode;
    }

    // 생성자(바이트 배열을 역직렬화하여 응답코드 저장)
    public CreateRoomResponsePacket(byte[] buffer) {
        ResponseCode = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, 2));
    }

    // 직렬화 메소드(객체를 바이트 배열로 변환)
    public byte[] Serialize() {
        // 직렬화(패킷 타입(2바이트), 응답코드)
        byte[] packetType = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)PacketType.CreateRoomResponse));
        byte[] responseCode = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(ResponseCode));

        // 첫 2바이트를 제외한 패킷의 전체 크기(2바이트)
        short dataSize = (short)(packetType.Length + responseCode.Length);
        byte[] header = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(dataSize));

        // 버퍼 생성
        byte[] buffer = new byte[2 + dataSize];

        // 바이트 배열의 정보를 버퍼에 복사
        int offset = 0;
        foreach (byte[] b in new byte[][] { header, packetType, responseCode }) {
            Array.Copy(b, 0, buffer, offset, b.Length);
            offset += b.Length;
        }

        // 버퍼 반환
        return buffer;
    }
}
