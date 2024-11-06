using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FruitData", menuName = "Fruit/FruitData")]
public class FruitData : ScriptableObject
{
    public FruitAttribute attribute = new FruitAttribute();
    
    public void SetAttribute(FruitAttribute attribute)
    {
        this.attribute = attribute;
    }
}
[System.Serializable]
public class FruitAttribute
{
    public int id;
    public int level;
    public string name;
    public float mass;
    public float scaleX;
    public float scaleY;
    public int score;
    public string imgName;
    public NGUIAtlas atlas;
}