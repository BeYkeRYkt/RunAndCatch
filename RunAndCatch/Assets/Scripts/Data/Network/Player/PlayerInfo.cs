using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public static PlayerInfo DEFAULT = new PlayerInfo(-1, "default", -1, -1);

    public int mId;
    public string mNickname;
    public int mCharacterId;
    public int mPerkId;

    public PlayerInfo(int id, string nickname, int characterId, int perkId)
    {
        this.mId = id;
        this.mNickname = nickname;
        this.mCharacterId = characterId;
        this.mPerkId = perkId;
    }
}
