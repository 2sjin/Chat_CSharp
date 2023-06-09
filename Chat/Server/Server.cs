﻿using Core;
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

    // Key는 유저 ID, Value는 소켓
    public ConcurrentDictionary<string, Socket> ClientsDict { get; } = new ConcurrentDictionary<string, Socket>();

    // 생성자: 서버 소켓 생성
    public Server(string ip, int port, int backlog) {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        serverSocket.Bind(endPoint);    // 서버 소켓에 IP와 Port 할당
        serverSocket.Listen(backlog);   // 백로그 큐 생성
    }

    // 비동기 방식(TAP)으로 클라이언트의 연결을 수락하는 메소드
    public async Task StartAsync() {
        while (true) {
            try {
                Socket clientSocket = await serverSocket.AcceptAsync();     // 클라이언트의 요청 수락
                PrintLog($"[{clientSocket.RemoteEndPoint}] Connected.");
                ThreadPool.QueueUserWorkItem(ReceiveAsync, clientSocket);   // 스레드풀의 작업 큐에 메소드를 대기시킴
            }
            catch (SocketException e) {
                Console.WriteLine(e);
            }
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

            try {
                while (true) {

                    #region 헤더 수신하기
                    
                    // 2개의 Task를 동시에 시작
                    var t1 = clientSocket.ReceiveAsync(headerBuffer, SocketFlags.None);     // 헤더 수신하기
                    var t2 = Task.Delay(1000 * 30);     // 30초 대기(비동기)

                    // 2개의 Task 중 먼저 끝난 Task를 반환함
                    Task taskDoneEarlier = await Task.WhenAny(t1, t2);

                    // 헤더를 수신하지 못하고 30초가 경과하면 연결 해제
                    if (taskDoneEarlier == t2) {
                        PrintLog($"[{clientSocket.RemoteEndPoint}] Disconnected.");
                        PrintLog($"[ID: {id}] [Nickname: {nickname}] Logged Out.");
                        await RemoveClientSocket(id, nickname, roomName, clientSocket);
                        return;
                    }

                    // 수신한 헤더의 크기 저장
                    int receivedHeaderSize = await t1;

                    // 더 이상 수신할 헤더가 없으면 연결 해제
                    if (receivedHeaderSize < 1) {
                        PrintLog($"[{clientSocket.RemoteEndPoint}] Disconnected.");
                        PrintLog($"[ID: {id}] [Nickname: {nickname}] Logged Out.");
                        await RemoveClientSocket(id, nickname, roomName, clientSocket);
                        return;
                    }
                    // 헤더를 1바이트만 수신할 경우 남은 1바이트 수신
                    else if (receivedHeaderSize == 1) {
                        await clientSocket.ReceiveAsync(new ArraySegment<byte>(headerBuffer, 1, 1), SocketFlags.None);
                    }
                    #endregion 헤더 수신하기 끝

                    #region 데이터 수신하기
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
                    #endregion 데이터 수신하기 끝

                    // 패킷 타입 가져오기
                    PacketType packetType = (PacketType)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(dataBuffer));

                    // 패킷 타입: 로그인 요청 패킷
                    if (packetType == PacketType.LoginRequest) {
                        LoginRequestPacket packet1 = new LoginRequestPacket(dataBuffer);        // 로그인 요청 패킷 생성(재구성)

                        // 클라이언트 딕셔너리에 (ID, 소켓) 추가
                        // Key(ID)가 이미 존재하는 경우, 덮어쓰기 후 별도의 이벤트 실행
                        ClientsDict.AddOrUpdate(packet1.Id, clientSocket, (oldKey, oldValue) => {
                            DuplicatePacket packet = new DuplicatePacket();     // 중복 접속 패킷 생성
                            oldValue.Send(packet.Serialize());                  // 클라이언트에 중복 접속 패킷 전송
                            return clientSocket;
                        });

                        PrintLog($"[ID: {packet1.Id}] [Nickname: {packet1.Nickname}] Logged In.");
                        id = packet1.Id;
                        nickname = packet1.Nickname;

                        LoginResponsePacket packet2 = new LoginResponsePacket(200);             // 로그인 응답 패킷 생성
                        await clientSocket.SendAsync(packet2.Serialize(), SocketFlags.None);    // 클라이언트에 응답 패킷 전송
                    }

                    // 패킷 타입: 방 생성 요청 패킷
                    else if (packetType == PacketType.CreateRoomRequest) {
                        CreateRoomRequestPacket packet1 = new CreateRoomRequestPacket(dataBuffer);   // 방 생성 요청 패킷 생성
                        Room room = new Room();     // 방 객체 생성

                        // 딕셔너리에 방 저장
                        if (RoomsDict.TryAdd(packet1.RoomName, room)) {
                            roomName = packet1.RoomName;
                            room.UsersDict.TryAdd(id, nickname);    // 방에 입장한 유저 정보 저장
                            PrintLog($"[{roomName}] {nickname} Created Room.");
                            CreateRoomResponsePacket packet2 = new CreateRoomResponsePacket(200);   // 방 생성 응답 패킷 생성
                            await clientSocket.SendAsync(packet2.Serialize(), SocketFlags.None);    // 클라이언트에 응답 패킷 전송
                        }

                        else {
                            PrintLog($"[{roomName}] {nickname} Create Failed...");
                            CreateRoomResponsePacket packet2 = new CreateRoomResponsePacket(500);   // 방 생성 응답 패킷 생성
                            await clientSocket.SendAsync(packet2.Serialize(), SocketFlags.None);    // 클라이언트에 응답 패킷 전송
                        }
                    }

                    // 패킷 타입: 방 목록 요청 패킷
                    else if (packetType == PacketType.RoomListRequest) {       // 방 목록 요청 패킷
                        RoomListResponsePacket packet = new RoomListResponsePacket(RoomsDict.Keys);   // 방 목록 응답 패킷 생성
                        await clientSocket.SendAsync(packet.Serialize(), SocketFlags.None);            // 클라이언트에 응답 패킷 전송
                    }

                    // 패킷 타입: 방 입장 요청 패킷
                    else if (packetType == PacketType.EnterRoomRequest) {
                        EnterRoomRequestPacket packet1 = new EnterRoomRequestPacket(dataBuffer);   // 방 입장 요청 패킷 생성

                        // 방의 유저 딕셔너리에 입장하는 유저 (ID, 닉네임) 추가
                        if (RoomsDict.TryGetValue(packet1.RoomName, out var room)) {
                            roomName = packet1.RoomName;
                            room.UsersDict.TryAdd(id, nickname);    // (ID, 닉네임) 추가
                            PrintLog($"[{roomName}] {nickname} Entered Room.");
                            EnterRoomResponsePacket packet2 = new EnterRoomResponsePacket(200);     // 방 입장 응답 패킷 생성
                            await clientSocket.SendAsync(packet2.Serialize(), SocketFlags.None);    // 클라이언트에 응답 패킷 전송

                            // 방 안에 있는 유저 목록 순회
                            await Task.Delay(100);
                            foreach (var user in room.UsersDict) {
                                // 내 클라이언트에 내 정보를 전송할 필요는 없음
                                if (user.Value == nickname)
                                    continue;

                                // 상대방 클라이언트에 내 정보 전송
                                if (ClientsDict.TryGetValue(user.Key, out var otherClient)) {
                                    UserEnterPacket packet3 = new UserEnterPacket(nickname);
                                    await otherClient.SendAsync(packet3.Serialize(), SocketFlags.None);
                                }

                                // 내 클라이언트에 상대방 정보 전송
                                UserEnterPacket packet4 = new UserEnterPacket(user.Value);
                                await clientSocket.SendAsync(packet4.Serialize(), SocketFlags.None);
                            }
                        }
                    }

                    // 패킷 타입: 유저 퇴장 패킷
                    else if (packetType == PacketType.UserLeave) {
                        UserLeavePacket packet = new UserLeavePacket(dataBuffer);   // 유저 퇴장 패킷 생성

                        PrintLog($"[{roomName}] {nickname} Left Room.");

                        // 방의 유저 딕셔너리에서 퇴장하는 유저 (ID, 닉네임) 삭제
                        if (RoomsDict.TryGetValue(roomName, out var room)) {
                            room.UsersDict.TryRemove(id, out _);

                            // 방에 아무도 없으면 방 삭제
                            if (room.UsersDict.IsEmpty)
                                RoomsDict.TryRemove(roomName, out _);

                            roomName = "";

                            // 유저 퇴장 패킷을 상대 클라이언트에 전송
                            foreach (var user in room.UsersDict) {
                                if (ClientsDict.TryGetValue(user.Key, out var otherClient)) {
                                    await otherClient.SendAsync(packet.Serialize(), SocketFlags.None);
                                }
                            }
                        }
                    }

                    // 패킷 타입: 채팅(메시지 보내기)
                    else if (packetType == PacketType.Chat) {
                        ChatPacket packet = new ChatPacket(dataBuffer);     // 채팅(매시지) 패킷 생성

                        // 방 안에 있는 유저 목록 순회
                        if (RoomsDict.TryGetValue(roomName, out var room)) {
                            foreach (var user in room.UsersDict) {
                                // 채팅(메시지) 패킷을 상대 클라이언트에 전송
                                if (ClientsDict.TryGetValue(user.Key, out var client)) {
                                    await client.SendAsync(packet.Serialize(), SocketFlags.None);
                                }
                            }
                        }

                        PrintLog($"[{roomName}] {packet.Nickname}: {packet.Message}");
                    }

                }
            }

            // 서버 측에서 소켓 예외 발생 시, 해당 클라이언트 소켓 제거
            catch (SocketException e) {
                Console.WriteLine(e);
                await RemoveClientSocket(id, nickname, roomName, clientSocket);
            }

            // 서버 측에서 기타 예외 발생 시, 해당 클라이언트 소켓 제거
            catch (Exception e) {
                Console.WriteLine(e);
                await RemoveClientSocket(id, nickname, roomName, clientSocket);
            }
        }
    }

    // 특정 클라이언트 소켓을 서버에서 제거
    private async Task RemoveClientSocket(string id, string nickname, string roomName, Socket clientSocket) {
 
        // 클라이언트 딕셔너리에서 예외가 발생한 클라이언트 삭제
        if (ClientsDict.TryGetValue(id, out var client) && client == clientSocket)
            ClientsDict.TryRemove(id, out _);

        // 방의 유저 딕셔너리에서 퇴장하는 유저 (ID, 닉네임) 삭제
        if (RoomsDict.TryGetValue(roomName, out var room)) {
            UserLeavePacket packet = new UserLeavePacket(nickname);   // 유저 퇴장 패킷 생성
            room.UsersDict.TryRemove(id, out _);

            // 방에 아무도 없으면 방 삭제
            if (room.UsersDict.IsEmpty)
                RoomsDict.TryRemove(roomName, out _);

            roomName = "";

            // 유저 퇴장 패킷을 상대 클라이언트에 전송
            foreach (var user in room.UsersDict) {
                if (ClientsDict.TryGetValue(user.Key, out var otherClient)) {
                    await otherClient.SendAsync(packet.Serialize(), SocketFlags.None);
                }
            }
        }

        // 클라이언트 소켓의 자원 해제
        clientSocket.Dispose();
    }

    // 로그 출력 메소드(날짜 및 시간 표시)
    private void PrintLog(string? msg) {
        string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Console.WriteLine($"[{now}] {msg}");
    }
}
