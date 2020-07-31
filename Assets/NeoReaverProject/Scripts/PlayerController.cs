using System.Runtime.CompilerServices;
using System.Xml.Schema;
using ExitGames.Client.Photon;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace NeoReaverProject.Scripts {

public class PlayerController : RollbackBehaviour {
    PoolManager _projectileManager;

    public bool isLocal;
    [SerializeField] public int _playerId = -1;

    [SerializeField] Transform _shootPosition;
    [SerializeField] float _projectileSpeed = 1.5f;

    //Player shoo parameters
    [SerializeField] float _timeBetweenShootsTick = 0.2f;
    private Timer _timerBetweenShoots;

    PlayerMovement _playerMovement;

    // Start is called before the first frame update
    void Start() {
        _timerBetweenShoots = new Timer(_timeBetweenShootsTick);
        
        _projectileManager = ProjectileManager.Instance.poolManager;
        _playerMovement = GetComponent<PlayerMovement>();
        InScreenManager._instance.RegisterObject(gameObject);
        
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

    public void SetupPlayer(int playerId) {
        _playerId = playerId;
    }

    public void SetupLocal(bool isLocal) {
        GetComponent<SpriteRenderer>().color = isLocal ? Color.green : Color.white;
    }
    
    public override void Simulate() {
        _timerBetweenShoots.Simulate();
        if (_timerBetweenShoots.ShouldExecute()) {
            if (RollbackManager.rbInputManager.GetInput((int)CustomInputManager.ActionsCode.SHOOT, _playerId)) {
                _projectileManager.CreateObject(_shootPosition.position, transform.rotation, _projectileSpeed);
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
