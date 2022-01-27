using UnityEngine;
using System.Collections.Generic;
using OperationBlackwell.Core;
using OperationBlackwell.AI;
using OperationBlackwell.Player;

namespace OperationBlackwell.Boss {
	public class BossController : Singleton<BossController> {
		[SerializeField] private Boss boss_;
		[SerializeField] private List<BossStage> stages_;

		[Header("Barriers")]
		[SerializeField] private List<GameObject> barriers_;
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
			GridCombatSystem.instance.BossReenabled += OnBossReenabled;
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
			foreach(GameObject obj in barriers_) {
				obj.SetActive(true);
			}
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
			GridCombatSystem.instance.GetOrderList().GetQueue().RemoveAll(x => x.GetUnit() == (CoreUnit)boss_);
			if(id == 2) {
				// move boss to location and let him wait there till all enemies are dead
				GameController con = GameController.instance;
				Tilemap.Node node = con.grid.GetGridObject(boss_.transform.position);
				node.ClearUnitGridCombat();
				boss_.transform.position = new Vector3(103.5f, 18.5f, 0);
				node = con.grid.GetGridObject(boss_.transform.position);
				node.SetUnitGridCombat(boss_);
				boss_.UnloadBoss();
			}
			SpawnUnits(id);
		}

		private void OnBossReenabled() {
			boss_.ReloadBoss();
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
