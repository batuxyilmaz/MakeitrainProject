using UnityEngine;

public class ShakeObject : MonoBehaviour
{
    // How long the object should shake for.
    public float shakeDuration = 5f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.03f;
    public float decreaseFactor = 1.0f;

    private Vector3 originalPos;
    private bool isShaking;
    public GameObject selfGameObject;

    private void OnEnable()
    {
        originalPos = selfGameObject.transform.localPosition;
    }

    private void Update()
    {
        if (!isShaking)
        {
            return;
        }

        if (shakeDuration > 0)
        {
            selfGameObject.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            selfGameObject.transform.localPosition = originalPos;
            isShaking = false;

           // GetComponent<ShakeObject>().enabled = false;
        }
    }

    public void ShakeIt(float duration = .1f)
    {
        isShaking = true;
        shakeDuration = duration;
    }
}
