using UnityEngine;
using UnityEngine.UI;
using OperationBlackwell.Core;
using OperationBlackwell.Puzzles;

namespace OperationBlackwell.UI {
	public class HUDController : MonoBehaviour {

		[SerializeField] private Text actionPointsText_;
		[SerializeField] private GameObject hud_;

		private void Start() {
			GridCombatSystem.instance.OnUnitActionPointsChanged += UpdateActionPoints;
			CutsceneController.instance.OnCutsceneStart += HideHUD;
			CutsceneController.instance.OnCutsceneEnd += ShowHUD;
			PuzzleController.instance.PuzzleStarted += HideHUD;
			GameController.instance.PuzzleEnded += ShowHUD;
		}

		private void OnDestroy() {
			GridCombatSystem.instance.OnUnitActionPointsChanged -= UpdateActionPoints;
			CutsceneController.instance.OnCutsceneStart -= HideHUD;
			CutsceneController.instance.OnCutsceneEnd -= ShowHUD;
			PuzzleController.instance.PuzzleStarted -= HideHUD;
			GameController.instance.PuzzleEnded -= ShowHUD;
		}

		private void UpdateActionPoints(object sender, GridCombatSystem.UnitEvent args) {
			if(args.unit != null) {
				actionPointsText_.text = args.unit.GetActionPoints().ToString();
			} else {
				actionPointsText_.text = "0";
			}
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
