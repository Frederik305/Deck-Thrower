using System.Collections; 
using UnityEngine;
using UnityEngine.EventSystems;

// This class handles the scaling animation of a button when the mouse hovers over it
public class ButtonScaleAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale; // Variable to store the original scale of the button
    public float scaleFactor = 1.2f; // Factor by which the button will scale up
    public float animationDuration = 0.2f; // Duration of the scaling animation

    private void Start()
    {
        // Store the original scale of the button when the script starts
        originalScale = transform.localScale; 
    }

    // Method called when the pointer enters the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines(); // Stop any ongoing animations
        StartCoroutine(AnimateScale(originalScale * scaleFactor)); // Start the scaling up animation
    }

    // Method called when the pointer exits the button
    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines(); // Stop any ongoing animations
        StartCoroutine(AnimateScale(originalScale)); // Start the scaling down animation
    }

    // Coroutine to animate the scaling of the button
    private IEnumerator AnimateScale(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale; // Store the starting scale
        float elapsedTime = 0f; // Initialize elapsed time

        // Animate the scale over the specified duration
        while (elapsedTime < animationDuration)
        {
            // Interpolate between the starting scale and the target scale
            transform.localScale = Vector3.Lerp(startScale, targetScale, (elapsedTime / animationDuration));
            elapsedTime += Time.deltaTime; // Increment elapsed time
            yield return null; // Wait for the next frame
        }

        // Ensure the final scale is set to the target scale
        transform.localScale = targetScale; 
    }
}