using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static public GameManager Instance;
    GameManager()
    {
        Instance = this;
    }

    internal GameStatesManager gameStatesManagerInstance;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI projectilesUI;
    [SerializeField] private TextMeshProUGUI objectifUI;

    [Header("Tweaking")]
    [SerializeField] internal int maxProjectiles = 10;
    [SerializeField] internal int targetsObjectif = 5;


    internal int currentObjectif = 0;
    internal int currentProjectiles;

    internal List<ElfTarget> targetsList = new List<ElfTarget>();


    // Use this for initialization
    void Awake()
    {
        currentProjectiles = maxProjectiles;
        gameStatesManagerInstance = GetComponent<GameStatesManager>();
        targetsList.AddRange(FindObjectsOfType<ElfTarget>());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetGame()
    {
        currentProjectiles = maxProjectiles;
        currentObjectif = targetsObjectif;
        objectifUI.text = string.Format("<sprite=0> x{0}", currentObjectif);
        projectilesUI.text = string.Format("<sprite=10> x{0}", currentProjectiles);
    }

    public void TargetHit()
    {
        currentObjectif++;
        objectifUI.text = string.Format("<sprite=0> x{0}", currentObjectif);
    }

    public void UseProjectile()
    {
        currentProjectiles--;
        projectilesUI.text = string.Format("<sprite=10> x{0}", currentProjectiles);
        if (currentProjectiles <= 0)
        {
            //if free play, go to the invitation page
            if (gameStatesManagerInstance.currentGameState == E_GameState.FreePlay)
            {
                gameStatesManagerInstance.ChangeGameState(E_GameState.DisplayInvitations);
            }

            //if prize play, show score and what the player has won
            if (gameStatesManagerInstance.currentGameState == E_GameState.PrizePlay)
            {
                if (currentObjectif >= targetsObjectif)
                {
                    //win
                    gameStatesManagerInstance.ChangeGameState(E_GameState.DisplayScore, true);
                }
                else
                {
                    //loose
                    gameStatesManagerInstance.ChangeGameState(E_GameState.DisplayScore, false);
                }
            }

        }
    }
}