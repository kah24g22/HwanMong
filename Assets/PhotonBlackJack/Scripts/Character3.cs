using UnityEngine;
using UnityEngine.SceneManagement;

public class Character3 : MonoBehaviour
{
    [SerializeField] private BlackJackPlayer m_blackJackPlayer; // 해당 캐릭터의 BlackJackPlayer 참조

    public void OnClick()
    {
        CharacterData.instance.characterName = "Character3";
        CharacterData.instance.selectedBlackJackPlayer = m_blackJackPlayer; // BlackJackPlayer 인스턴스 저장
        SceneManager.LoadScene("SoloGame");
    }
}
