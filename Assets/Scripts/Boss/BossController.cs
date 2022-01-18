using UnityEngine;
using System.Collections.Generic;
using OperationBlackwell.Core;
using OperationBlackwell.Player;

namespace OperationBlackwell.Boss {
	public class BossController : Singleton<BossController> {
		[SerializeField] private Boss boss_;
		[SerializeField] private List<BossStage> stages_;
		private int currentStage_;

		private void Start() {
			if(stages_.Count == 0) {
				Debug.LogError("No stages found in BossController");
			}

			foreach(BossStage stage in stages_) {
				stage.DisableUnits();
			}

			GridCombatSystem.instance.BossStarted += OnBossStarted;
		}

		public void SpawnUnits(int id) {
			int index = stages_.FindIndex(x => x.ID == id);
			if(index < 0) {
				Debug.LogError("Cannot find stage with id: " + id);
				return;
			}
			currentStage_ = index;
			stages_[index].SpawnUnits();
		}

		private void OnBossStarted() {
			boss_.LoadUnit();
			SpawnUnits(0);
		}

		public List<BossStage> GetStages() {
			return stages_;
		}

		public void BossDied() {
			GridCombatSystem.instance.EndBossStages();
		}
	}

	[System.Serializable]
	public struct BossStage {
		[SerializeField] private int id_;
		[SerializeField] private float triggerHealth_;

		public int ID {
			get { return id_; }
		}

		public float triggerHealth {
			get { return triggerHealth_; }
		}
		public List<SpawnUnits> units;

		public void DisableUnits() {
			foreach(SpawnUnits unit in units) {
				unit.unit.gameObject.SetActive(false);
			}
		}

		public void SpawnUnits() {
			foreach(SpawnUnits unit in units) {
				unit.unit.gameObject.SetActive(true);
				unit.unit.transform.position = unit.spawnPosition;
				unit.unit.LoadUnit();
			}
		}
	}

	[System.Serializable]
	public struct SpawnUnits {
		public AIUnit unit;
		public Vector3 spawnPosition;
	}
}
