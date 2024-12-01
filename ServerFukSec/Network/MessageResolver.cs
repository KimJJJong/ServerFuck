using System;

public class MessageResolver
{
    // ���������͸� ����, ��Ŷ���� �ڸ���.
    private byte[] _messageBuffer;
    // ó�� �ؾߵ� �����ͱ����� ��ġ
    private int _curPosition;

    public MessageResolver(int bufferSize)
    {
        _messageBuffer = new byte[bufferSize];
    }

    /// <summary>
    /// ���� ����Ʈ �迭�� ó��
    /// </summary>
    /// <param name="bufffer">���� ����Ʈ �迭</param>
    /// <param name="offset">���� ����Ʈ �迭 ��ġ</param>
    /// <param name="transferred">���� ����</param>
    /// <param name="onComplete">�ϳ��� ��Ŷ�� ����� ������ ȣ��</param>
    public void OnReceive(byte[] bufffer, int offset, int transferred, Action<byte[]> onComplete)
    {
        // ��Ŷ����
        // [body����:2byte][protocolID:2byte][body:������ŭ]

        // ���� ����Ʈ�迭�� �޽������� ������ġ ���ʿ� ���� �Ѵ�.
        // (�����ҹ���Ʈ �迭, �����ҹ���Ʈ �迭 ��ġ, ������Ʈ �迭, ������Ʈ��ġ, ����)
        Array.Copy(bufffer, offset, _messageBuffer, _curPosition, transferred);
        // ������ŭ ��ġ�� �ű��.
        _curPosition += transferred;

        while (_curPosition > 0)
        {
            // ���� ���� ũ�Ⱑ ������� �۴ٸ� ���޾ƾߵȴ�.
            if (_curPosition < NetDefine.HEADER_SIZE)
            {
                return;
            }

            // ��� ������ �����´�.
            // BitConverter : ����Ʈ �迭�� �о Ư�� �ڷ������� ��ȯ.
            short size = BitConverter.ToInt16(_messageBuffer, 0);
            short protocolID = BitConverter.ToInt16(_messageBuffer, 2);

            // ��Ŷ�� �ϼ����� �ʾҴٸ� ���޾ƾߵȴ�.
            if (_curPosition < size)
            {
                return;
            }

            // �ϳ��� ��Ŷ�� �ϼ���, �迭�� �����ؼ� �ѱ�
            byte[] clone = new byte[size];
            Array.Copy(_messageBuffer, clone, size);

            // ó���Լ� ȣ��
            onComplete?.Invoke(clone);

            // �ϳ��� ��Ŷ�� ó�������Ƿ� _curPosition ��ġ�� �ű�, _messageBuffer�� ó���Ѹ�ŭ�� �������� �ű�
            _curPosition -= size;
            Array.Copy(_messageBuffer, size, _messageBuffer, 0, _curPosition);
        }
    }
}