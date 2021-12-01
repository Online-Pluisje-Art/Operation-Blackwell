using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
		private Image leftChar_;
		private Image rightChar_;
		private TextMeshProUGUI leftName_;
		private TextMeshProUGUI rightName_;
		private TextMeshProUGUI speakingName_;
		private TextMeshProUGUI text_;

		private void Awake() {
			if(Instance == null) {
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
			leftChar_ = children_[4].GetComponent<Image>();
			rightChar_ = children_[5].GetComponent<Image>();
			leftName_ = children_[6].GetComponent<TextMeshProUGUI>();
			rightName_ = children_[7].GetComponent<TextMeshProUGUI>();
			speakingName_ = children_[8].GetComponent<TextMeshProUGUI>();
			text_ = children_[9].GetComponent<TextMeshProUGUI>();

			SetActive(false);
		}

		private void Show(int index) {
			currentCutscene_ = cutscenes_[index];
			currentMessageIndex_ = 0;
			SelectNextText();
			SetActive(true);
			SetChildActive(leftButton_, false);
			SetChildActive(endButton_, false);
		}

		private void Hide() {
			currentCutscene_ = null;
			SetActive(false);
		}

		public override void StartCutscene(int index) {
			GridCombatSystem.Instance.SetState(GridCombatSystem.State.Cutscene);
			currentCutsceneIndex_ = index;
			Show(index);
		}

		public override void EndCutscene() {
			GridCombatSystem.Instance.SetState(GridCombatSystem.State.OutOfCombat);
			Hide();
		}

		private void SetActive(bool active) {
			foreach(GameObject child in children_) {
				child.SetActive(active);
			}
		}

		private void SetChildActive(GameObject child, bool active) {
			child.SetActive(active);
		}

		private void SelectNextText() {
			if(currentCutscene_ == null) {
				return;
			}
			ResetSpeaking();
			Cutscene.CharacterMessage messageObject = currentCutscene_.GetMessageObject(currentMessageIndex_);
			if(messageObject == null) {
				return;
			}
			messageObject.character.SetSpeaking(true);
			leftChar_.sprite = currentCutscene_.GetCharacterLeft().GetSprite();
			leftName_.text = currentCutscene_.GetCharacterLeft().GetName();
			rightChar_.sprite = currentCutscene_.GetCharacterRight().GetSprite();
			rightName_.text = currentCutscene_.GetCharacterRight().GetName();
			speakingName_.text = messageObject.character.GetName();
			text_.text = messageObject.message;
		}

		private void ResetSpeaking() {
			if(currentCutscene_ == null) {
				return;
			}
			currentCutscene_.GetCharacterLeft().SetSpeaking(false);
			currentCutscene_.GetCharacterRight().SetSpeaking(false);
		}

		public void NextButtonPressed() {
			currentMessageIndex_++;
			SelectNextText();
			if(currentMessageIndex_ >= currentCutscene_.GetMessageCount() - 1) {
				SetChildActive(endButton_, true);
			}
			if(currentMessageIndex_ != 0) {
				SetChildActive(leftButton_, true);
			}
		}

		public void PrevButtonPressed() {
			currentMessageIndex_--;
			if(currentMessageIndex_ < 0) {
				currentMessageIndex_ = 0;
			}
			if(currentMessageIndex_ < currentCutscene_.GetMessageCount() - 1) {
				SetChildActive(endButton_, false);
			}
			if(currentMessageIndex_ == 0) {
				SetChildActive(leftButton_, false);
			}
			SelectNextText();
		}

		public void EndButtonPressed() {
			EndCutscene();
		}
	}
}
