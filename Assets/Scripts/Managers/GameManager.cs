using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviour
{
    public GameObject[] SpawnPoints;
    GameObject[] players;
    // Start is called before the first frame update
    void Awake()
    {
        PhotonNetwork.Instantiate("PlayerSpawner", new Vector3(0, 0, 0), Quaternion.identity, 0);
    }

    public void PutPlayersOnSpawn()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        print(players[0].name);
        foreach(GameObject sp in SpawnPoints)
        {
            foreach(GameObject p in players)
            {
                if (p.name + "SP" == sp.name)
                {
                    print("putting player");
                    p.transform.position = new Vector3(sp.transform.position.x, 0.0024f, sp.transform.position.z);
                    p.transform.rotation = sp.transform.rotation;
                    break;
                }
            }
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            PhotonNetwork.LoadLevel("End");
        }
    }
    public IEnumerator CheckStunnedPlayers()
    {
        yield return new WaitForSeconds(3f);
        int playersStunned = 0;

        PlayerScript[] pLayerScripts = GameObject.FindObjectsOfType<PlayerScript>();

        foreach (PlayerScript player in pLayerScripts)
        {
            if (player.isStunned)
            {
                playersStunned++;
            }
        }

        if (playersStunned == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            PhotonNetwork.LoadLevel("GameLoseScreen");
        }

    }

}
