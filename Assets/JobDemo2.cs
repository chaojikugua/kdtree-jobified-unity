using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class JobDemo2 : MonoBehaviour {

    struct VelocityJob : IJob
    {
        [ReadOnly]
        public NativeArray<Vector3> velocity;
        public NativeArray<Vector3> position;
        
        public float deltaTime;
 
        public void Execute()
        {
            for (int i = 0; i < position.Length; i++){
                position[i] = position[i] + velocity[i] * deltaTime;
            }
        }
        
    }
 	NativeArray<Vector3> position1;
 	NativeArray<Vector3> velocity1;
 	
 	Vector3[] position2;
 	Vector3[] velocity2;
 	
 	
 	void Start(){
 		int n = 50000;
 		
 		position1 = new NativeArray<Vector3>(n, Allocator.Persistent);
 		velocity1 = new NativeArray<Vector3>(n, Allocator.Persistent);
 		
 		position2 = new Vector3[n];
 		velocity2 = new Vector3[n];
 		
 		for (var i = 0; i < velocity1.Length; i++){
            velocity1[i] = new Vector3(0, 10, 0);
            velocity2[i] = new Vector3(0, 10, 0);
        }
 	}
 	
 	
 	int jobMode = 0;
 	void Update(){
 		if(jobMode == 0){
 			Update1();
 		}
 		else if(jobMode == 1){
 			Update2();
 		}
 		else if(jobMode == 2){
 			Update3();
 		}
 		
 		
 		if(Input.GetKeyDown(KeyCode.A)){
 			jobMode = 0;
 			Debug.Log("jobMode "+jobMode);
 		}
 		else if(Input.GetKeyDown(KeyCode.B)){
 			jobMode = 1;
 			Debug.Log("jobMode "+jobMode);
 		}
 		else if(Input.GetKeyDown(KeyCode.C)){
 			jobMode = 2;
 			Debug.Log("jobMode "+jobMode);
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
    
    void Update3(){
    	float deltaTime = Time.deltaTime;
    	for(int i=0; i<position1.Length; i++){
    		position2[i] = position2[i] + velocity2[i] * deltaTime;
    	}
    }
    
    void OnApplicationQuit(){
		position1.Dispose();
		velocity1.Dispose();
	}
    
}
