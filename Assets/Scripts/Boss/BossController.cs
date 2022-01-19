using UnityEngine;
using System.Collections.Generic;
using OperationBlackwell.Core;
using OperationBlackwell.AI;
using OperationBlackwell.Player;

namespace OperationBlackwell.Boss {
	public class BossController : Singleton<BossController> {
		[SerializeField] private Boss boss_;
		[SerializeField] private List<BossStage> stages_;
		private int currentStage_;
		private List<int> loadedStages_;

		private void Start() {
			loadedStages_ = new List<int>();

			if(stages_.Count == 0) {
				Debug.LogError("No stages found in BossController");
			}

			foreach(BossStage stage in stages_) {
				stage.DisableUnits();
			}

			GridCombatSystem.instance.BossStarted += OnBossStarted;
			boss_.BossStageTrigged += OnBossStageTriggered;
		}

		public void SpawnUnits(int id) {
			int index = stages_.FindIndex(x => x.ID == id);
			if(index < 0) {
				Debug.LogError("Cannot find stage with id: " + id);
				return;
			}
			currentStage_ = index;
			stages_[index].SpawnUnits();
			loadedStages_.Add(id);
		}

		private void OnBossStarted() {
			AIController.instance.AddBoss(boss_);
			SpawnUnits(1);
		}

		public List<BossStage> GetStages() {
			return stages_;
		}

		public void BossDied() {
			GridCombatSystem.instance.EndBossStages();
			foreach(BossStage stage in stages_) {
				foreach(SpawnUnits unit in stage.units) {
					unit.unit.Die();
				}
			}
		}

		private void OnBossStageTriggered(int id) {
			if(loadedStages_.Contains(id)) {
				return;
			}
			SpawnUnits(id);
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
			List<AIUnit> unitsToSpawn = new List<AIUnit>();
			foreach(SpawnUnits unit in units) {
				unit.unit.gameObject.SetActive(true);
				unit.unit.transform.position = unit.spawnPosition;
				GameController.instance.GetGrid().GetGridObject(unit.unit.GetPosition())
					.SetUnitGridCombat(unit.unit);
				unitsToSpawn.Add(unit.unit);
			}
			AIController.instance.SpawnUnits(unitsToSpawn);
		}
	}

	[System.Serializable]
	public struct SpawnUnits {
		public AIUnit unit;
		public Vector3 spawnPosition;
	}
}
