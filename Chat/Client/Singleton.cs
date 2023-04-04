using System.Net;
using System.Net.Sockets;

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

    // Singleton 객체
    private static Singleton? instance;
    public static Singleton Instance {
        get {
            if (instance == null)
                instance = new Singleton();
            return Singleton.Instance;
        }
    }

    // 비동기 방식으로 서버와 클라이언트를 연결하는 메소드
    public async Task ConnectAsync() {
        // 클라이언트의 연결 요청을 수락함
        await Socket.ConnectAsync(endPoint);

        // TAP(작업 기반 비동기 패턴): 스레드풀의 작업 큐에 메소드를 대기시킴
        ThreadPool.QueueUserWorkItem(ReceiveAsync, Socket);
    }

    // 비동기 방식으로 데이터를 수신하는 메소드
    private async void ReceiveAsync(object? sender) {
        Socket socket = (Socket)sender;

        // 헤더(데이터의 크기) 수신하기
        byte[] headerBuffer = new byte[2];  // 헤더 버퍼
        while (true) {
            // 헤더 수신
            int receivedHeaderSize = await socket.ReceiveAsync(headerBuffer, SocketFlags.None);
            // 수신할 헤더가 없으면 연결 해제
            if (receivedHeaderSize < 1) {
                Console.WriteLine("클라이언트 연결 해제됨");
                socket.Dispose();
                return;
            }
            // 헤더를 1바이트만 수신할 경우 남은 1바이트 수신
            else if (receivedHeaderSize == 1) {
                await socket.ReceiveAsync(new ArraySegment<byte>(headerBuffer, 1, 1), SocketFlags.None);
            }
        }

        // 데이터 수신하기
        int receivedDataSize = 0;  // 지금까지 수신한 데이터의 크기
        short totalDataSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(headerBuffer));  // 전체 데이터의 크기
        byte[] dataBuffer = new byte[totalDataSize];     // 데이터 버퍼

        // 데이터 버퍼 내의 모든 데이터 수신하기
        while (receivedDataSize < totalDataSize) {
            int tmp = await socket.ReceiveAsync(new ArraySegment<byte>(dataBuffer, receivedDataSize, totalDataSize - receivedDataSize),
                                                SocketFlags.None);
            receivedDataSize += tmp;
        }
    }
}
