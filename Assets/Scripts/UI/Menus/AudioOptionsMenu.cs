using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace OperationBlackwell.UI {
	public class AudioOptionsMenu : Menu {

		[SerializeField] private Slider backgroundMusicSlider_;
		[SerializeField] private Slider soundEffectSlider_;
		[SerializeField] private AudioMixer backgroundMusicMixer_;
		[SerializeField] private AudioMixer soundEffectMixer_;

		private void Start() {
			UpdateSliders();
		}

		public override void ReturnOrExit() {
			audioOptionsMenu_.SetActive(false);
			optionsMenu_.SetActive(true);
		}

		private void UpdateSliders() {
			float backgroundMusicVolume;
			backgroundMusicMixer_.GetFloat("MusicVolume", out backgroundMusicVolume);
			backgroundMusicSlider_.value = Mathf.Pow(10, backgroundMusicVolume / 20);

			float soundEffectVolume;
			soundEffectMixer_.GetFloat("MusicVolume", out soundEffectVolume);
			soundEffectSlider_.value = Mathf.Pow(10, soundEffectVolume / 20);
		}
	}
}
