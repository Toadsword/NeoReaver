using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Packages.EZRollback.Runtime.Scripts.Utils;
using UnityEngine;

public class ProjectileManager : Singleton<ProjectileManager> {
    public PoolManager poolManager;

    void Start() {
        poolManager = GetComponent<PoolManager>();
    }
}
