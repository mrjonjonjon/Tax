using System.Collections;
using UnityEngine;

public class Shredder : MonoBehaviour
{
    public Transform targetTransform; // Target position and rotation
    public float time; // Duration of the lerp
        public float shredtime; // Duration of the lerp

    public float shredSpeed = 0.1f; // Speed of the shredding motion
    public float vibrationIntensity = 0.05f; // Intensity of the vibration
    public float vibrationSpeed = 50f; // Speed of the vibration

    public Transform testTransform;

    public void Start()
    {
        Shred(testTransform);
    }

    // Coroutine to move and rotate the object
    public void Shred(Transform toShred)
    {
        StartCoroutine(MoveToShredder(toShred));
    }

    private IEnumerator MoveToShredder(Transform toShred)
    {
        Vector3 startPosition = toShred.position;
        Quaternion startRotation = toShred.rotation;

        Vector3 targetPosition = targetTransform.position;
        Quaternion targetRotation = targetTransform.rotation;

        float elapsedTime = 0f;

        // Phase 1: Move to the shredder
        while (elapsedTime < time)
        {
            // Calculate progress (0 to 1)
            float t = elapsedTime / time;

            // Lerp position and rotation
            toShred.position = Vector3.Lerp(startPosition, targetPosition, t);
            toShred.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the final position and rotation are set
        toShred.parent=transform;
        toShred.position = targetPosition;
        toShred.rotation = targetRotation;

        // Phase 2: Shredding process
        yield return StartCoroutine(Shredding(toShred));
    }

    private IEnumerator Shredding(Transform toShred)
    {
        Vector3 originalShredderPosition = transform.position;
        float curtime=0f;
        while (curtime < shredtime) // Simulate shredding until the object moves out of view
        {
            // Move the object downward in local space to simulate shredding
            toShred.localPosition += new Vector3(0f,0f, shredSpeed * Time.deltaTime);

            // Add vibration effect to the shredder
            Vector3 vibration = new Vector3(
                Mathf.Sin(Time.time * vibrationSpeed) * vibrationIntensity,
                0f,
                Mathf.Cos(Time.time * vibrationSpeed) * vibrationIntensity
            );

            transform.position = originalShredderPosition + vibration;
            curtime+=Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Reset shredder vibration position
        transform.position = originalShredderPosition;

        // Destroy the shredded object
        Destroy(toShred.gameObject);
    }
}
