using UnityEngine;
using System.Collections;

public class ContentCentricARManager : MonoBehaviour 
{
	public GameObject[] markerObjects;
	public bool fullscreen = false;
	public float alignment = 0.5f;
	public bool reorientBranding = true;
	float lastSpottedTime;
	
	StringWrapper stringWrapper;
	
	void Awake() 
	{
		// Initialize String
		stringWrapper = new StringWrapper(null, camera, reorientBranding, fullscreen, alignment);

		// Load some image targets
		for (uint i = 0; i < markerObjects.Length; i++)
		{
			stringWrapper.LoadImageMarker("Marker " + (i + 1), "png");
		}

		// Prevent the iOS keyboard from introducing an unwanted
		// black frame when rotating
		iPhoneKeyboard.autorotateToLandscapeLeft = false;
		iPhoneKeyboard.autorotateToLandscapeRight = false;
		iPhoneKeyboard.autorotateToPortrait = false;
		iPhoneKeyboard.autorotateToPortraitUpsideDown = false;
	}
	
	void Update() 
	{
		// Perform marker image tracking
		uint markerCount = stringWrapper.Update();
		
		// Handle detected markers
		for (uint i = 0; i < markerCount; i++)
		{
			// Fetch tracker data for this marker
			StringWrapper.MarkerInfo markerInfo = stringWrapper.GetDetectedMarkerInfo(i);
			
			if (markerInfo.imageID < markerObjects.Length)
			{
				// Orient the camera according to the marker
				transform.parent = markerObjects[markerInfo.imageID].transform;

				transform.localRotation = Quaternion.Inverse(markerInfo.rotation);
				transform.localPosition = transform.localRotation * -markerInfo.position;
				
				lastSpottedTime = Time.time;
				
				break;
			}
		}
		
		// Marker not spotted? Point camera away
		if (lastSpottedTime != Time.time)
		{
			transform.localPosition = new Vector3(1000000, 0, 0);
			transform.localRotation = Quaternion.identity;
		}
	}
}
