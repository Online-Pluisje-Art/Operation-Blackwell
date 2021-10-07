using UnityEngine;

public class GridLeon : MonoBehaviour {
	public Transform hexPrefab;
	public Transform hexPrefab2;

	public int gridWidth = 11;
	public int gridHeight = 11;

	// These values have been carefully chosen after a lot of 'calculating'.
	private float hexWidth_ = 1.732f;
	private float hexHeight_ = 2.0f;

	private Vector3 startPos_;

	public int[,] hexArray;

	void Start() {
		hexArray = new int[gridWidth, gridHeight];
		CalcstartPos_();
		CreateGrid();
	}

	// Calculate Start Position (of a Hex).
	void CalcstartPos_()	{
		float offset = 0;
		if(gridHeight / 2 % 2 != 0) {
			offset = hexWidth_ / 2;
		}

		float x = -hexWidth_ * (gridWidth / 2) - offset;
		float z = hexHeight_ * 0.75f * (gridHeight / 2);

		startPos_ = new Vector3(x, 0, z);
	}

	/*
	 * Makes a Vector3 from a Vector2.
	 * gridPos: Vector2 with a grid position.
	 * Returns: Vector3 worldspace.
	 */
	Vector3 CalcWorldPos(Vector2 gridPos) {
		float offset = 0;
		if(gridPos.y % 2 != 0) {
			offset = hexWidth_ / 2;
		}

		float x = startPos_.x + gridPos.x * hexWidth_ + offset;
		float z = startPos_.z - gridPos.y * hexHeight_ * 0.75f;

		return new Vector3(x, 0, z);
	}

	// Generates the grid and shows it on the game scene.
	void CreateGrid() {
		for(int x = 0; x < gridWidth; x++)	{
			for(int z = 0; z < gridHeight; z++) {
				Transform hex;
				if(Random.value > 0.5f) {
					hex = Instantiate(hexPrefab) as Transform;
				} else {
					hex = Instantiate(hexPrefab2) as Transform;
				}
				Vector2 gridPos = new Vector2(x, z);
				hex.position = CalcWorldPos(gridPos);
				hex.parent = this.transform;
				hex.name = "Hexagon " + x + "|" + z;
				Hex hex2 = hex.gameObject.AddComponent<Hex>();
				hex2.coordinateX = x;
				hex2.coordinateZ = z;
				hexArray[x, z] = 1;
				hex2.hexArray = hexArray;
			}
		}
	}
}
