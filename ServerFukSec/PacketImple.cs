// SC : ����->Ŭ��, CS : Ŭ��->����, REL : ������
//using UnityEngine;
using System.Runtime.InteropServices;
//using UnityEngine.Playables;

public enum EProtocolID
{
    SC_REQ_USERINFO,           // ���� -> Ŭ�� : ���� ���� ��û
    CS_ANS_USERINFO,           // Ŭ�� -> ���� : ���� ���� ����
    SC_ANS_USERLIST,           // ���� -> Ŭ�� : 
//    CS_REQ_CHANGE_TEAM,        // �� ������ �ʿ� ����
    REL_GAME_READY,            // ���� -> Ŭ�� : ���� �غ� ���� ������
    CS_GAME_READY_OK,          // Ŭ�� -> ���� : ���� �غ� �Ϸ�
    SC_GAME_START,             // ���� -> Ŭ�� : ���� ����


//    REL_PLAYER_POSITION,       // ���� ������ ������ �ʿ���
//    REL_PLAYER_FIRE,           // �츮 ���ӿ��� �ʿ����� ���� ������
//    REL_PLAYER_ANIMATION,      // ���� -> Ŭ�� : ���� �ִϸ��̼� ������ ?�츮�ʿ��� �ִϸ��̼Ǳ��� ������ �ʿ䰡 ���� ������?
    REL_PLAYER_DAMAGE,         // ���� -> Ŭ�� : ���� ������ �̺�Ʈ ������
                               // �츮 ���ӿ��� �ʿ����� ���� ������
    SC_GAME_END,               // ���� -> Ŭ�� : ���� ����

    //Modified Packets
    SC_MANA_UPDATE,            // ���� -> Ŭ�� : ���� ���� ������Ʈ
    CS_SUMMON_UNIT,            // Ŭ�� -> ���� : ���� ��ȯ ��û
    REL_UNIT_SUMMONED,         // ���� -> Ŭ�� : ���� ��ȯ �Ϸ� ������
    REL_UNIT_STATE,            // ���� -> Ŭ�� : ���� ���� ������Ʈ
    REL_UNIT_ACTION,           // ���� -> Ŭ�� : ���� �ൿ �̺�Ʈ
    REL_GRID_UPDATE,           // ���� -> Ŭ�� : �� ���� ���� ���� ������Ʈ
    SC_GAME_STATE_UPDATE,      // ���� -> Ŭ�� : ���� ���� ���� ������Ʈ
    CS_PLAYER_ACTION,          // Ŭ�� -> ���� : �÷��̾� �׼� ��û (�̵�, ���� ��)
    REL_ACTION_RESULT,         // ���� -> Ŭ�� : �÷��̾� �׼� ��� ������

    SC_ERROR_MESSAGE,          // ���� -> Ŭ�� : ���� �޽��� ����


}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketReqUserInfo : Packet
{
   // [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
    public int uid;
    //public ETeam team;

    public PacketReqUserInfo()
        : base((short)EProtocolID.SC_REQ_USERINFO)
    {
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketAnsUserInfo : Packet
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
    public string id;
    //public bool host;

    public PacketAnsUserInfo()
        : base((short)EProtocolID.CS_ANS_USERINFO)
    {
    }
}

// ���������� �迭�� ���� ��Ҵ� struct�� �ؾ� ������ �Ȼ����. 
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UserInfo
{
    public int uid;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
    public string id;
   // public ETeam team;
   //public bool host;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketAnsUserList : Packet
{
    public int userNum;
    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 20)]
    public UserInfo[] userInfos = new UserInfo[20];
    public PacketAnsUserList()
        : base((short)EProtocolID.SC_ANS_USERLIST)
    {
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketGameReady : Packet
{
    public PacketGameReady()
        : base ((short)EProtocolID.REL_GAME_READY)
    {
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketGameReadyOk : Packet
{
    public PacketGameReadyOk()
        : base((short)EProtocolID.CS_GAME_READY_OK)
    {
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GameStartInfo
{
    public int uid;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
    public string id;
    public int roomNum;
   // public ETeam team;
   // public Vector3Int position;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketGameStart : Packet
{
    public int userNum;
    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 20)]
    public GameStartInfo[] startInfos = new GameStartInfo[20];

    public PacketGameStart()
        : base((short)EProtocolID.SC_GAME_START)
    {
    }
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketPlayerDamage : Packet
{
    public int attackUID;       // ���� �÷��̾�
    public int targetUID;       // ���� �÷��̾�
    public PacketPlayerDamage()
        : base ((short)EProtocolID.REL_PLAYER_DAMAGE)
    {
    }
}



////////////Modefied Packet///////////////
///
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketManaUpdate : Packet
{
    public int currentMana;
    public int maxMana;
    public PacketManaUpdate()
        : base((short)EProtocolID.SC_MANA_UPDATE)
    {
    }
}



[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketSummonUnit : Packet
{
    public int cardID;
    public Vector3Int position;

    public PacketSummonUnit()
        : base((short)EProtocolID.CS_SUMMON_UNIT)
    {
    }
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketUnitSummoned : Packet
{
    public int unitUID;         // ��ȯ�� ������ ���� ID
    public int cardID;          // ī�� ID
    public Vector3Int position;    // ��ȯ�� ��ġ
    public int ownerUID;        // ���� �������� UID

    public PacketUnitSummoned()
        : base((short)EProtocolID.REL_UNIT_SUMMONED)
    {
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketUnitState : Packet
{
    public int unitUID;         // ������Ʈ ��� ������ UID
    public Vector3Int position;    // ������ ���� ��ġ
    public int currentHealth;   // ������ ���� ü��

    public PacketUnitState()
        : base((short)EProtocolID.REL_UNIT_STATE)
    {
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketUnitAction : Packet
{
    public int unitUID;         // �ൿ�ϴ� ������ UID
    public int targetUID;       // ���� �Ǵ� ��ų ����� UID
    public int actionType;      // �ൿ Ÿ�� (��: ����, ��ų ��)

    public PacketUnitAction()
        : base((short)EProtocolID.REL_UNIT_ACTION)
    {
    }
}



[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketGridUpdate : Packet
{
    public Vector2Int position;
    public int ownerUID;

    public PacketGridUpdate()
        : base((short)EProtocolID.REL_GRID_UPDATE)
    {
    }
}



[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketGameEnd : Packet
{
  //  public ETeam winningTeam;

    public PacketGameEnd()
        : base((short)EProtocolID.SC_GAME_END)
    {
    }
}


////////////////////
///
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Vector2Int
{
    public int x;
    public int y;

    public Vector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Vector3Int
{
    public int x;
    public int y;
    public int z;

    public Vector3Int(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

