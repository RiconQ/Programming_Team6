using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "ItemReward", menuName = "Reward/ItemReward")]
public class ItemReward : ScriptableObject
{
    public List<RewardTable> rewardDataTable = new List<RewardTable>();
    public List<RewardInfo> rewardInfos = new List<RewardInfo>();
    public List<ItemInfo> itemInfos = new List<ItemInfo>();

    [HideInInspector] public string rewardTableJson;
    [HideInInspector] public string rewardInfoJson;
    [HideInInspector] public string itemInfoJson;

    public void SetRewardTable()
    {
        rewardDataTable.Clear();
        if (!File.Exists(rewardTableJson))
        {
            Debug.LogError("File Not Found");
            return;
        }

        string jsonContent = File.ReadAllText(rewardTableJson);
        JsonData jsonData = JsonMapper.ToObject(jsonContent);

        for (int i = 0; i < jsonData.Count; i++)
        {
            var tempData = new RewardTable();
            tempData.ID = (int)(double)jsonData[i][0];
            tempData.Min_Lv = (int)(double)jsonData[i][1];
            tempData.Max_Lv = (int)(double)jsonData[i][2];
            tempData.Reward = new List<List<int>>();
            //Debug.Log(jsonData[i].Count);
            for (int j = 3; j < jsonData[i].Count; j++)
            {
                List<int> tempArray = new List<int>();
                tempArray =
                    jsonData[i][j].ToString()
                    .Split(',')
                    .Select(s => int.Parse(s.Trim()))
                    .ToList();
                tempData.Reward.Add(tempArray);
            }

            rewardDataTable.Add(tempData);
        }
    }

    public void SetRewardInfo()
    {
        rewardInfos.Clear();
        if (!File.Exists(rewardInfoJson))
        {
            Debug.LogError("File Not Found");
            return;
        }

        string jsonContent = File.ReadAllText(rewardInfoJson);
        JsonData jsonData = JsonMapper.ToObject(jsonContent);

        for (int i = 0; i < jsonData.Count; i++)
        {
            var tempData = new RewardInfo();
            tempData.ID = (int)(double)jsonData[i][0];
            tempData.Kind = (int)(double)jsonData[i][1];
            tempData.Key = (int)(double)jsonData[i][2];
            tempData.Amount = (int)(double)jsonData[i][3];

            rewardInfos.Add(tempData);
        }
    }

    public void SetItemInfo()
    {
        itemInfos.Clear();
        if (!File.Exists(itemInfoJson))
        {
            Debug.LogError("File Not Found");
            return;
        }

        string jsonContent = File.ReadAllText(itemInfoJson);
        JsonData jsonData = JsonMapper.ToObject(jsonContent);

        for (int i = 0; i < jsonData.Count; i++)
        {
            var tempData = new ItemInfo();
            tempData.ID = (int)(double)jsonData[i][0];
            tempData.Item_Kind = (int)(double)jsonData[i][1];
            tempData.Item_Name = jsonData[i][2].ToString();
            tempData.Item_Img = jsonData[i][3].ToString();

            itemInfos.Add(tempData);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
}

/// <summary>
/// 보상 데이터 테이블
/// </summary>
[Serializable]
public class RewardTable
{
    public int ID;
    public int Min_Lv;
    public int Max_Lv;
    public List<List<int>> Reward;
}

/// <summary>
/// 아이템 묶음 테이블
/// </summary>
[Serializable]
public class RewardInfo
{
    public int ID;
    public int Kind;
    public int Key;
    public int Amount;
}

/// <summary>
/// 아이템 테이블(임의제작)
/// </summary>
[Serializable]
public class ItemInfo
{
    public int ID;
    public int Item_Kind;
    public string Item_Name;
    public string Item_Img;
}
