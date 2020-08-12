using Packages.EZRollback.Runtime.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : Singleton<GameUIManager>
{
    public enum GameUIState {
        EMPTY,
        LOCAL_LOBBY,
        GAME_LOBBY,
        COUNTDOWN,
        INGAME
    }

    GameUIState _currentState = GameUIState.EMPTY;
    
    [SerializeField] float _startGameDelay = 3.0f;
    
    [Header("Panels")]
    [SerializeField] GameObject _connectionPanel;
    [SerializeField] GameObject _startGameMenuPanel;
    [SerializeField] GameObject _countdownPanel;

    [Header("Input")]
    [SerializeField] public Text _appIdInput;
    [SerializeField] public Text _gameVersionInput;
    [SerializeField] public Text _nickNameInput;
    
    [Header("Others")]
    [SerializeField] Text _countdownText;
    
    Timer _countdownTimer;
    bool isCountingDown = false;
    
    // Start is called before the first frame update
    void Start() {
        isCountingDown = false;
        _connectionPanel.SetActive(false);
        _startGameMenuPanel.SetActive(false);
        _countdownPanel.SetActive(false);
        
        ChangeUIState(GameUIState.LOCAL_LOBBY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCountdown(float timeToRemoveFromCountdown) {
        ChangeUIState(GameUIState.COUNTDOWN);
        
        _countdownTimer = new Timer(_startGameDelay - timeToRemoveFromCountdown);
    }

    public void ChangeUIState(GameUIState newState) {
        SetPanelActive(_currentState, false);
        SetPanelActive(newState, true);
        _currentState = newState;
    }

    private void SetPanelActive(GameUIState newState, bool newActive) {
        switch (newState) {
            case GameUIState.EMPTY:
                break;
            case GameUIState.GAME_LOBBY:
                _startGameMenuPanel.SetActive(newActive);
                break;
            case GameUIState.INGAME:
                break;
            case GameUIState.COUNTDOWN:
                _countdownPanel.SetActive(newActive);
                break;
            case GameUIState.LOCAL_LOBBY:
                _connectionPanel.SetActive(newActive);
                break;
        }
    }
}
