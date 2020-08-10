using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUiController : MonoBehaviour {
    [SerializeField] Text _playerNumberText;
    [SerializeField] Text _pingText;
    [SerializeField] GameObject _spectatorPanel; 


    public void UpdatePlayerText(string newPlayerName) {
        _playerNumberText.text = newPlayerName + "\n \\/";
    }
    
    public void UpdatePing(int newValue) {
        Debug.Log("new ping : " + newValue);
        _pingText.text = newValue.ToString() + "ms";
    }

    public void SetSpectator() {
        _spectatorPanel.SetActive(true);
    }
}
