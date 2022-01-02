using UnityEngine;
using UnityEngine.UI;
using OperationBlackwell.Core;

namespace OperationBlackwell.UI {
	public class ActivePlayer : MonoBehaviour {
		[SerializeField] private Image activePlayer_;

		private void Start() {
			GridCombatSystem.instance.OnUnitDeselect += HidePlayer;
			GridCombatSystem.instance.OnUnitSelect += ShowPlayer;
		}

		private void OnDestroy() {
			GridCombatSystem.instance.OnUnitDeselect -= HidePlayer;
			GridCombatSystem.instance.OnUnitSelect -= ShowPlayer;
		}

		private void ShowPlayer(object sender, GridCombatSystem.UnitEvent args) {
			if(GridCombatSystem.instance.GetActiveUnit() != null) {
				if(GridCombatSystem.instance.GetActiveUnit().GetName() == "Adam") {
					activePlayer_.sprite = Resources.Load<Sprite>("Textures/Adam/AdamCutscene");
					activePlayer_.gameObject.SetActive(true);
				}
			}
		}

		private void HidePlayer(object sender, System.EventArgs args) {
			activePlayer_.gameObject.SetActive(false);
		}
	}
}
