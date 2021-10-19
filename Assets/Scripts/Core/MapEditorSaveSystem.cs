using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public static class SaveSystem {

		private const string SAVE_EXTENSION = "json";

		private static readonly string SAVE_FOLDER = Application.streamingAssetsPath + "/SavesWorld/";
		private static bool isInit_ = false;

		public static void Init() {
			if(!isInit_) {
				isInit_ = true;
				// Test if Save Folder exists.
				if(!Directory.Exists(SAVE_FOLDER)) {
					// Create Save Folder.
					Directory.CreateDirectory(SAVE_FOLDER);
				}
			}
		}

		public static void Save(string fileName, string saveString, bool overwrite) {
			Init();
			string saveFileName = fileName;
			if(!overwrite) {
				// Make sure the Save Number is unique so it doesnt overwrite a previous save file.
				int saveNumber = 1;
				while(File.Exists(SAVE_FOLDER + saveFileName + "." + SAVE_EXTENSION)) {
					saveNumber++;
					saveFileName = fileName + "_" + saveNumber;
				}
				// saveFileName is unique.
			}
			File.WriteAllText(SAVE_FOLDER + saveFileName + "." + SAVE_EXTENSION, saveString);
		}

		public static string Load(string fileName) {
			Init();
			if(File.Exists(SAVE_FOLDER + fileName + "." + SAVE_EXTENSION)) {
				string saveString = File.ReadAllText(SAVE_FOLDER + fileName + "." + SAVE_EXTENSION);
				return saveString;
			} else {
				return null;
			}
		}

		public static string LoadMostRecentFile() {
			Init();
			DirectoryInfo directoryInfo = new DirectoryInfo(SAVE_FOLDER);
			// Get all the save files.
			FileInfo[] saveFiles = directoryInfo.GetFiles("*." + SAVE_EXTENSION);
			// Cycle through all save files and identify the most recent one.
			FileInfo mostRecentFile = null;
			foreach(FileInfo fileInfo in saveFiles) {
				if(mostRecentFile == null) {
					mostRecentFile = fileInfo;
				} else {
					if(fileInfo.LastWriteTime > mostRecentFile.LastWriteTime) {
						mostRecentFile = fileInfo;
					}
				}
			}

			// If theres a save file, load it, if not return null.
			if(mostRecentFile != null) {
				string saveString = File.ReadAllText(mostRecentFile.FullName);
				return saveString;
			} else {
				return null;
			}
		}

		public static void SaveObject(object saveObject, System.String name) {
			SaveObject(name, saveObject, false);
		}

		public static void SaveObject(string fileName, object saveObject, bool overwrite) {
			Init();
			// Pretty print please!
			string json = JsonUtility.ToJson(saveObject, true);
			Save(fileName, json, overwrite);
		}

		public static TSaveObject LoadMostRecentObject<TSaveObject>() {
			Init();
			string saveString = LoadMostRecentFile();
			if(saveString != null) {
				TSaveObject saveObject = JsonUtility.FromJson<TSaveObject>(saveString);
				return saveObject;
			} else {
				return default(TSaveObject);
			}
		}

		public static TSaveObject LoadObject<TSaveObject>(string fileName) {
			Init();
			string saveString = Load(fileName);
			if(saveString != null) {
				TSaveObject saveObject = JsonUtility.FromJson<TSaveObject>(saveString);
				return saveObject;
			} else {
				return default(TSaveObject);
			}
		}

	}
}
