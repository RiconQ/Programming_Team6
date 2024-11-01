using UnityEditor;
using UnityEngine;

/*
[CustomEditor(typeof(ExcelToJson))]
public class ExcelToJsonConverterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ExcelToJson converter = (ExcelToJson)target;
        GUILayout.Label("Excel File Path", EditorStyles.boldLabel);

        if (GUILayout.Button("Select Excel File Path"))
        {
            string path = EditorUtility.OpenFilePanel("Select Excel File", "", "xlsx");
            if (!string.IsNullOrEmpty(path))
            {
                converter.excelFilePath = path;
            }
        }

        EditorGUILayout.TextField("Excel File Path", converter.excelFilePath);

        GUILayout.Label("Json Output Path", EditorStyles.boldLabel);
        if(GUILayout.Button("Select Json Output Path"))
        {
            string path = EditorUtility.SaveFilePanel("Select Json Output Path", "", "output.json", "json");
            if(!string.IsNullOrEmpty(path))
            {
                converter.jsonOutputPath = path;
            }
        }

        EditorGUILayout.TextField("Json File Path", converter.jsonOutputPath);

        if(GUILayout.Button("Convert Excel To Json"))
        {
            converter.ConvertExcelToJson();
        }

        EditorUtility.SetDirty(converter);
    }
}
*/
