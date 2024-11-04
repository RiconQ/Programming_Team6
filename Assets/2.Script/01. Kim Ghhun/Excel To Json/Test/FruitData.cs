using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName ="FruitData", menuName ="Fruit/FruitData")]
public class FruitData : ScriptableObject
{
    public TestFruit[] fruits;

    public void UpdateFruitAttribute(string jsonPath)
    {
        if(!File.Exists(jsonPath))
        {
            Debug.LogError("File Not Found");
            return;
        }

        string jsonContent = File.ReadAllText(jsonPath);
        JsonData jsonData = JsonMapper.ToObject(jsonContent);
       
        for (int i = 0; i < fruits.Length; i++)
        {
            FruitAttribute attribute = new FruitAttribute();
            attribute.id = i;
            attribute.name = jsonData[i]["Name"].ToString();
            attribute.mass = (float)(double)jsonData[i]["Mass"];
            attribute.scaleX = (float)(double)jsonData[i]["Scale_X"];
            attribute.scaleY = (float)(double)jsonData[i]["Scale_Y"];
            attribute.score = (int)(double)jsonData[i]["Synthesis_Score"];
            attribute.imgName = jsonData[i]["Default_Img"].ToString();

            fruits[i].SetAttribute(attribute);
        }
    }
}

[System.Serializable]
public class FruitAttribute
{
    public int id;
    public string name;
    public float mass;
    public float scaleX;
    public float scaleY;
    public int score;
    public string imgName;
}