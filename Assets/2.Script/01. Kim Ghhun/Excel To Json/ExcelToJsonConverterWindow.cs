using UnityEditor;
using UnityEngine;
using System.IO;

public class ExcelToJsonConverterWindow : EditorWindow
{
    private string excelFilePath; // ���� ���� ���
    private string jsonOutputPath; // JSON ��� ���

    // â�� ���� ���� �޴� �׸� �߰�
    [MenuItem("Tools/Excel to JSON Converter")]
    public static void ShowWindow()
    {
        ExcelToJsonConverterWindow window = GetWindow<ExcelToJsonConverterWindow>("Excel to JSON Converter");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Excel File Path", EditorStyles.boldLabel);

        if (GUILayout.Button("Select Excel File"))
        {
            string path = EditorUtility.OpenFilePanel("Select Excel File", "", "xlsx");
            if (!string.IsNullOrEmpty(path))
            {
                excelFilePath = path;
            }
        }

        EditorGUILayout.TextField("Excel File Path", excelFilePath);

        GUILayout.Space(10);

        GUILayout.Label("Json Output Path", EditorStyles.boldLabel);

        string outputFileName = Path.GetFileNameWithoutExtension(excelFilePath);
        if (GUILayout.Button("Select Json Output Path"))
        {
            string path = EditorUtility.SaveFilePanel("Select Json Output Path", "", outputFileName , "json");
            if (!string.IsNullOrEmpty(path))
            {
                jsonOutputPath = path;
            }
        }

        EditorGUILayout.TextField("Json File Path", jsonOutputPath);

        GUILayout.Space(10);

        if (GUILayout.Button("Convert Excel To Json"))
        {
            // ExcelToJson Ŭ������ �ν��Ͻ��� �����Ͽ� ��ȯ �޼��� ȣ��
            ExcelToJson converter = new ExcelToJson
            {
                excelFilePath = excelFilePath,
                jsonOutputPath = jsonOutputPath
            };

            // ��ȯ �޼��� ȣ��
            converter.ConvertExcelToJson();
        }
    }
}