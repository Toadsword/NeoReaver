using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Packages.EZRollback.Runtime.Scripts;
using Packages.EZRollback.Runtime.Scripts.Utils;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool gameStarted = false;

    public void StartGame() {
        gameStarted = true;
        RollbackManager.Instance.registerFrames = true;
    }
}
