using UnityEngine;
using System.Collections;

namespace OperationBlackwell.Core {
	public class PathQueue {
		public int startX;
		public int startY;
		public int endX;
		public int endY;
		public GridPathfinding.OnPathCallback callback;

		public PathQueue(int _startX, int _startY, int _endX, int _endY, GridPathfinding.OnPathCallback _callback) {
			startX = _startX;
			startY = _startY;
			endX = _endX;
			endY = _endY;
			callback = _callback;
		}
	}
}
