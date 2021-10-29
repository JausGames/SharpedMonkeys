using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map01 : Map
{
    Vector3[]  positions = new Vector3[] {   new Vector3( -14.0f, 7.0f, 9.0f),
                                        new Vector3(13f, 5f,9f),
                                        new Vector3(2.5f, 1f, -15f),
                                        new Vector3( -14.0f, 7.0f, -1f)
                                    };
    string mapName = "Map01";
    string sceneName = "WzrdMap";

    override public Vector3[] GetPositions()
    {
        Debug.Log("Map, GetPositions : PositionsLength = " + positions.Length);
        return positions;
    }
    override public string GetName()
    {
        Debug.Log("Map, GetName : MapName = " + mapName);
        return mapName;
    }
    override public string GetSceneName()
    {
        Debug.Log("Map, GetName : MapName = " + mapName);
        return sceneName;
    }
}

