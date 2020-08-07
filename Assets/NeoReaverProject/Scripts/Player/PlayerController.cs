using System.Runtime.CompilerServices;
using System.Xml.Schema;
using ExitGames.Client.Photon;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace NeoReaverProject.Scripts {

public class PlayerController : RollbackBehaviour {
    
    public bool isLocal;
    [SerializeField] public int _playerId = -1;
    [SerializeField] bool registerPlayer = false;
    

    [SerializeField] Transform _shootPosition;
    [SerializeField] Transform _spriteTransform;
    [SerializeField] float _projectileSpeed = 1.5f;

    //Player shoo parameters
    [SerializeField] float _timeBetweenShootsTick = 0.2f;
    private Timer _timerBetweenShoots;

    [SerializeField] PlayerUiController _playerUiController;
    
    PlayerMovement _playerMovement;

    public PlayerUiController GetPlayerUiController() {
        return _playerUiController;
    }

    // Start is called before the first frame update
    void Start() {
        if (registerPlayer) {
            _playerId = RollbackManager.rbInputManager.AddPlayer();
        }
        
        _playerMovement = GetComponent<PlayerMovement>();
        InScreenManager._instance.RegisterObject(gameObject);
        
        _timerBetweenShoots = new Timer(_timeBetweenShootsTick);
        _timerBetweenShoots.Reset();
    }

    // Update is called once per frame
    void Update() {
        if (_playerId == -1) {
            return;
        }
        Vector2 newDirection;
        newDirection.x = RollbackManager.rbInputManager.GetAxis(RollbackInputManager.AxisEnum.HORIZONTAL, _playerId);
        newDirection.y = RollbackManager.rbInputManager.GetAxis(RollbackInputManager.AxisEnum.VERTICAL, _playerId);
        _playerMovement.SetDirection(newDirection);
    }

    public void SetupPlayer(int playerId, string playerName) {
        _playerId = playerId;
        GetPlayerUiController().UpdatePlayerText(playerName);
    }

    public Transform GetRotationTransform() {
        return _spriteTransform;
    }
    
    public void SetupLocal(bool isLocal) {
        _spriteTransform.GetComponent<SpriteRenderer>().color = isLocal ? Color.green : Color.white;
    }
    
    public override void Simulate() {
        if (_playerId == -1) {
            return;
        }
        
        Vector2 newDirection;
        newDirection.x = RollbackManager.rbInputManager.GetAxis(RollbackInputManager.AxisEnum.HORIZONTAL, _playerId);
        newDirection.y = RollbackManager.rbInputManager.GetAxis(RollbackInputManager.AxisEnum.VERTICAL, _playerId);
        _playerMovement.SetDirection(newDirection);
        
        _timerBetweenShoots.Simulate();
        if (_timerBetweenShoots.ShouldExecute()) {
            Debug.Log("RollbackManager.rbInputManager.GetInputDown(" + (int)CustomInputManager.ActionsCode.SHOOT + ",  " + _playerId + ")");
            if (RollbackManager.rbInputManager.GetInputDown((int)CustomInputManager.ActionsCode.SHOOT, _playerId)) {
                ProjectileManager.Instance.poolManager.CreateObject(_shootPosition.position, _playerMovement.spriteTransform.rotation, _projectileSpeed);
                _timerBetweenShoots.Reset();
            }
        }
    }

    public override void SetValueFromFrameNumber(int frameNumber) {
        _timerBetweenShoots.SetValueFromFrameNumber(frameNumber);
    }

    public override void DeleteFrames(int numFramesToDelete, RollbackManager.DeleteFrameMode deleteMode) {
        _timerBetweenShoots.DeleteFrames(numFramesToDelete, deleteMode);
    }

    public override void SaveFrame() {
        _timerBetweenShoots.SaveFrame();
    }
}
}
