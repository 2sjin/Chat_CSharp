using Core;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace Server;

internal class Server {
    // 서버 소켓(IPv4, 연결지향, TCP)
    private Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    // 여러 스레드에서 접근 가능한 딕셔너리
    // Key는 방 이름, Value는 방 객체
    public ConcurrentDictionary<string, Room> RoomsDict { get; } = new ConcurrentDictionary<string, Room>();

    // 생성자: 서버 소켓 생성
    public Server(string ip, int port, int backlog) {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        serverSocket.Bind(endPoint);    // 서버 소켓에 IP와 Port 할당
        serverSocket.Listen(backlog);   // 백로그 큐 생성
    }

    // 비동기 방식(TAP)으로 클라이언트의 연결을 수락하는 메소드
    public async Task StartAsync() {
        while (true) {
            Socket clientSocket = await serverSocket.AcceptAsync();     // 클라이언트의 요청 수락
            Console.WriteLine(clientSocket.RemoteEndPoint);
            ThreadPool.QueueUserWorkItem(ReceiveAsync, clientSocket);   // 스레드풀의 작업 큐에 메소드를 대기시킴
        }
    }

    // 비동기 방식으로 패킷을 수신하는 메소드
    private async void ReceiveAsync(object? sender) {
        Socket clientSocket = (Socket)sender!;

        string id = "";
        string nickname = "";
        string roomName = "";

        while (true) {
            // 헤더(패킷의 크기) 수신하기
            byte[] headerBuffer = new byte[2];  // 헤더 버퍼
            while (true) {
                // 헤더 수신
                int receivedHeaderSize = await clientSocket.ReceiveAsync(headerBuffer, SocketFlags.None);
                // 수신할 헤더가 없으면 연결 해제
                if (receivedHeaderSize < 1) {
                    Console.WriteLine("클라이언트 연결 해제됨");
                    clientSocket.Shutdown(SocketShutdown.Both);     // 스트림 연결 종료(Send 및 Receive 불가)
                    clientSocket.Dispose();                         // 소켓 자원 해제
                    return;
                }
                // 헤더를 1바이트만 수신할 경우 남은 1바이트 수신
                else if (receivedHeaderSize == 1) {
                    await clientSocket.ReceiveAsync(new ArraySegment<byte>(headerBuffer, 1, 1), SocketFlags.None);
                }

                // 데이터 수신하기
                int receivedDataSize = 0;  // 지금까지 수신한 데이터의 크기
                short totalDataSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(headerBuffer));  // 전체 데이터의 크기
                byte[] dataBuffer = new byte[totalDataSize];     // 데이터 버퍼

                // 데이터 버퍼 내의 모든 데이터 수신하기
                while (receivedDataSize < totalDataSize) {
                    int tmp = await clientSocket.ReceiveAsync(new ArraySegment<byte>(dataBuffer, receivedDataSize,
                                                                totalDataSize - receivedDataSize), SocketFlags.None);
                    receivedDataSize += tmp;
                }

                // 패킷의 타입에 따른 동작
                PacketType packetType = (PacketType)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(dataBuffer));
                switch (packetType) {
                    case PacketType.LoginRequest:   // 로그인 요청 패킷
                        LoginRequestPacket packet1 = new LoginRequestPacket(dataBuffer);        // 로그인 요청 패킷 생성(재구성)
                        Console.WriteLine($"id:{packet1.Id} nickname:{packet1.Nickname}");
                        id = packet1.Id;
                        nickname = packet1.Nickname;

                        LoginResponsePacket packet2 = new LoginResponsePacket(200);             // 로그인 응답 패킷 생성
                        await clientSocket.SendAsync(packet2.Serialize(), SocketFlags.None);    // 클라이언트에 응답 패킷 전송
                        break;

                    case PacketType.CreateRoomRequest:      // 방 생성 요청 패킷
                        CreateRoomRequestPacket packet3 = new CreateRoomRequestPacket(dataBuffer);   // 방 생성 요청 패킷 생성
                        Room room = new Room();     // 방 객체 생성

                        // 딕셔너리에 방 저장
                        if (RoomsDict.TryAdd(packet3.RoomName, room)) {
                            roomName = packet3.RoomName;
                            room.UsersDict.TryAdd(id, nickname);    // 방에 입장한 유저 정보 저장
                            Console.WriteLine("created room: " + roomName);
                            CreateRoomResponsePacket packet4 = new CreateRoomResponsePacket(200);   // 방 생성 응답 패킷 생성
                            await clientSocket.SendAsync(packet4.Serialize(), SocketFlags.None);    // 클라이언트에 응답 패킷 전송
                        }

                        else {
                            Console.WriteLine("created failed");
                            CreateRoomResponsePacket packet4 = new CreateRoomResponsePacket(500);   // 방 생성 응답 패킷 생성
                            await clientSocket.SendAsync(packet4.Serialize(), SocketFlags.None);    // 클라이언트에 응답 패킷 전송
                        }
                        break;

                    case PacketType.RoomListRequest:       // 방 목록 요청 패킷
                        // 방 목록 딕셔너리에서 방 이름들(Keys)만 전달
                        RoomListResponsePacket packet5 = new RoomListResponsePacket(RoomsDict.Keys);   // 방 목록 응답 패킷 생성
                        await clientSocket.SendAsync(packet5.Serialize(), SocketFlags.None);            // 클라이언트에 응답 패킷 전송
                        break;

                    case PacketType.EnterRoomRequest:       // 방 입장 요청 패킷
                        EnterRoomRequestPacket packet6 = new EnterRoomRequestPacket(dataBuffer);   // 방 입장 요청 패킷 생성
                        if (RoomsDict.TryGetValue(packet6.RoomName, out var room2)) {
                            roomName = packet6.RoomName;
                            room2.UsersDict.TryAdd(id, nickname);
                            Console.WriteLine($"{roomName} : {nickname} 입장 성공");
                            EnterRoomResponsePacket packet7 = new EnterRoomResponsePacket(200);     // 방 입장 응답 패킷 생성
                            await clientSocket.SendAsync(packet6.Serialize(), SocketFlags.None);    // 클라이언트에 응답 패킷 전송
                        }
                        break;
                }
            }
        }
    }
}
