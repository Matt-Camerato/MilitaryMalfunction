using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MainMenuManager : MonoBehaviour
{
    //[Header("UI Elements")]
    //[SerializeField] private

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Quit()
    {
        
    }
}
