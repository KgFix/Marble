using UnityEngine;
using System.Collections;

public class BossAnimations : MonoBehaviour
{
    public Animator animator;

    [Header("Attack Delay Settings")]
    [Tooltip("Minimum time (seconds) to wait between attacks.")]
    public float minAttackDelay = 3f;

    [Tooltip("Maximum time (seconds) to wait between attacks.")]
    public float maxAttackDelay = 6f;

    // Attack animation state names
    private readonly string[] attackAnimations = { "Hammer_Right", "Hammer_Left", "Spin" };
    private readonly string phase1EndAnimation = "Phase1_End";

    private bool phase1Ended = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        StartCoroutine(AnimationRoutine());
    }

    IEnumerator AnimationRoutine()
    {
        // Initial delay at scene start
        yield return new WaitForSeconds(4f);

        while (!phase1Ended)
        {
            // Pick a random attack animation
            string attack = attackAnimations[Random.Range(0, attackAnimations.Length)];
            animator.Play(attack);

            // Wait for the attack animation to finish
            yield return StartCoroutine(WaitForAnimationToEnd(attack));

            // Wait a random time between minAttackDelay and maxAttackDelay seconds before next attack
            float waitTime = Random.Range(minAttackDelay, maxAttackDelay);
            yield return new WaitForSeconds(waitTime);

            // External condition to end phase 1 goes here
            // if (/* external condition */) {
            //     phase1Ended = true;
            // }
        }

        // Play phase1_end animation and wait for it to finish
        animator.Play(phase1EndAnimation);
        yield return StartCoroutine(WaitForAnimationToEnd(phase1EndAnimation));
        // Do not reset or play any more animations; leave the rig as is
    }

    IEnumerator WaitForAnimationToEnd(string stateName)
    {
        // Wait for the animator to actually enter the state
        yield return null;

        int stateHash = Animator.StringToHash(stateName);

        while (true)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.shortNameHash == stateHash)
            {
                if (stateInfo.normalizedTime >= 0.98f)
                    break;
            }
            yield return null;
        }
    }
}
