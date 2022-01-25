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
		public void ChangeActiveWeaponIndicator(int index) {
			if(index == 0) {
				swordButton_.GetComponent<Image>().sprite = swordButtonTextureSelected_;
				gunButton_.GetComponent<Image>().sprite = gunButtonTextureUnselected_;
			} else if(index == 1) {
				swordButton_.GetComponent<Image>().sprite = swordButtonTextureUnselected_;
				gunButton_.GetComponent<Image>().sprite = gunButtonTextureSelected_;
			}
		}
	}
}
