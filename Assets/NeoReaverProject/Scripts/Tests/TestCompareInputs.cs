using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCompareInputs : MonoBehaviour {

    [SerializeField] RollbackInputBaseActions _input1;
    [SerializeField] RollbackInputBaseActions _input2;

    [SerializeField] int _hash1;
    [SerializeField] int _hash2;

    
    [SerializeField] bool areSame;
    [SerializeField] bool sameHashCode;
    
    // Start is called before the first frame update
    void Start()
    {
        _input1 = new RollbackInputBaseActions(5);
        _input2 = new RollbackInputBaseActions(5);
    }

    // Update is called once per frame
    void Update()
    {
        _hash1 = _input1.GetHashCode();
        _hash2 = _input2.GetHashCode();
        
        areSame = _input1.Equals(_input2);
        sameHashCode = _hash1 == _hash2;
    }
}
