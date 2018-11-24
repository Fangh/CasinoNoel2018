using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public enum E_GameMode
{
    TrainingMode,
    PrizeMode
}

public class GameModeManager : MonoBehaviour
{
    [Header("References")]
    public GameObject titleObject;


    [Header("Tweaking")]
    [SerializeField] private int maxNbRetry = 10;


    private int currentNbRetry = 0;
    private E_GameMode currentGameMode;
    
    void Awake()
    {
    }

	// Use this for initialization
	void Start ()
    {
        ChangeGameMode(E_GameMode.TrainingMode);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void InitializeNewGameMode()
    {
        if(currentGameMode == E_GameMode.TrainingMode)
        {
            currentNbRetry = int.MaxValue;
        }
        else
        {
            currentNbRetry = maxNbRetry;
        }
    }

    void ChangeGameMode(E_GameMode newMode)
    {
        titleObject.GetComponent<Animator>().Play("DisplayTitle");
        titleObject.GetComponent<Animator>().GetBehaviour<AnimationClipBehavior>().exitCallbacks.Add(InitializeNewGameMode);
        if(newMode == E_GameMode.TrainingMode)
        {
            titleObject.GetComponent<TextMeshPro>().text = "Mode Entrainement";
        }
        else
        {
            titleObject.GetComponent<TextMeshPro>().text = string.Format("Vous avez {0} boules de neige pour gagner un cadeau !!", maxNbRetry);
        }
        currentGameMode = newMode;
    }
}