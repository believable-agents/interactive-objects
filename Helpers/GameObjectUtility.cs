using UnityEngine;

public class GameObjectUtility 
{
	private const string ReferenceManagerPath = "Assets/ReferenceManager.asset";
	private static ReferenceManager _referenceManager;
	private static ReferenceManager ReferenceManager {
		get {
			if (_referenceManager == null) {
				// TODO: Investigate if can be solved using ScriptableObjects
				_referenceManager = GameObject.Find ("ReferenceManager").GetComponent<ReferenceManager>();

//				// if it does not exists, create it
//				if (!File.Exists (ReferenceManagerPath)) {
//					var so = ScriptableObject.CreateInstance<ReferenceManager> ();
//					AssetDatabase.CreateAsset (so, ReferenceManagerPath);
//				}
//				// now load the asset
//				_referenceManager = AssetDatabase.LoadAssetAtPath (ReferenceManagerPath, typeof(ReferenceManager)) as ReferenceManager;
			}
			return _referenceManager;
		}
	}

//	public static string GetPath (GameObject gameObject)
//	{
//		if (gameObject == null)
//			return string.Empty;
//
//		string path = "/" + gameObject.name;
//		Transform transform = gameObject.transform;
//
//		while (transform.parent != null)
//		{
//			transform = transform.parent;
//			path = "/" + transform.gameObject.name + path;
//		}
//
//		return path;
//	}
//
//	public static GameObject GetGameObject (string path)
//	{
//		return GameObject.Find (path);
//	}

	public static GameObject GetGameObject (int id)
	{
		if (id == 0) {
			return null;
		}
		return ReferenceManager.GetObject (id);
	}

	public static int AddGameObject (GameObject obj) {
		if (obj == null) {
			Debug.Log ("Set NULL");
			return 0;
		}
		var id = ReferenceManager.AddObject (obj);
//		EditorUtility.SetDirty (ReferenceManager);
		return id;
	}

	public static void AttachToParent(Transform parent, Transform child, string attachmentPoint, Vector3 postion, Quaternion rotation) {
//		Debug.Log ("Attaching");

		var bone = FindChildRecursive (parent, attachmentPoint); //  _player.transform.FindChild (boneName);

		if (bone == null) {
			Debug.LogWarning ("No attachment point!: " + attachmentPoint);
			return;
		}

		child.parent = bone;
		child.localPosition = postion;
		child.localRotation = rotation; // Quaternion.Euler (rotation);
	}

	public static Transform FindChildRecursive(Transform transform, string childName) {
		if (transform.FindChild (childName) != null) {
			return transform.FindChild (childName);
		}

		for (var i=0; i<transform.childCount; i++) {
			var child = FindChildRecursive (transform.GetChild (i), childName);
			if (child != null) {
				return child;
			}
		}
		return null;
	}


}
