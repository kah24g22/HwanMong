using Photon.Realtime;
using UnityEngine;

public class BlackJackPlayer : MonoBehaviour
{
    private bool m_isBust = false;
    private bool m_isBlackJack = false;
    private Hand m_hand;



    public bool IsTurnOn;
    public Player Player;

    public bool IsBust { get { return m_isBust; } }
    public bool IsBlackJack { get { return m_isBlackJack; } }
    public Hand Hand { get { return m_hand; } }

    private void Awake()
    {
        m_hand = new Hand();
    }

    public void PlayerReset()
    {
        if (Hand == null)
            m_hand = new Hand();

        m_isBlackJack = false;
        m_isBust = false;
        IsTurnOn = true;
        Hand.Clear();
    }

    public void BustCheck()
    {
        m_isBust = Hand.IsBust();
        if (m_isBust)
        {
            IsTurnOn = false;
        }
    }
    public void BlackJackCheck()
    {
        m_isBlackJack = Hand.IsBlackJack();
    }

}

