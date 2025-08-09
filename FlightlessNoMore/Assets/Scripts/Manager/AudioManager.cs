using UnityEngine;
using UnityEngine.Audio;
using Cysharp.Threading.Tasks;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource rocketLoopSource;
    [SerializeField] private AudioSource carLoopSource;
    [SerializeField] private AudioSource flyLoopSource;
    [SerializeField] private AudioSource waterLoopSource; // 水中ループ用のAudioSource

    [Header("Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private SoundData[] bgmSounds; // BGM用のSoundData
    [SerializeField] private SoundData[] sfxSounds; // SFX用のSound

    // --- 初期化関連キー ---
    private const string KEY_INIT = "AudioInitialized";
    private const string KEY_BGM = "BGMVolume";
    private const string KEY_SFX = "SFXVolume";
    private const float DEFAULT_BGM = 0.5f;
    private const float DEFAULT_SFX = 0.5f;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // ★ 初回起動時だけデフォルト値に設定
        InitializeAudioPrefsIfFirstLaunch();

        // 保存された音量設定を適用
        SetBGMVolume(PlayerPrefs.GetFloat(KEY_BGM, DEFAULT_BGM));
        SetSFXVolume(PlayerPrefs.GetFloat(KEY_SFX, DEFAULT_SFX));

        // 安全ガード付き自動再生
        if (bgmSource != null && bgmSounds != null && bgmSounds.Length > 0)
        {
            var first = bgmSounds[0];
            if (!bgmSource.isPlaying && !string.IsNullOrEmpty(first.soundName) && first.clip != null)
            {
                PlayBGM(first.soundName);
            }
        }
    }
    public void SetBGMVolume(float volume)
    {
        float v = Mathf.Clamp01(volume);
        float dB = Mathf.Log10(Mathf.Max(v, 0.0001f)) * 20f;

        if (audioMixer != null)
        {
            if (!MixerHasParam("BGMVolume"))
                Debug.LogWarning("[Audio] Mixerに 'BGMVolume' がExposedされていません");
            else
                audioMixer.SetFloat("BGMVolume", dB);
        }

        PlayerPrefs.SetFloat(KEY_BGM, v);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        float v = Mathf.Clamp01(volume);
        float dB = Mathf.Log10(Mathf.Max(v, 0.0001f)) * 20f;

        if (audioMixer != null)
        {
            if (!MixerHasParam("SFXVolume"))
                Debug.LogWarning("[Audio] Mixerに 'SFXVolume' がExposedされていません");
            else
                audioMixer.SetFloat("SFXVolume", dB);
        }

        PlayerPrefs.SetFloat(KEY_SFX, v);
        PlayerPrefs.Save();
    }

    public void PlayBGM(string soundName)
    {
        foreach (var sound in bgmSounds)
        {
            if (sound.soundName == soundName)
            {
                if (sound.clip == null)
                {
                    Debug.LogError($"BGM '{soundName}' は見つかったが clip が未設定です");
                    return;
                }
                bgmSource.clip = sound.clip;
                bgmSource.volume = sound.volume;
                bgmSource.loop = sound.loop;
                bgmSource.Play();
                return;
            }
        }
        Debug.LogWarning($"BGM '{soundName}' が見つかりません");
    }

    // SFXをPlay/Stop方式で再生・停止する
    public void PlaySFX(string soundName, float pitch = 1f, float startTime = 0f)
    {
        foreach (var sound in sfxSounds)
        {
            if (sound.soundName == soundName)
            {
                if (sound.clip == null)
                {
                    Debug.LogError($"SFX '{soundName}' は見つかったが clip が未設定です");
                    return;
                }
                if (sfxSource == null)
                {
                    Debug.LogError("sfxSource が未割り当てです");
                    return;
                }

                sfxSource.pitch = pitch;
                sfxSource.clip = sound.clip;
                sfxSource.volume = sound.volume;
                sfxSource.loop = false;
                sfxSource.time = Mathf.Clamp(startTime, 0f, sound.clip.length);
                sfxSource.Play();
                return;
            }
        }
        Debug.LogWarning($"SFX '{soundName}' が見つかりません");
    }


    public void StopSFX(string soundName)
    {
        foreach (var sound in sfxSounds)
        {
            if (sound.soundName == soundName)
            {
                if (sfxSource.isPlaying && sfxSource.clip == sound.clip)
                {
                    sfxSource.Stop();
                }
                return;
            }
        }
    }

    public async void PlayRocketLoopSFX(string soundName)
    {
        foreach (var sound in sfxSounds)
        {
            if (sound.soundName == soundName)
            {
                if (sound.clip == null)
                {
                    Debug.LogError($"RocketLoopSFX '{soundName}' は clip 未設定です");
                    return;
                }

                // ★ 同一クリップ再生中なら無視
                if (rocketLoopSource.isPlaying && rocketLoopSource.clip == sound.clip) return;

                rocketLoopSource.clip = sound.clip;
                rocketLoopSource.loop = true;
                rocketLoopSource.volume = 0f;
                rocketLoopSource.Play();
                await FadeRocketLoopVolume(0f, sound.volume, 0.2f);
                return;
            }
        }
        Debug.LogWarning($"ループSFX '{soundName}' が見つかりません");
    }

    public async void StopRocketLoopSFX()
    {
        if (rocketLoopSource.isPlaying)
        {
            float currentVolume = rocketLoopSource.volume;
            await FadeRocketLoopVolume(currentVolume, 0f, 0.2f);
            rocketLoopSource.Stop();
        }
    }

    private async UniTask FadeRocketLoopVolume(float from, float to, float duration)
    {
        float elapsed = 0f;
        rocketLoopSource.volume = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rocketLoopSource.volume = Mathf.Lerp(from, to, elapsed / duration);
            await UniTask.Yield();
        }
        rocketLoopSource.volume = to;
    }

    public void PlayCarLoopSFX(string soundName)
    {
        foreach (var sound in sfxSounds)
        {
            if (sound.soundName == soundName)
            {
                if (sound.clip == null)
                {
                    Debug.LogError($"CarLoopSFX '{soundName}' は clip 未設定です");
                    return;
                }
                if (carLoopSource.isPlaying && carLoopSource.clip == sound.clip) return;

                carLoopSource.clip = sound.clip;
                carLoopSource.volume = sound.volume;
                carLoopSource.loop = true;
                carLoopSource.Play();
                return;
            }
        }
        Debug.LogWarning($"CarLoopSFX '{soundName}' が見つかりません");
    }

    public void StopCarLoopSFX()
    {
        if (carLoopSource.isPlaying)
        {
            carLoopSource.Stop();
        }
    }

    public void SetCarLoopVolume(float volume)
    {
        carLoopSource.volume = Mathf.Clamp01(volume);
    }

    public void PlayFlyLoopSFX(string soundName)
    {
        foreach (var sound in sfxSounds)
        {
            if (sound.soundName == soundName)
            {
                if (sound.clip == null)
                {
                    Debug.LogError($"FlyLoopSFX '{soundName}' は clip 未設定です");
                    return;
                }
                if (flyLoopSource.isPlaying && flyLoopSource.clip == sound.clip) return;

                flyLoopSource.clip = sound.clip;
                flyLoopSource.volume = sound.volume;
                flyLoopSource.loop = true;
                flyLoopSource.Play();
                return;
            }
        }
        Debug.LogWarning($"FlyLoopSFX '{soundName}' が見つかりません");
    }

    public void StopFlyLoopSFX()
    {
        if (flyLoopSource.isPlaying)
        {
            flyLoopSource.Stop();
        }
    }

    public void SetFlyLoopVolume(float volume)
    {
        flyLoopSource.volume = Mathf.Clamp01(volume);
    }

    public void PlayWaterLoopSFX(string soundName)
    {
        foreach (var sound in sfxSounds)
        {
            if (sound.soundName == soundName)
            {
                if (sound.clip == null)
                {
                    Debug.LogError($"WaterLoopSFX '{soundName}' は clip 未設定です");
                    return;
                }
                if (waterLoopSource == null)
                {
                    Debug.LogError("waterLoopSource が未割り当てです");
                    return;
                }

                if (waterLoopSource.isPlaying && waterLoopSource.clip == sound.clip) return;

                waterLoopSource.clip = sound.clip;
                waterLoopSource.volume = sound.volume;
                waterLoopSource.loop = true;
                waterLoopSource.Play();
                return;
            }
        }
        Debug.LogWarning($"WaterLoopSFX '{soundName}' が見つかりません");
    }
    public void StopWaterLoopSFX()
    {
        if (waterLoopSource.isPlaying)
        {
            waterLoopSource.Stop();
        }
    }
    public void PauseAllAudio()
    {
        bgmSource?.Pause();
        sfxSource?.Pause();
        rocketLoopSource?.Pause();
        carLoopSource?.Pause();
        flyLoopSource?.Pause();
        waterLoopSource?.Pause();
    }

    public void ResumeAllAudio()
    {
        bgmSource?.UnPause();
        sfxSource?.UnPause();
        rocketLoopSource?.UnPause();
        carLoopSource?.UnPause();
        flyLoopSource?.UnPause();
        waterLoopSource?.UnPause();
    }

    public void StopAllLoopSFX()
    {
        rocketLoopSource?.Stop();
        carLoopSource?.Stop();
        flyLoopSource?.Stop();
        waterLoopSource?.Stop();
    }
    private void InitializeAudioPrefsIfFirstLaunch()
    {
        if (!PlayerPrefs.HasKey(KEY_INIT))
        {
            PlayerPrefs.SetFloat(KEY_BGM, DEFAULT_BGM);
            PlayerPrefs.SetFloat(KEY_SFX, DEFAULT_SFX);
            PlayerPrefs.SetInt(KEY_INIT, 1);
            PlayerPrefs.Save();
            Debug.Log("[Audio] 初回起動として音量を初期化しました");
        }
    }
    private bool MixerHasParam(string param)
    {
        if (audioMixer == null) return false;
        return audioMixer.GetFloat(param, out _);
    }
    [ContextMenu("Reset Audio Prefs (Next Start Initializes)")]
    private void ResetAudioPrefs()
    {
        PlayerPrefs.DeleteKey(KEY_BGM);
        PlayerPrefs.DeleteKey(KEY_SFX);
        PlayerPrefs.DeleteKey(KEY_INIT);
        PlayerPrefs.Save();
        Debug.Log("[Audio] Audio prefs reset. 次回 Start() で初期化されます。");
    }
}