using System.Collections.Concurrent;
using System.Net.Sockets;
//using UnityEngine;

/// <summary>
/// ���� ��� ����
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

        Console.WriteLine("���� ����");
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

        Console.WriteLine("���� ����");
        _run = false;

        // �۾� ť ó���� ������ �ߴ�
        _dispatcherThread?.Join();
    }

    private void OnClientConnected(Socket socket)
    {
        UserToken userToken = new UserToken(socket/*, PacketMessageDispatcher.Instance*/);
        userToken.onSessionClosed += OnSessionClosed;
        userToken.OnConnected();
        userToken.StartReceive();

        // �۾� ť�� Ŭ���̾�Ʈ ���� ó�� �߰�
        _mainThreadQueue.Enqueue(() =>
        {
            onClientConnected?.Invoke(userToken);
            Console.WriteLine("���� ����");
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
            Thread.Sleep(10); // CPU ������ ����
        }
    }
}