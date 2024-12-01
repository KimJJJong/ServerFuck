// SC : 서버->클라, CS : 클라->서버, REL : 릴레이
//using UnityEngine;
using System.Runtime.InteropServices;
//using UnityEngine.Playables;

public enum EProtocolID
{
    SC_REQ_USERINFO,           // 서버 -> 클라 : 유저 정보 요청
    CS_ANS_USERINFO,           // 클라 -> 서버 : 유저 정보 응답
    SC_ANS_USERLIST,           // 서버 -> 클라 : 
//    CS_REQ_CHANGE_TEAM,        // 팀 변경이 필요 없음
    REL_GAME_READY,            // 서버 -> 클라 : 게임 준비 상태 릴레이
    CS_GAME_READY_OK,          // 클라 -> 서버 : 게임 준비 완료
    SC_GAME_START,             // 서버 -> 클라 : 게임 시작


//    REL_PLAYER_POSITION,       // 유닛 단위의 관리가 필요함
//    REL_PLAYER_FIRE,           // 우리 게임에서 필요하지 않은 릴레이
//    REL_PLAYER_ANIMATION,      // 서버 -> 클라 : 유닛 애니메이션 릴레이 ?우리쪽에서 애니메이션까지 관리할 필요가 없지 않을까?
    REL_PLAYER_DAMAGE,         // 서버 -> 클라 : 유닛 데미지 이벤트 릴레이
                               // 우리 게임에서 필요하지 않은 릴레이
    SC_GAME_END,               // 서버 -> 클라 : 게임 종료

    //Modified Packets
    SC_MANA_UPDATE,            // 서버 -> 클라 : 마나 상태 업데이트
    CS_SUMMON_UNIT,            // 클라 -> 서버 : 유닛 소환 요청
    REL_UNIT_SUMMONED,         // 서버 -> 클라 : 유닛 소환 완료 릴레이
    REL_UNIT_STATE,            // 서버 -> 클라 : 유닛 상태 없데이트
    REL_UNIT_ACTION,           // 서버 -> 클라 : 유닛 행동 이벤트
    REL_GRID_UPDATE,           // 서버 -> 클라 : 맵 격자 점령 상태 업데이트
    SC_GAME_STATE_UPDATE,      // 서버 -> 클라 : 게임 전반 상태 업데이트
    CS_PLAYER_ACTION,          // 클라 -> 서버 : 플레이어 액션 요청 (이동, 공격 등)
    REL_ACTION_RESULT,         // 서버 -> 클라 : 플레이어 액션 결과 릴레이

    SC_ERROR_MESSAGE,          // 서버 -> 클라 : 에러 메시지 전달


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

// 마샬링으로 배열에 들어가는 요소는 struct로 해야 문제가 안생긴다. 
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
    public int attackUID;       // 때린 플레이어
    public int targetUID;       // 맞은 플레이어
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
    public int unitUID;         // 소환된 유닛의 고유 ID
    public int cardID;          // 카드 ID
    public Vector3Int position;    // 소환된 위치
    public int ownerUID;        // 유닛 소유자의 UID

    public PacketUnitSummoned()
        : base((short)EProtocolID.REL_UNIT_SUMMONED)
    {
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketUnitState : Packet
{
    public int unitUID;         // 업데이트 대상 유닛의 UID
    public Vector3Int position;    // 유닛의 현재 위치
    public int currentHealth;   // 유닛의 현재 체력

    public PacketUnitState()
        : base((short)EProtocolID.REL_UNIT_STATE)
    {
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketUnitAction : Packet
{
    public int unitUID;         // 행동하는 유닛의 UID
    public int targetUID;       // 공격 또는 스킬 대상의 UID
    public int actionType;      // 행동 타입 (예: 공격, 스킬 등)

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

