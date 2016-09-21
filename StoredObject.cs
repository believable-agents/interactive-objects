// This is the variable for all objects stored in an inventory
using UnityEngine;


[System.Serializable]
public class StoredObject {

	[SerializeField]
	private InventoryObject storedObject;
	public InventoryObject Object { get { return storedObject; } set { storedObject = value; } } //{get; private set;}		// Name of the object
	
	[SerializeField]
	private int number;
	public int Number { get { return number; } set { number = value; } } //{get; private set;}		// How many of this object are stored here
	
	public StoredObject() { }
	
	// Basic constructor
	public StoredObject (InventoryObject objectToStore, int addNumber) {
		storedObject = objectToStore;
		number = addNumber;
	}
	
	public void AddNumber (int addNumber) {Number += addNumber;}
	public void SubtractNumber (int subractNumber) {Number -= subractNumber;}
}