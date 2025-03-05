/*****************************************************
 * Script Name : CreditManager.cs
 * Author      : Pierre-Luc Ravacley
 * Date        : 2025-02-18
 * Description : Organise le backend de la scene Credit.
 * Version     : 1.0
 *****************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    public Animator creditsAnimator; // Assign the Animator for the credits animation
    public float animationDuration = 60f; // Desired duration for the credits animation in seconds
    public float waitTimeAfterAnimation = 5f; // Time to wait after the animation ends


    private void Start()
    {
        if (creditsAnimator != null)
        {
            // Adjust animation playback speed to fit the desired duration
            AdjustAnimationSpeed(animationDuration);

            // Play the credits animation
            creditsAnimator.Play("CreditScrolling");
            StartCoroutine(WaitForAnimation());
        }
        else
        {
            Debug.LogError("Animator is not assigned!");
        }
    }

    private void AdjustAnimationSpeed(float targetDuration)
    {
        // Get the length of the animation clip
        float clipLength = creditsAnimator.GetCurrentAnimatorStateInfo(0).length;

        if (clipLength > 0)
        {
            // Adjust the playback speed to match the target duration
            creditsAnimator.speed = clipLength / targetDuration;
        }
        else
        {
            Debug.LogError("Could not determine the length of the animation!");
        }
    }

    private IEnumerator WaitForAnimation()
    {
        // Wait for the animation to play for the specified duration
        yield return new WaitForSeconds(animationDuration);

        // Ensure the Animator stops after the animation
        creditsAnimator.enabled = false;

        // Wait additional time
        yield return new WaitForSeconds(waitTimeAfterAnimation);

        // Load the next scene
        SceneManager.LoadSceneAsync(0);
    }
}
