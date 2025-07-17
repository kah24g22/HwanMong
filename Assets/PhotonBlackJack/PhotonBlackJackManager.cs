using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class PhotonBlackJackManager : MonoBehaviour
{
    private PhotonBlackJackLogic gameLogic; // �ٽ� ���� ���� �ν��Ͻ�

    [SerializeField] BlackJackPlayer m_player1;
    [SerializeField] BlackJackPlayer m_player2;

    // UI ��� ����
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI turnText; // �� ǥ�� UI �߰�
    [SerializeField] private Button hitButton;
    [SerializeField] private Button standButton;
    [SerializeField] private Button restartButton;

    // �ð�ȭ
    [Header("Card Visuals")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform player1HandTransform;
    [SerializeField] private Transform player2HandTransform;
    [SerializeField] private float cardSpacing = 0.5f;

    private List<GameObject> player1CardObjects = new List<GameObject>();
    private List<GameObject> player2CardObjects = new List<GameObject>();

    private Dictionary<string, Sprite> cardSprites;

    void Awake() // Start ��� Awake���� �ʱ�ȭ�Ͽ� �ٸ� ��ũ��Ʈ���� ���� ����ǵ��� �� �� �ֽ��ϴ�.
    {
        gameLogic = new PhotonBlackJackLogic(m_player1, m_player2);
        //gameLogic.OnCardDealt += HandleCardDealt;
        gameLogic.OnGameEnded += HandleGameEnded;
        gameLogic.OnTurnChanged += HandleTurnChanged; // �� ���� �̺�Ʈ ����

        // UI ��ư ������ ���
        hitButton.onClick.AddListener(() => Hit());
        standButton.onClick.AddListener(() => Stand());
        restartButton.onClick.AddListener(() => StartGame());
    }

    void Start()
    {
        // ��������Ʈ ��Ʈ���� ��� ī�� �ո� �ε�
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
        // ���� ������ ī�� ������Ʈ�� �ı�
        foreach (GameObject card in player1CardObjects) Destroy(card);
        foreach (GameObject card in player2CardObjects) Destroy(card);
        player1CardObjects.Clear();
        player2CardObjects.Clear();

        resultText.text = "";
        turnText.text = ""; // �� �ؽ�Ʈ �ʱ�ȭ
        hitButton.interactable = true;
        standButton.interactable = true;
        restartButton.gameObject.SetActive(false);

        //gameLogic.StartGame(); // �ٽ� ���� ���� ����
        UpdateScores(); // �ʱ� ���� ������Ʈ
    }

    private void HandleCardDealt(Card card, Hand hand, bool isFaceUp)
    {
        Transform parentTransform;
        List<GameObject> cardObjectsList;

        if (hand == gameLogic.Player1.Hand)
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
        UpdateScores(); // ī�尡 �й�� ������ ���� ������Ʈ
    }

    private void HandleGameEnded(string message)
    {
        resultText.text = message;
        turnText.text = "Game Over";
        hitButton.interactable = false;
        standButton.interactable = false;
        restartButton.gameObject.SetActive(true);
        UpdateScores(); // ���� ���� �� ���� ī�� ��� ����
    }

    private void HandleTurnChanged(PlayerTurn currentPlayer)
    {
        turnText.text = $"{currentPlayer}'s Turn";
    }

    void UpdateScores()
    {
        player1ScoreText.text = "Player1: " + gameLogic.Player1.Hand.CalculateValue();
        player2ScoreText.text = "Player2: " + gameLogic.Player2.Hand.CalculateValue();

        // ���� �� �÷��̾� ���� �Ǵ� UI ������Ʈ ���� �߰� ����
        // ��: if (blackjackGame.CurrentPlayer == BlackjackGame.PlayerTurn.Player1) { /* Player1 UI ���� */ }
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
            Debug.LogError($"��������Ʈ�� ã�� �� �����ϴ�: {spriteName}");
            return null;
        }
    }

    public void Hit()
    {
        gameLogic.Hit();
    }
    public void Stand()
    {
        gameLogic.Stand();
    }

    public void UseItem(ItemType itemType, BlackJackPlayer user)
    {
        IItemEffect effect = null;
        switch (itemType)
        {
            case ItemType.Envelope:
                effect = new EnvelopeEffect();
                break;
            case ItemType.Scale:
                effect = new ScaleEffect();
                break;
            case ItemType.Hourglass:
                effect = new HourglassEffect();
                break;
            // 다른 아이템들도 여기에 추가
            case ItemType.Match:
                effect = new MatchEffect();
                break;
            case ItemType.Lock:
                effect = new LockEffect();
                break;
            case ItemType.CrystalBall:
                effect = new CrystalBallEffect();
                break;
        }

        if (effect != null)
        {
            //effect.Execute(gameLogic, user);
            user.RemoveItem(itemType); // 아이템 사용 후 인벤토리에서 제거
        }
        else
        {
            Debug.LogWarning($"Unknown item type: {itemType}");
        }
    }

}
