using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Map : MonoBehaviour
{
    //Vector3[] positions;
    //string mapName;

    abstract public Vector3[] GetPositions();
    abstract public string GetName();
    abstract public string GetSceneName();

}

