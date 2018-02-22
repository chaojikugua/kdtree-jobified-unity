using UnityEngine;
using System.Collections;

public class FPSCount : MonoBehaviour {
	
	public static FPSCount active;
	
	private float startTime = 0f;
	private float updateTime = 0f;
	
	[HideInInspector] public float fps = 0.0f;
	public float refreshTime = 1.0f;
	
	private int frameCount = 0;
	
	
// 	public bool displayMessage = false;
	
// 	[HideInInspector] public RTSMaster rtsm;
	
	
	
	IEnumerator CalculateFPS() {
		while(true) {
			fps = frameCount/(updateTime-startTime);
			startTime = Time.time;
			frameCount = 0;
			yield return new WaitForSeconds (refreshTime);
		}
	}
	
	
	void Awake(){
		active = this;
	}
	
	
	void Start () {
	
		startTime = Time.time;
	    updateTime = startTime;
	    
		StartCoroutine (CalculateFPS());
	
	}
	
	
	void Update () {
		updateTime = Time.time;
		frameCount++;
	}
	
	
	void OnGUI(){
// 		if(displayMessage){
			GUI.Label(new Rect(Screen.width * 0.05f, Screen.height * 0.9f, 500f, 20f),"FPS: " + fps);
// 		}	
	}
	
	
	
	
}
