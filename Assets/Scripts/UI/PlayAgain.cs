using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayAgain : MonoBehaviour
{
	public Button no;
	public Button yes;
	public bool playAgain = false;

	void Start()
	{
		no.onClick.AddListener(QuitGame);
		yes.onClick.AddListener(Rematch);
	}

	void QuitGame()
	{
		Destroy(MatchManager.instance.gameObject);

		List<Inputs.PlayerInputHandler> inputGame = new List<Inputs.PlayerInputHandler>();
		inputGame.AddRange(FindObjectsOfType<Inputs.PlayerInputHandler>());
		foreach (Inputs.PlayerInputHandler gameHandler in inputGame)
		{
			gameHandler.enabled = false;
		}

		List<Inputs.PlayerInputHandlerMenu> inputMenu = new List<Inputs.PlayerInputHandlerMenu>();
		inputMenu.AddRange(FindObjectsOfType<Inputs.PlayerInputHandlerMenu>());
		foreach (Inputs.PlayerInputHandlerMenu menuHandler in inputMenu)
		{
			menuHandler.enabled = true;
		}
		SceneManager.LoadScene("MainMenu");
	}
	void Rematch()
	{
		Debug.Log("REMATCH");
		gameObject.SetActive(false);
		MatchManager.instance.ResetGame();
	}
}
