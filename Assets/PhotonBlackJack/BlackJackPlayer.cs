using Photon.Realtime;
using UnityEngine;

public class BlackJackPlayer : MonoBehaviour
{
    private bool m_isBust = false;
    private bool m_isBlackJack = false;
    private Hand m_hand;
    private int m_life = 3; // 라이프 변수 추가됨



    public bool IsTurnOn;
    public Player Player;

    public bool IsBust { get { return m_isBust; } }
    public bool IsBlackJack { get { return m_isBlackJack; } }
    public Hand Hand { get { return m_hand; } }
    public int Life { get { return m_life; } } // 라이프 속성 추가됨

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

    // 라이프 감소 메서드 추가됨
    public void DecreaseLife()
    {
        m_life--;
    }

}

