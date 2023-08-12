using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public Dictionary<string, string> playerInfoList = new Dictionary<string, string>();
    public string currentTeamName;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void AddPlayerInfo(string playerName, string characterName) => playerInfoList.Add(playerName, characterName);




}
