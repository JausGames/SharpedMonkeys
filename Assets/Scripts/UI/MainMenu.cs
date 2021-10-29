using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Infos")]
    [SerializeField] private int nbPlayers;
    [SerializeField] private List<Inputs.PlayerInputHandlerMenu> inputs = new List<Inputs.PlayerInputHandlerMenu>();
    [SerializeField] GameObject MatchManagerPrefab;
    [SerializeField] List<GameObject> playerTypes = new List<GameObject>();
    [SerializeField] int inputCount = 0;

    [Header("Menus")]
    [SerializeField] List<GameObject> menus = new List<GameObject>();

    [Header("Menus - Player")]
    [SerializeField] List<GameObject> players = new List<GameObject>();
    [SerializeField] private List<Text> playerText = new List<Text>();
    [SerializeField] private List<Image> playerPicture = new List<Image>();
    [SerializeField] private List<Color> playerColors = new List<Color>();
    [SerializeField] private List<Button> PlayerSelect = new List<Button>();
    [SerializeField] private Sprite colorSprite;
    [SerializeField] private List<RectTransform> colorRect = new List<RectTransform>();
    [SerializeField] private List<Button> playerColorBtn = new List<Button>();
    [SerializeField] private Button next;
    [SerializeField] private Button quit;
    [SerializeField] float ANIM_TIME = 0.5f;
    [SerializeField] float animTime = 0f;

    [Header("Menus - Settings")]
    [SerializeField] List<Button> matchModeButtons = new List<Button>();
    [SerializeField] Text matchModeTxt;
    [SerializeField] MatchManager.GameMode gameMode;
    [SerializeField] Slider nbLimitSlider;
    [SerializeField] Text nbLimitTxt;
    [SerializeField] private Button play;
    [SerializeField] private Button back;
    [Header("Menus - Maps")]
    [SerializeField] MapHandler mapHandler;


    void Start()
    {
        //Debug.unityLogger.logEnabled = false;
        DontDestroyOnLoad(this.gameObject);
        ChangeMenu(0);
        next.onClick.AddListener(delegate { ChangeMenu(2); });
        back.onClick.AddListener(delegate { ChangeMenu(1); });
        play.onClick.AddListener(ChangeScene);
        quit.onClick.AddListener(QuitGame);

        

        for (int i = 0; i < playerText.Count; i++)
        {
            Debug.Log(playerText[i].text);
            playerText[i].text = playerTypes[0].name;
            playerPicture[i].sprite = playerTypes[0].GetComponent<Player>().GetPicture();
        }
        /*for (int i = 0; i < PlayerSelect.Count; i++)
        {
            Debug.Log("Debug ok : " + i);
            PlayerSelect[i].onClick.AddListener(delegate { ChangePlayer(i); });
        }*/
        PlayerSelect[0].onClick.AddListener(delegate { ChangePlayer(0); });
        PlayerSelect[1].onClick.AddListener(delegate { ChangePlayer(1); });
        PlayerSelect[2].onClick.AddListener(delegate { ChangePlayer(2); });
        PlayerSelect[3].onClick.AddListener(delegate { ChangePlayer(3); });
        PlayerSelect[4].onClick.AddListener(delegate { ChangePlayer(4); });
        PlayerSelect[5].onClick.AddListener(delegate { ChangePlayer(5); });
        PlayerSelect[6].onClick.AddListener(delegate { ChangePlayer(6); });
        PlayerSelect[7].onClick.AddListener(delegate { ChangePlayer(7); });

        matchModeButtons[0].onClick.AddListener(delegate 
        { 
            gameMode = MatchManager.GameMode.Kill; 
            matchModeTxt.text = gameMode.ToString();
            var plurial = "";
            if (nbLimitSlider.value > 1) plurial = "s"; 
            nbLimitTxt.text = nbLimitSlider.value + " " + matchModeTxt.text.ToLower() + plurial; 
        });
        matchModeButtons[1].onClick.AddListener(delegate 
        { 
            gameMode = MatchManager.GameMode.Life; 
            matchModeTxt.text = gameMode.ToString();
            var plurial = "";
            if (nbLimitSlider.value > 1) plurial = "s";
            nbLimitTxt.text = nbLimitSlider.value + " " + matchModeTxt.text.ToLower() + plurial; 
        });
        nbLimitSlider.onValueChanged.AddListener(delegate 
        {
            var plurial = "";
            if (nbLimitSlider.value > 1) plurial = "s";
            nbLimitTxt.text = nbLimitSlider.value + " " + matchModeTxt.text.ToLower() + plurial; 
        });

        var colors = PlayerColors.GetPlayerColors();
        playerColors.Add(colors[2]);
        playerColors.Add(colors[4]);
        playerColors.Add(colors[5]);
        playerColors.Add(colors[6]);
        for (int r = 0; r < colorRect.Count; r++)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                var btnGameObj = new GameObject("btnColor_" + i);
                btnGameObj.transform.parent = colorRect[r].transform;
                var btnRect = btnGameObj.AddComponent<RectTransform>();
                var img = btnGameObj.AddComponent<Image>();
                var btn = btnGameObj.AddComponent<Button>();
                img.sprite = colorSprite;
                btnRect.anchorMin = new Vector2((1f / (float)colors.Length) * (float)i, 0f);
                btnRect.anchorMax = new Vector2((1f / (float)colors.Length) * (float)i + (1f / (float)colors.Length), 1f);
                btnRect.anchoredPosition = new Vector2(0f, 0.5f);
                btnRect.pivot = new Vector2(0f, 0.5f);
                btnRect.offsetMin = new Vector2(5f, 5f);
                btnRect.offsetMax = new Vector2(-5f, -5f);
                btn.colors = PlayerColors.SetButtonColor(colors[i]);
                var playerid = r;
                var btnNb = i;
                btn.onClick.AddListener(delegate
                {
                    ChangePlayerColor(playerid, colors[btnNb]);
                });
                playerColorBtn.Add(btn);
            }
        }
        ChangePlayerColor(0, colors[3]);

    }
    private void Update()
    {
        var oldNbPlayer = nbPlayers;
        List<Inputs.PlayerInputHandlerMenu> inputLocal = new List<Inputs.PlayerInputHandlerMenu>(); 
        inputLocal.AddRange(FindObjectsOfType<Inputs.PlayerInputHandlerMenu>());
        inputs.Clear();
        inputs.AddRange(inputLocal);
        nbPlayers = inputs.Count;
        if (nbPlayers != oldNbPlayer) animTime = Time.time + ANIM_TIME;

        if (animTime < Time.time) return;

        if (inputs.Count >= 2) next.interactable = true;
        else if (inputs.Count >= 1 && menus[0].activeSelf) ChangeMenu(1); 
        else next.interactable = false;
        var nb4UI = Mathf.Max(nbPlayers, 1);
        var UIPadding = 0.30f - ((float)nbPlayers * 0.05f);
        var oldInputCount = inputCount;
        if (oldInputCount < inputs.Count) ChangePlayerColor(inputs.Count - 1, FindFirstFreeColor());
        if (oldInputCount > inputs.Count) FreeColor(inputs.Count);
        inputCount = inputs.Count;
        for (int i = 0; i < inputs.Count; i++)
        {
            players[i].SetActive(true);
            players[i].GetComponent<RectTransform>().anchorMin = Vector2.Lerp( new Vector2(UIPadding + ((1f - UIPadding * 2f) / nb4UI) * (float) i, 0f), players[i].GetComponent<RectTransform>().anchorMin, 0.8f);
            players[i].GetComponent<RectTransform>().anchorMax = Vector2.Lerp( new Vector2(UIPadding + ((1f - UIPadding * 2f) / nb4UI) * (float) (i + 1f), 1f), players[i].GetComponent<RectTransform>().anchorMax, 0.8f); ;
            PlayerSelect[2 * i].interactable = true;
            PlayerSelect[2 * i + 1].interactable = true;
        }
        for (int i = inputs.Count; i < 4; i++)
        {
            players[i].SetActive(false);
            PlayerSelect[2 * i].interactable = false;
            PlayerSelect[2 * i + 1].interactable = false;
        }
    }

    private Color FindFirstFreeColor()
    {
        var colors = PlayerColors.GetPlayerColors();
        for(int i = 2; i < colors.Length; i++)
        {
            var available = true;
            foreach(Color col in playerColors)
            {
                if (col == colors[i]) available = false;
            }
            if (available) return colors[i];
        }
        return colors[0];
    }

    private void ChangeScene()
    {
        if (MatchManager.instance == null) Instantiate(MatchManagerPrefab);
        if(gameMode == MatchManager.GameMode.Kill) MatchManager.instance.SetMatchSetting(gameMode, (int) nbLimitSlider.value, 0);
        else MatchManager.instance.SetMatchSetting(gameMode, 0, (int) nbLimitSlider.value);
        var map = mapHandler.GetMapByName("Map01");
        var spawnwPos = map.GetPositions();
        PlayerManager.instance.SetSpawnPositions(spawnwPos);
        var list = new List<GameObject>();
        for (int i = 0; i < inputs.Count; i++)
        {
            list.Add(FindNamePlayer(playerText[i].text));
        }
        Debug.Log("ChangeScene");
        MatchManager.instance.SetPlayers(list);
        SceneManager.LoadScene(map.GetSceneName());
        PlayerManager.instance.SetPlayerColors(playerColors);
        DisplayMenu(false);
    }


    public void ChangeMenu(int menuOrder)
    {
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);
        }
        menus[menuOrder].SetActive(true);
    }
    public void DisplayMenu(bool value)
    {
        if (!value)
            foreach (GameObject menu in menus)
            {
                menu.SetActive(value);
            }
        else menus[menus.Count - 1].SetActive(value);

        this.enabled = value;
    }
    public int FindColorId(Color color)
    {
        var colors = PlayerColors.GetPlayerColors();
        for(int i = 0; i < colors.Length; i++)
        {
            if (color == colors[i]) return i;
        }
        return 0;
    }
    private void ChangePlayerColor(int playerId, Color color)
    {
        var colors = PlayerColors.GetPlayerColors();
        var colorId = FindColorId(color);
        var oldColorId = FindColorId(playerColors[playerId]);
        for(int i = 0; i < 4; i++)
        {
            if(i != playerId)
                playerColorBtn[i * colors.Length + colorId].interactable = false;
        }
        for (int i = 0; i < 4; i++)
        {
            if (i != playerId)
                playerColorBtn[i * colors.Length + oldColorId].interactable = true;
        }
        playerColors[playerId] = color;
        playerPicture[playerId].color = color;
        playerText[playerId].text = PlayerColors.GetPlayerNames()[colorId];
    }
    private void FreeColor(int playerId)
    {
        var colors = PlayerColors.GetPlayerColors();
        var oldColorId = FindColorId(playerColors[playerId]);
        for (int i = 0; i < 4; i++)
        {
            if (i != playerId)
                playerColorBtn[i * colors.Length + oldColorId].interactable = true;
        }
    }

    private void ChangePlayer(int nb)
    {
        Debug.Log("Debug ok : ChangePlayer, nb = " + nb);
        switch (nb)
        {
            case 0:
                if (FindNameID(playerText[0].text) == 0) return;
                playerText[0].text = FindPlayerName(playerTypes[FindNameID(playerText[0].text) - 1]);
                playerPicture[0].sprite = FindSprite(FindNameID(playerText[0].text) - 1, -1);
                break;
            case 1:
                if (FindNameID(playerText[0].text) == playerTypes.Count - 1) return;
                playerText[0].text = FindPlayerName(playerTypes[FindNameID(playerText[0].text) + 1]);
                playerPicture[0].sprite = FindSprite(FindNameID(playerText[0].text) + 1, 1);
                break;
            case 2:
                if (FindNameID(playerText[1].text) == 0) return;
                playerText[1].text = FindPlayerName(playerTypes[FindNameID(playerText[1].text) - 1]);
                playerPicture[1].sprite = FindSprite(FindNameID(playerText[1].text) - 1, -1);
                break;
            case 3:
                if (FindNameID(playerText[1].text) == playerTypes.Count - 1) return;
                playerText[1].text = FindPlayerName(playerTypes[FindNameID(playerText[1].text) + 1]);
                playerPicture[1].sprite = FindSprite(FindNameID(playerText[1].text) + 1, 1);
                break;
            case 4:
                if (FindNameID(playerText[2].text) == 0) return;
                playerText[2].text = FindPlayerName(playerTypes[FindNameID(playerText[2].text) - 1]);
                playerPicture[2].sprite = FindSprite(FindNameID(playerText[2].text) - 1, -1);
                break;
            case 5:
                if (FindNameID(playerText[2].text) == playerTypes.Count - 1) return;
                playerText[2].text = FindPlayerName(playerTypes[FindNameID(playerText[2].text) + 1]);
                playerPicture[2].sprite = FindSprite(FindNameID(playerText[2].text) + 1, 1);
                break;
            case 6:
                if (FindNameID(playerText[3].text) == 0) return;
                playerText[3].text = FindPlayerName(playerTypes[FindNameID(playerText[3].text) - 1]);
                playerPicture[3].sprite = FindSprite(FindNameID(playerText[3].text) - 1, -1);
                break;
            case 7:
                if (FindNameID(playerText[3].text) == playerTypes.Count - 1) return;
                playerText[3].text = FindPlayerName(playerTypes[FindNameID(playerText[3].text) + 1]);
                playerPicture[3].sprite = FindSprite(FindNameID(playerText[3].text) + 1, 1);
                break;
            default:
                Debug.Log("Player selection Switch Case not correct");
                break;
        }
    }
    private string FindPlayerName(GameObject player)
    {
        return player.name;
    }
    private Sprite FindSprite(int playerNb, int prevOrNext)
    {
        if (playerNb >= playerTypes.Count || playerNb <= 0) return playerTypes[playerNb - prevOrNext].GetComponent<Player>().GetPicture();
        else return playerTypes[playerNb - prevOrNext].GetComponent<Player>().GetPicture();
    }
    private GameObject FindNamePlayer(string player)
    {
        //was used to find the different player in a list, there is only one here

        /*foreach(GameObject obj in playerTypes)
        {
            if (obj.name == player) return obj;
        }
        return null;*/
        return playerTypes[0];
    }
    private int FindNameID(string name)
    {
        foreach(GameObject obj in playerTypes)
        {
            if (name == obj.name) return playerTypes.IndexOf(obj);
        }
        return -1;
    }
    private void QuitGame()
    {
        Application.Quit();
    }
}
