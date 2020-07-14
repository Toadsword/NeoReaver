using System.Runtime.CompilerServices;
using System.Xml.Schema;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace NeoReaverProject.Scripts {

public class PlayerController : IRollbackBehaviour {
    PlayerMovement _playerMovement;
    ProjectileManager _projectileManager;

    [SerializeField] bool _localPlayer = false;
    [SerializeField] int _playerId = -1;

    [SerializeField] Transform _shootPosition;
    [SerializeField] float _projectileSpeed = 1.5f; 
    
    //Player shoo parameters
    [SerializeField] int _timeBetweenShootsTick = 20;
    private Timer _timerBetweenShoots;
    private float _horizontal = 0.0f;
    private float _vertical = 0.0f;
    
    // Start is called before the first frame update
    new void Start() {
        base.Start();
        _playerMovement = GetComponent<PlayerMovement>();
        _timerBetweenShoots = new Timer(_timeBetweenShootsTick);
        _projectileManager = FindObjectOfType<ProjectileManager>();

        if (_localPlayer) {
            _playerId = RollbackManager.rbInputManager.AddPlayer() - 1;
        }
    }

    // Update is called once per frame
    void Update() {
        if (_playerId == -1) {
            return;
        }
        
        if (_timerBetweenShoots.ShouldExecute) {
            if(RollbackManager.rbInputManager.GetInput((int)InputActionManager.InputType.SHOOT, _playerId))
            {
                _projectileManager.CreateProjectile(_shootPosition.position, transform.rotation, _projectileSpeed);
                _timerBetweenShoots.Reset();
            }
        }
    }

    public void SetPlayerId(int newControllerId) {
        _playerId = newControllerId;
    }

    public override void Simulate() {
        _horizontal = RollbackManager.rbInputManager.GetAxis(IRollbackInputManager.AxisEnum.HORIZONTAL, _playerId);
        _vertical = RollbackManager.rbInputManager.GetAxis(IRollbackInputManager.AxisEnum.VERTICAL, _playerId);
        
        _playerMovement.rbElements.value.direction = new Vector2(_horizontal, _vertical);
    }

    public override void SetValueFromFrameNumber(int frameNumber) { }

    public override void DeleteFrames(int numFramesToDelete, bool firstFrames) { }

    public override void SaveFrame() { }
}
}
