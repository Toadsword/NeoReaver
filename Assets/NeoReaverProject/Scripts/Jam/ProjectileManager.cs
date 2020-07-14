using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] GameObject _projectileGameObject;

    [SerializeField] int _numberOfProjectilesToInstantiate = 100;

    int _lastUsedIndex = 0; 
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _numberOfProjectilesToInstantiate; i++) {
            //Instantiate them behind the camera
            GameObject go = Instantiate(_projectileGameObject, new Vector3(0, 0, -20), Quaternion.identity);
            go.transform.parent = this.transform;
        }
    }

    public void CreateProjectile(Vector3 position, Vector2 speed) {
        Transform obj = transform.GetChild(_lastUsedIndex);
        obj.position = position;
        obj.gameObject.GetComponent<Movement>().speed = speed; 
        obj.gameObject.GetComponent<SelfDestruct>().SetGameobjectActive();
        _lastUsedIndex++;
    }
}
