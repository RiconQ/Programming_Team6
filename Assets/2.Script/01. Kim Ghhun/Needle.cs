using UnityEngine;

public class Needle : MonoBehaviour
{
    public GameObject fruit;

    public void UseNeedle()
    {
        fruit.SetActive(false);
    }
}
