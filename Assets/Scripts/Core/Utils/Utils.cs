using UnityEngine;
using System.Collections;

namespace OperationBlackwell.Core {
	public class Utils : MonoBehaviour {
		private static readonly Vector3 Vector3zero = Vector3.zero;
		private static readonly Vector3 Vector3one = Vector3.one;
		private static readonly Vector3 Vector3yDown = new Vector3(0, -1);
		
		// Graciously taken from the interwebs, draws a line like Debug.DrawLine does.
		public static void DrawLine(Vector3 start, Vector3 end, float duration = 0.2f, float startWidth = 0.01f, float endWidth = 0.01f) {
			GameObject myLine = new GameObject();
			myLine.transform.position = start;
			myLine.AddComponent<LineRenderer>();
			myLine.name = "GridLine" + start.x + "|" + start.y + "|" + end.x + "|" + end.y;
			LineRenderer lr = myLine.GetComponent<LineRenderer>();
			lr.material = (Material)Resources.Load("Materials/Line");
			lr.startColor = Color.white;
			lr.endColor = Color.white;
			lr.startWidth = startWidth;
			lr.endWidth = endWidth;
			lr.SetPosition(0, start);
			lr.SetPosition(1, end);
		}

		// Get Mouse Position in World with Z = 0f.
		public static Vector3 GetMouseWorldPosition() {
			Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
			vec.z = 0f;
			return vec;
		}

		public static Vector3 GetMouseWorldPositionWithZ() {
			return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
		}
		
		public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera) {
			return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
		}
		
