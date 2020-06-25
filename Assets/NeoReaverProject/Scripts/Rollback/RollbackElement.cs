using UnityEngine;

public abstract class RollbackElement{
    protected int totalSavedFrame = 0;

    public abstract void Init(GameObject gameObject);
    
    // Start is called before the first frame update
    public abstract void SaveFrame();

    public void GoToFrame(int frameNumber, bool deleteFrames) {
        if (totalSavedFrame < frameNumber) {
            Debug.LogError("Cannot go back from higher number of registered frames");
            return;
        }
        
        GoToFrame(frameNumber);
        
        if (deleteFrames) {
            DeleteFrames(totalSavedFrame, frameNumber);
        }
    }

    protected abstract void GoToFrame(int frameNumber);

    private void DeleteFrames(int fromFrameNumber, int toFrameNumber) {
        for (int i = toFrameNumber; i > fromFrameNumber; i--) {
            DeleteFrame(i);
        }
    }
    
    protected abstract void DeleteFrame(int frameNumber);
}