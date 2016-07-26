using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class BuildingDatabase : MonoBehaviour {
    private List<Building> database = new List<Building>();
    private JsonData buildingData;
	// Use this for initialization
	void Awake ()
	{
	    buildingData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/buildings.json"));
        ConstructBuildingDatabase();
        


	}




    public Building FetchBuildingByID(int id)
    {
        for (int i = 0; i < database.Count; i++)
        {
            if (database[i].ID == id)
            {
                return database[i];
            }
        }
        return null;
    }

    void ConstructBuildingDatabase()
    {
        for (int i = 0; i < buildingData.Count; i++)
        {

            database.Add(new Building((int)buildingData[i]["id"] ,(int)buildingData[i]["value"], buildingData[i]["title"].ToString(), buildingData[i]["resourceName"].ToString(),
                (int)buildingData[i]["cost"],float.Parse(buildingData[i]["createCount"].ToString()), (int)buildingData[i]["spawnUnitID"]));
        }
    }
}

public class Building
{

    public int ID { get; set; }
    public int Value { get; set; }
    public string Title { get; set; }
    public string ResourceName { get; set; }
    public int Cost { get; set; }
    public float CreateCount { get; set; }
    public int SpawnUnitID { get; set; }
    public Sprite Sprite { get; set; }


    public Building(int id, int value, string title, string resourceName, int cost, float createCount, int spawnUnitID)
    {
        this.ID = id;
        this.Value = value;
        this.Title = title;
        this.ResourceName = resourceName;
        this.Cost = cost;
        this.CreateCount = createCount;
        this.SpawnUnitID = spawnUnitID;
        this.Sprite = Resources.Load<Sprite>("Material/Sprite/building/" + resourceName);

    }

    public Building()//아무값이 없을땐 id -1로 설정
    {
        this.ID = -1;
    }
    

}