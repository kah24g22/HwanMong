using UnityEngine;
using UnityEngine.UI;

public class ShowChar : MonoBehaviour
{
    public Image charImage;
    public Text charName;
    void Start()
    {
        charImage.sprite = CharactorManager.instance.selectedCharSprite;
        charName.text = CharactorManager.instance.selectedCharName;
    }

}
