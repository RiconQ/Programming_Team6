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
            int idx = 0;
            FruitAttribute attribute = new FruitAttribute();
            attribute.index = (int)(double)jsonData[i][idx++];
            attribute.name = jsonData[i][idx++].ToString();
            attribute.mass = (float)(double)jsonData[i][idx++];
            attribute.scaleX = (float)(double)jsonData[i][idx++];
            attribute.scaleY = (float)(double)jsonData[i][idx++];

            fruits[i].SetAttribute(attribute);
        }
    }
}

[System.Serializable]
public class FruitAttribute
{
    public int index;
    public string name;
    public float mass;
    public float scaleX;
    public float scaleY;
}