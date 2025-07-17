using UnityEngine;

public class CharacterData : MonoBehaviour
{
    public static CharacterData instance;

    public string characterName;
    public GameObject character;

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
