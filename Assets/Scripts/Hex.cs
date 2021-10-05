using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour {
	// Unity Location of the X and Z position.
	private float unityPosX_, unityPosZ_;

	// X, Z Location in the grid (0,0 is top left).
	public int coordinateX, coordinateZ;

	private bool rowIsEven_;

	public int[,] hexArray;

	public Hex(int coordinateX, int coordinateZ) {
		this.coordinateX = coordinateX;
		this.coordinateZ = coordinateZ;
	}

	// Start is called before the first frame update.
	void Start() {
		unityPosX_ = transform.position.x;
		unityPosZ_ = transform.position.z;
		// Equals to 1 because rows count starts from 1 instead from 0 unlike the coordinate system.
		rowIsEven_ = (coordinateZ % 2 == 1);
	}

	// Add the two hex coordinates together.
	public Hex Add(Hex b) {
		return new Hex(coordinateX + b.coordinateX, coordinateZ + b.coordinateZ);
	}

	/*
	 * This can be used to calculate the coordinates of the hex of the given direction.
	 * int direction: values 0 to 5 (inclusive).
	 * 0 is top left, 1 is left, 2 is bottom left, 3 is bottom right, 4 is right, 5 is top right.
	 */
	public Hex Neighbor(int direction) {
		if(rowIsEven_) {
			return this.Add(Hex.directionsEven[direction]);
		} else {
			return this.Add(Hex.directionsOdd[direction]);
		}
	}

	// Print out all neighbors of a Hex.
	public Hex[] Neighbors() {
		Hex[] hexNeighbors = new Hex[6];
		for(int x = 0; x < 6; x++) {
			Hex neighbor = Neighbor(x);
			if(neighbor.coordinateX >= 0 && neighbor.coordinateZ >= 0) {
				if (hexArray[neighbor.coordinateX, neighbor.coordinateZ] == 1) {
					hexNeighbors[x] = neighbor;
				} else {
					hexNeighbors[x] = null;
				}
			}
		}
		return hexNeighbors;
	}

	// Directions to calculate neighbors if the row is odd.
	static public List<Hex> directionsOdd = new List<Hex>{
		new Hex(-1, -1),
		new Hex(-1, 0),
		new Hex(-1, 1),
		new Hex(0, -1),
		new Hex(0, 1),
		new Hex(1, 0)
	};

	// Directions to calculate neighbors if the row is even.
	static public List<Hex> directionsEven = new List<Hex>{
		new Hex(-1, 0),
		new Hex(0, -1),
		new Hex(0, 1),
		new Hex(1, -1),
		new Hex(1, 0),
		new Hex(1, 1)
	};
}
