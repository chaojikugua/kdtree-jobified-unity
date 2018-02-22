using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class JobDemo : MonoBehaviour {

    struct VelocityJob : IJob
    {
        [ReadOnly]
        public NativeArray<Vector3> velocity;
        public NativeArray<Vector3> position;
 
        public float deltaTime;
 
        public void Execute()
        {
            for (int i = 0; i < position.Length; i++)
                position[i] = position[i] + velocity[i] * deltaTime;
        }
    }
 	NativeArray<Vector3> position1;
 	NativeArray<Vector3> velocity1;
 	
 	void Start(){
 		position1 = new NativeArray<Vector3>(50000, Allocator.Persistent);
 		velocity1 = new NativeArray<Vector3>(50000, Allocator.Persistent);
 		
 		for (var i = 0; i < velocity1.Length; i++){
            velocity1[i] = new Vector3(0, 10, 0);
        }
 	}
 	
 	
 	bool isJobRunning = true;
 	void Update(){
 		if(isJobRunning){
 			Update1();
 		}
 		else{
 			Update2();
 		}
 		if(Input.GetKeyDown(KeyCode.A)){
 			isJobRunning = !isJobRunning;
 			Debug.Log("Is job running: "+isJobRunning);
 		}
 	}
 	
    void Update1(){
        var job = new VelocityJob()
        {
            deltaTime = Time.deltaTime,
            position = position1,
            velocity = velocity1
        };
 
        JobHandle jobHandle = job.Schedule();
 
        jobHandle.Complete();
 
//         Debug.Log(job.position[0]);

    }
    
    void Update2(){
    	float deltaTime = Time.deltaTime;
    	for(int i=0; i<position1.Length; i++){
    		position1[i] = position1[i] + velocity1[i] * deltaTime;
    	}
    }
    
    void OnApplicationQuit(){
		position1.Dispose();
		velocity1.Dispose();
	}
    
}
