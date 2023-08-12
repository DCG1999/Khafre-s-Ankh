using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class ScoreManager : MonoBehaviour
{
    int teamScore;
    PlayerInfo playerInfo;
    [SerializeField] int scoreIncrementValue;

    public string requestedData;
    void Awake()
    {
        DontDestroyOnLoad(this);
        playerInfo = GameObject.FindObjectOfType<PlayerInfo>();
        teamScore = 0;  
    }

    public int GetTeamScore()
    {
        return teamScore;
    }

    public void UpdateScore()
    {
        teamScore += scoreIncrementValue;
        GameObject.FindObjectOfType<HUDScript>().DisplayUpdatedScore();
        Debug.Log(teamScore);       
    }
    public void RegisterScoreToDB()
    {
        
        StartCoroutine(UploadScore());
    }

    public void RequestScoresFromDB()
    {

        StartCoroutine(RecieveScores());

    }
    IEnumerator UploadScore()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", playerInfo.currentTeamName);
        form.AddField("score", teamScore);


        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/SaveScore.php", form))
        {
            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form Upload Complete");
            }

        }
          
               
    }

    IEnumerator RecieveScores()
    {
        string uri = "http://localhost/sqlconnect/LoadScore.php";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if(webRequest.isNetworkError)
            {
                Debug.Log("Error : " + webRequest.error);
            }
            else
            {
                Debug.Log("Recieved : " + webRequest.downloadHandler.text);
                requestedData = webRequest.downloadHandler.text;
                GameObject.FindObjectOfType<LeaderboardScript>().ParseData();


            }
        }
    }
}
