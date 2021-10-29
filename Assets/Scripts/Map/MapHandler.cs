using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHandler : MonoBehaviour
{
    [SerializeField] private List<Map> maps;

    public Map GetMapByName(string name)
    {
        foreach (Map map in maps)
        {
            if (map.GetName() == name) return map;
        }
        return maps[0];
    }
}
