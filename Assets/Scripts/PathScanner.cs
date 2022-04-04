using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathScanner : MonoBehaviour
{
    private static int _amountOfUpdatedFences = 0;

    private static Object LOCK = new Object();

    public static void UpdateFences(int idk, bool idk2)
    {
        AstarPath.active.Scan();
    }

}
