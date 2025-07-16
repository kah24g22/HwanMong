using UnityEngine;

public class CharactorManager : MonoBehaviour
{
    public static CharactorManager instance;

    public string selectedCharName;
    public Sprite selectedCharSprite;

    private void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectChar(string name, Sprite sprite)
    {
        CharactorManager.instance.selectedCharName = name;
        CharactorManager.instance.selectedCharSprite = sprite;
    }
}
