using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUiController : MonoBehaviour {
    [SerializeField] Text playerNumberText;
    [SerializeField] Text pingText;


    public void UpdatePlayerText(string newPlayerName) {
        playerNumberText.text = newPlayerName + "\n \\/";
    }
    
    public void UpdatePing(int newValue) {
        pingText.text = "Ping : " + newValue.ToString() + "ms";
    }
}
