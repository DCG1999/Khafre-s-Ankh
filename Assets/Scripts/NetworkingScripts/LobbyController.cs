using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyController : MonoBehaviourPunCallbacks
{
    public InputField teamName_IF;
    private string teamName;
    public InputField hostPlayerName_IF;
    private string hostPlayerName;


    public InputField clientPlayerName_IF;
    private string clientPlayerName;
    public InputField roomCode_IF;

    PlayerInfo playerInfo;
    private void Start()
    {
        playerInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
    }

    public void CreateRoom()
    {
        teamName = teamName_IF.text;
        hostPlayerName = hostPlayerName_IF.text;

        int roomCode = Random.Range(1000, 9999);
        PhotonNetwork.NickName = hostPlayerName;
        playerInfo.currentTeamName = teamName;
        PhotonNetwork.CreateRoom(roomCode.ToString(), new RoomOptions { MaxPlayers = 4 });
      
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room... trying again");
        CreateRoom();
    }

    public void JoinRoom()
    {
        clientPlayerName = clientPlayerName_IF.text;

        string roomCode = roomCode_IF.text;
        PhotonNetwork.NickName = clientPlayerName;

        PhotonNetwork.JoinRoom(roomCode);


    }

    public override void OnJoinedRoom()
    {
        GameObject.FindObjectOfType<NavScript>().SyncedLoadLevel("CharSelectionScreen");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("WrongCode");
    }
}
