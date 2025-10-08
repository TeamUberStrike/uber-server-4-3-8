using System.Collections.Generic;
using Cmune.Util;

public static class CoroutineMonitor
{
    public readonly static Dictionary<string, RoutineState> Routines = new Dictionary<string, RoutineState>();

    public static void Start(string s)
    {
        if (!Routines.ContainsKey(s))
            Routines[s] = new RoutineState();

        Routines[s].Count++;
    }

    public static void Comment(string s, string comment)
    {
        if (Routines.ContainsKey(s))
            Routines[s].State = comment;
        else
            CmuneDebug.LogError("Comment '" + comment + "' dropped as Routine not started: " + s);
    }

    public static void Stop(string s)
    {
        if (Routines.ContainsKey(s))
            Routines[s].Count--;
        else
            CmuneDebug.LogError("Stop dropped as Routine not started: " + s);
    }
}

public class RoutineState
{
    public string Name;
    public int Count;
    public string State;
}