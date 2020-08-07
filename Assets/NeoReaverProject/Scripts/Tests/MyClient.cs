using ExitGames.Client.Photon;
using Packages.EZRollback.Runtime.Scripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class MyClient : MonoBehaviour {
    
    Logic _logic;

    const string ServerAdress = "";
    // string _appId = "6bf6b4e7-b37c-40e2-8548-11f41fbf3ae0";

    [SerializeField] GameObject _connectionPanel;
    [SerializeField] GameObject _startGamePanel;

    [SerializeField] Text _appIdInput;
    [SerializeField] Text _gameVersionInput;
    [SerializeField] Text _nickNameInput;

    private NetworkTimer inputRepeatTimer;
    
    // Start is called before the first frame update
    void Start() {
        _startGamePanel.SetActive(false);
        inputRepeatTimer = new NetworkTimer(10);
    }

    public void ConnectToServer() {
        _logic = new Logic();
        _logic.ConnectToMaster(
            ServerAdress,
            _appIdInput.text.ToString(), 
            _gameVersionInput.text.ToString(),
            _nickNameInput.text.ToString()
            );

        _logic.localPlayer.OnEventJoin += this.OnEventJoin;
        _logic.localPlayer.OnEventLeave += this.OnEventLeave;
        _logic.localPlayer.StateChanged += this.OnStateChanged;
        _logic.localPlayer.OpResponseReceived += this.OnOperationResponse;
        _logic.localPlayer.EventReceived += this.EventReceived;


        _connectionPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_logic != null) {
            _logic.UpdateLocal();

            if (_logic.LocalPlayerJoined())
            {
                RenderPlayers();
            }
        }
    }

    void OnGUI() {
        if (_logic != null){
            GUI.Label(new Rect(10, 10, 300, 30), this._logic.localPlayer.State.ToString());
            GUI.Label(new Rect(10, 40, 300, 30), "Number of players : " + this._logic.playerObjects.Count.ToString());
        }
    }
    
    /// <summary>
    /// Convert integer value to Color
    /// </summary>
    public static Color IntToColor(int colorValue)
    {
        float r = (byte)(colorValue >> 16) / 255.0f;
        float g = (byte)(colorValue >> 8) / 255.0f;
        float b = (byte)colorValue / 255.0f;

        return new Color(r, g, b);
    }

    public void StartGame() {
        _logic.StartGame();
    }

    /// <summary>
    /// Render cubes onto the scene
    /// </summary>
    void RenderPlayers()
    {
        lock (_logic.localPlayer)
        {
            foreach (CustomPlayer p in _logic.localPlayer.LocalRoom.Players.Values)
            {
                foreach (GameObject cube in _logic.playerObjects)
                {
                    if (cube.name == p.NickName)
                    {
                        //cube.transform.position = new Vector3(p.PosX / 10f, p.PosY/ 10f, 0);
                        break;
                    }
                }
            }
        }
    }

    private void OnStateChanged(ClientState fromState, ClientState toState) {
        switch (toState) {
            case ClientState.ConnectedToMasterServer:
                //Show room panel
                //this._logic.LocalPlayer.OpJoinRandomRoom();
                //_lobbyPanel.SetActive(true);
                break;
            case ClientState.JoinedLobby:
                
                break;
        }
    }

    public void OnOperationResponse(OperationResponse operationResponse) {
        switch (operationResponse.OperationCode) {
            case OperationCode.JoinRandomGame:
            case OperationCode.JoinGame:
                break;
        }
    }

    private void EventReceived(EventData obj) {
        if (obj.Code == CustomConstants.EvStartGame) {
            Debug.Log("Revieced : EVSETUPDONE");
            
            _startGamePanel.SetActive(false);
            
            GameManager.Instance.StartGame();
        }
    }

    private void OnEventJoin(CustomPlayer customPlayer) {
        if (_logic.GetLocalPlayerId() == 0) {
            _startGamePanel.SetActive(true);
        }
    }
    private void OnEventLeave(CustomPlayer customPlayer) {
        if (_logic.GetLocalPlayerId() == 0) {
            _startGamePanel.SetActive(true);
        }
    }
}
