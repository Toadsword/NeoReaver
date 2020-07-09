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
        Debug.Log("coucou Start");
    }

    // Update is called once per frame
    void Update() {
        if (controllerId == -1) {
            return;
        }
        
        _horizontal = RollbackManager.inputQueue.GetAxis(InputQueue.AxisEnum.HORIZONTAL, controllerId);
        _vertical = RollbackManager.inputQueue.GetAxis(InputQueue.AxisEnum.VERTICAL, controllerId);
    }

    public void SetControllerId(int newControllerId) {
        controllerId = newControllerId;
    }

    public override void Simulate() {
        int currentFrameNum = RollbackManager.inputQueue.GetCurrentFrameNumberValue();
            
        Debug.Log("Before : _horizontal : " + _horizontal);
        Debug.Log("Before _vertical : " + _vertical);
        
        _horizontal = RollbackManager.inputQueue.GetAxis(InputQueue.AxisEnum.HORIZONTAL, controllerId, currentFrameNum);
        _vertical = RollbackManager.inputQueue.GetAxis(InputQueue.AxisEnum.VERTICAL, controllerId, currentFrameNum);
        
        Debug.Log("After : _horizontal : " + _horizontal);
        Debug.Log("After _vertical : " + _vertical);
        _playerMovement.rbElements.value.direction = new Vector2(_horizontal, _vertical);
    }

    public override void GoToFrame(int frameNumber) { }

    public override void DeleteFrames(int numFramesToDelete, bool firstFrames) { }

    public override void SaveFrame() { }
}
}
