using UnityEngine;

namespace OperationBlackwell.LevelTransitions {
	[CreateAssetMenu(fileName = "New Object", menuName = "Levels/TransitionObject")]
	public class Object : ScriptableObject {
		[SerializeField] private Sprite image_;
		[SerializeField] private string name_;
		[SerializeField][TextArea] private string description_;

		public Sprite GetSprite() {
			return image_;
		}

		public string GetName() {
			return name_;
		}

		public string GetDescription() {
			return description_;
		}
	}
}
