using UnityEngine;
using System.Collections;

public class SelfDestroyWithAnimation : MonoBehaviour
{
    [SerializeField]
    private int animationCycles = 1;

    private Animator animator;
    private float animationLength;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            StartCoroutine(PlayAndDestroy());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator PlayAndDestroy()
    {
        for (int i = 0; i < animationCycles; i++)
        {
            float elapsed = 0f;
            while (elapsed < animationLength)
            {
                if (!GameManager.Instance.GamePause)
                {
                    elapsed += Time.deltaTime;
                    if (animator != null && animator.speed == 0)
                    {
                        animator.speed = 1;
                    }
                }
                else
                {
                    if (animator != null && animator.speed != 0)
                    {
                        animator.speed = 0;
                    }
                }
                yield return null;
            }

            if (i < animationCycles - 1 && animator != null)
            {
                animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
            }
        }

        Destroy(gameObject);
    }
}
