using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	public AudioMixerGroup mixer;

	public SoundGroup[] soundGroupList;
	public Sound[] soundList;

	public static Dictionary<string, AudioSource> sounds = new();
	public static Dictionary<string, AudioSource[]> soundGroups = new();

	public static AudioManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this) { Destroy(gameObject); return; }
		else Instance = this;
		DontDestroyOnLoad(this);

		foreach (Sound sound in soundList)
		{
			GameObject g = new(sound.name);
			g.transform.SetParent(transform);

			AudioSource source = g.AddComponent<AudioSource>();

			source.clip = sound.clip;
			source.playOnAwake = false;
			source.loop = false;

			source.outputAudioMixerGroup = mixer;

			sounds.Add(sound.name, source);
		}

		foreach (SoundGroup group in soundGroupList)
		{
			List<AudioSource> sources = new();
			foreach (string name in group.soundNames) sources.Add(sounds[name]);
			soundGroups.Add(group.name, sources.ToArray());
		}
	}

	private void Start()
	{
		SetAudioMixerVolume(SaveManager.save.volume);
	}

	public static void SetAudioMixerVolume(float volume)
	{
		// https://en.wikipedia.org/wiki/Decibel#Uses
		// https://www.youtube.com/watch?v=V_Bf__ynKLE&themeRefresh=1
		AudioManager.Instance.mixer.audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
	}

	public static void Play(AudioSource audio)
	{
		Play(audio, Random.Range(0.9f, 1.0f));
	}

	public static void Play(AudioSource audio, float volume)
	{
		audio.clip = audio.clip;
		audio.volume = volume;
		// !!!!! only positive audio values!!!
		audio.pitch = Random.Range(1f, 1.05f);

		audio.Play();
	}

	public static void PlaySound(string sound)
	{
		PlaySound(sound, Random.Range(0.9f, 1.0f));
	}

	public static void PlaySound(string sound, float volume)
	{
		Play(AudioManager.sounds[sound], volume);
	}

	public static void PlaySoundGroup(string name)
	{
		PlaySoundGroup(name, Random.Range(0.9f, 1.0f));
	}
	public static void PlaySoundGroup(string name, float volume)
	{
		AudioSource[] sounds = AudioManager.soundGroups[name];
		AudioSource sound = sounds[Random.Range(0, sounds.Length)];
		Play(sound, volume);
	}


	[System.Serializable]
	public class Sound
	{
		public string name;
		public AudioClip clip;
	}
	[System.Serializable]
	public class SoundGroup
	{
		public string name;
		public string[] soundNames;
	}
}