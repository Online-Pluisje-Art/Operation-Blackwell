using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace OperationBlackwell.Core {
    public class VolumeControl : MonoBehaviour {
        
        public AudioMixer mixer;

        public void SetFloat(float value) {
            mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        }

    }
}
