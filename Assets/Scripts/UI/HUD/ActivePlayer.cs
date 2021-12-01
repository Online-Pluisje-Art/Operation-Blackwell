using UnityEngine;
using UnityEngine.UI;
using OperationBlackwell.Core;

namespace OperationBlackwell.UI {
	public class ActivePlayer : MonoBehaviour {
		[SerializeField] private Image activePlayer_;

		private void Update() {
			if(GridCombatSystem.Instance.GetState() == GridCombatSystem.State.UnitSelected) {
				if(!activePlayer_.gameObject.activeSelf) {
					if(GridCombatSystem.Instance.GetActiveUnit() != null) {
						if(GridCombatSystem.Instance.GetActiveUnit().name == "Adam") {
							activePlayer_.sprite = Resources.Load<Sprite>("Textures/Adam/AdamCutscene");
							activePlayer_.gameObject.SetActive(true);
						}
					}
				}
			} else {
				activePlayer_.gameObject.SetActive(false);
			}
		}
	}
}
