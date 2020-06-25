using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementRollback : RollbackElement {

    List<Vector2> _directions;
    
    PlayerMovement _refPlayerMovement;
    
    public override void Init(GameObject gameObject) {
        _refPlayerMovement = gameObject.GetComponent<PlayerMovement>();

        _directions = new List<Vector2>();
    }

    public override void SaveFrame() {
        _directions.Add(_refPlayerMovement.direction);
        totalSavedFrame++;
    }

    protected override void GoToFrame(int frameNumber) {
        _refPlayerMovement.direction = _directions[frameNumber - 1];
    }

    protected override void DeleteFrame(int frameNumber) {
        _directions.RemoveAt(frameNumber - 1);
        totalSavedFrame = _directions.Count;
    }
}
