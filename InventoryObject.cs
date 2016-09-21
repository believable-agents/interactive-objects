// This is the variable for all objects stored in an inventory
using UnityEngine;
using System.Collections;

public enum AttachmentPoint {
	None,
	RightHand,
	LeftHand
}

public enum ObjectType {
	Drink,
	Food,
	Other
}

[System.Serializable]
public class InventoryObject : MonoBehaviour {

	[SerializeField]
	private string name;
	public string Name { get { return name; }  set { name = value; } }//{get; private set;}		// Name of the object

	[SerializeField]
	private string tag;
	public string Tag { get { return tag; } set {tag = value; } } // {get; private set;}		// Tag on the object
	
	[SerializeField]
	private float size;
	public float Size { get { return size; } set { size = value; } } //{get; private set;}		// Size/Weight of the object
	
	[SerializeField]
	private Texture icon;
	public Texture Icon  { get { return icon; } set { icon = value; } } // {get; private set;}		// Icon to represent the object in the GUI
	
	[SerializeField]
	private AttachmentPoint attachTo;
	public AttachmentPoint AttachTo { get { return attachTo; } set { attachTo = value; } }
	
//	[SerializeField]
//	public Vector3 localPosition;
//	public Vector3 LocalPosition  { get { return localPosition; } set { localPosition = value; }  }
//	
//	[SerializeField]
//	public Vector3 localRotation;
//	public Vector3 LocalRotation  { get { return localRotation; } set { localRotation = value; }  }
	
	public InventoryObject() { }

	// Returns true if the given name is that of this object
	public bool CompareName (string NameToCompare) {
		if(NameToCompare.CompareTo(Name) == 0) {
			return true;
		}
		return false;
	}
	
	public void Attach(Transform actor) {
		GameObjectUtility.AttachToParent (actor, transform, attachTo.ToString(), transform.localPosition, transform.localRotation);
	}
}