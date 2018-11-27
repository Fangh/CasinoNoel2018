using DG.Tweening;
using System.Collections;
using UnityEngine;

[System.Serializable]
public struct MinMaxTime
{
    public float min;
    public float max;

    public MinMaxTime(float _min, float _max)
    {
        min = _min;
        max = _max;
    }
}

public enum E_ElfState
{
    Hidden,
    Visible,
    Stunned
}

public class ElfTarget : MonoBehaviour
{
    [Header("Tweaking")]
    [SerializeField] private MinMaxTime timeToMove = new MinMaxTime(1f, 5f);
    [SerializeField] private MinMaxTime timeIdle = new MinMaxTime(1f, 5f);
    [SerializeField] private MinMaxTime timeHidden = new MinMaxTime(1f, 5f);
    [SerializeField] private float timeStunned = 3f;
    [SerializeField] private float movementAmplitude = 1f;

    [Header("References")]
    [SerializeField] private GameObject aim;

    private Vector3 originalPos;
    Sequence mySequence;
    private Animator animatorController;

    internal E_ElfState currentState { get; private set; }

    void Awake()
    {
        originalPos = transform.position;
        animatorController = GetComponent<Animator>();
        mySequence = DOTween.Sequence();
        currentState = E_ElfState.Hidden;
        aim.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {
        mySequence.AppendInterval(Random.Range(timeHidden.min, timeHidden.max)) //wait timeHidden
          .Append(transform.DOMove(originalPos + (transform.up * movementAmplitude), Random.Range(timeToMove.min, timeToMove.max))) //move up in timeToMove
          .AppendCallback( () => { currentState = E_ElfState.Visible; }) //you are now visible
          .AppendInterval(Random.Range(timeIdle.min, timeIdle.max)) //wait timeIdle
          .AppendCallback( () => { animatorController.SetTrigger("Miss"); })
          .Append(transform.DOMove(originalPos, Random.Range(timeToMove.min, timeToMove.max))) //move to your original Pos
          .AppendCallback(() => { currentState = E_ElfState.Hidden; }) // you are now Hidden
          .AppendCallback(ResetAnimator) //reset your face
          .SetLoops(-1); //loop
        Stop();
    }

    void ResetAnimator()
    {
        if (!animatorController.GetCurrentAnimatorStateInfo(0).IsName("HappyAnim"))
        {
            animatorController.SetTrigger("Idle");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Stop()
    {
        transform.position = originalPos;
        currentState = E_ElfState.Hidden;
        aim.SetActive(false);
        mySequence.Pause();
    }

    public void Play()
    {
        mySequence.Restart();
    }

    public void ToggleAim(bool toggle)
    {
        aim.SetActive(toggle);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.TargetHit();
        animatorController.SetTrigger("Hit");
        currentState = E_ElfState.Stunned;
        mySequence.Pause();
        StartCoroutine("WaitBeforeUnPause");
    }

    IEnumerator WaitBeforeUnPause()
    {
        yield return new WaitForSeconds(timeStunned);
        mySequence.Play();
    }
}