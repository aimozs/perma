using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
	public bool debugAudio;

	public AudioSource audioSource;

	public AudioClip rain;
	public AudioClip thunder;
	public AudioClip wind;
	public AudioClip cicadas;
	public AudioClip birds;


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

	public void PlaySound(AudioClip sound) {
		audioSource.PlayOneShot(sound);
	}

	public IEnumerator TransitionToVolume(float volume){
		float elapsedTime = 0;

			while (elapsedTime < 3f) {
			audioSource.volume = Mathf.Lerp(audioSource.volume, volume, elapsedTime / 3f);
				elapsedTime += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
	}

	public void PlayRain(){
		if(rain != null)
			PlaySound(rain);
	}

	public void PlayStorm(){
		if(thunder != null)
			PlaySound(thunder);
	}

	public void PlayCicadas(){
		if(cicadas != null)
			PlaySound(cicadas);
	}

	public void PlayBirds(){
		if(birds != null)
			PlaySound(birds);
	}
}
