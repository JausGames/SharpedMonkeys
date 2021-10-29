using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLauncher : MonoBehaviour
{
    [SerializeField] GameObject MatchManagerPrefab;
    [SerializeField] MapHandler mapHandler;
    [SerializeField] GameObject player;
    [SerializeField] List<Color> playerColors;
    // Start is called before the first frame update
    void Start()
    {
        if (MatchManager.instance == null) Instantiate(MatchManagerPrefab);
        MatchManager.instance.SetMatchSetting(MatchManager.GameMode.Kill, 10, 0);
        var map = mapHandler.GetMapByName("Map01");
        var spawnwPos = map.GetPositions();
        PlayerManager.instance.SetSpawnPositions(spawnwPos);
        var list = new List<GameObject>();
        list.Add(player);
        MatchManager.instance.SetPlayers(list);
        PlayerManager.instance.SetPlayerColors(playerColors);
        //DisplayMenu(false);
    }

}
