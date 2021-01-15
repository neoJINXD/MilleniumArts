using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Zone.Core.Utils;

// Author: Anik Patel
// Description: PlayMode Tests for the Timer MonoBehaviour with it's default TimerEndHandler
// Version: 1.0
// Changes: [N/A]
public class TimerMonoBehaviourTest
{

    private class TimerMonoTest : TimerMono
    {
        protected override void HandleTimerEnd()
        {
            Destroy(this);
        }
    }

    [UnityTest]
    public IEnumerator TimerMonoBehaviourTestWithScriptDestroying()
    {
        var gameObject = new GameObject();
        var timerBehaviour = gameObject.AddComponent<TimerMonoTest>();

        timerBehaviour.Initialize(1f);

        yield return new WaitForSeconds(1f);

        Assert.IsNull(gameObject.GetComponent<TimerMonoTest>());
        Assert.IsNotNull(gameObject);
    }
}

