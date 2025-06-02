using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Channel Reference")]
    [SerializeField] private SoundEventChannelSO sfxChannel;

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxAudioSource;

    private void OnEnable()
    {
        if (sfxChannel != null)
            sfxChannel.OnSoundRequested += PlaySound;
    }

    private void OnDisable()
    {
        if (sfxChannel != null)
            sfxChannel.OnSoundRequested -= PlaySound;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundManager: clip is null!");
            return;
        }

        sfxAudioSource.PlayOneShot(clip);
    }
}
