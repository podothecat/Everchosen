
using UnityEngine;



public class TribeSetManager : MonoBehaviour
{
    public static PlayerData PData = new PlayerData();
}

public class PlayerData
{
    public string UserID { get; set; }
    public string NickName { get; set; }
    public int Tribe { get; set; }
    public string TribeName { get; set; }
    public int Spell { get; set; }


    public PlayerData(string userid,string nickName, int tribe, string tribeName, int spell)
    {
        this.NickName = nickName;
        this.Tribe = tribe;
        this.UserID = userid;
        this.TribeName = tribeName;
        this.Spell = spell;
        
    }

    public PlayerData()
    {
        this.Tribe = -1;
    }
}
