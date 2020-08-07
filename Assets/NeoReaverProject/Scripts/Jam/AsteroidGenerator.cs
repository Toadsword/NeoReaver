using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidGenerator : RollbackBehaviour {
    
    private PoolManager _asteroidPoolManager;

    [SerializeField] Transform _topLeftScreen;
    [SerializeField] Transform _bottomRightScreen;
    
    [SerializeField] float _minSpeed = 1.0f;
    [SerializeField] float _maxSpeed = 3.0f;
    
    [SerializeField] float _minScale = 1.0f;
    [SerializeField] float _maxScale = 2.0f;

    [SerializeField] float _spawnRadius = 2.0f;

    [SerializeField] float _timeBetweenAsteroidSpawn = 0.2f;

    Timer _asteroidSpawnRate;

    void Start() {
        _asteroidPoolManager = GetComponent<PoolManager>();
        _asteroidSpawnRate = new Timer(_timeBetweenAsteroidSpawn);
        _asteroidSpawnRate.Reset();
    }

    private void CreateAsteroid() {
        float randomSpeed = Random.Range(-_maxSpeed, _maxSpeed);
        if (randomSpeed <= _minSpeed && randomSpeed >= -_minSpeed) {
            randomSpeed = _minScale * (TrueFalseRandom() ? 1.0f : -1.0f);
        }
            
        Vector3 randomPosition = new Vector3();
        if (TrueFalseRandom()) {
            //Top or bottom position
            randomPosition.x = Random.Range(_topLeftScreen.position.x - _spawnRadius, _bottomRightScreen.position.x + _spawnRadius);
            randomPosition.y = _topLeftScreen.position.y;
        } else {
            //Left or right spawn
            randomPosition.x = TrueFalseRandom() ? _topLeftScreen.position.x : _bottomRightScreen.position.x;
            randomPosition.y = Random.Range(_topLeftScreen.position.y - _spawnRadius, _bottomRightScreen.position.y + _spawnRadius);
        }

        Quaternion randomRotation = Random.rotation;
        randomRotation.eulerAngles = new Vector3(0.0f, 0.0f, randomRotation.eulerAngles.z);
        GameObject usedObject = _asteroidPoolManager.CreateObject(randomPosition, randomRotation, randomSpeed);

        //Applying scale
        float randomScale = Random.Range(_minScale, _maxScale);
        usedObject.transform.localScale = new Vector3(randomScale, randomScale, 1.0f);
    }
    
    public override void Simulate() {
        _asteroidSpawnRate.Simulate();
        
        if (_asteroidSpawnRate.ShouldExecute()) {
            CreateAsteroid();
            _asteroidSpawnRate.Reset();
        }
    }

    public override void SetValueFromFrameNumber(int frameNumber) {
        _asteroidSpawnRate.SetValueFromFrameNumber(frameNumber);
    }

    public override void DeleteFrames(int numFramesToDelete, RollbackManager.DeleteFrameMode deleteMode) {
        _asteroidSpawnRate.DeleteFrames(numFramesToDelete, deleteMode);
    }

    public override void SaveFrame() {
        _asteroidSpawnRate.SaveFrame();
    }

    private bool TrueFalseRandom() {
        return Random.Range(0, 1) < 0.5f;
    }
}