using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveController : MonoBehaviour
{
    [Header("Possible Enemies (from worst to best)")]
    public List<GameObject> possibleEnemies = new List<GameObject>();

    [Header("Enemies Remaining")]
    static public int enemiesRemaining = 0;

    [Header("Current Wave Cooldown")]
    static public float waveCooldown;

    [Header("Player")]
    public GameObject player;

    [Header("HUD")]
    public GameObject HUD;

    [Header("Intro Objects")]
    public GameObject introObjects;

    [Header("Ground for Barriers")]
    public GameObject ground;

    [Header("SFX")]
    public AudioSource SFXSource;

    [Header("Save Data")]
    static public string PLAYER_NAME = "No Name";
    static public float waveNum = 0;
    static public List<int> killedEnemies = new List<int>();

    //intro stuff
    private bool doneIntroSequence = false;
    private float introSequenceNum = 1;
    private GameObject introCanvas;
    private Image screenFade;
    private Image dialogFade;

    //death screen stuff
    private Image deathFade;

    void Start()
    {
        //figure out if intro sequence needs to run depending on what waveNum equals on startup
        if (waveNum != 0)
        {
            //this means save data was loaded and player has already done Intro
            doneIntroSequence = true;

            waveNum--;
        }
        else
        {
            //this means save data wasn't loaded (or possibly that player didn't make it to start of first wave, so might as well do intro again)
            doneIntroSequence = false;
        }

        //Set start waveCooldown
        waveCooldown = 1500;
        HUD.transform.GetChild(3).GetComponent<TMP_Text>().text = "Next Wave Will Begin In " + (waveCooldown / 100).ToString();

        //Set playername for UI based on what current saved player name is on file
        HUD.transform.GetChild(0).GetChild(3).GetComponent<TMP_Text>().text = "Sgt. " + PLAYER_NAME.ToString();
        HUD.transform.GetChild(5).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "† Sgt. " + PLAYER_NAME.ToString() + " †";

        //set up private referneces
        introCanvas = introObjects.transform.GetChild(1).gameObject;
        screenFade = HUD.transform.GetChild(HUD.transform.childCount - 1).GetComponent<Image>();
        deathFade = HUD.transform.GetChild(5).GetComponent<Image>();
        dialogFade = introCanvas.transform.GetChild(introCanvas.transform.childCount - 1).GetComponent<Image>();
        
    }

    void Update()
    {
        if (!doneIntroSequence)
        {
            //do intro fade, then run intro loop (goes to main game loop after)
            if (dialogFade.color.a <= 0)
            {
                //INTRO LOOP//
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SFXSource.GetComponent<SFXController>().ButtonPress();
                    introSequenceNum++;
                }

                if (introSequenceNum == 2)
                {
                    //shows DialogBox2
                    introCanvas.transform.GetChild(0).gameObject.SetActive(false);
                    introCanvas.transform.GetChild(1).gameObject.SetActive(true);
                }
                else if (introSequenceNum == 3)
                {
                    //shows DialogBox3
                    introCanvas.transform.GetChild(1).gameObject.SetActive(false);
                    introCanvas.transform.GetChild(2).gameObject.SetActive(true);
                }
                else if (introSequenceNum == 4)
                {
                    //ends intro
                    doneIntroSequence = true;
                }
            }
            else
            {
                //set intro value for screenFade and does intro fade
                screenFade.color = new Color(0, 0, 0, 0.6f);
                dialogFade.color = new Color(0, 0, 0, dialogFade.color.a - (0.5f * Time.deltaTime));
            }
        }
        else
        {
            if(screenFade.color.a <= 0)
            {
                //check if player is dead
                if(player != null)
                {
                    //this is for stopping anything that needs to start/stop when game is paused
                    if (!HUD.GetComponent<HUDController>().settingsOn)
                    {
                        //below is what happens when not paused

                        //MAIN GAME LOOP//

                        //this runs during each wave
                        if (enemiesRemaining == 0)
                        {
                            if (waveCooldown == 0)
                            {
                                nextWave();
                            }
                            else
                            {
                                //this runs during cooldown between waves
                                waveCooldown -= 1;
                                if (waveCooldown % 100 == 0)
                                {
                                    HUD.transform.GetChild(3).GetComponent<TMP_Text>().text = "Next Wave Will Begin In " + (waveCooldown / 100).ToString();
                                }

                                //make player regenerate health AND armor between rounds
                                if (player.GetComponent<PlayerController>().totalHealth < player.GetComponent<PlayerController>().maxHealth + player.GetComponent<PlayerController>().maxArmor)
                                {
                                    //increases with ++ because speed of regen doesn't matter
                                    player.GetComponent<PlayerController>().totalHealth++;
                                    if (player.GetComponent<PlayerController>().totalHealth > player.GetComponent<PlayerController>().maxHealth + player.GetComponent<PlayerController>().maxArmor)
                                    {
                                        player.GetComponent<PlayerController>().totalHealth = player.GetComponent<PlayerController>().maxHealth + player.GetComponent<PlayerController>().maxArmor;
                                    }

                                    //update health + armor HUD bars
                                    player.GetComponent<PlayerController>().updateHUDBars();
                                }
                            }
                        }
                        else
                        {
                            //this runs throughout entirety of each wave


                            //update enemies remaining counter
                            HUD.transform.GetChild(3).GetComponent<TMP_Text>().text = "Enemies Remaining: " + (enemiesRemaining).ToString();

                            //allow player to regenerate ONLY health during round
                            if (player.GetComponent<PlayerController>().totalHealth < player.GetComponent<PlayerController>().maxHealth)
                            {
                                //increases with regenRate because speed of regen DOES matter
                                player.GetComponent<PlayerController>().totalHealth += player.GetComponent<PlayerController>().regenRate * Time.deltaTime;
                                if (player.GetComponent<PlayerController>().totalHealth > player.GetComponent<PlayerController>().maxHealth)
                                {
                                    player.GetComponent<PlayerController>().totalHealth = player.GetComponent<PlayerController>().maxHealth;
                                }

                                //update health + armor HUD bars
                                player.GetComponent<PlayerController>().updateHUDBars();
                            }
                        }
                    }
                    else
                    {
                        //below is what happens when paused
                    }
                    //below is what happens regardless of paused or not
                }
                else
                {
                    //below is what happens if player is dead

                    //if player is dead, turn on death fade if not already on
                    //this only runs once
                    if (!deathFade.gameObject.activeSelf)
                    {
                        deathFade.gameObject.SetActive(true);
                        SFXSource.GetComponent<SFXController>().stopTankNoise();
                    }

                    //with death fade on, check for whether screen has fully faded on before moving on
                    if(deathFade.color.a <= 0.6)
                    {
                        deathFade.color = new Color(0, 0, 0, deathFade.color.a + (0.5f * Time.deltaTime));
                    }
                    else
                    {
                        //continue once screen has faded on

                        //set confirmed kills text and turn on UI of death screen if its not already on
                        //this only runs once
                        if (!deathFade.transform.GetChild(0).gameObject.activeSelf)
                        {
                            deathFade.transform.GetChild(0).gameObject.SetActive(true);
                            deathFade.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = "Total Confirmed Kills: " + killedEnemies.Count.ToString();
                            
                        }
                    }
                }
            }
            else
            {
                //turn off intro sequence stuff, turn on player + HUD script, while also fading screen
                //this happens instantly if intro is skipped, otherwise happens directly at end of intro
                introObjects.SetActive(false);
                player.SetActive(true);
                HUD.GetComponent<HUDController>().enabled = true;
                screenFade.color = new Color(0, 0, 0, screenFade.color.a - (0.5f * Time.deltaTime));
            }
        }
    }

    //wave stuff
    private float waveSegment;
    private float waveSegmentNum;
    private GameObject currentEnemy;
    private GameObject nextEnemy;
    private bool updatedWaveNum = false;

    private bool reachedEndofEnemies = false;

    public void nextWave()
    {
        //updates wave number display
        waveNum++;
        HUD.transform.GetChild(2).GetComponent<TMP_Text>().text = "Wave " + waveNum.ToString();

        //save new waveNum to file so player can quit and come back to game on the same wave number
        //also saves killedEnemies so that when player eventually dies, kill counter will show total number of killed enemies across all game sessions
        //killedEnemies isn't updated whenever enemy is killed because this could allow for player exploitation:
        //if player kills all but one enemy in a round, then leaves game and comes back at a later time,
        //all enemies that were killed previously will spawn again allowing for player to continue to rack up kills on easy enemy types
        SaveSystem.SavePlayerData();

        //updates variables
        //wave segment number counts from 1 to 5 for every 5 rounds to determine wave variant
        if (waveNum < 6)
        {
            waveSegmentNum = waveNum;
        }
        else
        {
            waveSegmentNum = (waveNum % 5);
        }

        //update waveSegment (for rounds 1-5, this is 0; rounds 6-10, this is 1; and so on...)
        waveSegment = Mathf.Floor((waveNum - 1) / 5);

        //update current and next enemy variables
        //if you have reached the last enemy type in possible enemies, game will make all enemies spawned be random
        if(possibleEnemies[(int)waveSegment] != null)
        {
            currentEnemy = possibleEnemies[(int)waveSegment];
        }
        else
        {
            currentEnemy = possibleEnemies[possibleEnemies.Count - 1];
        }

        if (possibleEnemies[(int)waveSegment + 1] != null)
        {
            nextEnemy = possibleEnemies[(int)waveSegment + 1];
        }
        else
        {
            nextEnemy = possibleEnemies[possibleEnemies.Count - 1];
            reachedEndofEnemies = true;
        }

        //spawn enemies based on waveSegmentNum
        //each segment number passes different amount of total current enemies and total next enemies to the spawnSegment method
        //using current and next enemies based on waveSegment
        //also update enemies remaining

        if (waveSegmentNum == 1)
        {
            spawnSegment(2, 0);
        }
        else if (waveSegmentNum == 2)
        {
            spawnSegment(3, 0);
        }
        else if (waveSegmentNum == 3)
        {
            spawnSegment(3, 1);
        }
        else if (waveSegmentNum == 4)
        {
            spawnSegment(1, 2);
        }
        else if (waveSegmentNum == 5)
        {
            spawnSegment(2, 3);
        }
    }

    private void spawnSegment(int currentEnemyCount, int nextEnemyCount)
    {
        //first determine spawn locations for each enemy
        //this is done through creating an array of spawn points
        //array length is set to amount of current enemies + amont of next enemies + amount of extra random enemies (if there are any based on waveSegment)

        Vector3[] randomSpawnLocations = new Vector3[currentEnemyCount + nextEnemyCount + (int)waveSegment];
        for (int i = 0; i < randomSpawnLocations.Length; i++)
        {
            //spawn point is chosen randomly from all spawnpoint child objects of the wave controller
            randomSpawnLocations[i] = transform.GetChild((int)Mathf.Floor(Random.Range(0, transform.childCount))).transform.position;
            //spawn location is then offset slightly by adding a vector with random x and y values
            randomSpawnLocations[i] += new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
        }

        //then spawn enemies at these points
        if (reachedEndofEnemies)
        {
            //spawns the usual amount of tanks but they are all random types from all possible enemy types
            for (int j = 0; j < randomSpawnLocations.Length; j++)
            {
                int spawnedEnemyNum = (int)Mathf.Floor(Random.Range(0, possibleEnemies.Count));
                GameObject randomEnemy = possibleEnemies[spawnedEnemyNum];
                GameObject spawnedEnemy = Instantiate(randomEnemy, randomSpawnLocations[j], Quaternion.identity);
                spawnedEnemy.GetComponent<EnemyController>().player = player;
                spawnedEnemy.GetComponent<EnemyController>().HUD = HUD.GetComponent<HUDController>();
                spawnedEnemy.GetComponent<EnemyController>().enemyNum = spawnedEnemyNum;
                spawnedEnemy.GetComponent<EnemyController>().Ground = ground;
                spawnedEnemy.GetComponent<EnemyController>().SFXSource = SFXSource;
            }
        }
        else
        {
            //else spawns usual amount of tanks

            //spawns current enemies
            for (int j = 0; j < currentEnemyCount; j++)
            {
                GameObject spawnedEnemy = Instantiate(currentEnemy, randomSpawnLocations[j], Quaternion.identity);
                spawnedEnemy.GetComponent<EnemyController>().player = player;
                spawnedEnemy.GetComponent<EnemyController>().HUD = HUD.GetComponent<HUDController>();
                spawnedEnemy.GetComponent<EnemyController>().enemyNum = possibleEnemies.IndexOf(currentEnemy);
                spawnedEnemy.GetComponent<EnemyController>().Ground = ground;
                spawnedEnemy.GetComponent<EnemyController>().SFXSource = SFXSource;
            }

            //spawns next enemies
            for (int k = 0; k < nextEnemyCount; k++)
            {
                GameObject spawnedEnemy = Instantiate(nextEnemy, randomSpawnLocations[currentEnemyCount + k], Quaternion.identity);
                spawnedEnemy.GetComponent<EnemyController>().player = player;
                spawnedEnemy.GetComponent<EnemyController>().HUD = HUD.GetComponent<HUDController>();
                spawnedEnemy.GetComponent<EnemyController>().enemyNum = possibleEnemies.IndexOf(nextEnemy);
                spawnedEnemy.GetComponent<EnemyController>().Ground = ground;
                spawnedEnemy.GetComponent<EnemyController>().SFXSource = SFXSource;
            }

            //plus spawns "waveSegment" amount of random tanks from tanks introduced (all tanks in possible tanks array less than or equal to current tank)
            if (waveSegment != 0)
            {
                for (int l = 0; l < waveSegment; l++)
                {
                    int spawnedEnemyNum = (int)Mathf.Floor(Random.Range(0, possibleEnemies.IndexOf(currentEnemy) + 1));
                    GameObject randomEnemy = possibleEnemies[spawnedEnemyNum];
                    GameObject spawnedEnemy = Instantiate(randomEnemy, randomSpawnLocations[currentEnemyCount + nextEnemyCount + l], Quaternion.identity);
                    spawnedEnemy.GetComponent<EnemyController>().player = player;
                    spawnedEnemy.GetComponent<EnemyController>().HUD = HUD.GetComponent<HUDController>();
                    spawnedEnemy.GetComponent<EnemyController>().enemyNum = spawnedEnemyNum;
                    spawnedEnemy.GetComponent<EnemyController>().Ground = ground;
                    spawnedEnemy.GetComponent<EnemyController>().SFXSource = SFXSource;
                }
            }
        }

        enemiesRemaining = randomSpawnLocations.Length;
    }
}
