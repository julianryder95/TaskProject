using System;
using UnityEngine;

/// <summary>
/// Audio playback manager for procedure narration and operator sound cues.
/// Supports indexed and name-based audio clip playback.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;

    private AudioSource audioSource;

    private void Awake()
    {
        EnsureAudioSource();
    }

    private void EnsureAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    public void Stop()
    {
        EnsureAudioSource();
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void Play(AudioClip clip)
    {
        if (clip == null)
            return;

        EnsureAudioSource();
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    public void PlayClip(int index)
    {
        if (audioClips == null || index < 0 || index >= audioClips.Length)
            return;

        Play(audioClips[index]);
    }

    public void PlayClip(string clipName)
    {
        if (audioClips == null || string.IsNullOrWhiteSpace(clipName))
            return;

        foreach (var clip in audioClips)
        {
            if (clip != null && string.Equals(clip.name, clipName, StringComparison.OrdinalIgnoreCase))
            {
                Play(clip);
                return;
            }
        }
    }
}