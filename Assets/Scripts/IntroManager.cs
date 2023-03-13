using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntroManager : MonoBehaviour
{
    public static IntroManager Instance;

    public bool doneIntro = false;

    [Header("UI Elements")]
    [SerializeField] private GameObject introDialogObj;
    [SerializeField] private TMP_Text introDialogText;

    private void Awake() => Instance = this;

    private void Start()
    {
        //if player is on wave 1, start intro sequence
        if(WaveManager.Instance.currentWave == 0) StartCoroutine(IntroDialog());
        else doneIntro = true; //otherwise intro is finished
    }

    private IEnumerator IntroDialog()
    {
        introDialogObj.SetActive(true);

        //set dialog text
        introDialogText.text = '"' + "Attention soldier! It appears that the systems in your " + 
        "armored vehicle have malfunctioned, making it impossible to stop moving forward!" + '"';

        while(!Input.GetKeyDown(KeyCode.Space)) yield return null; //wait for user input

        //set dialog text
        introDialogText.text = '"' + "Reports show massive amounts of enemy vehicles converging " +
        "on your location, so it doesn't look like you'll be getting out of this one alive..." + '"';
        
        yield return new WaitForSeconds(0.1f);
        while(!Input.GetKeyDown(KeyCode.Space)) yield return null; //wait for user input

        //set dialog text
        introDialogText.text = '"' + "Lucky for you, that doesn't matter! You're a soldier and that means NO GIVING UP! " +
        "You may not be able to stop that tank, but you can still turn and fire that cannon of yours, so GIVE 'EM HELL!" + '"';

        yield return new WaitForSeconds(0.1f);
        while(!Input.GetKeyDown(KeyCode.Space)) yield return null; //wait for user input

        //end intro dialog sequence
        introDialogObj.SetActive(false);
        doneIntro = true;
        WaveManager.Instance.currentWave = 1;
    }
}
