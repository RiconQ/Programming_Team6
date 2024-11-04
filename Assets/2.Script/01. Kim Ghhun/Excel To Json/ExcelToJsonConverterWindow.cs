using UnityEditor;
using UnityEngine;
using System.IO;

public class ExcelToJsonConverterWindow : EditorWindow
{
    private string excelFilePath; // 엑셀 파일 경로
    private string jsonOutputPath; // JSON 출력 경로

    // 창을 열기 위한 메뉴 항목 추가
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
            // ExcelToJson 클래스의 인스턴스를 생성하여 변환 메서드 호출
            ExcelToJson converter = new ExcelToJson
            {
                excelFilePath = excelFilePath,
                jsonOutputPath = jsonOutputPath
            };

            // 변환 메서드 호출
            converter.ConvertExcelToJson();
        }
    }
}