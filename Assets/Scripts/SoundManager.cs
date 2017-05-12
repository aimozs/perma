using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
	public bool debugAudio;

	public AudioClip rain;
	public AudioClip thunder;
	public AudioClip wind;
	public AudioClip cicadas;
	public AudioClip birds;
	public AudioClip shovel;

	private static AudioSource audioSource;
	// Use this for initialization

	private static SoundManager instance;
	public static SoundManager Instance{
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<SoundManager>();
			}
			return instance;
		}
	}

	#region 0.Basics

	void Awake(){
		audioSource = GetComponent<AudioSource>();
	}

//	void Start(){}

//	void Update(){}

	#endregion

	#region 1.Statics
		public static void PlaySound(AudioClip sound) {
			audioSource.PlayOneShot(sound);
		}

	public static void SetVolume(float vol){
			audioSource.volume = vol;
		}

	public static void PlayRain(){
			if(Instance.rain != null)
			PlaySound(Instance.rain);
	}

	public static void PlayShovel(){
		if(Instance.shovel != null)
			PlaySound(Instance.shovel);
	}

	public static void PlayStorm(){
		if(Instance.thunder != null)
			PlaySound(Instance.thunder);
		}

	public static void PlayCicadas(){
		if(Instance.cicadas != null)
			PlaySound(Instance.cicadas);
		}

	public static void PlayBirds(){
		if(Instance.birds != null)
			PlaySound(Instance.birds);
		}
	#endregion

	#region 2.Publics

	#endregion

	#region 3.Privates

	#endregion



	public IEnumerator TransitionToVolume(float volume){
		float elapsedTime = 0;

			while (elapsedTime < 3f) {
			audioSource.volume = Mathf.Lerp(audioSource.volume, volume, elapsedTime / 3f);
				elapsedTime += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
	}


}
