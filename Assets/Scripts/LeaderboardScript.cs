using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardScript : MonoBehaviour
{

    private string LeaderBoardData;

    int id = 0;
    string[] scores = new string[10];
    string[] teamNames = new string[10];

    ScoreManager scoreManager;

    public GameObject LeaderDataPanel;
    public GameObject LeaderDataContainer;

    List<GameObject> leaderDataPanelList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();
    }

    public void ProcessLeaderBoardData()
    {
        scoreManager.RequestScoresFromDB();
    }

    public void ParseData()
    {
        LeaderBoardData = scoreManager.requestedData;
        string[] rows = LeaderBoardData.Split(char.Parse("#"));

        for (id =0; id<rows.Length-1;id++)
        {
            teamNames[id] = rows[id].Split(","[0])[0];
            scores[id] = rows[id].Split(","[0])[1];      
        }
        CreateLeaderBoard();
    }

    void CreateLeaderBoard()
    {
        for(int i=0; i<teamNames.Length;i++)
        {
            GameObject playerData = GameObject.Instantiate(LeaderDataPanel);
            leaderDataPanelList.Add(playerData);    
            playerData.transform.SetParent(LeaderDataContainer.transform);

            playerData.transform.Find("Rank").GetComponent<Text>().text = (i+1).ToString();
            playerData.transform.Find("Name").GetComponent<Text>().text = teamNames[i];
            playerData.transform.Find("Score").GetComponent<Text>().text = scores[i];
        }
    }

    public void DestroyLeaderBoard()
    {
        foreach (GameObject g in leaderDataPanelList) { Destroy(g); }
        leaderDataPanelList.Clear();
    }
}
