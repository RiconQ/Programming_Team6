using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    public string itemName;
    public int itemValue;
    public int itemKind;
    public string itemImage;


    //������ ������Ʈ�� ����� ���� �ӽ� ��ũ��Ʈ
    public void Initialize(ItemInfo itemInfo)
    {
        itemName = itemInfo.Item_Name;
        itemValue = itemInfo.Item_Value;
        itemKind = itemInfo.Item_Kind;
        itemImage = itemInfo.Item_Img;


        Debug.Log($"Initialized item: {itemName}, Value: {itemValue}");
    }
}
