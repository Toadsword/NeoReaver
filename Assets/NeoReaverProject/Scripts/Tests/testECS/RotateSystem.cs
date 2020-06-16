using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public class RotateSystem : JobComponentSystem {
    
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        Debug.Log("OnUpdate RotateSystem");
        Entities.ForEach((ref Rotate rotate, ref RotationEulerXYZ euler) => {
            Debug.Log("coucuo");
            euler.Value.z += rotate.radiansPerSecond * Time.DeltaTime;
            Debug.Log("coucuo2");
        });
    }
}