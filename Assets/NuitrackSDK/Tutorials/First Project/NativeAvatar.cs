#region Description

// The script performs a direct translation of the skeleton using only the position data of the joints.
// Objects in the skeleton will be created when the scene starts.

#endregion


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Nuitrack/Example/TranslationAvatar")]
public class NativeAvatar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject PrefabJoint;
    [SerializeField] private GameObject snowBallPrefab;

    [Header("Tweaking")]
    [SerializeField] private nuitrack.JointType[] typeJoint;
    [SerializeField] private float velocityThreshold = 0.2f;
    [SerializeField] private float force = 1f;
    [SerializeField] private float cooldown = 2f;
    [SerializeField] private float skeletonSize = 0.001f;

    string message = "";
    GameObject[] CreatedJoint;

    float currentCooldown = 0f;
    bool canShoot = false;

    GameObject leftHand;
    GameObject rightHand;

    Vector3 leftLastPos;
    Vector3 rightLastPos;

    Vector3 leftPosDelta;
    Vector3 rightPosDelta;

    List<GameObject> targetList = new List<GameObject>();

    float lastLaunchForce = 0;

    internal GameObject currentTarget = null;

    void Start()
    {
        CreatedJoint = new GameObject[typeJoint.Length];
        for (int q = 0; q < typeJoint.Length; q++)
        {
            CreatedJoint[q] = Instantiate(PrefabJoint);
            CreatedJoint[q].transform.SetParent(transform);
        }
        message = "Skeleton created";
        currentCooldown = cooldown;
        targetList.AddRange(GameObject.FindGameObjectsWithTag("Target"));
    }

    void Update()
    {
        if (CurrentUserTracker.CurrentUser != 0)
        {
            nuitrack.Skeleton skeleton = CurrentUserTracker.CurrentSkeleton;
            message = "Skeleton found";

            for (int q = 0; q < typeJoint.Length; q++)
            {
                nuitrack.Joint joint = skeleton.GetJoint(typeJoint[q]);
                Vector3 newPosition = skeletonSize * joint.ToVector3();
                CreatedJoint[q].transform.localPosition = newPosition;
                if (leftHand == null && joint.Type == nuitrack.JointType.LeftHand)
                {
                    leftHand = CreatedJoint[q].transform.GetChild(0).gameObject;
                    leftPosDelta = leftHand.transform.position;
                    leftHand.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -180));
                    leftHand.transform.localScale = new Vector3(1, 1, 1);
                }
                if (rightHand == null && joint.Type == nuitrack.JointType.RightHand)
                {
                    rightHand = CreatedJoint[q].transform.GetChild(0).gameObject;
                    rightPosDelta = rightHand.transform.position;
                    rightHand.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
                    rightHand.transform.localScale = new Vector3(-1, 1, 1);
                }
            }

            leftPosDelta = leftHand.transform.position - leftLastPos;
            rightPosDelta = rightHand.transform.position - rightLastPos;

            if (rightHand != null && (rightPosDelta.magnitude > velocityThreshold || Input.GetKeyUp(KeyCode.Space)) && canShoot)
            {
                GameObject snowball = Instantiate(snowBallPrefab, rightHand.transform.position, Quaternion.identity);
                if(currentTarget == null)
                {
                    //if no target, send in front of you (taking the perspective into account)
                    snowball.GetComponent<Rigidbody>().velocity = (rightHand.transform.position - Camera.main.transform.position).normalized * force * rightPosDelta.magnitude;
                }
                else
                {
                    //if there is a target, launch ball to it
                    snowball.GetComponent<Rigidbody>().velocity = (currentTarget.transform.position - rightHand.transform.position).normalized * force * rightPosDelta.magnitude;
                }
                Destroy(snowball.gameObject, 5f);
                lastLaunchForce = rightPosDelta.magnitude;
                canShoot = false;
            }
        }
        else
        {
            message = "Skeleton not found";
        }

        if (currentCooldown > 0 && !canShoot)
            currentCooldown -= Time.deltaTime;
        else
        {
            canShoot = true;
            currentCooldown = cooldown;
        }
    }

    private void LateUpdate()
    {
        if (CurrentUserTracker.CurrentUser != 0)
        {
            leftLastPos = leftHand.transform.position;
            rightLastPos = rightHand.transform.position;
        }
    }

    // Display the message on the screen
    void OnGUI()
    {
        GUI.color = Color.red;
        GUI.skin.label.fontSize = 50;
        if (rightHand != null)
            GUILayout.Label("Your Launch with a force of " + (lastLaunchForce).ToString());
    }

    public Vector3 GetRandomTargetPos()
    {
        int rnd = Random.Range(0, targetList.Count);
        return targetList[rnd].transform.position;
    }

    public void DisableTarget()
    {
        if(currentTarget != null)
            StartCoroutine("WaitThenDisableTarget");
    }

    IEnumerator WaitThenDisableTarget()
    {
        yield return new WaitForSeconds(1.5f);
        currentTarget.GetComponentInParent<ElfTarget>().GetComponentInChildren<SpriteRenderer>().color = Color.white;
        currentTarget = null;
    }

}