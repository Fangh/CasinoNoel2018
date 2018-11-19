#region Description

// The script performs a direct translation of the skeleton using only the position data of the joints.
// Objects in the skeleton will be created when the scene starts.

#endregion


using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Nuitrack/Example/TranslationAvatar")]
public class NativeAvatar : MonoBehaviour
{
    [Header("References")]
    public GameObject PrefabJoint;
    public GameObject snowBallPrefab;

    [Header("Tweaking")]
    public nuitrack.JointType[] typeJoint;
    public float velocityThreshold = 0.2f;
    public float force = 1f;
    public float cooldown = 2f;


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

    List<GameObject> ballList = new List<GameObject>();

    float lastLaunchForce = 0;

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
        ballList.AddRange(GameObject.FindGameObjectsWithTag("Target"));
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
                Vector3 newPosition = 0.001f * joint.ToVector3();
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

            if (rightHand != null && rightPosDelta.magnitude > velocityThreshold && canShoot)
            {
                GameObject snowball = Instantiate(snowBallPrefab, rightHand.transform.position, Quaternion.identity);
                snowball.GetComponent<Rigidbody>().velocity = (rightHand.transform.position - GetRandomBallPos()) * (force * (1+rightPosDelta.magnitude));
                lastLaunchForce = 1+rightPosDelta.magnitude;
                canShoot = false;
            }
        }
        else
        {
            message = "Skeleton not found";
        }

        if(currentCooldown > 0 && !canShoot)
            currentCooldown -= Time.deltaTime;
        else
        {
            canShoot = true;
            currentCooldown = cooldown;
        }
        
    }

    public Vector3 GetRandomBallPos()
    {
        int rnd = Random.Range(0, ballList.Count);
        return ballList[rnd].transform.position;
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
        if( rightHand != null )
        GUILayout.Label("Your Launch with a force of "+(lastLaunchForce * 100).ToString());
    }
}