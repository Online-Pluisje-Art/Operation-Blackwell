using UnityEngine;

namespace OperationBlackwell.Core {
	/*
	 * Bar in the World, great for quickly making a health bar
	 * */
	public class WorldBar {
		private Outline outline_;
		private GameObject gameObject_;
		private Transform transform_;
		private Transform background_;
		private Transform bar_;

		public static int GetSortingOrder(Vector3 position, int offset, int baseSortingOrder = 5000) {
			return (int)(baseSortingOrder - position.y) + offset;
		}

		public class Outline {
			public float size = 1f;
			public Color color = Color.black;
		}

		// The ? in the parameter listing means that parameter is optional.
		public WorldBar(Transform parent, Vector3 localPosition, Vector3 localScale, Color? backgroundColor, Color barColor, float sizeRatio, int sortingOrder, Outline outline = null) {
			this.outline_ = outline;
			SetupParent(parent, localPosition);
			if(outline != null) {
				SetupOutline(outline, localScale, sortingOrder - 1);
			}
			if(backgroundColor != null) {
				SetupBackground((Color)backgroundColor, localScale, sortingOrder);
			}
			SetupBar(barColor, localScale, sortingOrder + 1);
			SetSize(sizeRatio);
		}

		private void SetupParent(Transform parent, Vector3 localPosition) {
			gameObject_ = new GameObject("WorldBar");
			transform_ = gameObject_.transform;
			transform_.SetParent(parent);
			transform_.localPosition = localPosition;
		}

		private void SetupOutline(Outline outline, Vector3 localScale, int sortingOrder) {
			Sprite sprite = Resources.Load<Sprite>("Textures/HUD/White_1x1");
			Utils.CreateWorldSprite(transform_, "Outline", sprite, new Vector3(0, 0), localScale + new Vector3(outline.size, outline.size), sortingOrder, outline.color);
		}
		
		private void SetupBackground(Color backgroundColor, Vector3 localScale, int sortingOrder) {
			Sprite sprite = Resources.Load<Sprite>("Textures/HUD/White_1x1");
			background_ = Utils.CreateWorldSprite(transform_, "Background", sprite, new Vector3(0, 0), localScale, sortingOrder, backgroundColor).transform;
		}

		private void SetupBar(Color barColor, Vector3 localScale, int sortingOrder) {
			GameObject barGO = new GameObject("Bar");
			bar_ = barGO.transform;
			bar_.SetParent(transform_);
			bar_.localPosition = new Vector3(-localScale.x / 2f, 0, 0);
			bar_.localScale = new Vector3(1, 1, 1);
			Sprite sprite = Resources.Load<Sprite>("Textures/HUD/White_1x1");
			Transform barIn = Utils.CreateWorldSprite(bar_, "BarIn", sprite, new Vector3(localScale.x / 2f, 0), localScale, sortingOrder, barColor).transform;
		}

		public void SetRotation(float rotation) {
			transform_.localEulerAngles = new Vector3(0, 0, rotation);
		}

		public void SetSize(float sizeRatio) {
			bar_.localScale = new Vector3(sizeRatio, 1, 1);
		}

		public void SetLocalScale(Vector3 localScale) {
			// Outline
			if(transform_.Find("Outline") != null) {
				// Has outline
				transform_.Find("Outline").localScale = localScale + new Vector3(outline_.size, outline_.size);
			}

			//Background
			background_.localScale = localScale;

			// Set Bar Scale
			bar_.localPosition = new Vector3(-localScale.x / 2f, 0, 0);
			Transform barIn = bar_.Find("BarIn");
			barIn.localScale = localScale;
			barIn.localPosition = new Vector3(localScale.x / 2f, 0);
		}

		public void SetColor(Color color) {
			bar_.Find("BarIn").GetComponent<SpriteRenderer>().color = color;
		}

		public void Show() {
			gameObject_.SetActive(true);
		}

		public void Hide() {
			gameObject_.SetActive(false);
		}

		// public Button_Sprite AddButton(System.Action ClickFunc, System.Action MouseOverOnceFunc, System.Action MouseOutOnceFunc) {
		//     Button_Sprite buttonSprite = gameObject.AddComponent<Button_Sprite>();
		//     if (ClickFunc != null)
		//         buttonSprite.ClickFunc = ClickFunc;
		//     if (MouseOverOnceFunc != null)
		//         buttonSprite.MouseOverOnceFunc = MouseOverOnceFunc;
		//     if (MouseOutOnceFunc != null)
		//         buttonSprite.MouseOutOnceFunc = MouseOutOnceFunc;
		//     return buttonSprite;
		// }

		public void DestroySelf() {
			if(gameObject_ != null) {
				Object.Destroy(gameObject_);
			}
		}
	}
}
