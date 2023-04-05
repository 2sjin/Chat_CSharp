using System.Net;
using System.Net.Sockets;
using Core;

namespace Client;

// 싱글톤(Singleton)
// 언제 어디에서나 접근할 수 있는 객체.
// 이 객체는 반드시 1개만 존재해야 한다.

internal class Singleton {
    static readonly IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("192.168.0.14"), 20000);

    // 프로퍼티
    public string Id { get; set; } = null!;
    public string Nickname { get; set; } = null!;
    public Socket Socket { get; } = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    public event EventHandler<EventArgs>? LoginResponsed;       // 로그인 성공 시 실행할 이벤트 핸들러
    public event EventHandler<EventArgs>? CreateRoomResponsed;  // 방 생성 성공 시 실행할 이벤트 핸들러
    public event EventHandler<EventArgs>? RoomListResponsed;    // 방 목록 새로고침 시 실행할 이벤트 핸들러
    public event EventHandler<EventArgs>? EnterRoomResponsed;   // 방 입장 성공 시 실행할 이벤트 핸들러

    // Singleton 객체
    private static Singleton? instance;
    public static Singleton Instance {
        get {
            if (instance == null)
                instance = new Singleton();
            return instance;
        }
    }

    // 비동기 방식(TAP)으로 서버에 연결을 요청하는 메소드
    public async Task ConnectAsync() {
        await Socket.ConnectAsync(endPoint);                    // 서버에 연결 요청
        ThreadPool.QueueUserWorkItem(ReceiveAsync, Socket);     // 스레드풀의 작업 큐에 메소드를 대기시킴
    }

    // 비동기 방식으로 패킷을 수신하는 메소드
    private async void ReceiveAsync(object? sender) {
        Socket socket = (Socket)sender!;

        // 헤더(패킷의 크기) 수신하기
        byte[] headerBuffer = new byte[2];  // 헤더 버퍼
        while (true) {
            // 헤더 수신
            int receivedHeaderSize = await socket.ReceiveAsync(headerBuffer, SocketFlags.None);
            // 수신할 헤더가 없으면 연결 해제
            if (receivedHeaderSize < 1) {
                Console.WriteLine("클라이언트 연결 해제됨");
                socket.Shutdown(SocketShutdown.Both);     // 스트림 연결 종료(Send 및 Receive 불가)
                socket.Dispose();                         // 소켓 자원 해제
                return;
            }
            // 헤더를 1바이트만 수신할 경우 남은 1바이트 수신
            else if (receivedHeaderSize == 1) {
                await socket.ReceiveAsync(new ArraySegment<byte>(headerBuffer, 1, 1), SocketFlags.None);
            }

            // 데이터 수신하기
            int receivedDataSize = 0;  // 지금까지 수신한 데이터의 크기
            short totalDataSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(headerBuffer));  // 전체 데이터의 크기
            byte[] dataBuffer = new byte[totalDataSize];     // 데이터 버퍼

            // 데이터 버퍼 내의 모든 데이터 수신하기
            while (receivedDataSize < totalDataSize) {
                int tmp = await socket.ReceiveAsync(new ArraySegment<byte>(dataBuffer, receivedDataSize,
                                                    totalDataSize - receivedDataSize), SocketFlags.None);
                receivedDataSize += tmp;
            }

            // 패킷의 타입에 따른 동작
            PacketType packetType = (PacketType)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(dataBuffer));
            switch (packetType) {
                case PacketType.LoginResponse:          // 로그인 응답 패킷
                    LoginResponsePacket packet1 = new LoginResponsePacket(dataBuffer);       // 로그인 응답 패킷 생성
                    LoginResponsed?.Invoke(packet1, EventArgs.Empty);                        // 로그인 응답 이벤트 호출
                    break;

                case PacketType.CreateRoomResponse:     // 방 생성 응답 패킷
                    CreateRoomResponsePacket packet2 = new CreateRoomResponsePacket(dataBuffer);    // 방 생성 응답 패킷 생성
                    CreateRoomResponsed?.Invoke(packet2, EventArgs.Empty);                          // 방 생성 응답 이벤트 호출
                    break;

                case PacketType.RoomListResponse:       // 방 목록 응답 패킷
                    RoomListResponsePacket packet3 = new RoomListResponsePacket(dataBuffer);        // 방 목록 응답 패킷 생성
                    RoomListResponsed?.Invoke(packet3, EventArgs.Empty);                            // 방 목록 응답 이벤트 호출
                    break;

                case PacketType.EnterRoomResponse:
                    EnterRoomResponsePacket packet4 = new EnterRoomResponsePacket(dataBuffer);     // 방 입장 응답 패킷 생성
                    EnterRoomResponsed?.Invoke(packet4, EventArgs.Empty);                          // 방 입장 응답 이벤트 호출
                    break;
            }
        }
    }
}
