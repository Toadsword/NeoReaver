using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] GameObject _object;

    [SerializeField] int _numberOfInstances = 100;

    int _lastUsedIndex = 0; 
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _numberOfInstances; i++) {
            //Instantiate them behind the camera
            GameObject go = Instantiate(_object, new Vector3(0, 0, -20), Quaternion.identity);
            go.transform.parent = this.transform;
            InScreenManager._instance.RegisterObject(go.transform);
        }
    }

    public GameObject CreateObject(Vector3 position, Quaternion rotation, float speed) {
        Debug.Log("Creating object");
        GameObject obj = transform.GetChild(_lastUsedIndex).gameObject;
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        
        var dir = Quaternion.AngleAxis(obj.transform.rotation.eulerAngles.z + 90.0f, Vector3.forward) * Vector3.right;
        obj.gameObject.GetComponent<Movement>().speed = dir.normalized * speed;
        if (obj.gameObject.GetComponent<SelfDestruct>()) {
            obj.gameObject.GetComponent<SelfDestruct>().SetGameObjectActive();
        } else {
            obj.gameObject.SetActive(true);
        }

        _lastUsedIndex++;
        _lastUsedIndex = _lastUsedIndex % _numberOfInstances;

        return obj;
    }
}
