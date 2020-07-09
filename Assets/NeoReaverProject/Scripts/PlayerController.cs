using System.Runtime.CompilerServices;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace NeoReaverProject.Scripts {

public class PlayerController : MonoBehaviour {
    private float _horizontal = 0.0f;
    private float _vertical = 0.0f;
    
    PlayerMovement _playerMovement;

    [SerializeField] int controllerId = 0;

    // Start is called before the first frame update
    void Start() {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update() {
        int currentFrameNum = RollbackManager.inputQueue.GetCurrentFrameNumberValue();
        
        _horizontal = RollbackManager.inputQueue.GetAxis(InputQueue.AxisEnum.HORIZONTAL, controllerId);
        _vertical = RollbackManager.inputQueue.GetAxis(InputQueue.AxisEnum.VERTICAL, controllerId);
    }

    void FixedUpdate() {
        //Input update
        _playerMovement.rbElements.value.direction = new Vector2(_horizontal, _vertical);
    }


    public void SetControllerId(int newControllerId) {
        controllerId = newControllerId;
    }
}
}
