using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Sound Event Channel")]
public class SoundEventChannelSO : ScriptableObject
{
    public UnityAction<AudioClip> OnSoundRequested;

    public void RaiseEvent(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundEventChannel: AudioClip is null!");
            return;
        }

        OnSoundRequested?.Invoke(clip);
    }
}
