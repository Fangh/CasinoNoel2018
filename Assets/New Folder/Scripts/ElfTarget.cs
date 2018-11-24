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

public class ElfTarget : MonoBehaviour
{
    [Header("Tweaking")]
    [SerializeField] private MinMaxTime timeToMove = new MinMaxTime(1f, 5f);
    [SerializeField] private MinMaxTime timeIdle = new MinMaxTime(1f, 5f);
    [SerializeField] private MinMaxTime timeHidden = new MinMaxTime(1f, 5f);
    [SerializeField] private float timeStunned = 3f;
    [SerializeField] private float movementAmplitude = 1f;

    private float originalY;
    Sequence mySequence;
    private Animator animatorController;

    void Awake()
    {
        originalY = transform.position.y;
        animatorController = GetComponent<Animator>();
        mySequence = DOTween.Sequence();
    }

    // Use this for initialization
    void Start()
    {
        mySequence.AppendInterval(Random.Range(timeHidden.min, timeHidden.max))
          .Append(transform.DOMoveY(originalY + movementAmplitude, Random.Range(timeToMove.min, timeToMove.max)))
          .AppendInterval(Random.Range(timeIdle.min, timeIdle.max))
          .Append(transform.DOMoveY(originalY, Random.Range(timeToMove.min, timeToMove.max)))
          .AppendCallback(ResetAnimator)
          .SetLoops(-1);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        animatorController.SetTrigger("Hit");
        mySequence.Pause();
        StartCoroutine("WaitBeforeUnPause");
    }

    IEnumerator WaitBeforeUnPause()
    {
        yield return new WaitForSeconds(timeStunned);
        mySequence.Play();
    }
}