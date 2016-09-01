
using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class TribeDatabase : MonoBehaviour {
    private readonly List<List<Tribe>> _database = new List<List<Tribe>>();
    public readonly List<Correction> CrDatabase =new List<Correction>();
    
    private JsonData _tribeData;
    
	// Use this for initialization
	void Awake ()
    {
            var data = Resources.Load("DB/tribes");
            _tribeData = JsonMapper.ToObject(data.ToString());

        ConstructTribeDatabase();
    }
    
    public List<Tribe> FetchBuildingById(int tribeid)
    {
        for (int i = 0; i < _database.Count; i++)
        {
            if (i == tribeid)
            {
                return _database[i];
            }
        }
        return null;
    }

    void ConstructTribeDatabase()
    {
        for (int i = 0; i < 4; i++)
        {
            _database.Add(new List<Tribe>());//list 초기화
            for (int j = 0; j < _tribeData[i]["building"].Count; j++)
            {
                _database[i].Add(new Tribe((int)_tribeData[i]["building"][j]["buildingID"], (int) _tribeData[i]["building"][j]["value"],
                    _tribeData[i]["building"][j]["buildingresourceName"].ToString(), (int) _tribeData[i]["building"][j]["cost"], float.Parse(_tribeData[i]["building"][j]["createCount"].ToString()),
                    (int) _tribeData[i]["building"][j]["spawnUnitID"], float.Parse(_tribeData[i]["building"][j]["unitpower"].ToString()), _tribeData[i]["building"][j]["unitKind"].ToString(), _tribeData[i]["building"][j]["unitresourceName"].ToString()));
            }
        }
        
            for (int j = 0; j < _tribeData[4]["correction"].Count; j++)
            {
            CrDatabase.Add(new Correction((int)_tribeData[4]["correction"][j]["footMan"], (int)_tribeData[4]["correction"][j]["bowMan"], (int)_tribeData[4]["correction"][j]["horseMan"], (int)_tribeData[4]["correction"][j]["skyMan"]));
            }
        
    }
}



public class Tribe
{
    public int BuildingId { get; set; }
    public int Value { get; set; }
    public string BuildingResourceName { get; set; }
    public int Cost { get; set; }
    public float CreateCount { get; set; }
    public int SpawnUnitId { get; set; }
    public float UnitPower { get; set; }
    public string UnitKind { get; set; }
    public string UnitResourceName { get; set; }
    public Sprite BuildingSprite { get; set; }
    public Sprite BUnitSprite { get; set; }
    public Sprite RUnitSprite { get; set; }


    public Tribe(int buildingID, int value, string buildingResourceName, int cost, float createCount, int spawnUnitID, float unitPower,string unitKind, string unitResourceName)
    {
        this.BuildingId = buildingID;
        this.Value = value;
        this.BuildingResourceName = buildingResourceName;
        this.Cost = cost;
        this.CreateCount = createCount;
        this.SpawnUnitId = spawnUnitID;
        this.UnitPower = unitPower;
        this.UnitKind = unitKind;
        this.UnitResourceName = unitResourceName;
        this.BuildingSprite = Resources.Load<Sprite>("Sprite/building/" + buildingResourceName);
        this.BUnitSprite = Resources.Load<Sprite>("Sprite/unit/B-" + unitResourceName);
        this.RUnitSprite = Resources.Load<Sprite>("Sprite/unit/R-" + unitResourceName);

    }
    public Tribe()//아무값이 없을땐 id -1로 설정
    {
        this.BuildingId = -1;
    }
}

public class Correction
{
    public int FootMan { get; set; }
    public int BowMan { get; set; }
    public int HorseMan { get; set; }
    public int SkyMan { get; set; }

    public Correction(int footman, int bowman, int horseman, int skyman)
    {
        this.FootMan = footman;
        this.BowMan = bowman;
        this.HorseMan = horseman;
        this.SkyMan = skyman;
    }
}