using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

[CreateAssetMenu(fileName ="FruitData", menuName ="Fruit/FruitDataImporter")]
public class FruitDataImporter : ScriptableObject
{
    public FruitData[] fruits;
    public UIAtlas atlas;

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
            attribute.id = (int)(double)jsonData[i]["ID"];
            attribute.name = jsonData[i]["Name"].ToString();
            attribute.level = (int)(double)jsonData[i]["Fruit_Lv"];
            attribute.scaleX = (float)(double)jsonData[i]["Scale_X"];
            attribute.scaleY = (float)(double)jsonData[i]["Scale_Y"];
            attribute.mass = (float)(double)jsonData[i]["Mass"];
            attribute.score = (int)(double)jsonData[i]["Synthesis_Score"];
            attribute.spawnProb = (float)(double)jsonData[i]["Spawn_Probability"];
            attribute.itemProb = (float)(double)jsonData[i]["Item_Probability"];
            // attribute.animTime = (float)(double)jsonData[i]["Spawn_AnimTime"];
            // attribute.synEffect = jsonData[i]["Synthesis_Effect"].ToString();
            attribute.imgName = jsonData[i]["Spawn_Img"].ToString();
            // attribute.defaultImg = jsonData[i]["Default_Img"].ToString();
            // attribute.spawnAnim = jsonData[i]["Spawn_Anim"].ToString();
            attribute.atlas = atlas;

            fruits[i].SetAttribute(attribute);
        }

    }
}