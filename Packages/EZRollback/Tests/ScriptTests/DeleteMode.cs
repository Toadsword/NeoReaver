using System.Collections;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Packages.EZRollback.Runtime.Scripts;
using Packages.EZRollback.Runtime.Scripts.RollbackBehaviours;
using Packages.EZRollback.Tests.Runtime.InputDelayComparer.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;

public class DeleteMode
{
    [UnityTest]
    public IEnumerator CheckBufferWorks() {
        
        //Setup RollbackManager with the input manager
        if (RollbackManager.Instance.GetComponent<SampleRollbackInputManager>() == null) {
            RollbackManager.Instance.gameObject.AddComponent<SampleRollbackInputManager>();
            RollbackManager.Instance.ResetRbInputManagerEvents();
        }

        RollbackManager.Instance.bufferSize = 5;
        RollbackManager.Instance.bufferRestriction = true;
        RollbackManager.Instance.registerFrames = true;
        RollbackManager.Instance.ClearRollbackManager();

        RollbackManager.rbInputManager.AddPlayer();

        GameObject randomObject = new GameObject();

        randomObject.AddComponent<PositionRollback>();
        
        RollbackManager.Instance.Simulate(10);
        
        Assert.True(RollbackManager.Instance.GetDisplayedFrameNum() == 5);
        Assert.Pass();
        
        yield break;
    }
    
    [UnityTest]
    public IEnumerator CheckAccessAfterBufferDeletion() {
        
        //Setup RollbackManager with the input manager
        if (RollbackManager.Instance.GetComponent<SampleRollbackInputManager>() == null) {
            RollbackManager.Instance.gameObject.AddComponent<SampleRollbackInputManager>();
            RollbackManager.Instance.ResetRbInputManagerEvents();
        }

        RollbackManager.Instance.bufferSize = 5;
        RollbackManager.Instance.bufferRestriction = true;
        RollbackManager.Instance.ClearRollbackManager();

        RollbackManager.rbInputManager.AddPlayer();

        GameObject randomObject = new GameObject();

        RollbackManager.RegisterRollbackBehaviour(randomObject.AddComponent<SpeedmoveRollback>());
        RollbackManager.RegisterRollbackBehaviour(randomObject.AddComponent<PositionRollback>());
        randomObject.GetComponent<SpeedmoveRollback>().direction = Vector3.down;
        yield return new WaitForSeconds(0.02f);

        for (int i = 1; i <= 10; i++) {
            RollbackManager.Instance.Simulate(1);
            Vector3 valueToTest = Vector3.down * Time.fixedDeltaTime * i;
            Debug.Log("1");
            Assert.True(randomObject.transform.position == valueToTest);
            Debug.Log("2");
            Assert.True(randomObject.GetComponent<PositionRollback>().GetPositionRB().value == valueToTest);
            Debug.Log("3");
            Assert.True(randomObject.GetComponent<PositionRollback>().GetPositionRB().GetValue(5) == valueToTest);
            yield return new WaitForSeconds(0.02f);
        }
        
        Assert.True(RollbackManager.Instance.GetDisplayedFrameNum() == 5);
        Assert.Pass();
        
        yield break;
    }
}
