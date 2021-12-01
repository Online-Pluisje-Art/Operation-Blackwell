using UnityEngine;

namespace OperationBlackwell.LevelTransitions {
	[CreateAssetMenu(fileName = "New Transition", menuName = "Levels/Transition")]
	public class LevelTransitions : ScriptableObject {
		[SerializeField] private Object startObject_;
		[SerializeField] private Object destinationObject_;
		[SerializeField] private Sprite plane_;
	}
}
