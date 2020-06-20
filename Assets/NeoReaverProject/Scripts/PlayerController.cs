using UnityEngine;
using UnityEngine.Serialization;

namespace NeoReaverProject.Scripts {

public class PlayerController : MonoBehaviour {
    private float _horizontal = 0.0f;
    private float _vertical = 0.0f;

    PlayerMovement _playerMovement;
    
    // Start is called before the first frame update
    void Start() {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update() {
        _horizontal = InputActionManager.GetAxis(InputActionManager.AxisType.HORIZONTAL);
        _vertical = InputActionManager.GetAxis(InputActionManager.AxisType.VERTICAL);
    }

    void FixedUpdate() {
        //Input update
        _playerMovement.direction = new Vector2(_horizontal, _vertical);
    }
}
}
