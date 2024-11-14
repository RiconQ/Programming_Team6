using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FruitData", menuName = "Fruit/FruitData")]
[System.Serializable]
public class FruitData : ScriptableObject
{
    public FruitAttribute attribute = new FruitAttribute();
    
    public void SetAttribute(FruitAttribute attribute)
    {
        this.attribute = attribute;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
}
[System.Serializable]
public class FruitAttribute
{
    public int id;
    public string name;
    public int level;
    public float scaleX;
    public float scaleY;
    public float mass;
    public int score;
    public float spawnProb;
    public float itemProb;
    // public float animTime;
    // public string synEffect;
    public string imgName;
    // public string defaultImg;
    // public string spawnAnim;
    public UIAtlas atlas;
}