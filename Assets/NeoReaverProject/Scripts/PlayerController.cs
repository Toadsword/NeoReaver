using System.Runtime.CompilerServices;
using System.Xml.Schema;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace NeoReaverProject.Scripts {

public class PlayerController : IRollbackBehaviour {
    private float _horizontal = 0.0f;
    private float _vertical = 0.0f;
    
    PlayerMovement _playerMovement;

    [SerializeField] int controllerId = -1;

    // Start is called before the first frame update
    new void Start() {
        base.Start();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update() {
        if (controllerId == -1) {
            return;
        }
    }

    public void SetControllerId(int newControllerId) {
        controllerId = newControllerId;
    }

    public override void Simulate() {
        _horizontal = RollbackManager._instance.inputQueue.GetAxis(InputQueue.AxisEnum.HORIZONTAL, controllerId);
        _vertical = RollbackManager._instance.inputQueue.GetAxis(InputQueue.AxisEnum.VERTICAL, controllerId);
        
        _playerMovement.rbElements.value.direction = new Vector2(_horizontal, _vertical);
    }

    public override void SetValueFromFrameNumber(int frameNumber) { }

    public override void DeleteFrames(int numFramesToDelete, bool firstFrames) { }

    public override void SaveFrame() { }
}
}
