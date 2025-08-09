using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Data/SoundData")]
public class SoundData : ScriptableObject
{
    public string soundName;        // 例: "jump", "bgm_title"
    public AudioClip clip;          // 音声素材
    [Range(0f, 1f)] public float volume = 1f;
    public bool loop = false;       // BGMはtrue、SEはfalse
}
