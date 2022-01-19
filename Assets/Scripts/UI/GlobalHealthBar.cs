using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using OperationBlackwell.Core;

namespace OperationBlackwell.UI {
	public class GlobalHealthBar : MonoBehaviour {
		[SerializeField] private RectTransform barTemplate_;
		[SerializeField] private CoreUnit unit_;

		private float fillValueMax_; // Total value for entire Mercenary Bar
		private float fillValue_; // Currently filled value
		private float fillValuePerBar_; // Filled value amount per each bar
		private List<Bar> barList_;
		private GameObject gameObject_;

		private void Start() {
			gameObject_ = barTemplate_.gameObject;
			gameObject_.SetActive(false);

			Setup(unit_.GetHealth(), 1);
			unit_.HealthChanged += OnHealthChanged;
			GridCombatSystem.instance.BossStarted += OnBossStarted;
			GridCombatSystem.instance.BossEnded += OnBossEnded;
		}

		// Set up the total Mercenary Bar using the given values
		public void Setup(float fillValueMax_, int barAmount) {
			this.fillValueMax_ = fillValueMax_;
			fillValue_ = 0f;
			fillValuePerBar_ = fillValueMax_ / barAmount;

			barList_ = new List<Bar>();
			for(int i = 0; i < barAmount; i++) {
				Vector2 barSize = new Vector2(1000, 35);
				float barSizeOffset = 0f;
				Vector2 barAnchoredPosition = new Vector2((barSize.x + barSizeOffset) * i, -65);
				Bar bar = CreateBar(barAnchoredPosition, barSize);
				barList_.Add(bar);
			}
			Hide();
			SetFillValue(fillValueMax_);
		}

		// Create a single Bar
		private Bar CreateBar(Vector2 anchoredPosition, Vector2 size) {
			barTemplate_.anchoredPosition = anchoredPosition;
			barTemplate_.sizeDelta = new Vector2(size.x, size.y);
			barTemplate_.SetAsFirstSibling();

			Bar bar = new Bar(barTemplate_);

			return bar;
		}

		private void OnHealthChanged(object sender, float health) {
			SetFillValue(health);
		}

		// Set the fill value for the entire Mercenary Bar, calculate the fillAmount for each inner bar
		private void SetFillValue(float fillValue_) {
			this.fillValue_ = fillValue_;

			for(int i = 0; i < barList_.Count; i++) {
				Bar bar = barList_[i];
				float barValueMin = i * fillValuePerBar_;
				float barValueMax =(i + 1) * fillValuePerBar_;
				if(fillValue_ <= barValueMin) {
					// Fill value doesnt reach this bar
					bar.SetFillAmount(0f);
				} else {
					if(fillValue_ >= barValueMax) {
						// Fill value completely fills this bar
						bar.SetFillAmount(1f);
					} else {
						// Fill value is somewhere in between
						float barFillAmount =(fillValue_ - barValueMin) /(barValueMax - barValueMin);
						bar.SetFillAmount(barFillAmount);
					}
				}
			}
		}

		private void OnBossStarted() {
			Show();
		}

		private void OnBossEnded() {
			Hide();
		}

		public void Show() {
			foreach(Bar bar in barList_) {
				bar.Show();
			}
		}

		public void Hide() {
			foreach(Bar bar in barList_) {
				bar.Hide();
			}
		}

		// Represents a single Bar
		private class Bar {
			private RectTransform barRectTransform;
			private Image barFilledImage;

			public Bar(RectTransform barRectTransform) {
				this.barRectTransform = barRectTransform;
				barFilledImage = barRectTransform.Find("fill").GetComponent<Image>();

				SetFillAmount(0f);
			}

			public void SetFillAmount(float fillAmount) {
				if(fillAmount >= 1f) {
					if(barFilledImage.fillAmount < 1f) {
						// Previous frame was under 1f
					}
				}
				barFilledImage.fillAmount = fillAmount;
			}

			public void Show() {
				barRectTransform.gameObject.SetActive(true);
			}

			public void Hide() {
				barRectTransform.gameObject.SetActive(false);
			}
		}
	}
}
