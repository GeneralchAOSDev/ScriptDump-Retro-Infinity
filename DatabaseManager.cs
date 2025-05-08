using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Text;
[Serializable] public class ScoreEntity{
    public int Rank;
    public string Name;
    public int Score;
}

[Serializable]
public class scorelistWrapper{
    public ScoreEntity[] scores;
}

public class DatabaseManager : MonoBehaviour
{
    public ScoreEntity[] ScoreList;
    public ScoreEntity ScoreEnt;
    public string ServerKey;
    private string currentJSON;
    [SerializeField] TMP_Text newName;
    [SerializeField] GameObject scoreEntry;
    [SerializeField] Transform scoreContainer; 
    public Color oddColor = new Color(0.1f, 1f, 0.1f, 1f); // CRT green
    public Color evenColor = new Color(0.1f, 1f, 0.1f, 1f); // CRT green

    private void Awake(){
        //dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        if(scoreEntry != null )
            StartCoroutine(LoadData());
    }

    public void SaveJsonWithBackup(string json, string mainKey = "HighScores")
    {
        // Save main
        PlayerPrefs.SetString(mainKey, json);
    
        // Save backup
        string backupKey = mainKey + "_backup";
        PlayerPrefs.SetString(backupKey, json);
    
        PlayerPrefs.Save(); // Force save to disk
    }

    public string RestoreJsonBackup(string mainKey = "HighScores")
    {
        string backupKey = mainKey + "_backup";
    
        if (PlayerPrefs.HasKey(backupKey))
        {
            string backupJson = PlayerPrefs.GetString(backupKey);
            PlayerPrefs.SetString(mainKey, backupJson); // Restore
            PlayerPrefs.Save();
            return backupJson;
        }

        Debug.LogWarning("No backup found for " + mainKey);
        return null;
    }

    public void UpdateData(){
        
        StartCoroutine(PushData());
    }

   IEnumerator PushData()
    {
        Debug.Log("Pushing Data");

        yield return StartCoroutine(LoadData());

        List<ScoreEntity> currentScores = new List<ScoreEntity>();

        if (!string.IsNullOrEmpty(currentJSON))
        {
            try
            {
                scorelistWrapper wrap = JsonUtility.FromJson<scorelistWrapper>(currentJSON);
                if (wrap != null && wrap.scores != null)
                    currentScores.AddRange(wrap.scores);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to parse existing JSON: " + e);
            }
        }

        string tempName = newName.text;
        if (tempName.Length > 17)
            tempName = tempName.Substring(0, 17);

        currentScores.Add(new ScoreEntity { Name = tempName, Score = (int)GridScript.score });

        currentScores.Sort((a, b) => b.Score.CompareTo(a.Score));

        for (int i = 0; i < currentScores.Count; i++)
            currentScores[i].Rank = i + 1;

        scorelistWrapper updatedWrapper = new scorelistWrapper
        {
            scores = currentScores.ToArray()
        };

        string newJson = JsonUtility.ToJson(updatedWrapper, true);
        Debug.Log("Final JSON to upload: " + newJson);

        string url = $"https://retroinfinityscoreboard-default-rtdb.firebaseio.com/scores/{ServerKey}.json";

        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(newJson);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log("Upload succeeded");
        else
            Debug.LogError("Upload failed: " + request.error);

        GridScript.difficulty = 3;
        SceneManager.LoadScene("mainMenu");
    }

    public void GetData(){
       
        StartCoroutine(LoadData());
    }
    IEnumerator LoadData()
    {
        string url = $"https://retroinfinityscoreboard-default-rtdb.firebaseio.com/scores/{ServerKey}.json";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.timeout = 3;
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonData = request.downloadHandler.text;
            Debug.Log("Data Retrieved: " + jsonData);
            SaveJsonWithBackup(jsonData);
            currentJSON = jsonData;
        }
        else
        {
            Debug.LogError("Failed to retrieve data, using backup.");
            currentJSON = RestoreJsonBackup();
        }

        scorelistWrapper wrap = JsonUtility.FromJson<scorelistWrapper>(currentJSON);

        if (scoreEntry != null)
        {
            ScoreEntity[] scoreList = wrap.scores;

            for (int i = 0; i < scoreList.Length; i++)
            {
                Vector3 spawnPos = scoreEntry.transform.position;
                spawnPos.y -= i * .25f;
                var lastScore = Instantiate(scoreEntry, spawnPos, Quaternion.identity, scoreContainer);
                Color myColor;
                if(i % 2 == 0)
                     myColor = oddColor;
                else
                    myColor = evenColor;
                // Set text values
                var tempRank = lastScore.transform.Find("Rank");
                if (tempRank != null)
                {
                    var rankText = tempRank.GetComponent<TMP_Text>();
                    if (rankText != null)
                        rankText.text = $"{scoreList[i].Rank}";

                    var rankGlow = tempRank.GetComponent<RetroGlo>();
                    if (rankGlow != null)
                        rankGlow.baseColor = myColor;
                }

                var tempName = lastScore.transform.Find("Name");
                if (tempName != null)
                {
                    var nameText = tempName.GetComponent<TMP_Text>();
                    if (nameText != null)
                        nameText.text = $"{scoreList[i].Name}";

                    var nameGlow = tempName.GetComponent<RetroGlo>();
                    if (nameGlow != null)
                        nameGlow.baseColor = myColor;
                }

                var tempScore = lastScore.transform.Find("Score");
                if (tempScore != null)
                {
                    var scoreText = tempScore.GetComponent<TMP_Text>();
                    if (scoreText != null)
                        scoreText.text = $"{scoreList[i].Score}";

                    var scoreGlow = tempScore.GetComponent<RetroGlo>();
                    if (scoreGlow != null)
                        scoreGlow.baseColor = myColor;
                }
            }


            scoreEntry.SetActive(false);
        }
    }

}
