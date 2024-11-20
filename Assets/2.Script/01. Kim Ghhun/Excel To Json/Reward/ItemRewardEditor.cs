using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
[CustomEditor(typeof(ItemReward))]
public class ItemRewardEditor : Editor
{
    private string excelFilePath;

    private int rewardTableSheet;
    //private int rewardInfoSheet;
    private int itemInfoSheet;

    public override void OnInspectorGUI()
    {
        ItemReward itemReward = (ItemReward)target;

        DrawDefaultInspector();

        if(GUILayout.Button("Select Excel File"))
        {
            excelFilePath = EditorUtility.OpenFilePanel("Select Excel File", "", "xlsx");
        }
        rewardTableSheet = EditorGUILayout.IntField("Enter Reward Table Sheet", rewardTableSheet);
        //rewardInfoSheet = EditorGUILayout.IntField("Enter Reward Info Sheet", rewardInfoSheet);
        itemInfoSheet = EditorGUILayout.IntField("Enter Item Info Sheet", itemInfoSheet);

        EditorGUILayout.TextField("Excel File Path", excelFilePath);

        GUILayout.Space(20);
        if (GUILayout.Button("Load File and Update Reward"))
        {
            if (!string.IsNullOrEmpty(excelFilePath))
            {
                ExcelToJson converter = new ExcelToJson();
                converter.excelFilePath = excelFilePath;

                converter.jsonOutputPath = Path.ChangeExtension(excelFilePath + $"_Sheet_{rewardTableSheet}", "json");
                converter.ConvertExcelToJson(rewardTableSheet);
                itemReward.rewardTableJson = converter.jsonOutputPath;
                itemReward.SetRewardTable();

                ///converter.jsonOutputPath = Path.ChangeExtension(excelFilePath + $"_Sheet_{rewardInfoSheet}", "json");
                ///converter.ConvertExcelToJson(rewardInfoSheet);
                ///itemReward.rewardInfoJson = converter.jsonOutputPath;
                ///itemReward.SetRewardInfo();

                converter.jsonOutputPath = Path.ChangeExtension(excelFilePath + $"_Sheet_{itemInfoSheet}", "json");
                converter.ConvertExcelToJson(itemInfoSheet);
                itemReward.itemInfoJson = converter.jsonOutputPath;
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