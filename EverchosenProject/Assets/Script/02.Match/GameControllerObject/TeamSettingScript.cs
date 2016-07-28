using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;

public class TeamSettingScript : MonoBehaviour
{
    public int playerTeam; //플레이어 
    public int tribeid;
    public string playerbuilding;
    public string Enemybuilding;


    private TribeDatabase tribeDB;
    public List<Tribe> tribeDataToAdd;
    // Use this for initialization
    void Awake ()
    {
       tribeDB = GetComponent<TribeDatabase>();
       SetPlayerTeam(playerTeam,tribeid);
        
    }


    void SetPlayerTeam(int playerteamNumber, int tribeid) // 종족 팀 설정;
    {
        playerTeam = playerteamNumber;

        if (playerTeam == 1)
        {
            playerbuilding = "Team1building";
            Enemybuilding = "Team2building";
        }
        else if (playerTeam == 2)
        {
            playerbuilding = "Team2building";
            Enemybuilding = "Team1building";
            
        }

        tribeDataToAdd = tribeDB.FetchBuildingByID(tribeid);
    }

    public string[] playerTeamSetting()//팀설정함수
    {

        string[] data = new string[2];

        data[0] = playerbuilding;
        data[1] = Enemybuilding;

        return data;
    }
}
