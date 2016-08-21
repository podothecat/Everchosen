using System;
using UnityEngine;
using System.Collections;



public class TribeSetManager : MonoBehaviour
{

    public static PlayerData PData = new PlayerData();
    
}


public class PlayerData
{
  
    public string UserID { get; set; }
    public int Tribe { get; set; }
    public string TribeName { get; set; }
    public int Spell { get; set; }


    public PlayerData(string userid,int tribe, string tribeName, int spell)
    {
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
