using System.Collections.Concurrent;

namespace Core;

public class Room {
    // 여러 스레드에서 접근 가능한 딕셔너리
    // Key는 ID, Value는 닉네임
    public ConcurrentDictionary<string, string> Users { get; } = new ConcurrentDictionary<string, string>();
}
