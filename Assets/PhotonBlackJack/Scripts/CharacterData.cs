using UnityEngine;

public class CharacterData : MonoBehaviour
{
    public static CharacterData instance;

    public string characterName;
    public GameObject character;
    public BlackJackPlayer selectedBlackJackPlayer; // 선택된 BlackJackPlayer 인스턴스 추가

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

    public void SetImage()
    {
        // 씬 로드 시 Player1ScoreText GameObject와 BlackJackPlayer 컴포넌트 자동 찾기
        GameObject player1ScoreTextObject = GameObject.Find("Player1ScoreText"); // "Player1ScoreText" 이름의 GameObject 찾기
        if (player1ScoreTextObject != null)
        {
            // CharacterData의 character 필드는 GameObject 자체를 참조하므로, 여기서는 Player1ScoreText 오브젝트를 할당합니다.
            this.character = player1ScoreTextObject;
            BlackJackPlayer player1BlackJackPlayer = player1ScoreTextObject.GetComponent<BlackJackPlayer>();
            if (player1BlackJackPlayer != null)
            {
                this.selectedBlackJackPlayer = player1BlackJackPlayer; // CharacterData의 selectedBlackJackPlayer 필드에 할당
                Debug.Log("[CharacterData] Found Player1ScoreText and BlackJackPlayer component on it.");
            }
            else
            {
                Debug.LogWarning("[CharacterData] Player1ScoreText GameObject found, but BlackJackPlayer component not found on it.");
            }
        }
        else
        {
            Debug.LogWarning("[CharacterData] Player1ScoreText GameObject not found in current scene.");
        }
    }
}
