using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NavScript : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "End" || SceneManager.GetActiveScene().name == "CharSelectionScreen" || SceneManager.GetActiveScene().name == "GameLoseScreen") 
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                LoadMainMenu();
            }
        }

        if(SceneManager.GetActiveScene().name == "GameScene")
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                GameObject.FindObjectOfType<HUDScript>().DisplayPauseMenu();
            }
        }
    }

    public void LoadMainMenu()
    {
        PhotonNetwork.Disconnect();
    }

    public void QuitGame() => Application.Quit();



    public void SyncedLoadLevel(string levelName)
    {
        PhotonNetwork.LoadLevel(levelName);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(cause);
        PlayerInfo playerInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
        playerInfo.playerInfoList.Clear();
        playerInfo.currentTeamName = null;
        SceneManager.LoadScene("MenuScreen");
    }

}
