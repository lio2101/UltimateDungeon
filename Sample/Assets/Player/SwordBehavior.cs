using System.Collections;
using UnityEngine;

public class SwordBehavior : MonoBehaviour
{
    [SerializeField] private float _swingDuration = 1f;
    [SerializeField] private int _swordDamage = 20;


    private float _swingAngle = 75;
    private bool _isSwing;
    private bool _hasHit;

    public float SwingDuration => _swingDuration;

    public void Swing()
    {
        StartCoroutine(SwingRoutine());
    }


    private IEnumerator SwingRoutine()
    {
        _isSwing = true;
        yield return new WaitForSeconds(_swingDuration);
        _isSwing = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (_isSwing)
        {
            if(other.TryGetComponent<BossController>(out BossController boss))
            {
                boss.Life.TakeDamage(_swordDamage);
                Debug.Log(@"Boss hit");
                if(boss.Life.CurrentHealth <= 0)
                {
                    Debug.Log(@"Boss dead");
                    GameManager.Instance.Reset();
                    GameManager.Instance.AdaptDifficulty();
                }
            }
            else if (other.TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.Life.TakeDamage(_swordDamage);
                Debug.Log(@"Player hit");

                if (player.Life.CurrentHealth <= 0)
                {
                    Debug.Log(@"Player dead");
                    GameManager.Instance.Reset();
                    GameManager.Instance.AdaptDifficulty();
                }
            }
        }
    }



}
