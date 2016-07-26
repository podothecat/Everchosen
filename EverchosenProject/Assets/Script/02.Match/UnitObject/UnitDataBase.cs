using System;
using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;//파일텍스트 읽어오기위함

public class UnitDataBase : MonoBehaviour {
    private List<Unit> database = new List<Unit>();
    private JsonData unitData;

    void Awake()
    {
        unitData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath+"/StreamingAssets/Units.json"));//
        ConstructUnitDatabase();
      
      
    }


    public Unit FetchUnitByID(int id)
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

    void ConstructUnitDatabase()
    {
        for (int i = 0; i < unitData.Count; i++)
        {
            
            database.Add(new Unit((int)unitData[i]["id"],unitData[i]["title"].ToString(),unitData[i]["resourceName"].ToString() ,float.Parse(unitData[i]["value"].ToString()), float.Parse(unitData[i]["power"].ToString())));
        }
    }


}

public class Unit
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string ResourceName { get; set; }
    public float Value { get; set; }
    public float Power { get; set; }
    public Sprite Sprite { get; set; }

    public Unit(int id, string title,string resourceName, float value, float power)
    {
        this.ID = id;
        this.Title = title;
        this.ResourceName = resourceName;
        this.Value = value;
        this.Power = power;
        this.Sprite = Resources.Load<Sprite>("Material/Sprite/unit/" + resourceName);
    }

    public Unit() //정보가 없을땐 아이디를 -1로 처리
    {
        this.ID = -1;
    }
}
