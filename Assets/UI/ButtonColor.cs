
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ButtonColor : MonoBehaviour
{
    private ColorBlock colorBlock;
    private Dropdown dropdown;
    private static Slider hueSlider;
    private static GameObject hueSliderObject;
    private Button button;
    private Toggle toggle;
    private Image image;

    public static float hue = 0.6f;
    private float oldHue = 0.6f;

    void Start()
    {
        if (gameObject.name.Equals("HueSlider"))
        {
            HueStart();
        }
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        toggle = GetComponent<Toggle>();
        dropdown = GetComponent<Dropdown>();
        colorBlock = ColorBlock.defaultColorBlock;

        SetColor();

        //InvokeRepeating ("CheckForColorChange", 0.1f, 0.1f);
    }

    private void HueStart()
    {
        hueSliderObject = gameObject;
        hueSlider = GetComponent<Slider>();
        if (PlayerPrefs.HasKey("ColorHue"))
        {
            hue = PlayerPrefs.GetFloat("ColorHue");
            hueSlider.value = hue;
        }
        else
        {
            PlayerPrefs.SetFloat("ColorHue", 0.6f);
        }
    }



    void Update()
    {
        if (hue != oldHue)
        {
            SetColor();
            oldHue = hue;
        }

    }

    void SetColor()
    {
        colorBlock.normalColor = Color.HSVToRGB(hue, 1.0f, 0.28f);      // (0.5833333f, 1.0f, 0.2666667f)
        colorBlock.highlightedColor = Color.HSVToRGB(hue, 0.8f, 0.35f); // (0.5833333f, 0.8f, 0.3333333f)
        colorBlock.pressedColor = Color.HSVToRGB(hue, 0.4f, 0.6f);

        if (button != null)
        {
            button.colors = colorBlock;
        }
        else if (dropdown != null)
        {
            dropdown.colors = colorBlock;
        }
        else if (toggle != null)
        {
            toggle.colors = colorBlock;
        }
        else if (image != null)
        {
            image.color = colorBlock.normalColor;
        }
        else if (hueSlider != null)
        {
            hueSlider.colors = colorBlock;
        }
    }

    public void ChangeHue()
    {
        if (hueSlider == null)
        {
            hueSlider = GetComponent<Slider>();
        }
        hue = hueSlider.value;
        PlayerPrefs.SetFloat("ColorHue", hue);
        PlayerPrefs.Save();
    }

    public static void ResetHue()
    {
        if (hueSlider == null)
        {
            hueSlider = hueSliderObject.GetComponent<Slider>();
        }
        hue = 0.6f;
        hueSlider.value = hue;
    }

}



