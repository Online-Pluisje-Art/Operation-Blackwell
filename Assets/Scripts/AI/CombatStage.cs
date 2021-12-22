using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OperationBlackwell.Player;

namespace OperationBlackwell.AI {
	[CreateAssetMenu(fileName = "New Combat Stage", menuName = "AI/Combat Stage")]
	public class CombatStage : ScriptableObject {
		[SerializeField] private int id_;
		[SerializeField] private List<AIUnit> units_;

		public int ID {
			get { return id_; }
		}

		public List<AIUnit> GetAIUnits() {
			return units_;
		}
	}
}
