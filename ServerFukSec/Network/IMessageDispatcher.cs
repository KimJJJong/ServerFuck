/// <summary>
/// ��Ŷ �޽��� ó���� ���� �������̽�
/// </summary>
public interface IMessageDispatcher
{
    void OnMessage(UserToken user, byte[] buffer);
}