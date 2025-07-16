// 파일 경로: D:/School/Major/GameJam1/HwanMong/Assets/PhotonBlackJack/Items/ItemType.cs

/// <summary>
/// 게임에 존재하는 아이템의 종류를 정의하는 열거형입니다.
/// </summary>
public enum ItemType
{
    /// <summary>
    /// 아무 아이템도 아님을 나타냅니다. (기본값)
    /// </summary>
    None = 0,

    /// <summary>
    /// 상대방의 뒤집힌 카드 중 하나를 랜덤하게 제거하는 '성냥' 아이템입니다.
    /// </summary>
    Match = 1,

    /// <summary>
    /// 다음 턴 상대방의 Stand 행동을 금지하는 '자물쇠' 아이템입니다.
    /// </summary>
    Lock = 2,

    /// <summary>
    /// 덱의 다음 카드를 미리 보는 '수정 구슬' 아이템입니다.
    /// </summary>
    CrystalBall = 3,

    /// <summary>
    /// 다음 히트 카드를 상대방에게 보이지 않게 하는 '편지봉투' 아이템입니다.
    /// </summary>
    Envelope = 4,

    /// <summary>
    /// 현재 라이프를 1 증가시키는 '저울' 아이템입니다.
    /// </summary>
    Scale = 5,

    /// <summary>
    /// 즉시 새 라운드를 시작하는 '모래시계' 아이템입니다.
    /// </summary>
    Hourglass = 6,

    // 나중에 새로운 아이템을 추가할 때, 아래에 이어서 추가하시면 됩니다.
}
