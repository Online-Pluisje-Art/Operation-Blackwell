using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using OperationBlackwell.Core;

namespace OperationBlackwell.UI {
	public class CutsceneController : BaseCutsceneController {
		public static CutsceneController Instance { get; private set; }

		[SerializeField] private Cutscene[] cutscenes_;
		private Cutscene currentCutscene_;
		private int currentCutsceneIndex_;
		private int currentMessageIndex_;

		private List<GameObject> children_;
		private GameObject base_;
		private GameObject leftButton_;
		private GameObject rightButton_;
		private GameObject endButton_;
		private GameObject leftChar_;
		private GameObject rightChar_;
		private GameObject leftName_;
		private GameObject rightName_;
		private GameObject speakingName_;
		private GameObject text_;

		private void Awake() {
			if (Instance == null) {
				Instance = this;
			} else {
				Destroy(gameObject);
				return;
			}
			children_ = new List<GameObject>();
		}

		private void Start() {
			currentCutscene_ = null;
			foreach(Transform child in transform) {
				children_.Add(child.gameObject);
			}
			base_ = children_[0];
			leftButton_ = children_[1];
			rightButton_ = children_[2];
			endButton_ = children_[3];
			leftChar_ = children_[4];
			rightChar_ = children_[5];
			leftName_ = children_[6];
			rightName_ = children_[7];
			speakingName_ = children_[8];
			text_ = children_[9];

			// This is here for testing now, need to remove this later and change the true to false once done.
			Show(0);
			SetActive(true);
		}

		public void Show(int index) {
			currentCutscene_ = cutscenes_[index];
			leftChar_.GetComponent<Image>().sprite = currentCutscene_.GetCharacterLeft().GetSprite();
			SetActive(true);
		}

		public void Hide() {
			currentCutscene_ = null;
			SetActive(false);
		}

		public override void StartCutscene() {
			Show(currentCutsceneIndex_);
		}

		public override void EndCutscene() {
			Hide();
			currentCutsceneIndex_++;
		}

		private void SetActive(bool active) {
			foreach(GameObject child in children_) {
				child.SetActive(active);
			}
		}

		private void SetChildActive(GameObject child, bool active) {
			child.SetActive(active);
		}

		public void SelectNextText() {

		}

		public void SelectPreviousText() {

		}

		public void EndButtonPressed() {
			EndCutscene();
		}
	}
}
