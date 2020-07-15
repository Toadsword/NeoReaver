using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NeoReaverProject.Scripts;
using UnityEngine;
using Packages.EZRollback.Runtime.Scripts;

public class CameraScale : RollbackBehaviour {
    List<PlayerController> _players;

    RollbackElement<float> calculatedSizes = new RollbackElement<float>();

    Camera _camera;
    
    Vector2 upRightPoint;
    Vector2 downLeftPoint;

    [SerializeField] float minCameraSize = 5.0f;
    [SerializeField] float maxCameraSize = 10.0f;

    [SerializeField] float radiusAroundPlayer = 5.0f;
    
    // Start is called before the first frame update
    void Start() {
        _camera = GetComponent<Camera>();
        
        PlayerController[] foundPlayers = GameObject.FindObjectsOfType<PlayerController>();
        _players = new List<PlayerController>();
        foreach (PlayerController player in foundPlayers) {
            _players.Add(player);
        }
    }

    public void RegisterNewPlayer(PlayerController playerController) {
        if (!_players.Contains(playerController)) {
            _players.Add(playerController);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Simulate();
    }

    private void CalculateNewCamera() {
        if (_players.Count == 0)
            return;
        
        upRightPoint = Vector2.negativeInfinity;
        downLeftPoint = Vector2.positiveInfinity;
        //Calculate points
        foreach (PlayerController player in _players) {
            if (upRightPoint.x < player.transform.position.x + radiusAroundPlayer) {
                upRightPoint.x = player.transform.position.x + radiusAroundPlayer;
            }
            if (upRightPoint.y < player.transform.position.y + radiusAroundPlayer) {
                upRightPoint.y = player.transform.position.y + radiusAroundPlayer;
            }

            if (downLeftPoint.x > player.transform.position.x - radiusAroundPlayer) {
                downLeftPoint.x = player.transform.position.x - radiusAroundPlayer;
            }
            if (downLeftPoint.y > player.transform.position.y - radiusAroundPlayer) {
                downLeftPoint.y = player.transform.position.y - radiusAroundPlayer;
            }
        }
        
        CalculateCameraPosition();
        CalculateCameraSize();
    }

    private void CalculateCameraPosition() {
        var position = transform.position;
        position = (upRightPoint + downLeftPoint) / 2.0f;
        position = new Vector3(position.x, position.y, -10.0f);
        transform.position = position;
    }

    private void CalculateCameraSize() {
        float ratio = Screen.width / (float)Screen.height;

        Vector2 diffSize = upRightPoint - downLeftPoint;
        diffSize.x = Mathf.Abs(diffSize.x);
        diffSize.y = Mathf.Abs(diffSize.y);

        float calculatedSize = Mathf.Max(diffSize.x / ratio, diffSize.y) / 2.0f;


        if (calculatedSize < minCameraSize)
            calculatedSize = minCameraSize;
        if (calculatedSize > maxCameraSize)
            calculatedSize = maxCameraSize;

        _camera.orthographicSize = calculatedSize;
        calculatedSizes.value = calculatedSize;
    }

    public override void Simulate() {
        CalculateNewCamera();
    }

    public override void SetValueFromFrameNumber(int frameNumber) {
        calculatedSizes.SetValueFromFrameNumber(frameNumber);
        _camera.orthographicSize = calculatedSizes.value;
    }

    public override void DeleteFrames(int numFrames, RollbackManager.DeleteFrameMode deleteMode) {
        calculatedSizes.DeleteFrames(numFrames, deleteMode);
    }

    public override void SaveFrame() {
        calculatedSizes.SaveFrame();
    }
}
