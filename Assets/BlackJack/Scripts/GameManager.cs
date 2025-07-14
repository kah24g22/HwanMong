using UnityEngine;
using UnityEngine.UI; // UI 요소를 사용하기 위해 추가
using System.Collections.Generic;
using TMPro; // TextMeshPro를 사용하기 위해 추가

public class GameManager : MonoBehaviour
{
    public Deck deck;
    public List<Card> playerHand;
    public List<Card> dealerHand;

    // UI 요소 연결
    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private TextMeshProUGUI dealerScoreText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button hitButton;
    [SerializeField] private Button standButton;
    [SerializeField] private Button restartButton;

    // 시각화
    [Header("Card Visuals")]
    [SerializeField] private GameObject cardPrefab; // 1단계에서 만든 카드 프리팹
    [SerializeField] private Transform playerHandTransform; // 플레이어 카드가 놓일 위치
    [SerializeField] private Transform dealerHandTransform; // 딜러 카드가 놓일 위치
    [SerializeField] private float cardSpacing = 0.5f; // 카드 사이의 간격

    private List<GameObject> playerCardObjects = new List<GameObject>();
    private List<GameObject> dealerCardObjects = new List<GameObject>();

    // 스프라이트 시트의 모든 스프라이트를 저장할 딕셔너리
    private Dictionary<string, Sprite> cardSprites;

    void Start()
    {
        // [새로운 로직] 스프라이트 시트에서 모든 카드 앞면 로드
        cardSprites = new Dictionary<string, Sprite>();
        // "Sprites/Cards/MyCardSheet" 부분은 실제 파일 경로와 이름으로 변경하세요.
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Cards/CuteCards");

        foreach (Sprite sprite in sprites)
        {
            // 딕셔너리에 '스프라이트 이름'을 Key로, '스프라이트 데이터'를 Value로 저장
            cardSprites.Add(sprite.name, sprite);
        }

        // UI 버튼 리스너 등록 (코드로 하는 방법)
        hitButton.onClick.AddListener(() => Hit());
        standButton.onClick.AddListener(() => Stand());
        restartButton.onClick.AddListener(() => StartGame());

        StartGame();
    }

    public void StartGame()
    {
        // 이전 게임의 카드 오브젝트들 파괴
        foreach (GameObject card in playerCardObjects) Destroy(card);
        foreach (GameObject card in dealerCardObjects) Destroy(card);
        playerCardObjects.Clear();
        dealerCardObjects.Clear();

        deck = new Deck();
        deck.CreateDeck();
        deck.Shuffle();

        playerHand = new List<Card>();
        dealerHand = new List<Card>();

        resultText.text = "";
        hitButton.interactable = true;
        standButton.interactable = true;
        restartButton.gameObject.SetActive(false);

        DealCardToPlayer();
        DealCardToDealer(true);
        DealCardToPlayer();
        DealCardToDealer(false);

        UpdateScores();
    }

    // 플레이어에게 카드 분배
    void DealCardToPlayer()
    {
        Card newCard = deck.Deal();
        playerHand.Add(newCard);

        GameObject newCardObject = Instantiate(cardPrefab, playerHandTransform);
        playerCardObjects.Add(newCardObject);

        // 위치 정렬
        newCardObject.transform.localPosition = new Vector3(cardSpacing * (playerCardObjects.Count - 1), 0, 0);

        // 올바른 카드 이미지로 변경
        newCardObject.GetComponent<SpriteRenderer>().sprite = GetCardSprite(newCard);
    }

    // 딜러에게 카드 분배
    void DealCardToDealer(bool isFaceUp)
    {
        Card newCard = deck.Deal();
        dealerHand.Add(newCard);

        GameObject newCardObject = Instantiate(cardPrefab, dealerHandTransform);
        dealerCardObjects.Add(newCardObject);

        // 위치 정렬
        newCardObject.transform.localPosition = new Vector3(cardSpacing * (dealerCardObjects.Count - 1), 0, 0);

        // 공개 여부에 따라 이미지 변경
        if (isFaceUp)
        {
            newCardObject.GetComponent<SpriteRenderer>().sprite = GetCardSprite(newCard);
        }
        else
        {
            //cardBackSprite 변수 대신 딕셔너리에서 직접 찾아 할당합니다.
            newCardObject.GetComponent<SpriteRenderer>().sprite = cardSprites["Card_Back"];
        }
    }

    // 카드 데이터에 맞는 스프라이트 찾아오기
    Sprite GetCardSprite(Card card)
    {
        // 카드 데이터(suit, rank)를 조합하여 스프라이트 시트에서 사용한 이름 만들기
        string spriteName = $"{card.suit}_{card.rank}";

        // 딕셔너리에서 해당 이름의 스프라이트가 있는지 확인
        if (cardSprites.ContainsKey(spriteName))
        {
            // 있다면 해당 스프라이트 반환
            return cardSprites[spriteName];
        }
        else
        {
            // 없다면 오류 메시지 출력 및 null 반환
            Debug.LogError($"스프라이트를 찾을 수 없습니다: {spriteName}");
            return null;
        }
    }

    public void Hit()
    {
        DealCardToPlayer(); // 기존 로직을 이 함수 호출로 대체
        UpdateScores();

        if (CalculateHandValue(playerHand) > 21)
        {
            EndGame("Player Busts! Dealer Wins.");
        }
    }

    public void Stand()
    {
        hitButton.interactable = false;
        standButton.interactable = false;

        // 딜러의 숨겨진 카드 공개
        dealerCardObjects[1].GetComponent<SpriteRenderer>().sprite = GetCardSprite(dealerHand[1]);
        UpdateScores(); // 공개 후 점수 다시 업데이트

        // 딜러 턴
        while (CalculateHandValue(dealerHand) < 17)
        {
            DealCardToDealer(true); // 딜러는 공개된 카드를 받음
            UpdateScores();
        }

        DetermineWinner();
    }

    void DetermineWinner()
    {
        int playerScore = CalculateHandValue(playerHand);
        int dealerScore = CalculateHandValue(dealerHand);

        if (dealerScore > 21 || playerScore > dealerScore)
        {
            EndGame("Player Wins!");
        }
        else if (playerScore < dealerScore)
        {
            EndGame("Dealer Wins!");
        }
        else
        {
            EndGame("Push! (Draw)");
        }
    }

    void EndGame(string message)
    {
        resultText.text = message;
        hitButton.interactable = false;
        standButton.interactable = false;
        restartButton.gameObject.SetActive(true);
    }

    void UpdateScores()
    {
        playerScoreText.text = "Player: " + CalculateHandValue(playerHand);
        dealerScoreText.text = "Dealer: " + CalculateHandValue(dealerHand);
    }

    int CalculateHandValue(List<Card> hand)
    {
        int value = 0;
        int aceCount = 0;
        foreach (Card card in hand)
        {
            value += card.value;
            if (card.rank == "A")
            {
                aceCount++;
            }
        }

        // Ace 처리 (11 또는 1)
        while (value > 21 && aceCount > 0)
        {
            value -= 10;
            aceCount--;
        }
        return value;
    }
}
