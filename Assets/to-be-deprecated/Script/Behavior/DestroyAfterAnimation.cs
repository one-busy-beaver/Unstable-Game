using UnityEngine;
using System.Collections;

public class DestroyAfterAnimation : MonoBehaviour
{
    [Tooltip("How many times should the animation play before destroying?")]
    [SerializeField] private int playCount = 1;
    [SerializeField] private int layerIndex = 0;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(DestroyAfterLoops());
    }

    private IEnumerator DestroyAfterLoops()
    {
        int completedLoops = 0;

        while (completedLoops < playCount)
        {
            // 1. Wait for the animation to start (in case of transitions)
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime < 1f);

            // 2. Wait for the animation to reach the end (normalizedTime >= 1)
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime >= 1f);

            completedLoops++;
        }

        Destroy(gameObject);
    }
}