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

    // 이벤트 핸들러
    public event EventHandler<EventArgs>? LoginResponsed;       // 로그인 성공 시 실행할 이벤트 핸들러
    public event EventHandler<EventArgs>? CreateRoomResponsed;  // 방 생성 성공 시 실행할 이벤트 핸들러
    public event EventHandler<EventArgs>? RoomListResponsed;    // 방 목록 새로고침 시 실행할 이벤트 핸들러
    public event EventHandler<EventArgs>? EnterRoomResponsed;   // 방 입장 성공 시 실행할 이벤트 핸들러
    public event EventHandler<EventArgs>? UserEnterResponsed;   // 유저 입장 시 실행할 이벤트 핸들러
    public event EventHandler<EventArgs>? UserLeaveResponsed;   // 유저 퇴장 시 실행할 이벤트 핸들러
    public event EventHandler<EventArgs>? ChatResponsed;        // 채팅(메시지 보내기) 시 실행할 이벤트 핸들러

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

        // 5초(5000ms)마다 Heartbeat 패킷을 날림
        System.Timers.Timer timer = new System.Timers.Timer(5000);
        timer.Elapsed += async (sender, e) => {
            HeartbeatPacket packet = new HeartbeatPacket();                 // Heartbeat 패킷 생성
            await Socket.SendAsync(packet.Serialize(), SocketFlags.None);   // Heartbeat 패킷 직렬화 및 전송
        };

        try {
            while (true) {

                #region 헤더 수신하기

                int receivedHeaderSize = await socket.ReceiveAsync(headerBuffer, SocketFlags.None);

                // 수신할 헤더가 없으면 연결 해제
                if (receivedHeaderSize < 1) {
                    Console.WriteLine("클라이언트 연결 해제됨");
                    timer.Dispose();        // Heartbeat 타이머 소멸
                    socket.Shutdown(SocketShutdown.Both);     // 스트림 연결 종료(Send 및 Receive 불가)
                    socket.Dispose();                         // 소켓 자원 해제
                    return;
                }
                // 헤더를 1바이트만 수신할 경우 남은 1바이트 수신
                else if (receivedHeaderSize == 1) {
                    await socket.ReceiveAsync(new ArraySegment<byte>(headerBuffer, 1, 1), SocketFlags.None);
                }
                #endregion 헤더 수신하기 끝

                #region 데이터 수신하기
                int receivedDataSize = 0;  // 지금까지 수신한 데이터의 크기
                short totalDataSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(headerBuffer));  // 전체 데이터의 크기
                byte[] dataBuffer = new byte[totalDataSize];     // 데이터 버퍼

                // 데이터 버퍼 내의 모든 데이터 수신하기
                while (receivedDataSize < totalDataSize) {
                    int tmp = await socket.ReceiveAsync(new ArraySegment<byte>(dataBuffer, receivedDataSize,
                                                        totalDataSize - receivedDataSize), SocketFlags.None);
                    receivedDataSize += tmp;
                }
                #endregion 데이터 수신하기 끝

                // 패킷의 타입에 따른 동작
                PacketType packetType = (PacketType)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(dataBuffer));
                switch (packetType) {
                    // 로그인 응답 패킷
                    case PacketType.LoginResponse:
                        LoginResponsePacket packet1 = new LoginResponsePacket(dataBuffer);       // 로그인 응답 패킷 생성
                        LoginResponsed?.Invoke(packet1, EventArgs.Empty);                        // 로그인 응답 이벤트 호출
                        timer.Start();      // Heartbeat 타이머 시작
                        break;

                    // 방 생성 응답 패킷
                    case PacketType.CreateRoomResponse:
                        CreateRoomResponsePacket packet2 = new CreateRoomResponsePacket(dataBuffer);    // 방 생성 응답 패킷 생성
                        CreateRoomResponsed?.Invoke(packet2, EventArgs.Empty);                          // 방 생성 응답 이벤트 호출
                        break;

                    // 방 목록 응답 패킷
                    case PacketType.RoomListResponse:
                        RoomListResponsePacket packet3 = new RoomListResponsePacket(dataBuffer);        // 방 목록 응답 패킷 생성
                        RoomListResponsed?.Invoke(packet3, EventArgs.Empty);                            // 방 목록 응답 이벤트 호출
                        break;

                    // 방 입장 응답 패킷
                    case PacketType.EnterRoomResponse:
                        EnterRoomResponsePacket packet4 = new EnterRoomResponsePacket(dataBuffer);     // 방 입장 응답 패킷 생성
                        EnterRoomResponsed?.Invoke(packet4, EventArgs.Empty);                          // 방 입장 응답 이벤트 호출
                        break;

                    // 유저 입장 패킷
                    case PacketType.UserEnter:
                        UserEnterPacket packet5 = new UserEnterPacket(dataBuffer);
                        UserEnterResponsed?.Invoke(packet5, EventArgs.Empty);
                        break;

                    // 유저 퇴장 패킷
                    case PacketType.UserLeave:
                        UserLeavePacket packet6 = new UserLeavePacket(dataBuffer);
                        UserLeaveResponsed?.Invoke(packet6, EventArgs.Empty);
                        break;

                    // 채팅(메시지 보내기) 패킷
                    case PacketType.Chat:
                        ChatPacket packet7 = new ChatPacket(dataBuffer);
                        ChatResponsed?.Invoke(packet7, EventArgs.Empty);
                        break;

                    // 중복 접속 패킷
                    case PacketType.Duplicate:
                        socket.Shutdown(SocketShutdown.Send);
                        MessageBox.Show("다른 클라이언트에서 접속을 요청하여 현재 클라이언트를 종료합니다.", "Notice",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Environment.Exit(0);
                        break;
                }
            }
        }

        // 클라이언트 측에서 예외 발생 시, 메시지박스 출력 후 종료
        catch (Exception e) {
            MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(1);
        }
    }
}
