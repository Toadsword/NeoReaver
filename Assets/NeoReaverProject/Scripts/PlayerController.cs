using System.Runtime.CompilerServices;
using System.Xml.Schema;
using ExitGames.Client.Photon;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace NeoReaverProject.Scripts {

public class PlayerController : RollbackBehaviour {
    PlayerMovement _playerMovement;
    PoolManager _projectileManager;

    [SerializeField] bool _localPlayer = false;
    [SerializeField] int _playerId = -1;

    [SerializeField] Transform _shootPosition;
    [SerializeField] float _projectileSpeed = 1.5f;

    //Player shoo parameters
    [SerializeField] float _timeBetweenShootsTick = 0.2f;
    private Timer _timerBetweenShoots;
    private float _horizontal = 0.0f;
    private float _vertical = 0.0f;

    // Start is called before the first frame update
    void Start() {
        _playerMovement = GetComponent<PlayerMovement>();
        _timerBetweenShoots = new Timer(_timeBetweenShootsTick);
        _projectileManager = GameObject.Find("ProjectileManager").GetComponent<PoolManager>();

        InScreenManager._instance.RegisterObject(gameObject);
        
        if (_localPlayer) {
            _playerId = RollbackManager.rbInputManager.AddPlayer() - 1;
        }
        _timerBetweenShoots.Reset();
    }

    // Update is called once per frame
    void Update() {
        if (_playerId == -1) {
            return;
        }
    }

    public void SetPlayerId(int newControllerId) {
        _playerId = newControllerId;
    }

    public override void Simulate() {
        _horizontal = RollbackManager.rbInputManager.GetAxis(RollbackInputManager.AxisEnum.HORIZONTAL, _playerId);
        _vertical = RollbackManager.rbInputManager.GetAxis(RollbackInputManager.AxisEnum.VERTICAL, _playerId);

        _playerMovement.rbElements.value.direction = new Vector2(_horizontal, _vertical);

        _timerBetweenShoots.Simulate();
        if (_timerBetweenShoots.ShouldExecute()) {
            if (RollbackManager.rbInputManager.GetInput((int) InputActionManager.InputType.SHOOT, _playerId)) {
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
