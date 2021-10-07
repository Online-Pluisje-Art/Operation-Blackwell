// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class Utils {
		// Graciously taken from the interwebs, draws a line like Debug.DrawLine does.
		public static void DrawLine(Vector3 start, Vector3 end, float duration = 0.2f) {
			GameObject myLine = new GameObject();
			myLine.transform.position = start;
			myLine.AddComponent<LineRenderer>();
			myLine.name = "GridLine" + start.x + "|" + start.z + "|" + end.x + "|" + end.z;
			LineRenderer lr = myLine.GetComponent<LineRenderer>();
			lr.material = (Material)Resources.Load("Materials/Line");
			lr.startColor = Color.white;
			lr.endColor = Color.white;
			lr.SetWidth(0.1f, 0.1f);
			lr.SetPosition(0, start);
			lr.SetPosition(1, end);
		}

		/*
		* CodeMonkey code, grab me some 3d mouse position.
		* Requires a collider below the play grid for our use case!
		* TODO: This collider grid is hacked in now, make this nicer.
		*/
		public static Vector3 GetMouseWorldPosition3d() {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f)) {
				return raycastHit.point;
			} else {
				return Vector3.zero;
			}
		}
	}
}
