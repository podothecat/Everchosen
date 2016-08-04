using System;
using UnityEngine;
using System.Collections;



public class TribeSetManager : MonoBehaviour
{


    public static Player1Data p1Data = new Player1Data();
    public static Player2Data p2Data = new Player2Data();
    public static void SetUserData()
    {
        GameObject playerData = GameObject.Find("ButtonControllerObject");
        p1Data= new Player1Data(playerData.GetComponent<MainButtonController>().tribeNumberData, "MonJon", null);
    }


    public static void SetEnemyData()
    {
       p2Data = new Player2Data(1,"HI",null);
    }


}


public class Player1Data
{
    public int Tribe { get; set; }
    public string UserID { get; set; }
    public int[] Spell { get; set; }


    public Player1Data(int tribe, string userid, int[] spell)
    {
        this.Tribe = tribe;
        this.UserID = userid;
        this.Spell = spell;
    }

    public Player1Data()
    {
        this.Tribe = -1;
    }
}

public class Player2Data
{
    public int Tribe { get; set; }
    public string UserID { get; set; }
    public int[] Spell { get; set; }

    public Player2Data(int tribe, string userid, int[] spell)
    {
        this.Tribe = tribe;
        this.UserID = userid;
        this.Spell = spell;
    }
    public Player2Data()
    {
        this.Tribe = -1;
    }
}