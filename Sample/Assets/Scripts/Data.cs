using UnityEngine;

public class Data : MonoBehaviour
{




    public static Data Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance =
            this;
        DontDestroyOnLoad(this);
    }
}
