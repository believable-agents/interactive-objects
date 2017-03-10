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

    [HideInInspector]
    public string Name;     // Name of the object

    [SerializeField]
    [HideInInspector]
    public string Tag; // {get; private set;}		// Tag on the object
	
	[SerializeField]
    [HideInInspector]
    public float Size;	// Size/Weight of the object
	
	[SerializeField]
    [HideInInspector]
    public Texture Icon; // {get; private set;}		// Icon to represent the object in the GUI
	
	[SerializeField]
    public AttachmentPoint AttachTo;

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
		if(NameToCompare.CompareTo(name) == 0) {
			return true;
		}
		return false;
	}

    Transform owner;
    void Awake()
    {
        if (transform.GetComponent<MeshRenderer>() == null)
        {
            owner = transform.parent;
        } else
        {
            owner = transform;
        }
    }
	
	public void Attach(Transform actor) {
        //var info = transform.FindChild("Inventory");
        //if (info == null)
        //{
        //    Debug.LogError("Inventory object needs to have child object with name 'Inventory'");
        //    return;
        //}
		GameObjectUtility.AttachToParent (actor, this.owner, AttachTo.ToString(), transform.localPosition, transform.localRotation);
	}
}