using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    [SerializeField] GameObject playerPrefab;
    public GameObject UI;
    public GameObject healtbar;
    public List<Player> players;
    public List<Player> alive;
    public List<float> reviveCooldownList = new List<float>();
    public List<bool> aliveList = new List<bool>();
    public List<float> spawnPosWaitList = new List<float>();
    public float REVIVE_COOLDOWN = 3f;
    public float SPAWN_WAIT_TIME = 1f;
    public Material[] materials;
    [SerializeField] Vector3[] positions;
    Quaternion[] rotations;

    Vector3[] UIPositions = new Vector3[] 
    {   new Vector3(-14*Screen.width/30, -Screen.height/4),
        new Vector3(14*Screen.width/30 , Screen.height/4),
        new Vector3(-14*Screen.width/30, Screen.height/4),
        new Vector3(14*Screen.width/30, -Screen.height/4)
    };
    [SerializeField] bool onlineTest = false;

    #region Singleton

    private void Awake()
    {
        instance = this;
        if (onlineTest) SetSpawnPositions(new Vector3[] { Vector3.forward * 3f + Vector3.right * 3f + Vector3.up, 
                                                            Vector3.forward * 3f - Vector3.right * 3f + Vector3.up, 
                                                            -Vector3.forward * 3f - Vector3.right * 3f + Vector3.up, 
                                                            -Vector3.forward * 3f + Vector3.right * 3f + Vector3.up});
    }
    public static PlayerManager GetInstance()
    {
        return instance;
    }
    #endregion

    


    private void Update()
    {
        foreach (Player player in players)
        {
            if (!player.GetAlive()) alive.Remove(player);
        }

        for(int i = 0; i < reviveCooldownList.Count; i++)
        {
            if (!aliveList[i] && reviveCooldownList[i] <= Time.time && players[i].GetIsSpawnable())
            {
                SpawnPlayer(FindIdByPlayer(players[i]));
            }
        }
    }

    public List<Player> GetPlayerList()
    {
        return players;
    }
    public void RegisterKill(Player killer, Player victim)
    {
        reviveCooldownList[FindIdByPlayer(victim)] = Time.time + REVIVE_COOLDOWN;
        aliveList[FindIdByPlayer(victim)] = false;
        alive.Remove(victim);
        MatchManager.instance.RegisterKill(killer, victim);
    }
    public void RegisterSuicide(Player victim)
    {
        reviveCooldownList[FindIdByPlayer(victim)] = Time.time + REVIVE_COOLDOWN;
        aliveList[FindIdByPlayer(victim)] = false;
        alive.Remove(victim);
        MatchManager.instance.RegisterSuicide(victim);
    }
    public int FindIdByPlayer(Player player)
    {
        for(int i = 0; i < players.Count; i++)
        {
            if (players[i] == player) return i;
        }
        return -1;
    }
    public void SetMatchUp()
    {
        Debug.Log("PlayerManager, Set Match Up");
        alive.Clear();
        for (int i = 0; i < players.Count; i++)
        {
            Debug.Log("Set Match Up :" + players[i]);
            alive.Add(players[i]);
            players[i].SetAlive(true);
            players[i].ResetHealth();
            players[i].StopMotion();
            players[i].transform.localPosition = positions[i];
            players[i].transform.localRotation = rotations[i];
            //players[i].transform.localRotation = Quaternion.LookRotation(Vector3.forward, Vector3.zero - positions[i].x * Vector3.right + positions[i].z * Vector3.forward);
            players[i].controller.SetCanMove(false);
            players[i].combat.SetCanMove(false);
            players[i].Revive();
            players[i].SetSpawnable(true);
            players[i].PlaySpawnAnim();
        }
        SetUIUp();
    }

    internal List<Player> GetSpawnablePlayer()
    {
        var spawnableList = new List<Player>();
        for(int i = 0; i < players.Count; i++)
        {
            if (players[i].GetIsSpawnable()) spawnableList.Add(players[i]);
        }
        return spawnableList;
    }

    private void SpawnPlayer(int id)
    {
        var availableSpot = new List<int>();
        for(int i = 0; i < positions.Length; i++)
        {
            if (spawnPosWaitList[i] <= Time.time) availableSpot.Add(i);
        }
        players[id].SetAlive(true);
        players[id].ResetHealth();
        players[id].StopMotion();
        var rnd = Random.Range(0, availableSpot.Count - 1);
        spawnPosWaitList[availableSpot[rnd]] = Time.time + SPAWN_WAIT_TIME;
        players[id].transform.localPosition = positions[availableSpot[rnd]];
        //players[id].transform.localRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        players[id].transform.localRotation = rotations[availableSpot[rnd]];
        players[id].controller.SetCanMove(true);
        players[id].combat.SetCanMove(true);
        players[id].Revive();
        alive.Add(players[id]);
        aliveList[id] = true;
        players[id].PlaySpawnAnim();
    }
    public void SetUIUp()
    {
        /*UI = transform.parent.Find("UI").gameObject;
        for (int i = 0; i < players.Count; i++)
        {
            //Create and set healthBar
            var bar = Instantiate(healtbar);
            bar.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 30, Screen.height / 3);
            bar.transform.SetParent(UI.transform.Find("Canvas"));
            bar.GetComponent<Healthometer>().setPlayer(players[i]);
            bar.GetComponent<Healthometer>().setMaxHealth(200f);
            //Set Color to player and Healthbar
            //players[i].SetHatColor(materials[i]);
            
            var fill = bar.transform.Find("fill").gameObject;
            fill.GetComponent<Image>().color = UIColor.instance.GetColorByString(materials[i].name);
            var hearth = bar.transform.Find("hearth").gameObject;
            hearth.GetComponent<Image>().color = UIColor.instance.GetColorByString("light" + UppercaseFirst(materials[i].name));

            //Place healthbar
            var rect = bar.GetComponent<RectTransform>();
            rect.anchoredPosition = UIPositions[i];
        }*/

        List<Inputs.PlayerInputHandlerMenu> inputMenu = new List<Inputs.PlayerInputHandlerMenu>();
        inputMenu.AddRange(FindObjectsOfType<Inputs.PlayerInputHandlerMenu>());
        foreach(Inputs.PlayerInputHandlerMenu menuHandler in inputMenu)
        {
            menuHandler.enabled = false;
        }

        List<Inputs.PlayerInputHandler> inputGame = new List<Inputs.PlayerInputHandler>();
        inputGame.AddRange(FindObjectsOfType<Inputs.PlayerInputHandler>());
        foreach (Inputs.PlayerInputHandler gameHandler in inputGame)
        {
            gameHandler.enabled = true;
        }
    }
    
    public void SetCanMove()
    {
        Debug.Log("PlayerManager, Set Can Move");
        foreach (Player player in players)
        {
            player.controller.SetCanMove(true);
            player.combat.SetCanMove(true);
        }

    }
    string UppercaseFirst(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }
    public void SetSpawnPositions(Vector3[] pos)
    {
        Debug.Log("PlayerManager, SetSpawnPositions : PositionsLength = " + pos.Length );
        positions = pos;
        rotations = new Quaternion[pos.Length];
        for(int i = 0; i < positions.Length; i++)
        {
            spawnPosWaitList.Add(0f);
            rotations[i] = Quaternion.LookRotation(Vector3.zero - positions[i].x * Vector3.right + positions[i].z * Vector3.forward, Vector3.up);
        }
    }
    public Vector3[] GetSpawnPosition()
    {
        return positions;
    }
    public Quaternion[] GetSpawnRotation()
    {
        return rotations;
    }
    public void SpawnPlayers(List<GameObject> pls)
    {
        Debug.Log("PlayerManager, SpawnPlayer : Count : " + pls.Count);

        Debug.Log("PlayerManager, SpawnPlayer : PositionsLength : " + positions.Length);
        for (int i = 0; i < pls.Count; i++)
        {
            Debug.Log("PlayerManager, SpawnPlayer : Instantiate Player");
            var obj = Instantiate(pls[i]);
            obj.transform.SetParent(this.transform);
            var player = obj.GetComponent<Player>();
            player.SetPlayerIndex(i);
            players.Add(player);
            reviveCooldownList.Add(0f);
            aliveList.Add(true);
        }
        SetMatchUp();

    }

    //Use ONLINE
    public void AddPlayerToList(ulong clientId)
    {
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient networkClient)) return;
        if (!networkClient.PlayerObject.TryGetComponent<Player>(out Player newPlayer)) return;

        players.Add(newPlayer);
    }
    public void DeletePlayers()
    {
        foreach (Player player in players)
        {
            Destroy(player.gameObject);
        }
        players.Clear();
        alive.Clear();
    }

    public void SetPlayerColors(List<Color> playerColors)
    {
        for(int i = 0; i < players.Count; i++)
        {
            players[i].SetColor(playerColors[i]);
        }
    }
}

