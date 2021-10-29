using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthometer : MonoBehaviour
{
    public Slider slider;
    public Text nbText;
    public Image fillImage;
    public Image LogoImage;
    [SerializeField] private Player player;
    

    void Start()
    {
        slider = gameObject.GetComponent<Slider>();
        GetComponentInParent<Canvas>().worldCamera = FindObjectOfType<Camera>();
    }

    private void FixedUpdate()
    {
        setHealth();    
    }

    private float GetHealth()
    {
        return player.GetHealthForUI();
    }

    public void setMaxHealth(float value)
    {
        slider.maxValue = value;
    }

    public void setHealth()
    {
        var health = GetHealth();
        nbText.text = health.ToString();
        slider.value = health;
    }
    public void setPlayer(Player pl)
    {
        player = pl;
    }
    public void SetColor(Color color)
    {
        var h = 0f;
        var s = 0f;
        var v = 0f;
        Color.RGBToHSV(color, out h, out s, out v);
        if(v == 0f) fillImage.color = Color.HSVToRGB(h, s * 0.7f, 0.2f);
        else fillImage.color = Color.HSVToRGB(h, s * 0.7f, v);
        LogoImage.color = Color.HSVToRGB(h, s, v * 0.7f);
    }
}
