using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    PhotonView pv;
    PlayerInfo playerInfo;
    PlayerScript character;

    [HideInInspector] public string characterName;
    void Awake()
    {
        pv = GetComponent<PhotonView>();
        playerInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
        SpawnPlayer();
        this.name = "PlayerSpawner";
    }

    void SpawnPlayer()
    {
        if (pv.IsMine)
        {
            GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            print(pv.Owner.NickName);
            characterName = playerInfo.playerInfoList[pv.Owner.NickName];
            //thisCharacter = characterName;
            PhotonNetwork.Instantiate(characterName, new Vector3(0, 0, 0), Quaternion.identity, 0);
            character = GameObject.FindObjectOfType<PlayerScript>();
            character.gameObject.name = characterName;
            gameManager.PutPlayersOnSpawn();

            GameObject camPrefab = Resources.Load("Camera") as GameObject;
            GameObject camera = Instantiate(camPrefab, Vector3.zero, Quaternion.identity);
            camera.gameObject.GetComponent<CameraFollowScript>().target_Player = character.transform;


            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(GameObject.Find("tempCam").gameObject);
            }

        }

     //   AssignCharacters();
    }
}
