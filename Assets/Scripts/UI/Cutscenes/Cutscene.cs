using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.UI {
	[CreateAssetMenu(fileName = "New Cutscene", menuName = "Cutscenes/Cutscene")]
	public class Cutscene : ScriptableObject {
		[System.Serializable]
		public class CharacterMessage {
			public CutsceneCharacter character;
			[TextArea] public string message;
		}
		
		[SerializeField] private CutsceneCharacter characterLeft_;
		[SerializeField] private CutsceneCharacter characterRight_;
		[SerializeField] private List<CharacterMessage> messages_;

		public CharacterMessage GetMessageObject(int index) {
			if(index >= messages_.Count) {
				return null;
			}
			return messages_[index];
		}

		public int GetMessageCount() {
			return messages_.Count;
		}

		public CutsceneCharacter GetCharacterLeft() {
			return characterLeft_;
		}

		public CutsceneCharacter GetCharacterRight() {
			return characterRight_;
		}
	}
}
