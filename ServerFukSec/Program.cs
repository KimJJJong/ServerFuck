class Program
{
    static void Main(string[] args)
    {
        // 서버 포트 설정 및 backlog (최대 연결 대기 수) 설정
        const int port = NetDefine.PORT;
       // const int backlog = 10;

        // 서버 인스턴스 생성 및 초기화
        Server server = new Server();
        server.Start();

        Console.WriteLine("게임 서버가 시작되었습니다.");
        Console.WriteLine($"포트: {port}");
        Console.WriteLine("클라이언트의 연결을 기다립니다...\n");

        // 서버의 메인 루프 실행 (종료 명령 대기)
        while (true)
        {
            string command = Console.ReadLine();
            if (command.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("서버 종료 중...");
                server.Stop();
                break;
            }
            else if (command.Equals("list", StringComparison.OrdinalIgnoreCase))
            {
                server.PrintConnectedClients();
            }
        }
    }
}