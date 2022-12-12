using UnityEngine;
using UnityEngine.UI;
using static System.Runtime.CompilerServices.RuntimeHelpers;


public class SettingsSceneManager : MonoBehaviour
{
    bool savingSettings;

    public int activeElement;
    public GameObject menuObj;
    public ButtonRef[] menuOptions;
    public Button applyButton;
    public Slider volumeSlider;

    // Start is called before the first frame update
    void Start()
    {
        menuObj.SetActive(true);

        applyButton.onClick.AddListener(applyClick);
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
        AudioListener.volume = PlayerPrefs.GetFloat("volume");
    }

    void applyClick()
    {
        var newVolume = volumeSlider.value;
        PlayerPrefs.SetFloat("volume", newVolume);
        AudioListener.volume = PlayerPrefs.GetFloat("volume");

        PlayerPrefs.SetInt("volumeChanged", 1);

        MySceneManager.GetInstance().RequestLevelLoad(SceneType.main, "intro");
    }

    // Update is called once per frame
    void Update()
    {
        if (!savingSettings) // if not already loading the level
        {
            // indicate the selected option
            menuOptions[activeElement].selected = true;

            // change the selected option based on input
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                menuOptions[activeElement].selected = false;

                if (activeElement > 0)
                {

                    activeElement--;
                }
                else
                {
                    activeElement = menuOptions.Length - 1;
                }
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                menuOptions[activeElement].selected = false;

                if (activeElement < menuOptions.Length - 1)
                {
                    activeElement++;
                }
                else
                {
                    activeElement = 0;
                }
            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                MySceneManager.GetInstance().RequestLevelLoad(SceneType.main, "intro");
            }

            //and if we hit space again
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Jump"))
            {
                //then load the level

                // Debug.Log("load");
                // loadingLevel = true;
                // StartCoroutine("LoadLevel");
                // menuOptions[activeElement].transform.localScale *= 1.2f;
            }
        }
    }
}
