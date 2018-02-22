using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class JobDemo1 : MonoBehaviour {

    struct VelocityJob1 : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Vector3> velocity;
        public NativeArray<Vector3> position;
 
        public float deltaTime;
 
        public void Execute(int i)
        {
			position[i] = position[i] + velocity[i] * deltaTime;
        }
    }
    
    struct VelocityJob2
    {
        [ReadOnly]
        public NativeArray<Vector3> velocity;
        public NativeArray<Vector3> position;
 
        public float deltaTime;
 
        public void Execute(int i)
        {
			position[i] = position[i] + velocity[i] * deltaTime;
        }
    }
    
    struct VelocityJob3
    {
        [ReadOnly]
        public Vector3[] velocity;
        public Vector3[] position;
 
        public float deltaTime;
 
        public void Execute(int i)
        {
			position[i] = position[i] + velocity[i] * deltaTime;
        }
    }
    
 	NativeArray<Vector3> position1;
 	NativeArray<Vector3> velocity1;
 	
 	Vector3[] position3;
 	Vector3[] velocity3;
 	
 	void Start(){
 		int n = 100000;
 		
 		position1 = new NativeArray<Vector3>(n, Allocator.Persistent);
 		velocity1 = new NativeArray<Vector3>(n, Allocator.Persistent);
 		
 		position3 = new Vector3[n];
 		velocity3 = new Vector3[n];
 		
 		for (var i = 0; i < velocity1.Length; i++){
            velocity1[i] = new Vector3(0, 10, 0);
            velocity3[i] = new Vector3(0, 10, 0);
        }
 	}
 	
 	
 	int jobMode = 1;
 	void Update(){
 		if(jobMode == 1){
 			Update1();
 		}
 		else if(jobMode == 2){
 			Update2();
 		}
 		else if(jobMode == 3){
 			Update3();
 		}
 		
 		
 		if(Input.GetKeyDown(KeyCode.A)){
 			jobMode = 1;
 			Debug.Log("jobMode "+jobMode);
 		}
 		else if(Input.GetKeyDown(KeyCode.B)){
 			jobMode = 2;
 			Debug.Log("jobMode "+jobMode);
 		}
 		else if(Input.GetKeyDown(KeyCode.C)){
 			jobMode = 3;
 			Debug.Log("jobMode "+jobMode);
 		}
 	}
 	
    void Update1(){
    	int processorCount = System.Environment.ProcessorCount;

        var job = new VelocityJob1()
        {
            deltaTime = Time.deltaTime,
            position = position1,
            velocity = velocity1
        };
 
        JobHandle jobHandle = job.Schedule(position1.Length, processorCount);
 
        jobHandle.Complete();
    }
    
    void Update2(){
    	var job = new VelocityJob2(){
    		deltaTime = Time.deltaTime,
            position = position1,
            velocity = velocity1
    	};
				
    	for(int i=0; i<position1.Length; i++){
    		job.Execute(i);
    	}
    }
    
    void Update3(){
    	var job = new VelocityJob3(){
    		deltaTime = Time.deltaTime,
            position = position3,
            velocity = velocity3
    	};
    	
    	for(int i=0; i<position3.Length; i++){
    		job.Execute(i);
    	}
    }
    
    void OnApplicationQuit(){
		position1.Dispose();
		velocity1.Dispose();
	}
    
}
