[System.Serializable]
public class InputState{
    public bool isUp;
    public bool isDown;
    public bool isHeld;

    public void Reset() {
        isDown = false;
        isUp = false;
    }
}
