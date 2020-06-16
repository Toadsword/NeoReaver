using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZRollback.Core.RollbackData {

public class Transform : ScriptableObject {
    public List<Vector3> position;
    public List<Vector3> rotation;
    public List<Vector3> scale;
}
}
