using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRendererRollback : RollbackElement {
    List<Color> _colors;

    SpriteRenderer _refSpriteRenderer;
    
    public override void Init(GameObject gameObject) {
        _refSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _colors = new List<Color>();
    }

    public override void SaveFrame() {
        _colors.Add(_refSpriteRenderer.color);
        totalSavedFrame++;
    }

    protected override void GoToFrame(int frameNumber) {
        _refSpriteRenderer.color = _colors[frameNumber - 1];
    }

    protected override void DeleteFrame(int frameNumber) {
        _colors.RemoveAt(frameNumber - 1);
        totalSavedFrame = _colors.Count;
    }
}
