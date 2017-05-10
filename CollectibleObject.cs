using UnityEngine;
using System.Collections;
//using UnityEditor;

public class CollectibleObject : Interactive {

	public int Number = 1;	// Number of objects included - Usually one
	public InventoryObject prefab;	// Reference to prefab

	void Start () {
		if (prefab == null) {
			Debug.LogError("Collectible object needs inventory prefab");
		}
	}

	#region implemented abstract members of Interactive
	public override IEnumerator InteractWithObject (GameObject sender) {
		Component[] inventories;

		// Create a list of all inventories the agent is carrying
		inventories = sender.GetComponentsInChildren<Inventory> ();
		// Initialise largestInventory with the default inventory
		Inventory largestInventory = sender.GetComponent<Inventory> ();

		// DebugConsole.Log("Searching for Inventories...");

		// Find the largest inventory with enough space
		foreach(Inventory currentInventory in inventories) {
			if(currentInventory.MaxSize > largestInventory.MaxSize && currentInventory.MaxSize - currentInventory.CurrentSize > prefab.Size) {
				largestInventory = currentInventory;
			}
		}

		// Try to put the item in the selected inventory, and destroy the object if it fits
		if(largestInventory.AddObject(prefab, Number)) {
			Destroy (gameObject);
		}

		if(sender.tag == "Player") {
			sender.transform.Find("Avatar").GetComponent<InteractionTracker>().InteractionList.Remove(gameObject);
		}

		yield break;
	}
	#endregion
}
