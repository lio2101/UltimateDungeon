using System.Collections;
using UnityEngine;

public class SwordBehavior : MonoBehaviour
{
    [SerializeField] private float _swingDuration = 0.2f;

    private float _swingAngle = 75;


    public void Swing()
    {
        StartCoroutine(SwingRoutine());
    }

    private IEnumerator SwingRoutine()
    {
        float t = 0.0f;

        while (t < _swingDuration)
        {
            float newAngle = Mathf.Lerp(0, _swingAngle, (t / _swingDuration));
            transform.localRotation = Quaternion.Euler(0f, 0f, newAngle);
            t += Time.deltaTime;

            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

}
