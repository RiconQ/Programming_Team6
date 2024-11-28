using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance = null;

    [Header("----------Item Duration")]
    [SerializeField] GameObject endObject;
    [SerializeField] Transform controlPoint;

    [Header("About Item")]
    [SerializeField] GameObject itemOnlyData;
    [SerializeField] List<GameObject> inventoryList;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }




}
