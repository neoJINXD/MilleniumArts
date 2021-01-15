using NUnit.Framework;
using Zone.Core.Utils;

// Author: Anik Patel
// Description: Tests for the POCO Timer class
// Version: 1.0
// Changes: [N/A]
public class TimerTest
{
    
    [Test]
    [TestCase(1f)]
    [TestCase(3.14f)]
    [TestCase(-3.14f)]
    public void TimerDurationIsSetTest(float duration)
    {
        var timer = new Timer(duration);

        Assert.IsTrue(timer._duration == duration);
    }

    [Test]
    public void TickingNotGoingBelowZero()
    {
        var timer = new Timer(1f);

        timer.Tick(2f);

        Assert.IsTrue(timer._duration == 0f);
    }

    [Test]
    public void EventRaised()
    {
        var timer = new Timer(1f);
        bool eventRaised = false;

        timer.OnTimerEnd += () => eventRaised = true;

        timer.Tick(1f);

        Assert.IsTrue(eventRaised);
    }

    [Test]
    public void EventNotRaised()
    {
        var timer = new Timer(1f);
        bool eventRaised = false;

        timer.OnTimerEnd += () => eventRaised = true;

        timer.Tick(0.5f);

        Assert.IsFalse(eventRaised);
    }

    
}
