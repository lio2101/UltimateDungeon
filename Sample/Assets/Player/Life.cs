using UnityEngine;

public class Life : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _maxHealth = 100;

    [Header ("Log")]
    [SerializeField] private int _currentHealth;

    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int amount)
    {

    }

    public void Regenerate(int amount)
    {

    }

}
