using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameEndManager : MonoBehaviour
{
    public TextMeshProUGUI lootText; 
    ScoreManager scoreManager;

    void Awake()
    {
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();

        if(PhotonNetwork.IsMasterClient)
            scoreManager.RegisterScoreToDB();
    }

    void Start()
    {
        lootText.text = "Loot : " + scoreManager.GetTeamScore().ToString();
    }

}
