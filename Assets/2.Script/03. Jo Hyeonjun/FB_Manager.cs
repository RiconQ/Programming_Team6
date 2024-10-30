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
        // Firebase Database에 대한 참조를 저장
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
        // 새로운 키를 생성하여 고유한 노드를 만들기 위해 Push() 메서드를 사용
        string key = dbReference.Child("scores").Push().Key;

        // 새로운 점수 객체 생성
        Score newScore = new Score(name, score);

        // 점수 객체를 JSON 형식으로 변환
        string json = JsonUtility.ToJson(newScore);

        // DataBase에 JSON 값을 설정
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

    // 점수 정보를 담기 위한 클래스
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
