using UnityEditor;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
[CustomEditor(typeof(FruitDataImporter))]
public class FruitDataEditor : Editor
{
    private string excelFilePath;
    private string jsonFilePath;
    private string excelToJsonPath;

    public int sheetNum;

    public override void OnInspectorGUI()
    {
        FruitDataImporter fruitData = (FruitDataImporter)target;

        DrawDefaultInspector();

        //if(GUILayout.Button("Select Excel File"))
        //{
        //    excelFilePath = EditorUtility.OpenFilePanel("Select Excel File", "", "xlsx");
        //    excelToJsonPath = Path.ChangeExtension(excelFilePath, "json");
        //    jsonFilePath = string.Empty;
        //}
        //sheetNum = EditorGUILayout.IntField("Enter Sheet Number", sheetNum);
        //
        //EditorGUILayout.TextField("Excel File Path", excelFilePath);
        //
        GUILayout.Space(15);

        if (GUILayout.Button("Select Json File"))
        {
            jsonFilePath = EditorUtility.OpenFilePanel("Select Json File", "", "json");
            excelFilePath = string.Empty;
        }
        EditorGUILayout.TextField("Json File Path", jsonFilePath);

        GUILayout.Space(15);
        
        if (GUILayout.Button("Load Json and Update Attribute"))
        {
            if (!string.IsNullOrEmpty(jsonFilePath))
            {
                fruitData.UpdateFruitAttribute(jsonFilePath);
                EditorUtility.SetDirty(fruitData);
                Debug.Log("Update Attribute");
            }
            //else if(!string.IsNullOrEmpty(excelFilePath))
            //{
            //    ExcelToJson converter = new ExcelToJson
            //    {
            //        excelFilePath = excelFilePath,
            //        jsonOutputPath = excelToJsonPath
            //    };
            //    converter.ConvertExcelToJson(sheetNum);
            //
            //    fruitData.UpdateFruitAttribute(excelToJsonPath);
            //}
            else
            {
                Debug.LogError("Have to select Json file");
            }
        }
        
        GUILayout.Space(10);
    }
}
#endif