using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
[CustomEditor(typeof(ItemReward))]
public class ItemRewardEditor : Editor
{
    //private string excelFilePath;
    private string rewardTableJson;
    private string itemInfoJson;

    private int rewardTableSheet;
    //private int rewardInfoSheet;
    private int itemInfoSheet;

    public override void OnInspectorGUI()
    {
        ItemReward itemReward = (ItemReward)target;

        DrawDefaultInspector();

        if(GUILayout.Button("Select Reward Table Json File"))
        {
            rewardTableJson = EditorUtility.OpenFilePanel("Select Reward Table File", "", "json");
        }
        //rewardTableSheet = EditorGUILayout.IntField("Enter Reward Table Sheet", rewardTableSheet);
        //rewardInfoSheet = EditorGUILayout.IntField("Enter Reward Info Sheet", rewardInfoSheet);
        //itemInfoSheet = EditorGUILayout.IntField("Enter Item Info Sheet", itemInfoSheet);

        if (GUILayout.Button("Select Item Info Json File"))
        {
            itemInfoJson = EditorUtility.OpenFilePanel("Select Item Info File", "", "json");
        }

        EditorGUILayout.TextField("Reward Table File Path", rewardTableJson);
        EditorGUILayout.TextField("Item Info File Path", itemInfoJson);

        GUILayout.Space(20);
        if (GUILayout.Button("Load File and Update Reward"))
        {
            if (!string.IsNullOrEmpty(rewardTableJson) &&!string.IsNullOrEmpty(itemInfoJson))
            {
                itemReward.rewardTableJson = this.rewardTableJson;
                itemReward.SetRewardTable();

                itemReward.itemInfoJson = this.itemInfoJson;
                itemReward.SetItemInfo();
            }
            else
            {
                Debug.LogError("Have to select file");
            }
        }
        GUILayout.Space(20);
    }
}
#endif