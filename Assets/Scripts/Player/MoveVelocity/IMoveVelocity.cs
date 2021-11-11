using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Player {
	public interface IMoveVelocity {
		void SetVelocity(Vector3 velocityVector);
		void Disable();
		void Enable();
	}
}
