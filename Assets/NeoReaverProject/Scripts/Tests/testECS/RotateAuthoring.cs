using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RotateAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
    [SerializeField] float degreesPerSecond;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        dstManager.AddComponentData(entity, new Rotate{radiansPerSecond = math.radians(degreesPerSecond)});
        dstManager.AddComponentData(entity, new PostRotationEulerXYZ());
        Debug.Log("Converted !");
    }
}
