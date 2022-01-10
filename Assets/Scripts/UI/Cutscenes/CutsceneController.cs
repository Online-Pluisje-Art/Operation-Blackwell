using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using OperationBlackwell.Core;
using OperationBlackwell.LevelTransitions;

namespace OperationBlackwell.UI {
	public class CutsceneController : Singleton<CutsceneController> {
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
		public System.EventHandler<System.EventArgs> OnCutsceneStart;
		public System.EventHandler<System.EventArgs> OnCutsceneEnd;

		private void Start() {
			currentCutscene_ = null;
			children_ = new List<GameObject>();
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

			// Subscribe to events
			if(GridCombatSystem.instance != null) {
				GridCombatSystem.instance.CutsceneTriggered += StartCutscene;
			}
			if(LevelTransitionController.instance != null) {
				LevelTransitionController.instance.LeveltransitionCutsceneTriggered += StartCutscene;
			}
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

		public void StartCutscene(object sender, int index) {
			if(sender is GridCombatSystem combatsystem) {
				combatsystem.SetState(GridCombatSystem.State.Cutscene);
			}
			currentCutsceneIndex_ = index;
			OnCutsceneStart?.Invoke(this, System.EventArgs.Empty);
			Show(index);
		}

		public void EndCutscene() {
			if(LevelTransitionController.instance.IsTransitioning()) {
				GridCombatSystem.instance.SetState(GridCombatSystem.State.Transition);
				LevelTransitionController.instance.TransitionDone?.Invoke(this, System.EventArgs.Empty);
			} else {
				GridCombatSystem.instance.SetState(GridCombatSystem.State.OutOfCombat);
			}
			OnCutsceneEnd?.Invoke(this, System.EventArgs.Empty);
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
			CutsceneCharacter character = messageObject.character;
			if(character != currentCutscene_.GetCharacterLeft() && character != currentCutscene_.GetCharacterRight()) {
				if(messageObject.direction == Cutscene.CharacterMessage.Direction.Left) {
					currentCutscene_.SetCharacterLeft(character);
				} else {
					currentCutscene_.SetCharacterRight(character);
				}
			}
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