		public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera) {
			Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
			return worldPosition;
		}

		/*
		 * The following functions are taken from CodeMonkey.
		 * They come from the MeshUtils class in his utilities, but to
		 * avoid polluting the codebase with everything, we grab only what we need.
		 */
		public static void CreateEmptyMeshArrays(int quadCount, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles) {
			vertices = new Vector3[4 * quadCount];
			uvs = new Vector2[4 * quadCount];
			triangles = new int[6 * quadCount];
		}

		private static Quaternion[] cachedQuaternionEulerArr_;
		private static void CacheQuaternionEuler() {
			if(cachedQuaternionEulerArr_ != null) {
				return;
			}
			cachedQuaternionEulerArr_ = new Quaternion[360];
			for(int i = 0; i < 360; i++) {
				cachedQuaternionEulerArr_[i] = Quaternion.Euler(0, 0, i);
			}
		}

		private static Quaternion GetQuaternionEuler(float rotFloat) {
			int rot = Mathf.RoundToInt(rotFloat);
			rot = rot % 360;
			if(rot < 0) {
				rot += 360;
			}
			//if (rot >= 360) rot -= 360;
			if(cachedQuaternionEulerArr_ == null) {
				CacheQuaternionEuler();
			}
			return cachedQuaternionEulerArr_[rot];
		}

		public static void AddToMeshArrays(Vector3[] vertices, Vector2[] uvs, int[] triangles, int index, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11) {
			// Relocate vertices.
			int vIndex = index * 4;
			int vIndex0 = vIndex;
			int vIndex1 = vIndex + 1;
			int vIndex2 = vIndex + 2;
			int vIndex3 = vIndex + 3;

			baseSize *= .5f;

			bool skewed = baseSize.x != baseSize.y;
			if(skewed) {
				vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
				vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
				vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
				vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
			} else {
				vertices[vIndex0] = pos + GetQuaternionEuler(rot - 270) * baseSize;
				vertices[vIndex1] = pos + GetQuaternionEuler(rot - 180) * baseSize;
				vertices[vIndex2] = pos + GetQuaternionEuler(rot - 90) * baseSize;
				vertices[vIndex3] = pos + GetQuaternionEuler(rot - 0) * baseSize;
			}

			// Relocate UVs.
			uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
			uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
			uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
			uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

			// Create triangles.
			int tIndex = index * 6;

			triangles[tIndex + 0] = vIndex0;
			triangles[tIndex + 1] = vIndex3;
			triangles[tIndex + 2] = vIndex1;

			triangles[tIndex + 3] = vIndex1;
			triangles[tIndex + 4] = vIndex3;
			triangles[tIndex + 5] = vIndex2;
		}

		public static Mesh CreateEmptyMesh() {
			Mesh mesh = new Mesh();
			mesh.vertices = new Vector3[0];
			mesh.uv = new Vector2[0];
			mesh.triangles = new int[0];
			return mesh;
		}

		public static Mesh CreateMesh(Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11) {
			return AddToMesh(null, pos, rot, baseSize, uv00, uv11);
		}

		public static Mesh AddToMesh(Mesh mesh, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11) {
			if(mesh == null) {
				mesh = CreateEmptyMesh();
			}
			Vector3[] vertices = new Vector3[4 + mesh.vertices.Length];
			Vector2[] uvs = new Vector2[4 + mesh.uv.Length];
			int[] triangles = new int[6 + mesh.triangles.Length];
				
			mesh.vertices.CopyTo(vertices, 0);
			mesh.uv.CopyTo(uvs, 0);
			mesh.triangles.CopyTo(triangles, 0);

			int index = vertices.Length / 4 - 1;
			//Relocate vertices
			int vIndex = index * 4;
			int vIndex0 = vIndex;
			int vIndex1 = vIndex + 1;
			int vIndex2 = vIndex + 2;
			int vIndex3 = vIndex + 3;

			baseSize *= .5f;

			bool skewed = baseSize.x != baseSize.y;
			if(skewed) {
				vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x,  baseSize.y);
				vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
				vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3( baseSize.x, -baseSize.y);
				vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
			} else {
				vertices[vIndex0] = pos + GetQuaternionEuler(rot - 270) * baseSize;
				vertices[vIndex1] = pos + GetQuaternionEuler(rot - 180) * baseSize;
				vertices[vIndex2] = pos + GetQuaternionEuler(rot - 90) * baseSize;
				vertices[vIndex3] = pos + GetQuaternionEuler(rot - 0) * baseSize;
			}
			
			//Relocate UVs
			uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
			uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
			uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
			uvs[vIndex3] = new Vector2(uv11.x, uv11.y);
			
			//Create triangles
			int tIndex = index * 6;
			
			triangles[tIndex + 0] = vIndex0;
			triangles[tIndex + 1] = vIndex3;
			triangles[tIndex + 2] = vIndex1;
			
			triangles[tIndex + 3] = vIndex1;
			triangles[tIndex + 4] = vIndex3;
			triangles[tIndex + 5] = vIndex2;
				
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uvs;

			//mesh.bounds = bounds;

			return mesh;
		}

		// Get Default Unity Font, used in text objects if no font given
		public static Font GetDefaultFont() {
			return Resources.GetBuiltinResource<Font>("Arial.ttf");
		}

		// Create a Sprite in the World, no parent
		public static GameObject CreateWorldSprite(string name, Sprite sprite, Vector3 position, Vector3 localScale, int sortingOrder, Color color) {
			return CreateWorldSprite(null, name, sprite, position, localScale, sortingOrder, color);
		}

		// Create a Sprite in the World
		public static GameObject CreateWorldSprite(Transform parent, string name, Sprite sprite, Vector3 localPosition, Vector3 localScale, int sortingOrder, Color color) {
			GameObject gameObject = new GameObject(name, typeof(SpriteRenderer));
			Transform transform = gameObject.transform;
			transform.SetParent(parent, false);
			transform.localPosition = localPosition;
			transform.localScale = localScale;
			SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
			spriteRenderer.sprite = sprite;
			spriteRenderer.sortingOrder = sortingOrder;
			spriteRenderer.color = color;
			return gameObject;
		}

		public static IEnumerator WaitSeconds(float seconds) {
			yield return new WaitForSeconds(seconds);
		}
	}
}
