using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Data : MonoBehaviour
{
    public NGUIAtlas rewardAtlas;  // NGUI 아틀라스
    private UISprite itemSprite; // NGUI Sprite 컴포넌트

    public string itemName;  // 아이템 이름
    public int itemValue;    // 아이템 값
    public int itemKind;     // 아이템 종류
    public string itemImage; // 아이템 이미지 이름

    // 아이템 초기화
    public void Initialize(ItemInfo itemInfo)
    {
       itemName = itemInfo.Item_Name;
       itemValue = itemInfo.Item_Value;
       itemKind = itemInfo.Item_Kind;
        itemImage = itemInfo.Item_Img; // 예: "YellowColor 2"

        // UISprite 컴포넌트를 찾아 설정
        itemSprite = GetComponent<UISprite>();
        if (itemSprite != null)
        {
            itemSprite.atlas = rewardAtlas; // 아틀라스 설정
            itemSprite.spriteName = itemImage; // 이미지 이름 설정

          
        }
        else
        {
            Debug.LogError("아이템 sprite 없음");
        }
    }
}
