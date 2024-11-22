using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    public string itemName;
    public int itemValue;
    public int itemKind;
    public string itemImage;


    //아이템 오브젝트를 만들기 위한 임시 스크립트
    public void Initialize(ItemInfo itemInfo)
    {
        itemName = itemInfo.Item_Name;
        itemValue = itemInfo.Item_Value;
        itemKind = itemInfo.Item_Kind;
        itemImage = itemInfo.Item_Img;


        Debug.Log($"Initialized item: {itemName}, Value: {itemValue}");
    }
}
