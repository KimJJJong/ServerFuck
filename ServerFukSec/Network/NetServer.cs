using System.Collections.Concurrent;
using System.Net.Sockets;
//using UnityEngine;

/// <summary>
/// 소켓 통신 서버
/// </summary>
public class NetServer
{
    private bool _run;
    private Listener _listener = new Listener();
    private ConcurrentQueue<Action> _mainThreadQueue = new ConcurrentQueue<Action>();
    private Thread _dispatcherThread;

    public event System.Action<UserToken> onClientConnected;

    public void Start(int backlog)
    {
        if (_run)
        {
            return;
        }

        _listener.onClientConnected += OnClientConnected;
        _listener.Start(NetDefine.PORT, backlog);

        Console.WriteLine("서버 시작");
        _run = true;

        _dispatcherThread = new Thread(DispatcherLoop);
        _dispatcherThread.Start();
    }

    public void End()
    {
        if (!_run)
        {
            return;
        }

        _listener.onClientConnected -= OnClientConnected;
        _listener.Stop();

        Console.WriteLine("서버 종료");
        _run = false;

        // 작업 큐 처리용 스레드 중단
        _dispatcherThread?.Join();
    }

    private void OnClientConnected(Socket socket)
    {
        UserToken userToken = new UserToken(socket/*, PacketMessageDispatcher.Instance*/);
        userToken.onSessionClosed += OnSessionClosed;
        userToken.OnConnected();
        userToken.StartReceive();

        // 작업 큐에 클라이언트 연결 처리 추가
        _mainThreadQueue.Enqueue(() =>
        {
            onClientConnected?.Invoke(userToken);
            Console.WriteLine("유저 접속");
        });
    }

    private void OnSessionClosed(UserToken token)
    {
        Console.WriteLine("OnSessionClosed");
    }

    private void DispatcherLoop()
    {
        while (_run)
        {
            while (_mainThreadQueue.TryDequeue(out var action))
            {
                action?.Invoke();
            }
            Thread.Sleep(10); // CPU 과부하 방지
        }
    }
}