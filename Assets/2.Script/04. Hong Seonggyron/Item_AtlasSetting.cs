using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_AtlasSetting : MonoBehaviour
{
    public NGUIAtlas rewardAtlas;  // NGUI ��Ʋ��
    private UISprite itemSprite; // NGUI Sprite ������Ʈ

    public string itemName;  // ������ �̸�
    public int itemValue;    // ������ ��
    public int itemKind;     // ������ ����
    public string itemImage; // ������ �̹��� �̸�

    // ������ �ʱ�ȭ
    public void Initialize(ItemInfo itemInfo)
    {
       itemName = itemInfo.Item_Name;
       itemValue = itemInfo.Item_Value;
       itemKind = itemInfo.Item_Kind;
        itemImage = itemInfo.Item_Img; // ��: "YellowColor 2"

        // UISprite ������Ʈ�� ã�� ����
        itemSprite = GetComponent<UISprite>();
        if (itemSprite != null)
        {
            itemSprite.atlas = rewardAtlas; // ��Ʋ�� ����
            itemSprite.spriteName = itemImage; // �̹��� �̸� ����

            // ������ �α�
            Debug.Log($"Initialized item sprite with Atlas: {rewardAtlas.name}, Image: {itemImage}");
        }
        else
        {
            Debug.LogError("UISprite component is missing on this object.");
        }
    }
}