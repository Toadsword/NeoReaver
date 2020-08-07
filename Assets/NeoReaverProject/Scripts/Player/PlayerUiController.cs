using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUiController : MonoBehaviour {
    [SerializeField] Text _playerNumberText;
    [SerializeField] Text _pingText;


    public void UpdatePlayerText(string newPlayerName) {
        _playerNumberText.text = newPlayerName + "\n \\/";
    }
    
    public void UpdatePing(int newValue) {
        _pingText.text = "Ping : " + newValue.ToString() + "ms";
    }
}
