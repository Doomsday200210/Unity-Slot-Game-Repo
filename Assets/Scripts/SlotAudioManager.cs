using UnityEngine;

public class SlotAudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip spinButtonSound;
    [SerializeField] private AudioClip reelSpinSound;
    [SerializeField] private AudioClip jackpotSound;
    [SerializeField] private AudioClip smallWinSound;
    [SerializeField] private AudioClip loseSound;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource soundEffectSource;
    [SerializeField] private AudioSource reelSource;

    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float soundEffectVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float reelVolume = 0.5f;

    public void PlaySpinButtonSound()
    {
        if (soundEffectSource == null || spinButtonSound == null)
            return;

        soundEffectSource.PlayOneShot(
            spinButtonSound,
            soundEffectVolume
        );
    }

    public void PlayReelSpinSound()
    {
        if (reelSource == null || reelSpinSound == null)
            return;

        reelSource.clip = reelSpinSound;
        reelSource.loop = true;
        reelSource.volume = reelVolume;

        if (!reelSource.isPlaying)
        {
            reelSource.Play();
        }
    }

    public void StopReelSpinSound()
    {
        if (reelSource == null)
            return;

        reelSource.Stop();
        reelSource.loop = false;
        reelSource.clip = null;
    }

    public void PlayJackpotSound()
    {
        if (soundEffectSource == null || jackpotSound == null)
            return;

        soundEffectSource.PlayOneShot(
            jackpotSound,
            soundEffectVolume
        );
    }

    public void PlaySmallWinSound()
    {
        if (soundEffectSource == null || smallWinSound == null)
            return;

        soundEffectSource.PlayOneShot(
            smallWinSound,
            soundEffectVolume
        );
    }

    public void PlayLoseSound()
    {
        if (soundEffectSource == null || loseSound == null)
            return;

        soundEffectSource.PlayOneShot(
            loseSound,
            soundEffectVolume
        );
    }
}