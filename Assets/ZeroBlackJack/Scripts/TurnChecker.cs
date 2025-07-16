using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnChecker : MonoBehaviour
{
    [SerializeField] private WinnerChecker winnerChecker;
    [SerializeField] private Hands hands1;
    [SerializeField] private Hands hands2;
    [SerializeField] private GameObject hitObject;
    [SerializeField] private GameObject standObject;
    [SerializeField] private GameObject replayButtonObject;
    [SerializeField] private TMP_Text turnText;

    private bool hands1Turn;
    private bool hands2Turn;
    private Button hitButton;
    private Button standButton;
    private Button replayButton;

    void Awake()
    {
        hitButton = hitObject.GetComponent<Button>();
        standButton = standObject.GetComponent<Button>();
        replayButton = replayButtonObject.GetComponent<Button>();
    }

    void Start()
    {
        TurnInit();
    }

    void Update()
    {
        PlayingChecker();
    }

    public void TurnChange()
    {
        hands1Turn = !hands1Turn;
    }

    private void PlayingChecker()
    {
        if (winnerChecker.isPlaying)
        {
            if (!hitObject.activeInHierarchy) hitObject.SetActive(true);
            if (!standObject.activeInHierarchy) standObject.SetActive(true);
            if (replayButtonObject.activeInHierarchy) replayButtonObject.SetActive(false);
        }
        else
        {
            if (hitObject.activeInHierarchy) hitObject.SetActive(false);
            if (standObject.activeInHierarchy) standObject.SetActive(false);
            if (replayButtonObject.activeInHierarchy) replayButtonObject.SetActive(true);
        }
    }

    private void TurnInit()
    {
        hands1Turn = true;
        HandsSet(hands1);
        HandsSet(hands2);
        hitButton.onClick.AddListener(hands1.HitButton);
    }

    private void HandsSet(Hands hands)
    {
        hands.Hit();
        hands.Hit();
        hands.ScoreUpdate();
    }
}
