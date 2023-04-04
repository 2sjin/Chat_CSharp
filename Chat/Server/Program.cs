namespace Server;

internal class Program {
    const string ip = "192.168.0.14";

    static async Task Main(string[] args) {
        Server server = new Server(ip, 20000, 10);
        await server.StartAsync();
    }
}