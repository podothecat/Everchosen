using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Client;

public class TeamSettingScript : MonoBehaviour
{
    public int PlayerTeam; //플레이어 
    private int _playertribeid;
    private int _enemytribeid;
    public string Playerbuilding;
    
    public string Enemybuilding;
    
    private TribeDatabase _tribeDb;
    public List<Tribe> PlayertribeDataToAdd;
    public List<Tribe> EnemytribeDataToAdd;
    // Use this for initialization
    void Awake ()
    {
        _tribeDb = GetComponent<TribeDatabase>();
        _playertribeid = TribeSetManager.PData.Tribe;

        if (ClientNetworkManager.EnemyMatchingData.Tribe == "Chaos")
        {
            _enemytribeid = 0;
        }
        else if (ClientNetworkManager.EnemyMatchingData.Tribe == "Dwarf")
        {
            _enemytribeid = 1;
        }
        else if (ClientNetworkManager.EnemyMatchingData.Tribe == "Green")
        {
            _enemytribeid = 2;
        }
        else if (ClientNetworkManager.EnemyMatchingData.Tribe == "Human")
        {
            _enemytribeid = 3;
        }
        Debug.Log("TeamColor Mine : " + ClientNetworkManager.EnemyMatchingData.TeamColor);

        if (ClientNetworkManager.EnemyMatchingData.TeamColor == 2)
        {
            SetPlayerTeam(1, _playertribeid, _enemytribeid);
        }
        else if (ClientNetworkManager.EnemyMatchingData.TeamColor == 1)
        {
            SetPlayerTeam(2, _playertribeid, _enemytribeid);
        }
    }
    
    void SetPlayerTeam(int playerteamNumber, int tribeid1, int tribeid2) // 종족 팀 설정;
    {
        PlayerTeam = playerteamNumber;

        if (PlayerTeam == 1)
        {
            Playerbuilding = "Player1building";
            Enemybuilding = "Player2building";
        }
        else if (PlayerTeam == 2)
        {
            Playerbuilding = "Player2building";
            Enemybuilding = "Player1building";
        }

        PlayertribeDataToAdd = _tribeDb.FetchBuildingById(tribeid1);
        EnemytribeDataToAdd = _tribeDb.FetchBuildingById(tribeid2);
        
    }

    public string[] PlayerTeamSetting()//팀설정함수 MaptouchScript에서 사용
    {
        string[] data = new string[2];

        data[0] = Playerbuilding;
        data[1] = Enemybuilding;

        return data;
    }
}
