using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectScreenManager : MonoBehaviour
{
    public int numberOfPlayers = 1;
    public List<PlayerInterfaces> plInterfaces = new List<PlayerInterfaces>();

    public List<string> characterIDs = new List<string>(); // List of character IDs
   
    bool loadLevel; // if we are loading the level  
    public bool bothPlayersSelected;

    CharacterManager charManager;

    #region Singleton
    public static SelectScreenManager instance;
    public static SelectScreenManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }
    #endregion

    void Start()
    {
        //we start by getting the reference to the character manager
        charManager = CharacterManager.GetInstance();
        numberOfPlayers = charManager.numberOfUsers;

        charManager.solo = (numberOfPlayers == 1);

    }


    void Update()
    {
        if (!loadLevel)
        {
            for (int i = 0; i < plInterfaces.Count; i++)
            {
                if (i < numberOfPlayers)
                {
                    if (Input.GetButtonUp("Fire2" + charManager.players[i].inputId))
                    {
                        plInterfaces[i].playerBase.hasCharacter = false;
                    }

                    if (!charManager.players[i].hasCharacter)
                    {
                        plInterfaces[i].playerBase = charManager.players[i];

                        HandleSelectScreenInput(plInterfaces[i], charManager.players[i].inputId);
                        HandleCharacterPreview(plInterfaces[i]);
                    }
                }
                else
                {
                    charManager.players[i].hasCharacter = true;
                }
            }
           
        }

        if(bothPlayersSelected)
        {
            Debug.Log("loading");
            StartCoroutine("LoadLevel"); // and start the coroutine to load the level
            loadLevel = true;
            bothPlayersSelected = false;
        }
        else
        {
            if(charManager.players[0].hasCharacter 
                && charManager.players[1].hasCharacter)
            {
                bothPlayersSelected = true;
            }
           
        }
    }
  
    void HandleSelectScreenInput(PlayerInterfaces pl, string playerId)
    {
        // Handles with the input type:
        float horizontal = Input.GetAxis("Horizontal" + playerId);
        if (horizontal != 0)
        {
            if (!pl.hitInputOnce)
            {
                if (horizontal > 0)
                {
                    pl.previewCharacterID = pl.activeCharacterID;
                    pl.activeCharacterID++;
                    if (pl.activeCharacterID >= characterIDs.Count)
                        pl.activeCharacterID = 0;
                }
                else
                {
                    pl.previewCharacterID = pl.activeCharacterID;
                    pl.activeCharacterID--;
                    if (pl.activeCharacterID < 0)
                        pl.activeCharacterID = characterIDs.Count - 1;
                }

                pl.timerToReset = 0;
                pl.hitInputOnce = true;
            }
        }

        if (horizontal == 0)
        {
            pl.hitInputOnce = false;
        }

        if (pl.hitInputOnce)
        {
            pl.timerToReset += Time.deltaTime;

            if (pl.timerToReset > 0.8f)
            {
                pl.hitInputOnce = false;
                pl.timerToReset = 0;
            }
        }

        // if the user presses space, he has selected a character
        if (Input.GetButtonUp("Fire1" + playerId))
        {
            // make a reaction on the character, because why not
            pl.createdCharacter.GetComponentInChildren<Animator>().Play("Kick");

            // pass the character to the character manager so that we know what prefab to create in the level
            pl.playerBase.playerPrefab =
               charManager.returnCharacterWithID(characterIDs[pl.activeCharacterID]).prefab;

            pl.playerBase.hasCharacter = true;
        }
    }

    IEnumerator LoadLevel()
    {
        yield return new WaitForSeconds(2);//after 2 seconds load the level

        if (charManager.solo)
        {
            MySceneManager.GetInstance().CreateProgression();
            MySceneManager.GetInstance().LoadNextOnProgression();
        }
        else
        {
            MySceneManager.GetInstance().RequestLevelLoad(SceneType.prog, "level_1");
        }

    }

    void HandleCharacterPreview(PlayerInterfaces pl)
    {
        if (pl.previewCharacterID != pl.activeCharacterID)
        {
            if (pl.createdCharacter != null) //delete the one we have now if we do have one
            {
                Destroy(pl.createdCharacter);
            }

            //and create another one
            GameObject go = Instantiate(
                CharacterManager.GetInstance().returnCharacterWithID(characterIDs[pl.activeCharacterID]).prefab,
                pl.charVisPos.position,
                Quaternion.identity) as GameObject;

            pl.createdCharacter = go;
            pl.previewCharacterID = pl.activeCharacterID;

            if (!string.Equals(pl.playerBase.playerId, charManager.players[0].playerId))
            {
                pl.createdCharacter.GetComponent<StateManager>().lookRight = false;
            }
        }
    }


    [System.Serializable]
    public class PlayerInterfaces
    {
        public Transform charVisPos; //the visualization position for player 1
        public GameObject createdCharacter; // the created character for player 1

        // variables for smoothing out input
        public bool hitInputOnce;
        public float timerToReset;

        public PlayerBase playerBase;

        public int activeCharacterID;
        public int previewCharacterID;
    }
}
