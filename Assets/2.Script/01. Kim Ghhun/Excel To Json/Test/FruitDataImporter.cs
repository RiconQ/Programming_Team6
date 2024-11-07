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
    public NGUIAtlas atlas;

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
            attribute.level = (int)(double)jsonData[i]["Fruit_Lv"];
            attribute.name = jsonData[i]["Name"].ToString();
            attribute.mass = (float)(double)jsonData[i]["Mass"];
            attribute.scaleX = (float)(double)jsonData[i]["Scale_X"];
            attribute.scaleY = (float)(double)jsonData[i]["Scale_Y"];
            attribute.score = (int)(double)jsonData[i]["Synthesis_Score"];
            attribute.imgName = jsonData[i]["Default_Img"].ToString();
            attribute.atlas = atlas;

            fruits[i].SetAttribute(attribute);

            UnityEditor.EditorUtility.SetDirty(fruits[i]);
            UnityEditor.AssetDatabase.SaveAssets();
        }
    }
}