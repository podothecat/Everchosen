using UnityEngine;
using System.Collections;
using System.Reflection.Emit;

public class TeamSettingScript : MonoBehaviour
{
    public int playerTeam; //플레이어 
    public string playerbuilding;
    public string Enemybuilding;
    // Use this for initialization
    void Awake ()
    {
       SetPlayerTeam(playerTeam);
    }


    void SetPlayerTeam(int playerteamNumber) // 종족 팀 설정;
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
    }

    public string[] playerTeamSetting()//팀설정함수
    {

        string[] data = new string[2];

        data[0] = playerbuilding;
        data[1] = Enemybuilding;

        return data;
    }
}
