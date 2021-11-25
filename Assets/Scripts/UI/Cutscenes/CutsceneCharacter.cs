using UnityEngine;

namespace OperationBlackwell.UI {
	[CreateAssetMenu(fileName = "New Character", menuName = "Cutscenes/Character")]
	public class CutsceneCharacter : ScriptableObject {
		[SerializeField] private string characterName_;
		[SerializeField] private Sprite characterSprite_;
		[SerializeField] private Sprite characterSpriteSpeaking_;
		[SerializeField] private AudioClip characterVoice_;
		[SerializeField] private bool speaking_;

		public string GetName() {
			return characterName_;
		}

		public Sprite GetSprite() {
			return characterSprite_;
		}

		public Sprite GetSpriteSpeaking() {
			return characterSpriteSpeaking_;
		}

		public AudioClip GetVoice() {
			return characterVoice_;
		}

		public bool IsSpeaking() {
			return speaking_;
		}

		public void SetSpeaking(bool speaking) {
			speaking_ = speaking;
		}
	}
}
