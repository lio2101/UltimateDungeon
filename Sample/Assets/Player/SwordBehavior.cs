using System.Collections;
using UnityEngine;

public class SwordBehavior : MonoBehaviour
{
    [SerializeField] private float _swingDuration = 0.2f;
    [SerializeField] private int _swordDamage = 20;


    private float _swingAngle = 75;
    private bool _isSwing;


    public void Swing()
    {
        StartCoroutine(SwingRoutine());
    }

    private IEnumerator SwingRoutine()
    {
        _isSwing = true;
        float t = 0.0f;

        // change to lerp in direction of player

        while (t < _swingDuration)
        {
            float newAngle = Mathf.Lerp(0, _swingAngle, (t / _swingDuration));
            transform.localRotation = Quaternion.Euler(0f, 0f, newAngle);
            t += Time.deltaTime;

            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        _isSwing = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (_isSwing)
        {
            if(other.TryGetComponent<BossController>(out BossController boss))
            {
                boss.Life.TakeDamage(_swordDamage);

                if(boss.Life.CurrentHealth <= 0)
                {
                    // reload scene
                    // Game Manager adapt Difficulty
                }
            }
        }
    }

}
