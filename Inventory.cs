using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// using UnityEditor;

public class Inventory : MonoBehaviour {
	
	public List<StoredObject> ItemList = new List<StoredObject>();	// Stores all of the items in the inventory
	public float MaxSize;		// Maximum total size the inventory can hold
	public float CurrentSize;	// Running total size of all objects in inventory

	void Start () {
		CurrentSize = 0f;
	}


	public StoredObject FindByName(string name) {
		return ItemList.Find (w => w != null && w.Object.Name == name);
	}

	public StoredObject FindOneByTag(string tag) {
		return ItemList.Find (w => w != null && w.Object.Tag == tag);
	}

	public List<StoredObject> FindAllByTag(string tag) {
		return ItemList.FindAll (w => w != null && w.Object.Tag == tag);
	}

	// Returns true if successful
	public bool AddObject (InventoryObject objectToStore, int objNumber) {

		// Check that there is room for the object
		if(CurrentSize + (objectToStore.Size * objNumber) > MaxSize) {
			Debug.Log("Not enough room!");
			return false;
		}

		// Check if there is already an instance of this object in the inventory
		var item = ItemList.Find(w => w.Object.Name == objectToStore.Name);
		// If not, create a new entry
		if (item == null) {
			ItemList.Add(new StoredObject(objectToStore, objNumber));
			Debug.Log("New Object Added!");
		}
		// Otherwise, just increase the entry by the amount given
		else {
			item.AddNumber(objNumber);
			// DebugConsole.Log("Object Added!");
		}

		// Track the total size
		CurrentSize += objectToStore.Size * objNumber;

		return true;
	}

	// Returns number removed
	public int RemoveObject (string objName, int objNumber) {

		// Check if the item exists in the inventory
		var item = ItemList.Find (w => w.Object.Name == objName);
		if(item == null) {
			Debug.LogError("No such object");
			return 0;
		}

		// If there are more objects than are being removed, remove them and return that number
		if(item.Number > objNumber) {
			item.SubtractNumber(objNumber);
			CurrentSize -= item.Object.Size * objNumber;
			// DebugConsole.Log("Object Removed");
			return objNumber;
		}
		// If trying to remove as many or more objects as there are, return the number of objects there were, and remove the object from the inventory
		else {
			CurrentSize -= item.Object.Size * item.Number;
			ItemList.Remove(item);
			// DebugConsole.Log("Objects All Removed");
			return item.Number;
		}
	}

	// Returns all the details of the object. Used for taking an object from an inventory and putting it in the real world
	public StoredObject RetrieveObject (string objName, int objNumber) {

		// Check that there is an instance of this object
		var item = ItemList.Find (w => w.Object.Name == objName);
		if(item == null) {
			return null;
		}

		// Get the details first, in case the entry is removed
		// TODO: Not sure why you do this ... StoredObject prefab = new StoredObject(item.Name, item.Tag, item.Size, 0, item.Prefab, item.Icon);
		item.AddNumber(RemoveObject(objName, objNumber));
		// Return a reference to the prefab
		return item;
	}
}
