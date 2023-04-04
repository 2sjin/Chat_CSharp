using Core;
using System.Net;
using System.Net.Sockets;

namespace Server;

internal class Server {
    // 서버 소켓(IPv4, 연결지향, TCP)
    private Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

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

    // 비동기 방식으로 데이터를 수신하는 메소드
    private async void ReceiveAsync(object? sender) {
        Socket clientSocket = (Socket)sender!;

        while (true) {
            // 헤더(데이터의 크기) 수신하기
            byte[] headerBuffer = new byte[2];  // 헤더 버퍼
            while (true) {
                // 헤더 수신
                int receivedHeaderSize = await clientSocket.ReceiveAsync(headerBuffer, SocketFlags.None);
                // 수신할 헤더가 없으면 연결 해제
                if (receivedHeaderSize < 1) {
                    Console.WriteLine("클라이언트 연결 해제됨");
                    clientSocket.Dispose();
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

                // 패킷 타입 확인
                PacketType packetType = (PacketType)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(dataBuffer));
                // 로그인 패킷인 경우, ID와 닉네임 출력
                if (packetType == PacketType.LoginRequest) {
                    LoginRequestPacket packet = new LoginRequestPacket(dataBuffer);
                    Console.WriteLine($"id:{packet.Id} nickname:{packet.Nickname}");
                }
            }
        }
    }
}
