using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	[RequireComponent(typeof(AudioSource))]
	public class SoundController : MonoBehaviour {

		private AudioSource audioData_;
		
		private void Awake() {
			audioData_ = GetComponent<AudioSource>();
			audioData_.Play(0);
		}

	}
}
