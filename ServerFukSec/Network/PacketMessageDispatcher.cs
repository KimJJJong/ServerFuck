/*using System.Collections.Concurrent;
using UnityEngine;

/// <summary>
/// ���ŵ� �޽����� ����Ƽ�� ���ν����忡�� ó���ϰ� �Ѵ�.
/// </summary>
public class PacketMessageDispatcher : MonoBehaviour, IMessageDispatcher
{
    // �̱���(Singleton) ���� : �ν��Ͻ��� �ϳ��� ����, �������� ����
    private static PacketMessageDispatcher _instance;
    public static PacketMessageDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject container = new GameObject("PacketMessageDispatcher");
                container.name = "PacketMessageDispatcher";
                _instance = container.AddComponent<PacketMessageDispatcher>();
            }

            return _instance;
        }
    }

    public struct PacketMessage
    {
        public UserToken token; // ����������ū
        public byte[] buffer;   // ��Ŷ

        public PacketMessage(UserToken token, byte[] buffer)
        {
            this.token = token;
            this.buffer = buffer;
        }
    }

    // �����忡 ������ Queue
    private ConcurrentQueue<PacketMessage> _messageQueue;

    public void Init()
    {
       _messageQueue = new ConcurrentQueue<PacketMessage>();
    }

    // ����Ƽ�� Update���� Queue�� ����ִ� ��Ŷ�� ��� ó���ϰ� �Ѵ�.
    private void Update()
    {
        // TryDequeue �����ϰ� ó�� ��Ҹ� �����´�.
        while (_messageQueue.TryDequeue(out PacketMessage msg))
        {
            if (!msg.token.IsConnected)
                continue;

            msg.token.OnMessage(msg.buffer);
        }
    }

    // ���� ��Ŷ�� �߰��Ѵ�.
    public void OnMessage(UserToken token, byte[] buffer)
    {
        _messageQueue.Enqueue(new PacketMessage(token, buffer));
    }
}*/
using System.Collections.Concurrent;
//using UnityEngine;

/// <summary>
/// ���ŵ� �޽����� ����Ƽ�� ���ν����忡�� ó���ϰ� �Ѵ�.
/// </summary>
using System;
using System.Collections.Concurrent;

public class PacketMessageDispatcher : IMessageDispatcher
{
    private static readonly Lazy<PacketMessageDispatcher> _instance =
        new Lazy<PacketMessageDispatcher>(() => new PacketMessageDispatcher());

    public static PacketMessageDispatcher Instance => _instance.Value;

    public struct PacketMessage
    {
        public UserToken Token; // ���� ���� ��ū
        public byte[] Buffer;   // ���ŵ� ��Ŷ

        public PacketMessage(UserToken token, byte[] buffer)
        {
            Token = token;
            Buffer = buffer;
        }
    }

    private readonly ConcurrentQueue<PacketMessage> _messageQueue;

    private PacketMessageDispatcher()
    {
        _messageQueue = new ConcurrentQueue<PacketMessage>();
    }

    // �޽����� ť�� �߰�
    public void OnMessage(UserToken token, byte[] buffer)
    {
        _messageQueue.Enqueue(new PacketMessage(token, buffer));
    }

    // ť�� �ִ� �޽����� ó��
    public void Dispatch()
    {
        while (_messageQueue.TryDequeue(out var msg))
        {
            if (!msg.Token.IsConnected)
                continue;

            // �ش� UserToken���� �޽����� ó��
            msg.Token.OnMessage(msg.Buffer);
        }
    }
}
