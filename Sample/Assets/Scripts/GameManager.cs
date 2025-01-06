using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    public static GameManager Instance { get; private set; }
    public PlayerController Player => _player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void TransferData()
    {

    }

    public void AdaptDifficulty()
    {

    }

    public void Reset()
    {

    }
}
