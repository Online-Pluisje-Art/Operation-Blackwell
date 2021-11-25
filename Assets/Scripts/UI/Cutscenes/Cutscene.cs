using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.UI {
	[CreateAssetMenu(fileName = "New Cutscene", menuName = "Cutscenes/Cutscene")]
	public class Cutscene : ScriptableObject {
		[System.Serializable]
		public struct CharacterMessage {
			public CutsceneCharacter character;
			public string message;
		}
		
		[SerializeField] private CutsceneCharacter characterLeft_;
		[SerializeField] private CutsceneCharacter characterRight_;
		[SerializeField] private List<CharacterMessage> messages_;

		public CharacterMessage GetMessageObject(int index) {
			return messages_[index];
		}
	}
}