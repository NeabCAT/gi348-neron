using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Option_ESC : MonoBehaviour
{
    public GameObject uiOption;
    public GameObject uiESC;

    public Slider Music_Vol;
    public Slider SFX_Vol;
    public AudioMixer mainAuio;

    void Start()
    {
        float music = PlayerPrefs.GetFloat("Music_Vol", 1f);
        float sfx = PlayerPrefs.GetFloat("SFX_Vol", 1f);

        Music_Vol.value = music;
        SFX_Vol.value = sfx;

        float musicdB = Mathf.Log10(Mathf.Max(0.0001f, music)) * 20;
        float sfxdB = Mathf.Log10(Mathf.Max(0.0001f, sfx)) * 20;

        mainAuio.SetFloat("Music_Vol", musicdB);
        mainAuio.SetFloat("SFX_Vol", sfxdB);
    }

    void Update()
    {
        ESCOpen();
    }

    public void Option()
    {
        uiOption.SetActive(true);
    }

    public void Back()
    {
        Time.timeScale = 1f;
        uiOption.SetActive(false);
    }

    public void ChangeMusicVolume()
    {
        float value = Music_Vol.value;

        float dB = Mathf.Log10(Mathf.Max(0.0001f, Music_Vol.value)) * 20;
        mainAuio.SetFloat("Music_Vol", dB);

        PlayerPrefs.SetFloat("Music_Vol", value);
    }
    public void ChangeSFXVolume()
    {
        float value = SFX_Vol.value;

        float dB = Mathf.Log10(Mathf.Max(0.0001f, SFX_Vol.value)) * 20;
        mainAuio.SetFloat("SFX_Vol", dB);

        PlayerPrefs.SetFloat("SFX_Vol", value);
    }

    public void ESCOpen()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0f;
            uiESC.SetActive(true);
        }
        
    }

    public void ESCBack()
    {
        Time.timeScale = 1f;
        uiESC.SetActive(false);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
