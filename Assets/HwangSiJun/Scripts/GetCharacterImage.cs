using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GetCharacterImage : MonoBehaviour
{
    private Image characterImage;
    private BlackJackPlayer currentCharacterPlayer;

    void Start()
    {
        if (CharacterData.instance != null && CharacterData.instance.selectedBlackJackPlayer != null)
        {
            currentCharacterPlayer = CharacterData.instance.selectedBlackJackPlayer;
            currentCharacterPlayer.OnLifeChanged += UpdateCharacterImage;
            UpdateCharacterImage(currentCharacterPlayer.Life); // 초기 이미지 설정
        }
        else
        {
            Debug.LogWarning("CharacterData instance or selectedBlackJackPlayer is null.");
        }
    }

    void OnDisable()
    {
        if (currentCharacterPlayer != null)
        {
            currentCharacterPlayer.OnLifeChanged -= UpdateCharacterImage;
        }
    }

    void Awake()
    {
        characterImage = GetComponent<Image>();
    }

    private void UpdateCharacterImage(int currentLife)
    {
        if (CharacterData.instance != null && !string.IsNullOrEmpty(CharacterData.instance.characterName))
        {
            string characterName = CharacterData.instance.characterName;
            // Resources 폴더 내의 {characterName} 폴더에서 {currentLife} 형식의 스프라이트를 로드
            Sprite loadedSprite = Resources.Load<Sprite>($"{characterName}/{currentLife}");
            if (loadedSprite != null)
            {
                characterImage.sprite = loadedSprite;
            }
            else
            {
                Debug.LogWarning($"Character sprite not found in Resources/{characterName}/{currentLife}");
            }
        }
        else
        {
            Debug.LogWarning("CharacterData instance or characterName is null or empty.");
        }
    }
}