using System;
using System.Collections;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Packages.EZRollback.Runtime.Scripts;
using Packages.EZRollback.Tests.Runtime.InputDelayComparer.Scripts;
using UnityEngine;
using UnityEngine.TestTools;

public class CheckInputCorrection
{
    const int NumFramesToSimulate = 10;
    const int HalfNumFramesToSimulate = 5;
    
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator CheckInputCorrections()
    {
        //Setup RollbackManager with the input manager
        if (RollbackManager.Instance.GetComponent<SampleRollbackInputManager>() == null) {
            RollbackManager.Instance.gameObject.AddComponent<SampleRollbackInputManager>();
            RollbackManager.Instance.ResetRbInputManagerEvents();
        }
        
        RollbackManager.Instance.bufferSize = -1;
        RollbackManager.Instance.bufferRestriction = false;
        RollbackManager.Instance.registerFrames = false;
        RollbackManager.Instance.ClearRollbackManager();
        
        Assert.True(RollbackManager.Instance.GetDisplayedFrameNum() == 0);
        Assert.True(RollbackManager.Instance.GetMaxFramesNum() == 0);

        int playerId = RollbackManager.rbInputManager.AddPlayer();

        yield return new WaitForSeconds(0.1f);

        RollbackManager.Instance.Simulate(NumFramesToSimulate);
        Assert.True(RollbackManager.Instance.GetDisplayedFrameNum() == NumFramesToSimulate);
        yield return new WaitForSeconds(0.1f);
        
        //Correct input of a certain frame number
        RollbackElementRollbackInputBaseActions playerInputHistory = RollbackManager.rbInputManager.GetPlayerInputHistory(playerId);
        
        RollbackInputBaseActions rbInput = new RollbackInputBaseActions(5);
        rbInput.SetHorizontalAxis(1.0f);
        rbInput.SetVerticalAxis(1.0f);
        rbInput.SetBit(3);

        Assert.True(playerInputHistory.CorrectValue(rbInput, HalfNumFramesToSimulate));
        
        //Resimulate frames
        yield return new WaitForSeconds(0.1f);
        RollbackManager.Instance.ReSimulate(NumFramesToSimulate);
        yield return new WaitForSeconds(0.1f);

        //Get corrected input
        RollbackInputBaseActions rbCorrectedInput = RollbackManager.rbInputManager.GetPlayerInputHistory(playerId).GetValue(HalfNumFramesToSimulate);
        Debug.Log(rbCorrectedInput.ToString());
        
        Assert.True(rbCorrectedInput.Equals(rbInput));
        Assert.True(Math.Abs(rbCorrectedInput.GetHorizontalAxis() - 1.0f) < 0.001f);
    }

    [UnityTest]
    public IEnumerator CheckMultipleInputCorrection() {
        //Setup RollbackManager with the input manager
        if (RollbackManager.Instance.GetComponent<SampleRollbackInputManager>() == null) {
            RollbackManager.Instance.gameObject.AddComponent<SampleRollbackInputManager>();
            RollbackManager.Instance.ResetRbInputManagerEvents();
        }
        
        RollbackManager.Instance.bufferSize = -1;
        RollbackManager.Instance.bufferRestriction = false;
        RollbackManager.Instance.registerFrames = false;
        RollbackManager.Instance.ClearRollbackManager();
        
        Assert.True(RollbackManager.Instance.GetDisplayedFrameNum() == 0);
        Assert.True(RollbackManager.Instance.GetMaxFramesNum() == 0);

        int playerId = RollbackManager.rbInputManager.AddPlayer();

        yield return new WaitForSeconds(0.1f);

        RollbackManager.Instance.Simulate(NumFramesToSimulate);
        Assert.True(RollbackManager.Instance.GetDisplayedFrameNum() == NumFramesToSimulate);
        yield return new WaitForSeconds(0.1f);
        
        //Correct input of a certain frame number
        RollbackElementRollbackInputBaseActions playerInputHistory = RollbackManager.rbInputManager.GetPlayerInputHistory(playerId);
        
        RollbackInputBaseActions rbInput = new RollbackInputBaseActions(5);
        rbInput.SetHorizontalAxis(1.0f);
        rbInput.SetVerticalAxis(1.0f);
        rbInput.SetBit(3);

        for (int i = 1; i < RollbackManager.Instance.GetDisplayedFrameNum(); i++) {
            Assert.True(playerInputHistory.CorrectValue(rbInput, i));
        }
        
        //Resimulate frames
        yield return new WaitForSeconds(0.1f);
        RollbackManager.Instance.ReSimulate(NumFramesToSimulate);
        yield return new WaitForSeconds(0.1f);

        //Get corrected input
        for (int i = 1; i < RollbackManager.Instance.GetDisplayedFrameNum(); i++) {
            RollbackInputBaseActions rbCorrectedInput = RollbackManager.rbInputManager.GetPlayerInputHistory(playerId).GetValue(i);
            Assert.True(rbCorrectedInput.Equals(rbInput));
            Assert.True(Math.Abs(rbCorrectedInput.GetHorizontalAxis() - 1.0f) < 0.001f);
        }
    }
}
