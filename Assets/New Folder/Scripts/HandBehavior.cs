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
        RaycastHit hit;
        Vector3 direction = transform.position - Camera.main.transform.position;
        Debug.DrawLine(transform.position, direction * 10, Color.cyan);
        if (Physics.Raycast(transform.position, direction, out hit, 100f, layersToDetect))
        {
            //if you touch the same as the last frame
            if (hit.transform.gameObject == lastTouchedElf)
                return;

            //if you exit an elf
            if (hit.transform.CompareTag("Background"))
            {
                if ( lastTouchedElf != null)
                {
                    lastTouchedElf = null;
                    avatar.DisableTarget();
                }
                return;
            }

            //if you touch an elf
            if (hit.transform.GetComponentInParent<ElfTarget>().currentState == E_ElfState.Visible)
            {
                lastTouchedElf = hit.transform.gameObject;
                avatar.currentTarget = lastTouchedElf;
                lastTouchedElf.GetComponentInParent<ElfTarget>().GetComponentInChildren<SpriteRenderer>().color = Color.red;
            }
        }
    }
}
