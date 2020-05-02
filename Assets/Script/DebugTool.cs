using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugTool
{
    public static bool printError(string message,string gameObjectName="",string componementName="")
    {
        Debug.Log("Error - " +componementName + "/" + gameObjectName + " - " + message);
        return false;
    }

    public static bool HasComponent<T>(this GameObject flag) where T : Component
    {
        if (flag.GetComponent<T>() != null)
            return true;
        else
        {
            printError(flag.name + "doesn't have a componement which is called");
            return false;
        }
    }



}
