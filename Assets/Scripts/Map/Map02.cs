using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map02 : Map
{
    Vector3[]  positions = new Vector3[] {   new Vector3(-3, 1, -3),
                                        new Vector3(3, 1, 3),
                                        new Vector3(-3, 1, 3),
                                        new Vector3(3, 1, -3)
                                    };
    string mapName = "Map02";
    string sceneName = "BaseGame";
        
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

