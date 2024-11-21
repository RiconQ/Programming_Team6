using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using static RewardTable;
using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "ItemReward", menuName = "Reward/ItemReward")]
public class ItemReward : ScriptableObject
{
    public List<RewardTable> rewardDataTable = new List<RewardTable>();
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
            tempData.reward = new List<LevelRewardTable>();
            tempData.userLevel = (int)(double)jsonData[i][0];
            //Debug.Log(jsonData[i].Count);
            for (int j = 1; j < jsonData[i].Count; j++)
            {
                var tmpReward = new LevelRewardTable();
                //Debug.Log(jsonData[i][j].ToString());
                var matches = Regex.Matches(jsonData[i][j].ToString(), @"\[(\d+),\s*(\d+),\s*(\d+)\]");

                foreach (Match match in matches)
                {
                    var tmpStruct = new RewardInfo();
                    //tmpStruct.Add(int.Parse(match.Groups[1].Value));
                    //tmpStruct.Add(int.Parse(match.Groups[2].Value));
                    //tmpStruct.Add(int.Parse(match.Groups[3].Value));
                    tmpStruct.kind = int.Parse(match.Groups[1].Value);
                    tmpStruct.value = int.Parse(match.Groups[2].Value);
                    tmpStruct.amount = int.Parse(match.Groups[3].Value);
                    // Debug.Log(tmpStruct.kind + " " + tmpStruct.value + " " + tmpStruct.amount);
                    tmpReward.rewardInfos.Add(tmpStruct);
                }
                tempData.reward.Add(tmpReward);
                //debug
                //for (int k = 0; k < tempData.reward.Count; j++)
                //{
                //    foreach(var item in tempData.reward[k])
                //    {
                //        Debug.Log(item.kind + " " + item.value + " " + item.amount);
                //    }
                //}
            }
            rewardDataTable.Add(tempData);

            Debug.Log("test " + rewardDataTable[i].userLevel + " And " + rewardDataTable[i].reward.Count);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
        
        //for(int i = 0;i < jsonData.Count; i++)
        //{
        //    Debug.Log("test 2 " + rewardDataTable[i].userLevel + " And " + rewardDataTable[i].reward.Count);
        //    for(int j = 0; j < rewardDataTable[i].reward.Count; j++)
        //    {
        //        Debug.Log("Test 3 " + rewardDataTable[i].reward[j].Count);
        //    }
        //}
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
            tempData.Item_Value = (int)(double)jsonData[i][0];
            tempData.Item_Kind = (int)(double)jsonData[i][1];
            tempData.Item_Name = jsonData[i][2].ToString();
            tempData.Item_Img = jsonData[i][3].ToString();

            itemInfos.Add(tempData);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif

       // Check();
    }

    public void Check()
    {
        Debug.Log($"Check Start {rewardDataTable.Count} And {rewardDataTable[0].reward is null}");
        for (int i = 0; i < rewardDataTable.Count; i++)
        {
            Debug.Log("test 2 " + rewardDataTable[i].userLevel + " And " + rewardDataTable[i].reward.Count);
            for (int j = 0; j < rewardDataTable[i].reward.Count; j++)
            {
                Debug.Log("Test 3 " + rewardDataTable[i].reward.Count);
            }
        }
    }
}

/// <summary>
/// 보상 데이터 테이블
/// </summary>
[Serializable]
public class RewardTable
{
    public int userLevel;
    public List<LevelRewardTable> reward = new List<LevelRewardTable>();
}

[Serializable]
public class LevelRewardTable
{
    public List<RewardInfo> rewardInfos = new List<RewardInfo>();
}

[Serializable]
public class RewardInfo
{
    public int kind;
    public int value;
    public int amount;
}
/// <summary>
/// 아이템 테이블(임의제작)
/// </summary>
[Serializable]
public class ItemInfo
{
    public int Item_Kind;
    public int Item_Value;
    public string Item_Name;
    public string Item_Img;
}
