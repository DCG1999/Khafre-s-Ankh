using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CharacterSelectionController : MonoBehaviourPunCallbacks
{ 
    public Text roomNumber_text;
    public Text playerNumber_text;

    public bool allConnected;

    private void Start()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 1)
        {
            allConnected = true;
            UpdatePlayerCount(true);
            Debug.Log("all connected");
        }
        else
            UpdatePlayerCount(false);


        if (PhotonNetwork.IsMasterClient)
        {
            GameObject.Find("StartGameBtn").GetComponent<Button>().interactable = true;
            UpdatePlayerCount(false);
        }
        roomNumber_text.text = PhotonNetwork.CurrentRoom.Name;
        PhotonNetwork.Instantiate("CharacterSelector", new Vector3(0, 0, 0), Quaternion.identity, 0);
        
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >=1)
        {
            allConnected = true;
            UpdatePlayerCount(true);
            Debug.Log("all connected");
        }
        else
            UpdatePlayerCount(false);
        ////  print(PhotonNetwork.CurrentRoom.PlayerCount);

    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player newPlayer)
    {
        allConnected = false;
        UpdatePlayerCount(false);
    }

    public void DisplayPlayerNames(string playerName, string characterName)
    {
        Button character_Btn = GameObject.Find(characterName).GetComponent<Button>();
        Text character_Text = character_Btn.GetComponentInChildren<Text>();
        character_Text.text = playerName;

        character_Btn.interactable = false;
    }
    
    private void UpdatePlayerCount(bool roomFull)
    {
        playerNumber_text.text = "Connected: " + PhotonNetwork.CurrentRoom.PlayerCount + "/4";
        if (roomFull)
            playerNumber_text.color = Color.green;
        else
            playerNumber_text.color = Color.red;
    }

    public void OnClick_LoadGame()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}
