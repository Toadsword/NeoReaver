using System.Runtime.CompilerServices;
using System.Xml.Schema;
using ExitGames.Client.Photon;
using Packages.EZRollback.Runtime.Scripts;
using UnityEditor;
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
    private RollbackTimer _rollbackTimerBetweenShoots;

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
        
        _rollbackTimerBetweenShoots = new RollbackTimer(_timeBetweenShootsTick);
        _rollbackTimerBetweenShoots.Reset();
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

    public void SetSpectator() {
        gameObject.SetActive(false);
        _playerUiController.SetSpectator();
    }
    
    public override void Simulate() {
        if (_playerId == -1) {
            return;
        }
        
        Vector2 newDirection;
        newDirection.x = RollbackManager.rbInputManager.GetAxis(RollbackInputManager.AxisEnum.HORIZONTAL, _playerId);
        newDirection.y = RollbackManager.rbInputManager.GetAxis(RollbackInputManager.AxisEnum.VERTICAL, _playerId);
        _playerMovement.SetDirection(newDirection);
        
        _rollbackTimerBetweenShoots.Simulate();
        if (_rollbackTimerBetweenShoots.ShouldExecute()) {
            if (RollbackManager.rbInputManager.GetInputDown((int)CustomInputManager.ActionsCode.SHOOT, _playerId)) {
                ProjectileManager.Instance.poolManager.CreateObject(_shootPosition.position, _playerMovement.spriteTransform.rotation, _projectileSpeed);
                _rollbackTimerBetweenShoots.Reset();
            }
        }
    }

    public override void SetValueFromFrameNumber(int frameNumber) {
        _rollbackTimerBetweenShoots.SetValueFromFrameNumber(frameNumber);
    }

    public override void DeleteFrames(int numFramesToDelete, RollbackManager.DeleteFrameMode deleteMode) {
        _rollbackTimerBetweenShoots.DeleteFrames(numFramesToDelete, deleteMode);
    }

    public override void SaveFrame() {
        _rollbackTimerBetweenShoots.SaveFrame();
    }
}
}
