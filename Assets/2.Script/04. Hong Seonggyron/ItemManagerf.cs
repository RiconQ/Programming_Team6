using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemManagerf : MonoBehaviour
{
    public static ItemManagerf Instance = null;

    [Header("----------Item Duration")]
    [SerializeField] GameObject endObject;
    [SerializeField] Transform controlPoint;

    [Header("About Item")]
    [SerializeField] GameObject itemOnlyData;
    [SerializeField] List<GameObject> inventoryList;


    [SerializeField] Transform targetBox;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Moving()
    {


    }

}
