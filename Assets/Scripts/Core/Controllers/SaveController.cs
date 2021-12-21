using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleFileBrowser;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class SaveController : MonoBehaviour {
		private readonly bool debugSaving_ = false;

		private string savePath_;
		private void Update() {
			HandleSaveLoad();
		}

		private void HandleSaveLoad() {
			if(Input.GetKeyDown(KeyCode.Backslash)) {
				StartCoroutine(getSaveNameForSaving());
			}
			if(Input.GetKeyDown(KeyCode.Equals)) {
				StartCoroutine(getSaveNameForLoading());
			}
		}

		IEnumerator getSaveNameForSaving() {
			FileBrowser.SetFilters(true, new FileBrowser.Filter("World Save Files", ".json"));
			FileBrowser.SetDefaultFilter(".json");
			yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, false, Application.streamingAssetsPath + "/SavesWorld/", null, "Save As", "Save");
			if(FileBrowser.Success) {
				Debug.Log(FileBrowserHelpers.GetFilename(FileBrowser.Result[0]));
				savePath_ = FileBrowserHelpers.GetFilename(FileBrowser.Result[0]);
				if(debugSaving_) {
					Debug.Log("Before: " + savePath_);
				}
				savePath_ = Path.GetFileNameWithoutExtension(savePath_);
				if(debugSaving_) {
					Debug.Log("After: " + savePath_);
				}
				GameController.Instance.tilemap.Save(savePath_);
			} else {
				Debug.Log("User cancelled the saving operation, not saving!");
			}
		}

		IEnumerator getSaveNameForLoading() {
			FileBrowser.SetFilters(true, new FileBrowser.Filter("World Save Files", ".json"));
			FileBrowser.SetDefaultFilter(".json");
			yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, Application.streamingAssetsPath + "/SavesWorld/", null, "Load File", "Load");
			if(FileBrowser.Success) {
				Debug.Log(FileBrowserHelpers.GetFilename(FileBrowser.Result[0]));
				savePath_ = FileBrowserHelpers.GetFilename(FileBrowser.Result[0]);
				if(debugSaving_) {
					Debug.Log("Before: " + savePath_);
				}
				savePath_ = Path.GetFileNameWithoutExtension(savePath_);
				if(debugSaving_) {
					Debug.Log("After: " + savePath_);
				}
				GameController.Instance.tilemap.Load(savePath_);
			} else {
				Debug.Log("User cancelled the loading operation, not loading!");
			}
		}
	}
}
