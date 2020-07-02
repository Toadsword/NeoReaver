using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidRotation : MonoBehaviour {
    [SerializeField] float minRotateSpeed = -120.0f;
    [SerializeField] float maxRotateSpeed = 120.0f;


    Vector3 currentRotateSpeed;
    // Start is called before the first frame update
    void Start() {
        currentRotateSpeed = new Vector3(0.0f, 0.0f, Random.Range(minRotateSpeed, maxRotateSpeed));
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(currentRotateSpeed * Time.deltaTime);
    }
}
