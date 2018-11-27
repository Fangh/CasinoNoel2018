using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum E_GameState
{
    FreePlay,
    PrizePlay,
    WaitingForPlayer,
    DisplayScore,
    DisplayInvitations
}

public class GameStatesManager : MonoBehaviour
{
    [Header("References")]
    public GameObject titleObject;

    [Header("Tweaking")]
    [SerializeField] private float waitingPlayerTime = 30;
    [SerializeField] private float timeBetweenServerRequests = 10;
    [SerializeField] private float displayPrizesTime = 10;
    [SerializeField] private float displayInvitationTime = 10;

    internal E_GameState currentGameState;

    private int currentRetry = 0;
    private float currentWaitingPlayerTime = 0;
    private float currentTimeBetweenServerRequests = 0;
    private float currentDisplayPrizesTime = 0;
    private float currentDisplayInvitationTime = 0;
    private Queue<object> playersQueue = new Queue<object>();
    private bool playerHasConfirmed = false;

    void Awake()
    {
        currentWaitingPlayerTime = waitingPlayerTime;
        currentTimeBetweenServerRequests = timeBetweenServerRequests;
        currentDisplayPrizesTime = displayPrizesTime;
        currentDisplayInvitationTime = displayInvitationTime;
    }

    // Use this for initialization
    void Start()
    {
        ChangeGameState(E_GameState.FreePlay);
    }

    // Update is called once per frame
    void Update()
    {
        //checking if someone is subscribed
        if (currentTimeBetweenServerRequests > 0)
        {
            currentTimeBetweenServerRequests -= Time.deltaTime;
        }
        else
        {
            currentTimeBetweenServerRequests = timeBetweenServerRequests;
            UpdatePlayersQueue();
        }

        //if a player is subscribed, start to wait its input
        if (playersQueue.Count > 0 && currentGameState == E_GameState.FreePlay)
        {
            ChangeGameState(E_GameState.WaitingForPlayer);
            playersQueue.Dequeue();
        }

        //currenlty waiting the player to click "PLAY" on their phones, or to quit
        if (currentGameState == E_GameState.WaitingForPlayer)
        {
            //decrease timer. If timer is below 0 sec, restart a freeplay
            if (currentWaitingPlayerTime > 0)
                currentWaitingPlayerTime -= Time.deltaTime;
            else
            {
                ChangeGameState(E_GameState.FreePlay);
            }

            //if the player has clicked play, start a play in which he will win something
            if (playerHasConfirmed)
            {
                CancelInvoke("AskServeurIfPlayerHasConfirmed");
                playerHasConfirmed = false;
                ChangeGameState(E_GameState.PrizePlay);
            }
            else
            {
                InvokeRepeating("AskServerIfPlayerHasConfirmed", 5f, 5f);
            }
        }

        if(currentGameState == E_GameState.DisplayScore)
        {
            if (currentDisplayPrizesTime > 0)
            {
                currentDisplayPrizesTime -= Time.deltaTime;
            }
            else
            {
                ChangeGameState(E_GameState.FreePlay);
            }
        }

        if(currentGameState == E_GameState.DisplayInvitations)
        {
            if(currentDisplayInvitationTime > 0)
            {
                currentDisplayInvitationTime -= Time.deltaTime;
            }
            else
            {
                ChangeGameState(E_GameState.FreePlay);
            }
        }
    }

    void UpdatePlayersQueue()
    {
        //check server

    }

    [ContextMenu("UpdatePlayersQueue")]
    void DEBUG_UpdatePlayersQueue()
    {
        object a = new object();
        playersQueue.Enqueue(a);
    }

    void AskServerIfPlayerHasConfirmed()
    {
        //need to check the server
        //playerHasConfirmed = response
    }

    [ContextMenu("PlayerHasConfirmed")]
    void DEBUG_PlayerHasConfirmed()
    {
        playerHasConfirmed = true;
    }

    void InitializeNewGameMode()
    {
        switch (currentGameState)
        {
            case E_GameState.FreePlay:
                foreach (ElfTarget e in GameManager.Instance.targetsList)
                {
                    e.Play();
                }
                GameManager.Instance.ResetGame();
                break;
            case E_GameState.PrizePlay:
                foreach (ElfTarget e in GameManager.Instance.targetsList)
                {
                    e.Play();
                }
                GameManager.Instance.ResetGame();
                break;
            case E_GameState.WaitingForPlayer:
                currentWaitingPlayerTime = waitingPlayerTime;
                break;
            case E_GameState.DisplayScore:
                currentDisplayPrizesTime = displayPrizesTime;
                break;
            case E_GameState.DisplayInvitations:
                currentDisplayInvitationTime = displayInvitationTime;
                break;
            default:
                break;
        }
    }

    internal void ChangeGameState(E_GameState newMode, object arg = null)
    {
        titleObject.GetComponent<Animator>().Play("DisplayTitle");
        titleObject.GetComponent<Animator>().GetBehaviour<AnimationClipBehavior>().exitCallbacks.Add(InitializeNewGameMode);
        switch (newMode)
        {
            case E_GameState.FreePlay:
                titleObject.GetComponent<TextMeshPro>().text = "Mode Libre, jouez autant que vous le voulez.";
                break;
            case E_GameState.PrizePlay:
                titleObject.GetComponent<TextMeshPro>().text = string.Format("Vous avez {0} boules de neige pour gagner un cadeau", GameManager.Instance.maxProjectiles);
                break;

            //Waiting for player : Stop the elves and Say that we are waiting the player to click PLAY on their smartphones
            case E_GameState.WaitingForPlayer:

                foreach (ElfTarget e in GameManager.Instance.targetsList)
                {
                    e.Stop();
                }

                titleObject.GetComponent<TextMeshPro>().text = string.Format("En attente de confirmation sur votre smartphone");
                break;

            //display score : stop every elves and display the prize
            case E_GameState.DisplayScore:

                foreach (ElfTarget e in GameManager.Instance.targetsList)
                {
                    e.Stop();
                }

                if ((bool)arg)
                {
                    titleObject.GetComponent<TextMeshPro>().text = string.Format("Vous avez gagne");
                }
                else
                {
                    titleObject.GetComponent<TextMeshPro>().text = string.Format("Vous avez perdu");
                }
                break;
            //display invitation : stop every elves and display the QRCode to start playing a real game
            case E_GameState.DisplayInvitations:

                foreach (ElfTarget e in GameManager.Instance.targetsList)
                {
                    e.Stop();
                }

                titleObject.GetComponent<TextMeshPro>().text = string.Format("Flashez ce QRCode pour demarrer une partie avec des cadeaux a gagner");
                break;
            default:
                break;
        }
        currentGameState = newMode;
    }
}