using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CharacterSelector : MonoBehaviour
{
    PhotonView pv;

    private GameObject characters;

    CharacterSelectionController characterSelectionController;
    PlayerInfo playerInfo;

    private void Awake()
    {           
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        characterSelectionController = GameObject.Find("CharSelectionController").GetComponent<CharacterSelectionController>();
        playerInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();

        characters = GameObject.Find("Characters");

        foreach (Transform character in characters.transform)
        {
            Button btn = character.gameObject.GetComponent<Button>();
            btn.onClick.AddListener(delegate { OnClick_CharacterSelect(btn); });
        }
    }

    public void OnClick_CharacterSelect(Button characterSelected)
    {
        if (characterSelectionController.allConnected)
        {
            if (pv.IsMine)
            {
                string Name = pv.Owner.NickName;
                string charName = characterSelected.name;
                pv.RPC("ChangeText", RpcTarget.AllBuffered, Name, charName);
            }
        }
    }

    [PunRPC]
    void ChangeText(string name, string character)
    {
        print(name + " selected " + character);
        characterSelectionController.DisplayPlayerNames(name, character);
        playerInfo.AddPlayerInfo(name, character);
    }

}
