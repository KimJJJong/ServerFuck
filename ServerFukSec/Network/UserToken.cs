using System;
using System.Collections.Generic;
using System.Net.Sockets;
//using UnityEngine;

/// <summary>
/// ������ ��� �Ͽ� ��,���ŵ��� �ϱ����� Ŭ����
/// </summary>
public class UserToken
{
    // ���Ͽ� ���� ����
    private enum EState
    {
        Idle,               // �����
        Connected,          // �����
        ReserveClosing,     // ���ᰡ �����, �����ִ� ��Ŷ�� ��� ���� �� ������ �ϱ� ���� ���°�
        Closed              // ������ ���� �����
    }

    private Socket _socket;
    private EState _curState = EState.Idle;

    private SocketAsyncEventArgs _receiveEventArgs;
    private SocketAsyncEventArgs _sendEventArgs;
    private MessageResolver _messageResolver = new MessageResolver(NetDefine.BUFFER_SIZE * 3);
    private IPeer _peer; // Peer ��ü. ���ø����̼ǿ��� �����Ͽ� ���. ��Ŷó���� �߰����� ������ �Ѵ�.
    private List<byte[]> _sendingList = new List<byte[]>();
    //private IMessageDispatcher _dispatcher; // ��Ŷ �޽����� ���ν����忡�� ó���ϱ����� Dispatcher

    public Socket Socket => _socket;
    public SocketAsyncEventArgs ReceiveEventArgs => _receiveEventArgs;
    public SocketAsyncEventArgs SendEventArgs => _sendEventArgs;
    public bool IsConnected => _curState == EState.Connected;
    public IPeer Peer => _peer;

    public event Action<UserToken> onSessionClosed; // Close������ ȣ��Ǵ� event

    public UserToken(Socket socket/*, IMessageDispatcher dispatcher*/)
    {
        _socket = socket;
     //   _dispatcher = dispatcher;

        // Receive�� SocketAsyncEventArgs ����
        _receiveEventArgs = new SocketAsyncEventArgs();
        _receiveEventArgs.UserToken = this;
        _receiveEventArgs.Completed += OnReceiveCompleted;
        _receiveEventArgs.SetBuffer(new byte[NetDefine.BUFFER_SIZE], 0, NetDefine.BUFFER_SIZE);
        // Send�� SocketAsyncEventArgs ����
        _sendEventArgs = new SocketAsyncEventArgs();
        _sendEventArgs.UserToken = this;
        _sendEventArgs.Completed += OnSendComplteted;
    }

    public void OnConnected()
    {
        _curState = EState.Connected;
    }

    public void SetPeer(IPeer peer)
    {
        _peer = peer;
        //Console.WriteLine($"Peer ���� {peer}");
    }

    public void StartReceive()
    {
        bool pending = false;
        try
        {
            // �񵿱� Receive
            pending = _socket.ReceiveAsync(_receiveEventArgs);
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
        }

        // ��������ʰ� �ٷ� Receive�� �Ǿ��ٸ� ����
        if (!pending)
        {
            OnReceiveCompleted(null, _receiveEventArgs);
        }
    }

    private void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
    {
        if (e.LastOperation == SocketAsyncOperation.Receive)
        {
            _messageResolver.OnReceive(e.Buffer, e.Offset, e.BytesTransferred, OnMessageComplete);
        }
        else
        {
            _socket.Close();
        }

        // �ٽ� Receive ����
        StartReceive();
    }

    private void OnMessageComplete(byte[] buffer)   //�� �ɴ�� �����Ϥ���
    {
        // ���ν����忡�� ��Ŷ�� ó���ϰ� �Ѵ�.
//        _dispatcher.OnMessage(this, buffer);
        PacketMessageDispatcher.Instance.OnMessage(this, buffer);
    }

    public void OnMessage(byte[] buffer)
    {
        if (_peer == null)
            return;

        Console.WriteLine($"[Received message]: {BitConverter.ToString(buffer)}");
        // protocolID �����´�
        short protocolID = BitConverter.ToInt16(buffer, 2);
        _peer.ProcessMessage(protocolID, buffer);
    }

    public void Close()
    {
        if (_curState == EState.Closed)
        {
            return;
        }

        _curState = EState.Closed;
        _socket.Close();

        _socket = null;

        _sendingList.Clear();

        /*   MainThreadDispatcher.Instance.Add(() =>
           {
               onSessionClosed?.Invoke(this);
           });*/
        // Unity ������ ����: MainThreadDispatcher�� ��ü
        onSessionClosed?.Invoke(this);

        _peer?.Remove();
        Console.WriteLine("Connection closed.");
    }

    public void Send(byte[] data)
    {
        lock (_sendingList)
        {
            _sendingList.Add(data);

            if (_sendingList.Count > 1)
            {
                // ť�� ���𰡰� ��� �ִٸ� ���� ���� ������ �Ϸ���� ���� �����̹Ƿ� ť�� �߰��� �ϰ� �����Ѵ�.
                // ���� �������� SendAsync�� �Ϸ�� ���Ŀ� ť�� �˻��Ͽ� �����Ͱ� ������ SendAsync�� ȣ���Ͽ� �������� ���̴�.
                return;
            }

            StartSend();
        }
    }

    public void Send(Packet packet)
    {
        Console.WriteLine($"Send: {packet}");
        Send(packet.ToByte());
    }

    /// <summary>
    /// �񵿱� ������ �����Ѵ�.
    /// </summary>
    void StartSend()
    {
        try
        {
            // Send�� �̶� ���۸� ä���ش�.
            _sendEventArgs.SetBuffer(_sendingList[0], 0, _sendingList[0].Length);

            // �񵿱� ���� ����.
            bool pending = _socket.SendAsync(_sendEventArgs);
            if (!pending)
            {
                OnSendComplteted(null, _sendEventArgs);
            }
        }
        catch (Exception e)
        {
            if (_socket == null)
            {
                Close();
                return;
            }

            Console.WriteLine("send error!! close socket. " + e.Message);
        }
    }

    public void OnSendComplteted(object sender, SocketAsyncEventArgs e)
    {
        if (e.BytesTransferred <= 0 || e.SocketError != SocketError.Success)
        {
            // ������ ���ܼ� �̹� ������ ����� ����� ���̴�.
            return;
        }

        lock (_sendingList)
        {
            // ���� ���� ����
            _sendingList.RemoveAt(0);
            // �������� �����ִٸ� ������.
            if (_sendingList.Count > 0)
            {
                StartSend();
                return;
            }

            // ���ᰡ ����� ���, ������ �� �������� ��¥ ���� ó���� �����Ѵ�.
            if (_curState == EState.ReserveClosing)
            {
                _socket.Shutdown(SocketShutdown.Send);
            }
        }
    }

    /// <summary>
    /// Ŭ���̾�Ʈ���� ���� ����� ���.
    /// </summary>
    public void Disconnect()
    {
        try
        {
            if (_sendingList.Count <= 0)
            {
                _socket.Shutdown(SocketShutdown.Send);
                return;
            }

            // ������ �����ִٸ�
            _curState = EState.ReserveClosing;
        }
        catch (Exception)
        {
            Close();
        }
    }
}
