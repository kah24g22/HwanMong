using UnityEngine;
using UnityEngine.SceneManagement;

public class CharactorManager : MonoBehaviour
{
    public static CharactorManager instance;

    public Sprite selectedCharSprite;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
