using TMPro;
using UnityEngine;

public class WinnerChecker : MonoBehaviour
{
    [SerializeField] private GameObject hands1;
    [SerializeField] private GameObject hands2;
    [SerializeField] private TMP_Text winnerText;

    public bool isPlaying;

    private Hands hands1Component;
    private Hands hands2Component;

    void Awake()
    {
        hands1Component = hands1.GetComponent<Hands>();
        hands2Component = hands2.GetComponent<Hands>();
        CheckerInit();
    }

    void Update()
    {
        BlackjackBustsFlagCheck();
    }

    private void CheckerInit()
    {
        winnerText.text = "";
        isPlaying = true;
    }

    private void WinnerAnnouncement(string winnerName)
    {
        isPlaying = false;
        winnerText.text = $"{winnerName} Win!";
    }

    private void BlackjackBustsFlagCheck()
    {
        if (hands1Component.isBlackjack) WinnerAnnouncement(hands1.name);
        else if (hands2Component.isBlackjack) WinnerAnnouncement(hands2.name);
        else if (hands1Component.isBust) WinnerAnnouncement(hands2.name);
        else if (hands2Component.isBust) WinnerAnnouncement(hands1.name);
    }
}
