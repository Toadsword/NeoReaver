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

    public void CreateProjectile(Vector3 position, Quaternion rotation, float speed) {
        GameObject obj = transform.GetChild(_lastUsedIndex).gameObject;
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        var dir = Quaternion.AngleAxis(obj.transform.rotation.eulerAngles.z + 90.0f, Vector3.forward) * Vector3.right;
        obj.gameObject.GetComponent<Movement>().speed = dir.normalized * speed;
        obj.gameObject.GetComponent<SelfDestruct>().SetGameobjectActive();
        _lastUsedIndex++;
        _lastUsedIndex = _lastUsedIndex % _numberOfProjectilesToInstantiate;
    }
}
