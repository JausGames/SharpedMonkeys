using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public enum GameMode {
        Kill,
        Life
    }
    [SerializeField] GameObject UI = null;
    [SerializeField] GameMode mode = GameMode.Life;
    [SerializeField] int[] nbKilled = null;
    [SerializeField] int nbMaxKilled = 0;
    [SerializeField] int[] nbKill = null;
    [SerializeField] int nbMaxKill = 0;
    [SerializeField] GameObject canvas = null;
    [SerializeField] GameObject playAgainUI = null;
    [SerializeField] GameObject timerUI = null;
    [SerializeField] PlayAgain playAgain = null;
    [SerializeField] int nbPlayers = 0;

    #region Singleton
    public static MatchManager instance;
    [SerializeField] public static PlayerManager playerManager;

    private void Awake()
    {
        instance = this;
        playerManager = GetComponentInChildren<PlayerManager>();
        UI = transform.Find("UI").gameObject;
        canvas = UI.transform.Find("Canvas").gameObject;
        timerUI = canvas.transform.Find("Timer").gameObject;
        playAgainUI = canvas.transform.Find("PlayAgain").gameObject;
        playAgain = playAgainUI.GetComponent<PlayAgain>();
    }

    #endregion

    public int GetNbKill(int id)
    {
        return nbKill[id];
    }
    public int GetNbKilled(int id)
    {
        return nbKilled[id];
    }
    public int GetMaxNbKill()
    {
        return nbMaxKill;
    }
    public int GetMaxNbKilled()
    {
        return nbMaxKilled;
    }
    public GameMode GetMode()
    {
        return mode;
    }
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        timerUI.GetComponent<Timer>().SetManager(this);
        playerManager.SetMatchUp();
        var playerCount = playerManager.GetPlayerList().Count;
        nbKill = new int[playerCount];
        nbKilled = new int[playerCount];
        timerUI.SetActive(true);
        var list = FindObjectsOfType<Inputs.PlayerInputHandler>();
        foreach (Inputs.PlayerInputHandler input in list)
        {
            input.FindPlayer();
        }
        foreach(Healthometer meter in FindObjectsOfType<Healthometer>())
        {
            var limit = nbMaxKill;
            if (mode == GameMode.Life) limit = nbMaxKilled;
            meter.setMaxHealth(limit);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        //if (playerManager.alive.Count <= 1 && !playAgain.playAgain) playAgainUI.SetActive(true);
    }

    public void RegisterKill(Player killer, Player victim)
    {
        nbKill[killer.GetIndex()]++;
        if(mode == GameMode.Kill && nbKill[killer.GetIndex()] == nbMaxKill && !playAgain.playAgain) playAgainUI.SetActive(true);
        nbKilled[victim.GetIndex()]++;
        if (mode == GameMode.Life && nbKilled[victim.GetIndex()] == nbMaxKilled && !playAgain.playAgain)
        {
            victim.SetSpawnable(false);
            if (PlayerManager.instance.GetSpawnablePlayer().Count <= 1) playAgainUI.SetActive(true); ;
        }
    }
    public void RegisterSuicide(Player victim)
    {
        nbKilled[victim.GetIndex()]++;
        if (mode == GameMode.Life && nbKilled[victim.GetIndex()] == nbMaxKilled && !playAgain.playAgain)
        {
            victim.SetSpawnable(false);
            if (PlayerManager.instance.GetSpawnablePlayer().Count <= 1) playAgainUI.SetActive(true); ;
        }
    }
    public void SetMatchSetting(GameMode mode, int nbMaxKill, int nbMaxKilled)
    {
        this.mode = mode;
        this.nbMaxKill = nbMaxKill;
        this.nbMaxKilled = nbMaxKilled;
    }

    public void StartGame()
    {
        timerUI.SetActive(false);
        playerManager.SetCanMove();
    }
    public void ResetGame()
    {
        for(int i = 0; i < nbKill.Length; i++)
        {
            nbKill[i] = 0;
            nbKilled[i] = 0;
        }
        playAgain.playAgain = false;
        playerManager.SetMatchUp();
        timerUI.SetActive(true);
    }
    public void SetPlayers(List<GameObject> players)
    {
        nbPlayers = players.Count;
        PlayerManager.instance.SpawnPlayers(players);
    }
    public void DeletePlayers()
    {
        PlayerManager.instance.DeletePlayers();
    }


}
