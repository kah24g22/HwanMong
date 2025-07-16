using UnityEngine;
using UnityEngine.UI;

public class ShowChar : MonoBehaviour
{
    public Image charImage;
    void Start()
    {
        charImage.sprite = CharactorManager.instance.selectedCharSprite;
    }

}
