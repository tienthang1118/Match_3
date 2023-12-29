using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
	#region Fields

	[Header("Audio Sources")]
	[Space]
	[SerializeField]
	protected AudioSource m_MusicAudioSource;
	[SerializeField]
	protected AudioSource m_InteractAudioSource;
	[SerializeField]
	protected AudioSource m_GameAudioSource;

	[Header("Music Clips")]
	[Space]
	[SerializeField]
	protected AudioClip m_MusicClip;

	[Header("Sound Clips")]
	[Space]
	[SerializeField]
	protected AudioClip m_SwapSound;
	[SerializeField]
	protected AudioClip m_SwapErrorSound;
	[SerializeField]
	protected AudioClip m_ClearPieceSound;
	[SerializeField]
	protected AudioClip m_LoseSound;
	[SerializeField]
	protected AudioClip m_WinSound;
	[SerializeField]
	protected AudioClip m_UIButtonSound;

	#endregion
	#region Methods

	public void PlayMusic()
	{
		m_MusicAudioSource.clip = m_MusicClip;
		m_MusicAudioSource.Play();
	}
	public void StopMusic()
    {
		m_MusicAudioSource.Stop();
	}
	public void PlaySoundOn(AudioSource audio, AudioClip clip)
	{
		audio.clip = clip;
		audio.Play();
	}

	public void PlayUIButtonSound()
	{
		PlaySoundOn(m_InteractAudioSource, m_UIButtonSound);
	}

	public void PlaySwapSound()
	{
		PlaySoundOn(m_InteractAudioSource, m_SwapSound);
	}
	public void PlaySwapErrorSound()
	{
		PlaySoundOn(m_InteractAudioSource, m_SwapErrorSound);
	}
	public void PlayClearPieceSound()
	{
		PlaySoundOn(m_GameAudioSource, m_ClearPieceSound);
	}
	public void PlayWinSound()
	{
		PlaySoundOn(m_GameAudioSource, m_WinSound);
	}
	public void PlayLoseSound()
	{
		PlaySoundOn(m_GameAudioSource, m_LoseSound);
	}

	#endregion

}