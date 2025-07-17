using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] AudioClip[] sfxs;
    [SerializeField] AudioSource audioSfx;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SFX(string name)
    {
        AudioClip clip=GetClipByName(name);
        if(clip!=null)
        {
            audioSfx.PlayOneShot(clip);
            StartCoroutine(DelayedSceneLoad());
        }
    }

    AudioClip GetClipByName(string name)
    {
        foreach (AudioClip clip in sfxs)
        {
            if (clip.name == name) return clip;

        }
        return null;
    }

    IEnumerator DelayedSceneLoad()
    {
        yield return new WaitForSeconds(0.4f); // 소리 길이에 맞춰 지연
        SceneManager.LoadScene("SsuregiScene");
    }
}
