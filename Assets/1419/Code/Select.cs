using UnityEngine;
using UnityEngine.SceneManagement;

public class Select : MonoBehaviour
{
    public Sprite characterSprite;
    public void LoadScene()
    {
        CharactorManager.instance.selectedCharSprite = characterSprite;
        SceneManager.LoadScene("SSuregiScene");
    }
}
