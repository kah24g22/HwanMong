using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    private BlackjackGame blackjackGame; // 핵심 게임 로직 인스턴스

    // UI 요소 연결
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI turnText; // 턴 표시 UI 추가
    [SerializeField] private Button hitButton;
    [SerializeField] private Button standButton;
    [SerializeField] private Button restartButton;

    // 시각화
    [Header("Card Visuals")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform player1HandTransform;
    [SerializeField] private Transform player2HandTransform;
    [SerializeField] private float cardSpacing = 0.5f;

    private List<GameObject> player1CardObjects = new List<GameObject>();
    private List<GameObject> player2CardObjects = new List<GameObject>();

    private Dictionary<string, Sprite> cardSprites;

    void Awake() // Start 대신 Awake에서 초기화하여 다른 스크립트보다 먼저 실행되도록 할 수 있습니다.
    {
        blackjackGame = new BlackjackGame();
        blackjackGame.OnCardDealt += HandleCardDealt;
        blackjackGame.OnGameEnded += HandleGameEnded;
        blackjackGame.OnTurnChanged += HandleTurnChanged; // 턴 변경 이벤트 구독

        // UI 버튼 리스너 등록
        hitButton.onClick.AddListener(() => Hit());
        standButton.onClick.AddListener(() => Stand());
        restartButton.onClick.AddListener(() => StartGame());
    }

    void Start()
    {
        // 스프라이트 시트에서 모든 카드 앞면 로드
        cardSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Cards/CuteCards");

        foreach (Sprite sprite in sprites)
        {
            cardSprites.Add(sprite.name, sprite);
        }

        StartGame();
    }

    public void StartGame()
    {
        // 이전 게임의 카드 오브젝트들 파괴
        foreach (GameObject card in player1CardObjects) Destroy(card);
        foreach (GameObject card in player2CardObjects) Destroy(card);
        player1CardObjects.Clear();
        player2CardObjects.Clear();

        resultText.text = "";
        turnText.text = ""; // 턴 텍스트 초기화
        hitButton.interactable = true;
        standButton.interactable = true;
        restartButton.gameObject.SetActive(false);

        blackjackGame.StartNewGame(); // 핵심 게임 로직 시작
        UpdateScores(); // 초기 점수 업데이트
    }

    private void HandleCardDealt(Card card, Hand hand, bool isFaceUp)
    {
        Transform parentTransform;
        List<GameObject> cardObjectsList;

        if (hand == blackjackGame.Player1Hand)
        {
            parentTransform = player1HandTransform;
            cardObjectsList = player1CardObjects;
        }
        else // hand == blackjackGame.Player2Hand
        {
            parentTransform = player2HandTransform;
            cardObjectsList = player2CardObjects;
        }

        GameObject newCardObject = Instantiate(cardPrefab, parentTransform);
        cardObjectsList.Add(newCardObject);

        newCardObject.transform.localPosition = new Vector3(cardSpacing * (cardObjectsList.Count - 1), 0, 0);

        if (isFaceUp)
        {
            newCardObject.GetComponent<SpriteRenderer>().sprite = GetCardSprite(card);
        }
        else
        {
            newCardObject.GetComponent<SpriteRenderer>().sprite = cardSprites["Card_Back"];
        }
        UpdateScores(); // 카드가 분배될 때마다 점수 업데이트
    }

    private void HandleGameEnded(string message)
    {
        resultText.text = message;
        turnText.text = "Game Over";
        hitButton.interactable = false;
        standButton.interactable = false;   
        restartButton.gameObject.SetActive(true);
        UpdateScores(true); // 게임 종료 시 딜러 카드 모두 공개
    }

    private void HandleTurnChanged(BlackjackGame.PlayerTurn currentPlayer)
    {
        turnText.text = $"{currentPlayer}'s Turn";
    }

    void UpdateScores(bool revealDealerCards = false)
    {
        player1ScoreText.text = "Player1: " + blackjackGame.Player1Hand.CalculateValue();
        player2ScoreText.text = "Player2: " + blackjackGame.Player2Hand.CalculateValue();

        // 현재 턴 플레이어 강조 또는 UI 업데이트 로직 추가 가능
        // 예: if (blackjackGame.CurrentPlayer == BlackjackGame.PlayerTurn.Player1) { /* Player1 UI 강조 */ }
    }

    Sprite GetCardSprite(Card card)
    {
        string spriteName = $"{card.suit}_{card.rank}";
        if (cardSprites.ContainsKey(spriteName))
        {
            return cardSprites[spriteName];
        }
        else
        {
            Debug.LogError($"스프라이트를 찾을 수 없습니다: {spriteName}");
            return null;
        }
    }

    public void Hit()
    {
        blackjackGame.Hit();
    }
    public void Stand()
    {
        blackjackGame.Stand();
    }
}
