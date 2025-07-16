using UnityEditor.PackageManager;
using UnityEngine;

public class CardSpriteRender : MonoBehaviour
{
    [SerializeField] private string imageName;
    [SerializeField] private SpriteRenderer frontRenderer;
    [SerializeField] private GameObject CardBack;
    [SerializeField] private GameObject CardFront;

    public Card data;

    private string cardName;
    private Sprite[] cardImages;

    public void SetData(Card data)
    {
        this.data = data;
    }

    public void SetSprite()
    {
        cardName = $"{data.suit.ToString()}-{data.rank.ToString()}";
        cardImages = Resources.LoadAll<Sprite>($"Sprites/{imageName}");

        foreach (Sprite sprite in cardImages)
        {
            if (sprite.name == cardName)
            {
                frontRenderer.sprite = sprite;
                break;
            }
        }
        if (frontRenderer.sprite == null) Debug.LogError("Card Image Is Missing!");
    }
}
