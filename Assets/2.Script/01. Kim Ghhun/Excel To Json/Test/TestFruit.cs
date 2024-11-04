using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFruit : MonoBehaviour
{
    public int level;
    public Rigidbody2D rb;

    public void SetAttribute(FruitAttribute attribute)
    {
        level = attribute.index;
        transform.localScale = new Vector3(attribute.scaleX, attribute.scaleY, 1);
        rb.mass = attribute.mass;
    }
}
