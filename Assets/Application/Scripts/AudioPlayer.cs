using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    public AudioClip[] audioClips;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }



    public void Stop()
    {
        audioSource.Stop();
    }

    public void Play(AudioClip clip)
    {
        if (clip == null)
            return;

        audioSource.clip = clip;
        audioSource.Play();
    }
}