using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using OperationBlackwell.Core;
	
namespace OperationBlackwell.UI {
	public class CursorController : Singleton<CursorController> {
		public event EventHandler<CursorChangedEventArgs> CursorChanged;
		public class CursorChangedEventArgs : EventArgs {
			public CursorType cursorType;
		}

		[SerializeField] private List<CursorAnimation> cursorAnimationList_;

		private CursorAnimation cursorAnimation_;
		private Dictionary<string, CursorType> cursorDictionary_;

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

		private void Start() {
			cursorDictionary_ = new Dictionary<string, CursorType>();
			foreach(CursorType cursorType in Enum.GetValues(typeof(CursorType))) {
				cursorDictionary_.Add(cursorType.ToString(), cursorType);
			}
			SetIntialCursorAnimation(cursorDictionary_["Arrow"]);
			GameController.instance.CursorChanged += OnCursorChanged;
		}

		private void OnDestroy() {
			GameController.instance.CursorChanged -= OnCursorChanged;
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

		private void OnCursorChanged(object sender, string type) {
			if(cursorDictionary_.ContainsKey(type) && cursorDictionary_[type] != cursorAnimation_.cursorType) {
				SetActiveCursorAnimation(GetCursorAnimation(cursorDictionary_[type]));
			} else if(!cursorDictionary_.ContainsKey(type)) {
				Debug.LogError("Cursor type " + type + " does not exist");
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
			CursorChanged?.Invoke(this, new CursorChangedEventArgs { cursorType = cursorType });
		}

		public CursorType GetActiveCursorType() {
			return cursorAnimation_.cursorType;
		}

		private void SetIntialCursorAnimation(CursorType cursorType) {
			SetActiveCursorAnimation(GetCursorAnimation(cursorType));
			CursorChanged?.Invoke(this, new CursorChangedEventArgs { cursorType = cursorType });
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
	}
}
