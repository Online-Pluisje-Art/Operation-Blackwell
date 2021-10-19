using UnityEngine;
using System;

namespace OperationBlackwell.Core {
	public interface IGameObject {
		void MoveTo(Vector3 targetPosition, Action onReachedPosition);
		Vector3 GetPosition();
		Team GetTeam();
		bool IsEnemy(IGameObject unitGridCombat);
	}
}
