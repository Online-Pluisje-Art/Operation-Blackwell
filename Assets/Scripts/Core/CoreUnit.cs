using UnityEngine;
using System;

namespace OperationBlackwell.Core {
	public abstract class CoreUnit : MonoBehaviour {
		public abstract void MoveTo(Vector3 targetPosition, Action onReachedPosition);
		public abstract Vector3 GetPosition();
		public abstract Team GetTeam();
		public abstract bool IsEnemy(CoreUnit unitGridCombat);
		public abstract int GetActionPoints();
		public abstract void SetActionPoints(int actionPoints);
		public abstract void ResetActionPoints();
	}
}
