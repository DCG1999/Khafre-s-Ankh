using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class HUDScript : MonoBehaviour
{
    public GameObject AbilityIndicator;
    public GameObject PauseMenu;
    
    public TextMeshProUGUI abilityTimer;

    public TextMeshProUGUI scoreText;

    ScoreManager scoreManager;
  //  PlayerScript playerScript;

   // PhotonView pv;

    void Start()
    {
        scoreText.text = "0";
        PauseMenu.SetActive(false); 
        AbilityIndicator.SetActive(true);
        abilityTimer.gameObject.SetActive(false);
        scoreManager = GameObject.FindObjectOfType<ScoreManager>(); 
    }

    public IEnumerator StartCountdown(float _countdownTimer)
    {
        abilityTimer.gameObject.SetActive(true);
        AbilityIndicator.SetActive(false);  
        while (_countdownTimer > 0)
        {
            abilityTimer.text = _countdownTimer.ToString();
            _countdownTimer--;
            yield return new WaitForSeconds(1f);
        }
        AbilityIndicator.SetActive(true);
        abilityTimer.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
    }

    public void DisplayPauseMenu()
    {
        PauseMenu.SetActive(true);
    }


    public void DisplayUpdatedScore()
    {
        int score = GameObject.FindObjectOfType<ScoreManager>().GetTeamScore();
        scoreText.text = score.ToString();  
    }





}
