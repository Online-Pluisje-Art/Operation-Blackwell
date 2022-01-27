using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OperationBlackwell.Core {
	public class WeaponSwitcher : MonoBehaviour {

		[Header("Sword")]
		[SerializeField] private GameObject swordButton_;
		[SerializeField] private Sprite swordButtonTextureSelected_;
		[SerializeField] private Sprite swordButtonTextureUnselected_;

		[Header("Gun")]
		[SerializeField] private GameObject gunButton_;
		[SerializeField] private Sprite gunButtonTextureSelected_;
		[SerializeField] private Sprite gunButtonTextureUnselected_;

		private Image swordButtonImage_;
		private Image gunButtonImage_;

		private void Awake() {
			swordButtonImage_ = swordButton_.GetComponent<Image>();
			gunButtonImage_ = gunButton_.GetComponent<Image>();
		}

		public void ChangeActiveWeaponIndicator(int index) {
			if(index == 0) {
				swordButtonImage_.sprite = swordButtonTextureSelected_;
				gunButtonImage_.sprite = gunButtonTextureUnselected_;
			} else if(index == 1) {
				swordButtonImage_.sprite = swordButtonTextureUnselected_;
				gunButtonImage_.sprite = gunButtonTextureSelected_;
			}
		}
	}
}
