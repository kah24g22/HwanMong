using Photon.Realtime;
using UnityEngine;

public class BlackJackPlayer : MonoBehaviour
{
    private bool m_isBust = false;
    private bool m_isBlackJack = false;
    private Hand m_hand;
    [SerializeField] private int m_maxLife = 3; // 라이프 최대치 변수 추가
    private int m_life; // 현재 라이프 변수 (초기화는 Awake에서)
    public bool IsStandDisabled { get; set; } // 스탠드 금지 상태 변수 추가

    public bool IsTurnOn;
    public Player Player;

    public bool IsBust { get { return m_isBust; } }
    public bool IsBlackJack { get { return m_isBlackJack; } }
    public Hand Hand { get { return m_hand; } }
    public int Life { get { return m_life; } } // 라이프 속성 추가됨
    public int MaxLife { get { return m_maxLife; } } // 라이프 최대치 속성 추가

    private void Awake()
    {
        m_hand = new Hand();
        m_life = m_maxLife; // Awake에서 현재 라이프를 최대치로 초기화
    }

    public void PlayerReset()
    {
        if (Hand == null)
            m_hand = new Hand();

        m_isBlackJack = false;
        m_isBust = false;
        IsTurnOn = true;
        Hand.Clear();
        IsStandDisabled = false; // 라운드 시작 시 스탠드 금지 상태 초기화
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

    // 라이프 증가 메서드 추가됨
    public void IncreaseLife()
    {
        if (m_life < m_maxLife)
        {
            m_life++;
        }
    }

}

