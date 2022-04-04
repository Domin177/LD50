using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathScanner : MonoBehaviour
{
    public static void UpdateFences()
    {
        AstarPath.active.Scan();
    }

}
