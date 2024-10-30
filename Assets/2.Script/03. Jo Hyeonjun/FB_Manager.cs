using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class FB_Manager : MonoBehaviour
{
    public InputField IDInput, ScoreInput;
    public TMP_Text recentT, bestT;
    private DatabaseReference dbReference;
    private void Start()
    {
        // Firebase Database�� ���� ������ ����
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void SubmitScore()
    {
        string Name = IDInput.text;
        if (int.TryParse(ScoreInput.text, out int score))
        {
            WriteNewScore(Name, score);
        }
        else
        {
            Debug.Log("Invalid score input.");
        }
    }
    private void WriteNewScore(string name, int score)
    {
        // ���ο� Ű�� �����Ͽ� ������ ��带 ����� ���� Push() �޼��带 ���
        string key = dbReference.Child("scores").Push().Key;

        // ���ο� ���� ��ü ����
        Score newScore = new Score(name, score);

        // ���� ��ü�� JSON �������� ��ȯ
        string json = JsonUtility.ToJson(newScore);

        // DataBase�� JSON ���� ����
        dbReference.Child("scores").Child(key).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Score submitted successfully!");
                recentT.text = "";
            }
            else
            {
                Debug.Log("Failed to submit score.");
            }
        });
    }

    // ���� ������ ��� ���� Ŭ����
    [System.Serializable]
    public class Score
    {
        public string name; 
        public int score; 

        public Score(string name, int score)
        {
            this.name = name;
            this.score = score;
        }
    }
}
