using UnityEngine;
using UnityEngine.UI;
using OperationBlackwell.Core;
using OperationBlackwell.Puzzles;

namespace OperationBlackwell.UI {
	public class HUDController : MonoBehaviour {

		[SerializeField] private Text actionPointsText_;
		[SerializeField] private Text weaponText_;
		[SerializeField] private Text turnCounterText_;
		[SerializeField] private GameObject hud_;

		private void Start() {
			GridCombatSystem.instance.OnUnitActionPointsChanged += UpdateActionPoints;
			GridCombatSystem.instance.OnWeaponChanged += UpdateWeapon;
			GridCombatSystem.instance.OnTurnEnded += UpdateTurnCounter;
			CutsceneController.Instance.OnCutsceneStart += HideHUD;
			CutsceneController.Instance.OnCutsceneEnd += ShowHUD;
			PuzzleController.Instance.PuzzleStarted += HideHUD;
			GameController.Instance.PuzzleEnded += ShowHUD;
		}

		private void OnDestroy() {
			GridCombatSystem.instance.OnUnitActionPointsChanged -= UpdateActionPoints;
			GridCombatSystem.instance.OnWeaponChanged -= UpdateWeapon;
			GridCombatSystem.instance.OnTurnEnded -= UpdateTurnCounter;
			CutsceneController.Instance.OnCutsceneStart -= HideHUD;
			CutsceneController.Instance.OnCutsceneEnd -= ShowHUD;
			PuzzleController.Instance.PuzzleStarted -= HideHUD;
			GameController.Instance.PuzzleEnded -= ShowHUD;
		}

		private void UpdateWeapon(object sender, string name) {
			weaponText_.text = "Current Weapon: " + name;
		}

		private void UpdateActionPoints(object sender, GridCombatSystem.UnitEvent args) {
			if(args.unit != null) {
				actionPointsText_.text = args.unit.GetActionPoints().ToString();
			} else {
				actionPointsText_.text = "0";
			}
		}

		private void UpdateTurnCounter(object sender, int turn) {
			turnCounterText_.text = "Turn: " + turn.ToString();
		}

		private void ShowHUD(object sender, System.EventArgs args) {
			hud_.SetActive(true);
		}

		private void ShowHUD(object sender, int args) {
			hud_.SetActive(true);
		}

		private void HideHUD(object sender, System.EventArgs args) {
			hud_.SetActive(false);
		}
	}
}
