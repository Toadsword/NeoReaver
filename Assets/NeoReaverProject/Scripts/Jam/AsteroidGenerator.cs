using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidGenerator : MonoBehaviour {
    [SerializeField] GameObject asteroidPrefab;
    
    [SerializeField] float _distanceMinRandom = 1.3f;
    [SerializeField] float _distanceMaxRandom = 3.0f;
    [SerializeField] float _distanceDeltaRandom = 0.5f;
    [SerializeField] float _scaleMinRandom = 1.0f;
    [SerializeField] float _scaleMaxRandom = 2.0f;

    [SerializeField] int downLeftPos = -200;
    [SerializeField] int upRightPos = 200;

    void Start() {
        GenerateAsteroids();
    }

    private void GenerateAsteroids() {
        Vector2 currentPos = new Vector2(downLeftPos, downLeftPos);
        while (currentPos.y < upRightPos) {
            while (currentPos.x < upRightPos) {
                currentPos.x += Random.Range(_distanceMinRandom, _distanceMaxRandom);
                Vector2 tempPos = currentPos;
                tempPos.x += Random.Range(-_distanceDeltaRandom, _distanceDeltaRandom);
                tempPos.y += Random.Range(-_distanceDeltaRandom, _distanceDeltaRandom);
                
                GameObject obj = Instantiate(asteroidPrefab, (Vector3) tempPos, Quaternion.identity);
                obj.transform.parent = this.transform;
                //Apply random scale
                float randomScale = Random.Range(_scaleMinRandom, _scaleMaxRandom);
                obj.transform.localScale = new Vector3(randomScale, randomScale, 1.0f);
            }

            currentPos.y += Random.Range(_distanceMinRandom, _distanceMaxRandom);
            currentPos.x = downLeftPos + Random.Range(_distanceMinRandom, _distanceMaxRandom);
        }
    }
}
