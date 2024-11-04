using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFruit : MonoBehaviour
{
    public string fruitName;
    public int score;
    public int level;
    public Rigidbody2D rb;

    public void SetAttribute(FruitAttribute attribute)
    {
        level = attribute.id;
        fruitName = attribute.name;
        score = attribute.score;
        transform.localScale = new Vector3(attribute.scaleX, attribute.scaleY, 1);
        rb.mass = attribute.mass;
    }
}
