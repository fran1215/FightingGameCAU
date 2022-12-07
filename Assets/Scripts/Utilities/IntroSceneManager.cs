using UnityEngine;
using System.Collections;

public class IntroSceneManager : MonoBehaviour {

    public GameObject startText;
    float timer;
    bool loadingLevel;
    bool init;

    public int activeElement;
    public GameObject menuObj;
    public ButtonRef[] menuOptions;

    void Start()
    {
        menuObj.SetActive(false);
    }

	void Update () {

        if (!init)
        {
            // it flickers the "Press Start" text
            timer += Time.deltaTime; 
            if (timer > 0.6f)
            {
                timer = 0;
                startText.SetActive(!startText.activeInHierarchy);
            }

            // Where Start == space
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Jump"))
            {
                init = true;
                startText.SetActive(false);
                menuObj.SetActive(true); //closes the text and opens the menu
            }
        }
        else
        {
            if(!loadingLevel) //if not already loading the level
            {
                // indicate the selected option
                menuOptions[activeElement].selected = true;

                // change the selected option based on input
                if(Input.GetKeyUp(KeyCode.UpArrow))
                {
                    menuOptions[activeElement].selected = false;

                    if(activeElement > 0)
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

                //and if we hit space again
                if (Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Jump"))
                {
                    //then load the level
                    Debug.Log("load");
                    loadingLevel = true;
                    StartCoroutine("LoadLevel");
                    menuOptions[activeElement].transform.localScale *= 1.2f;
                }                
            }
        }
        
	}

    IEnumerator LoadLevel()
    {
        if (activeElement == 0)
        {
            CharacterManager.GetInstance().numberOfUsers = 2;
            CharacterManager.GetInstance().players[1].playerType = PlayerBase.PlayerType.user;
        }

        yield return new WaitForSeconds(0.6f);

        string sceneToLoad = "";

        switch (activeElement)
        {
            case 0:
                sceneToLoad = "select";
                break;
            case 1:
                sceneToLoad = "settings";
                break;
            default:
                break;
        }

        MySceneManager.GetInstance().RequestLevelLoad(SceneType.main, sceneToLoad);
    }
}
