using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Pathfinding
{
	/// <summary>
	/// Sets the destination of an AI to the position of a specified object.
	/// This component should be attached to a GameObject together with a movement script such as AIPath, RichAI or AILerp.
	/// This component will then make the AI move towards the <see cref="target"/> set on this component.
	///
	/// See: <see cref="Pathfinding.IAstarAI.destination"/>
	///
	/// [Open online documentation to see images]
	/// </summary>
	[UniqueComponent(tag = "ai.destination")]
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_a_i_destination_setter.php")]
	public class AIDestinationSetter : VersionedMonoBehaviour
	{
		/// <summary>The object that the AI should move to</summary>
		public Transform target;
		public GameObject CCTVCamera;
		public float searchRadius;
		
		//private GameObject CCTV;

		IAstarAI ai;

		void OnEnable()
		{
			ai = GetComponent<IAstarAI>();
			// Update the destination right before searching for a path as well.
			// This is enough in theory, but this script will also update the destination every
			// frame as the destination is used for debugging and may be used for other things by other
			// scripts as well. So it makes sense that it is up to date every frame.
			if (ai != null) ai.onSearchPath += Update;
		}
		//     private void Start()
		//     {
		//targetObject = null;
		//     }
		void OnDisable()
		{
			if (ai != null) ai.onSearchPath -= Update;
		}

		/// <summary>Updates the AI's destination every frame</summary>
		void Update()
		{

			bool withinDistance = false;

			//CCTVCamera = GameObject.Find("Interactant/camera_0");

			//ts = CCTVCamera.GetComponent<SpriteRenderer>;
			float distance;
			distance = Vector3.Distance(transform.position, target.position);
			//CCTV = GameObject.Find("Interactant/");
			if (distance < searchRadius) withinDistance = true;
			//if(CCTVCamera.activeSelf == false) withinDistance = true;
			if (target != null && ai != null && withinDistance == true)
			{
				ai.destination = target.position;
			}
			else if (target != null && ai != null && CCTVCamera.activeSelf == false && withinDistance == true)
			{
				ai.destination = CCTVCamera.transform.position;

			}
			else if (target != null && ai != null && CCTVCamera.activeSelf == false)
			{
				ai.destination = CCTVCamera.transform.position;
			}
			else
			{
				ai.destination = gameObject.transform.position;
			}


		}
	}

}