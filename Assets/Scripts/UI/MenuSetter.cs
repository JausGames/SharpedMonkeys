using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSetter : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectsOfType<MainMenu>().Length == 0) Instantiate(menu);
        else FindObjectOfType<MainMenu>().DisplayMenu(true);
    }
}
