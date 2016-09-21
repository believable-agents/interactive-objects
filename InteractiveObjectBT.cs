using UnityEngine;
using System.Collections;
using DG.Tweening;
using NodeCanvas.BehaviourTrees;
using NodeCanvas;
using ViAgents;
using NodeCanvas.Framework;

public class InteractiveObjectBT : Interactive
{
    public Vector3 positionOffset;
	public Vector3[] positionOffsets; // where will agent stand when it starts interacting with object
	public Vector3 lookAt; // where will agent look 
	public BehaviourTree BT;

	//private BehaviourTree btInstance;

//	public bool forceTransform = true;

	private Transform tran;
	private ViAgent agent;
    public bool[] spots;

	#region implemented abstract members of Interactive

	public Vector3 OffsetPosition { get { return GetPosition (positionOffset); } }
	public Vector3 LookAtPosition { get { return GetPosition (lookAt); } }

    private int GetSpot()
    {
        if (positionOffsets == null || positionOffsets.Length == 0)
        {
            return -1;
        }

        if (spots.Length == 0 && (positionOffsets != null && positionOffsets.Length > 0))
        {
            spots = new bool[positionOffsets.Length];
        }

        for (var i = 0; i < spots.Length; i++)
        {
            if (spots[i] == false)
            {
                spots[i] = true;
                return i;
            }
        }
        throw new UnityException("All spots are taken!");
    }

	public IEnumerator InteractWithObject (GameObject sender, bool forceTransform, string animationState, System.Action<bool> callback)
	{
	    var spotIndex = GetSpot();
	    var spot = spotIndex < 0 ? this.positionOffset : this.positionOffsets[spotIndex];
//		Debug.Log("Starting interaction ...");

		if (BT == null) {
			Debug.LogError(name + " does not have a behaviour tree assigned");
			yield break;
		}

//		if (btInstance == null)
//		{
//			btInstance = (BehaviourTree) Instantiate(BT, transform.position, transform.rotation);
//			//btInstance..transform.parent = GameObject.Find("@RuntimeTrees").transform; // this.transform; //organization
//			btInstance.name = sender.name + "-" + BT.name;
////			btInstance.name = sender.name + "-" + BT.name;
//		}

		// get agent reference for log purposes
		this.agent = sender.GetComponent<ViAgent>();

		// we can force that user will appear at the defined location
		if (forceTransform)
		{
			// wait till avatar is at full stop
			if (!string.IsNullOrEmpty(animationState)) {
				yield return StartCoroutine(MecanimUtility.WaitForState(sender.GetComponent<Animator>(), animationState));
			}

			// itween to the new position
		    sender.transform.DOMove(GetPosition(spot), 0.5f);
			yield return new WaitForSeconds(0.5f);

			// now set the position
//			this.Log("Position set to: " + GetPosition(positionOffset));
			//sender.transform.position = GetPosition(positionOffset);

			// set look only if it is not empty
			if (!lookAt.Equals(Vector3.zero))
			{
				var p = GetPosition(lookAt);
				p = new Vector3(
				p.x,
				sender.transform.position.y,
				p.z);

//				this.Log("Look at: " + p);
				//sender.transform.LookAt(p);
				yield return StartCoroutine (AffineUtility.RotateTowards (sender.transform, p));
			} 
			this.Log("Position Forced");
		}

//		Debug.Log("Init action ...");

		// take blackboard from component
		var blackboard = sender.GetComponent<Blackboard> ();

		// if blackbord does not exists, try to take it from BT
		//if (blackboard == null) {
		//	blackboard = (Blackboard) BT.blackboard;
		//}

		//// add blackboard if it does not exist!
		//if (blackboard == null) {
		//	blackboard = gameObject.AddComponent<Blackboard> ();
		//}


		// copy all values from object blackboard
		blackboard.SetValue ("InteractiveObject", gameObject);
		blackboard.SetValue ("OffsetPosition", OffsetPosition);
		blackboard.SetValue ("LookAtPosition", LookAtPosition);
		//blackboard.SetValue ("Actor", sender);

	    BT.repeat = false;
	    var i = (BehaviourTree) Instantiate(BT);
        
        i.StartGraph (sender.transform, blackboard, (result) => GraphStoppedCallback(callback, result, spotIndex));

		this.Log ("Tree started.");

		yield return null;
	}

	private void GraphStoppedCallback(System.Action<bool> callback, bool result, int idx) {
		this.Log("Tree stopped.");

	    if (idx >= 0)
	    {
	        this.spots[idx] = false;
	    }


	    if (callback != null)
		{
			callback(result);
		}
	}

	public override IEnumerator InteractWithObject (GameObject sender) {
//		sender.transform.position = GetPosition(positionOffset);
//		
//		// set look only if it is not empty
//		if (!lookAt.Equals (Vector3.zero)) {	
//			var p = GetPosition(lookAt);
//			sender.transform.LookAt (
//				new Vector3(
//					p.x,
//					sender.transform.position.y,
//					p.z));
//		}
//		
//		// take blackboard from component
//		var blackboard = GetComponent<Blackboard> ();
//		
//		// add blackboard if it does not exist!
//		if (blackboard == null) {
//			blackboard = gameObject.AddComponent<Blackboard> ();
//		}
//		// copy all values from object blackboard
//		blackboard.SetDataValue ("interactiveObject", gameObject);
//		blackboard.SetDataValue ("actor", sender);
//		
//		BT.StartGraph (sender.transform.FindChild("Avatar"), blackboard, null);
		yield return null;
	}

	public void StopGraph() {
		if (this.BT != null)
		{
			this.BT.Stop();
		}
	}

	private void Log(string message) {
		if (this.agent != null)
		{
			agent.Log(string.Format("✫ ({0}) {1}", gameObject.name,  message));
		} else
		{
			Debug.Log(message);
		}
	}

	static Vector3 positionSize = new Vector3 (0.3f, 0.05f, 0.3f);
	static Vector3 lookAtSize = new Vector3 (0.2f, 0.05f, 0.2f);

	public void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
        Gizmos.DrawCube(GetPosition(positionOffset), positionSize);

        Gizmos.color = Color.yellow;
        if (positionOffsets != null)
	    {
	        foreach (var offset in positionOffsets)
	        {
	            Gizmos.DrawCube(GetPosition(offset), positionSize);
	        }
	    }

	    Gizmos.color = Color.blue;
		Gizmos.DrawCube(GetPosition(lookAt), lookAtSize);
	}

	Vector3 GetPosition(Vector3 offset) {
		var p = transform.position + offset;
		return  AffineUtility.RotateAroundPoint(p, transform.position, transform.rotation);
	}

	#endregion
}
