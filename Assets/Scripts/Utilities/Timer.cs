using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class Timer
{
    private static readonly Dictionary<string, Stopwatch> timers = new Dictionary<string, Stopwatch>();

    /// <summary>
    /// Starts a timer with the given name
    /// </summary>
    /// <param name="name">The name of the timer we want to start</param>
    public static void StartTest(string name)
    {
        if(timers.ContainsKey(name))
        {
            Debug.LogWarning("Timer with name '" + name + "' already exists");
            return;
        }
        Stopwatch watch = new Stopwatch();
        watch.Start();
        timers.Add(name, watch);
    }

    /// <summary>
    /// Stops the timer with the given name and returns/displays the result of the timer in milliseconds
    /// </summary>
    /// <param name="name">The name of the timer we want to end</param>
    /// <param name="displayInMilliseconds">Should the display be in milliseconds? (Seconds if false)</param>
    /// <returns>The result of the timer in milliseconds</returns>
    public static long EndTest(string name, bool printResults=true, bool displayInMilliseconds=true)
    {
        if(!timers.Remove(name, out Stopwatch watch))
        {
            Debug.LogWarning("Timer with name '" + name + "' could not be found");
            return -1;
        }
        watch.Stop();

        if(printResults)
        {
            Debug.Log("Timer <color=red>" + name + "</color> took " + GetWatchTimeFormatted(watch, displayInMilliseconds));
        }
        return watch.ElapsedMilliseconds;
    }

    private static string GetWatchTimeFormatted(Stopwatch watch, bool ms)
    {
        return ms ? $"{watch.ElapsedMilliseconds} milliseconds" : $"{(watch.ElapsedMilliseconds / 1000f):N4} seconds";
    }
}
