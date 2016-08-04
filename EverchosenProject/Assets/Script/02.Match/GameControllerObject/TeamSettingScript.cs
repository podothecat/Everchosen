using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;

public class TeamSettingScript : MonoBehaviour
{
    public int playerTeam; //플레이어 
    public int playertribeid;
    
    public string playerbuilding;


    public int Enemytribeid;
    public string Enemybuilding;


    private TribeDatabase tribeDB;
    public List<Tribe> PlayertribeDataToAdd;
    public List<Tribe> EnemytribeDataToAdd;
    // Use this for initialization
    void Awake ()
    {


        tribeDB = GetComponent<TribeDatabase>();
        playertribeid = TribeSetManager.p1Data.Tribe;
        Enemytribeid = TribeSetManager.p2Data.Tribe;
        SetPlayerTeam(playerTeam,playertribeid, Enemytribeid);
        
    }


    void SetPlayerTeam(int playerteamNumber, int tribeid1, int tribeid2) // 종족 팀 설정;
    {
        playerTeam = playerteamNumber;

        if (playerTeam == 1)
        {
            playerbuilding = "Player1building";
            Enemybuilding = "Player2building";
            
        }
        else if (playerTeam == 2)
        {
            playerbuilding = "Player2building";
            Enemybuilding = "Player1building";
          
        }

        PlayertribeDataToAdd = tribeDB.FetchBuildingByID(tribeid1);
        EnemytribeDataToAdd = tribeDB.FetchBuildingByID(tribeid2);



    }

    public string[] playerTeamSetting()//팀설정함수
    {

        string[] data = new string[2];

        data[0] = playerbuilding;
        data[1] = Enemybuilding;

        return data;
    }
}
