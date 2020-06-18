using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollbackComponent : MonoBehaviour
{
    public Dictionary<string, bool> rollbackedComponents = new Dictionary<string, bool>();
}
