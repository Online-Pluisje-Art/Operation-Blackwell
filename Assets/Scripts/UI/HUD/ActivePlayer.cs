using UnityEngine;
using UnityEngine.UI;
using OperationBlackwell.Core;

namespace OperationBlackwell.UI {
	public class ActivePlayer : MonoBehaviour {
		[SerializeField] private Image activePlayer_;

		private void Start() {
			GridCombatSystem.Instance.OnUnitDeselect += HidePlayer;
			GridCombatSystem.Instance.OnUnitSelect += ShowPlayer;
		}

		private void OnDestroy() {
			GridCombatSystem.Instance.OnUnitDeselect -= HidePlayer;
			GridCombatSystem.Instance.OnUnitSelect -= ShowPlayer;
		}

		private void ShowPlayer(object sender, GridCombatSystem.UnitEvent args) {
			if(GridCombatSystem.Instance.GetActiveUnit() != null) {
				if(GridCombatSystem.Instance.GetActiveUnit().GetName() == "Adam") {
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
