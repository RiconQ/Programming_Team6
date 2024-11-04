using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FruitData))]
public class FruitDataEditor : Editor
{
    private string jsonFilePath;

    public override void OnInspectorGUI()
    {
        FruitData fruitData = (FruitData)target;

        DrawDefaultInspector();

        if(GUILayout.Button("Select Json File"))
        {
            jsonFilePath = EditorUtility.OpenFilePanel("Select Json File", "", "json");
        }
        EditorGUILayout.TextField("Json File Path", jsonFilePath);

        GUILayout.Space(10);
        
        if (GUILayout.Button("Load Json and Update Attribute"))
        {
            if (!string.IsNullOrEmpty(jsonFilePath))
            {
                fruitData.UpdateFruitAttribute(jsonFilePath);
                EditorUtility.SetDirty(fruitData);
                Debug.Log("Update Attribute");
            }
            else
            {
                Debug.LogError("Have to select Json file");
            }
        }
        
        GUILayout.Space(10);
    }
}
