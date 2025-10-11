
//#define USELOGS

using System;
using UnityEngine;

public  enum  LogColor
{
        red,
        blue,
        yellow,
        green,
        white,
        black

}

public static class LogSystem
{
    public static LogColor LogColor = new LogColor();
    

    public static void LogEvent(string log, params object[] overload)
    {

#if USELOGS

		Debug.LogFormat(log, overload);
#endif

    }

    /// <summary>
    /// Logs the color event.
    /// </summary>
    /// <param name="log">Log.</param>
    /// <param name="color">Color.</param>
    public static void LogColorEvent(LogColor color, string log,  params object[] overload)
    {

#if USELOGS

        string logjoin = "<color=" + color.ToString() + ">" + log + "</color>";		
		Debug.LogFormat(logjoin, overload);
#endif

    }

    public static void LogErrorEvent(string log, params object[] overload) 
    {
#if USELOGS
			
		Debug.LogErrorFormat(log, overload);
#endif
    }

}
