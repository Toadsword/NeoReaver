using UnityEngine;
using UnityEngine.Serialization;

namespace NeoReaverProject.Scripts {

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {

    [FormerlySerializedAs("speed")] [SerializeField] float _speed = 5.0f;
    
    private float _horizontal = 0.0f;
    private float _vertical = 0.0f;
    

    private Rigidbody2D _rigid;
    // Start is called before the first frame update
    void Start() {
        _rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        _horizontal = InputActionManager.GetAxis(InputActionManager.AxisType.HORIZONTAL);
        _vertical = InputActionManager.GetAxis(InputActionManager.AxisType.VERTICAL);
        
        //Input update
        _rigid.velocity = new Vector2(_horizontal, _vertical) * _speed;
    }

    void FixedUpdate() {
        
    }
}
}
