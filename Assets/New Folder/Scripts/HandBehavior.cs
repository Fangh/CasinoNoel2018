using UnityEngine;

public class HandBehavior : MonoBehaviour
{
    [SerializeField] private LayerMask layersToDetect;
    private NativeAvatar avatar;
    private GameObject lastTouchedElf;

    // Use this for initialization
    void Start()
    {
        avatar = GetComponentInParent<NativeAvatar>();
    }

    // Update is called once per frame
    void Update()
    {
        //don't do anything if you are not in play mode
        if (!(GameManager.Instance.gameStatesManagerInstance.currentGameState == E_GameState.FreePlay
            || GameManager.Instance.gameStatesManagerInstance.currentGameState == E_GameState.PrizePlay))
        {
            return;
        }

        RaycastHit hit;
        Vector3 direction = transform.position - Camera.main.transform.position;
        Debug.DrawLine(transform.position, direction * 10, Color.cyan);
        if (Physics.Raycast(transform.position, direction, out hit, 100f, layersToDetect))
        {
            //if you touch the same elf as the last frame
            if (hit.transform.gameObject == lastTouchedElf)
                return;

            //if you exit an elf
            if (hit.transform.CompareTag("Background"))
            {
                if (lastTouchedElf != null)
                {
                    if (avatar.hasTarget)
                    {
                        lastTouchedElf = null;
                        avatar.DisableTarget();
                    }
                }
                return;
            }

            //if you touch an elf
            if (hit.transform.CompareTag("Target") 
                && hit.transform.GetComponentInParent<ElfTarget>().currentState == E_ElfState.Visible
                && !avatar.hasTarget)
            {
                lastTouchedElf = hit.transform.gameObject;
                lastTouchedElf.GetComponentInParent<ElfTarget>().ToggleAim(true);
                avatar.EnableTarget(lastTouchedElf);
            }
        }
    }
}
