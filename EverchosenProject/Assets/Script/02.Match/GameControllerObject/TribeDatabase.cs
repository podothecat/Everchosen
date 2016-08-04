
using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class TribeDatabase : MonoBehaviour {
    private List<List<Tribe>> database = new List<List<Tribe>>();
    private JsonData TribeData;

    

   
	// Use this for initialization
	void Awake ()
	{
	    TribeData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Tribes.json"));
        ConstructTribeDatabase();
	}




    public List<Tribe> FetchBuildingByID(int tribeid)
    {
        for (int i = 0; i < database.Count; i++)
        {
            if (i == tribeid)
            {
                return database[i];
            }
        }
        return null;
    }

    void ConstructTribeDatabase()
    {
        for (int i = 0; i < TribeData.Count; i++)
        {
            database.Add(new List<Tribe>());//list 초기화
            for (int j = 0; j < TribeData[i]["building"].Count; j++)
            {
             /*   Debug.Log((int)TribeData[i]["building"][j]["buildingID"]);
                Debug.Log((int)TribeData[i]["building"][j]["value"]);
                Debug.Log(TribeData[i]["building"][j]["buildingresourceName"].ToString());
                Debug.Log((int)TribeData[i]["building"][j]["cost"]);
                Debug.Log(float.Parse(TribeData[i]["building"][j]["createCount"].ToString()));
                Debug.Log((int)TribeData[i]["building"][j]["spawnUnitID"]);
                Debug.Log(float.Parse(TribeData[i]["building"][j]["unitpower"].ToString()));
                Debug.Log(TribeData[i]["building"][j]["unitresourceName"].ToString());*/
                
                database[i].Add(new Tribe((int)TribeData[i]["building"][j]["buildingID"], (int) TribeData[i]["building"][j]["value"],
                    TribeData[i]["building"][j]["buildingresourceName"].ToString(), (int) TribeData[i]["building"][j]["cost"], float.Parse(TribeData[i]["building"][j]["createCount"].ToString()),
                    (int) TribeData[i]["building"][j]["spawnUnitID"], float.Parse(TribeData[i]["building"][j]["unitpower"].ToString()), TribeData[i]["building"][j]["unitresourceName"].ToString()));
            }
        }
    }
}



public class Tribe
{
            
    public int BuildingID { get; set; }
    public int Value { get; set; }
    public string BuildingResourceName { get; set; }
    public int Cost { get; set; }
    public float CreateCount { get; set; }
    public int SpawnUnitID { get; set; }
    public float UnitPower { get; set; }
    public string UnitResourceName { get; set; }
    public Sprite BuildingSprite { get; set; }
    public Sprite UnitSprite { get; set; }


    public Tribe(int buildingID, int value, string buildingResourceName, int cost, float createCount, int spawnUnitID, float unitPower, string unitResourceName)
    {
        this.BuildingID = buildingID;
        this.Value = value;
        this.BuildingResourceName = buildingResourceName;
        this.Cost = cost;
        this.CreateCount = createCount;
        this.SpawnUnitID = spawnUnitID;
        this.UnitPower = unitPower;
        this.UnitResourceName = unitResourceName;
        this.BuildingSprite = Resources.Load<Sprite>("Sprite/building/" + buildingResourceName);
        this.UnitSprite = Resources.Load<Sprite>("Sprite/unit/" + unitResourceName);
    }
    public Tribe()//아무값이 없을땐 id -1로 설정
    {
        this.BuildingID = -1;
    }



}

/*.
public class Tribe
{

    public int TribeID { get; set; }

    public Tribe(int tribeID)
    {
        this.TribeID = tribeID;
    }
    

}

    */