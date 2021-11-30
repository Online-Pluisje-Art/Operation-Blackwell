using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace OperationBlackwell.Core {
	public class CursorController : MonoBehaviour {

		public static CursorController Instance { get; private set; }

		public event EventHandler<OnCursorChangedEventArgs> OnCursorChanged;
		public class OnCursorChangedEventArgs : EventArgs {
			public CursorType cursorType;
		}

		[SerializeField] private List<CursorAnimation> cursorAnimationList_;

		private CursorAnimation cursorAnimation_;

		private int currentFrame_;
		private float frameTimer_;
		private int frameCount_;

		public enum CursorType {
			Arrow,
			Grab,
			Select,
			Attack,
			Move
		}

		private void Awake() {
			Instance = this;
		}

		private void Start() {
			SetIntialCursorAnimation(CursorType.Arrow);
		}

		private void Update() {
			if(cursorAnimation_ == null) {
				return;
			}
			frameTimer_ -= Time.deltaTime;
			if(frameTimer_ <= 0f) {
				frameTimer_ += cursorAnimation_.frameRate;
				currentFrame_ = (currentFrame_ + 1) % frameCount_;
				Cursor.SetCursor(cursorAnimation_.textureArray[currentFrame_], cursorAnimation_.offset, CursorMode.Auto);
			}
		}

		public void SetActiveCursorType(CursorType cursorType) {
			if(cursorAnimation_ == null) {
				return;
			}
			if(cursorAnimation_.cursorType == cursorType) {
				return;
			}
			SetActiveCursorAnimation(GetCursorAnimation(cursorType));
			OnCursorChanged?.Invoke(this, new OnCursorChangedEventArgs { cursorType = cursorType });
		}

		public CursorType GetActiveCursorType() {
			return cursorAnimation_.cursorType;
		}

		private void SetIntialCursorAnimation(CursorType cursorType) {
			SetActiveCursorAnimation(GetCursorAnimation(cursorType));
			OnCursorChanged?.Invoke(this, new OnCursorChangedEventArgs { cursorType = cursorType });
		}

		private CursorAnimation GetCursorAnimation(CursorType cursorType) {
			foreach(CursorAnimation cursorAnimation_ in cursorAnimationList_) {
				if(cursorAnimation_.cursorType == cursorType) {
					return cursorAnimation_;
				}
			}
			// Couldn't find this CursorType!
			return null;
		}

		private void SetActiveCursorAnimation(CursorAnimation cursorAnimation_) {
			this.cursorAnimation_ = cursorAnimation_;
			currentFrame_ = 0;
			frameTimer_ = 0f;
			frameCount_ = cursorAnimation_.textureArray.Length;
		}


		[System.Serializable]
		public class CursorAnimation {
			public CursorType cursorType;
			public Texture2D[] textureArray;
			public float frameRate;
			public Vector2 offset;

		}
	}
}
