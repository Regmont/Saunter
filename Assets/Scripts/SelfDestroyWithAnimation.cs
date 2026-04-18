using UnityEngine;

public class SelfDestroyWithAnimation : MonoBehaviour
{
    private Animator animator;
    private float animationLength;
    private float elapsedTime = 0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (animator == null)
        {
            return;
        }

        if (GameManager.Instance.GamePause)
        {
            animator.speed = 0;
        }
        else
        {
            if (animator.speed == 0)
            {
                animator.speed = 1;
            }

            elapsedTime += Time.deltaTime;

            if (elapsedTime >= animationLength)
            {
                Destroy(gameObject);
            }
        }
    }
}
