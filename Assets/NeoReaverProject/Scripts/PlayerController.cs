using System.Runtime.CompilerServices;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace NeoReaverProject.Scripts {

public class PlayerController : MonoBehaviour {
    private float _horizontal = 0.0f;
    private float _vertical = 0.0f;

    [SerializeField] int framesInDecal = 10; 
    
    PlayerMovement _playerMovement;

    // Start is called before the first frame update
    void Start() {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update() {
        int currentFrameNum = RollbackManager.inputQueue.GetCurrentFrameNumberValue();
        
        _horizontal = RollbackManager.inputQueue.GetAxis(InputQueue.AxisEnum.HORIZONTAL, currentFrameNum - framesInDecal);
        _vertical = RollbackManager.inputQueue.GetAxis(InputQueue.AxisEnum.VERTICAL, currentFrameNum - framesInDecal);
    }

    void FixedUpdate() {
        //Input update
        _playerMovement.rbElements.value.direction = new Vector2(_horizontal, _vertical);
    }
}
}
