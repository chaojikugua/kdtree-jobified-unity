// KDTree.cs - A Stark, September 2009.

//	This class implements a data structure that stores a list of points in space.
//	A common task in game programming is to take a supplied point and discover which
//	of a stored set of points is nearest to it. For example, in path-plotting, it is often
//	useful to know which waypoint is nearest to the player's current
//	position. The kd-tree allows this "nearest neighbour" search to be carried out quickly,
//	or at least much more quickly than a simple linear search through the list.

//	At present, the class only allows for construction (using the MakeFromPoints static method)
//	and nearest-neighbour searching (using FindNearest). More exotic kd-trees are possible, and
//	this class may be extended in the future if there seems to be a need.

//	The nearest-neighbour search returns an integer index - it is assumed that the original
//	array of points is available for the lifetime of the tree, and the index refers to that
//	array.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;


public struct KDTreeStruct {
	
	public int pivotIndex;
	
//	Change this value to 2 if you only need two-dimensional X,Y points. The search will
//	be quicker in two dimensions.
	const int numDims = 3;
	
	[ReadOnly] NativeArray<Vector3> points;
	[ReadOnly] NativeArray<int> leftChilds;
	[ReadOnly] NativeArray<int> rightChilds;
	[ReadOnly] NativeArray<int> axies;
	
//	Make a new tree from a list of points.
	public void MakeFromPoints(NativeArray<Vector3> points1) {
		NativeArray<int> indices = Iota(points1.Length);
		
		points = points1;
		
		leftChilds = new NativeArray<int>(points1.Length, Allocator.Persistent);
		rightChilds = new NativeArray<int>(points1.Length, Allocator.Persistent);
		axies = new NativeArray<int>(points1.Length, Allocator.Persistent);
		
		for(int i=0; i<points1.Length; i++){
			leftChilds[i] = -1;
			rightChilds[i] = -1;
			axies[i] = -1;
		}
		
		MakeFromPointsInner(0, 0, points.Length - 1, indices, -1, -1, true);
		indices.Dispose();
	}
	

//	Recursively build a tree by separating points at plane boundaries.
	void MakeFromPointsInner(
					int depth,
					int stIndex, int enIndex,
					NativeArray<int> inds,
					int parentPivotIndex,
					int direction,
					bool isFirstTime
		) {
		
		
		int axis1 = depth % numDims;
		int splitPoint = FindPivotIndex(inds, stIndex, enIndex, axis1);
		
		if(isFirstTime){
			isFirstTime = false;
			pivotIndex = inds[splitPoint];
		}
		
		int pivotIndex1 = inds[splitPoint];
		
		axies[pivotIndex1] = axis1;
		
		if(parentPivotIndex > -1){
			if(direction == 0){
				leftChilds[parentPivotIndex] = pivotIndex1;
			}
			else if(direction == 1){
				rightChilds[parentPivotIndex] = pivotIndex1;
			}
		}
		
		int leftEndIndex = splitPoint - 1;
		
		if (leftEndIndex >= stIndex) {
			MakeFromPointsInner(depth + 1, stIndex, leftEndIndex, inds, pivotIndex1, 0, false);
		}
		
		int rightStartIndex = splitPoint + 1;
		
		if (rightStartIndex <= enIndex) {
			MakeFromPointsInner(depth + 1, rightStartIndex, enIndex, inds, pivotIndex1, 1, false);
		}
		
	}
	
	
	void SwapElements(NativeArray<int> arr, int a, int b) {
		int temp = arr[a];
		arr[a] = arr[b];
		arr[b] = temp;
	}
	

//	Simple "median of three" heuristic to find a reasonable splitting plane.
	int FindSplitPoint(NativeArray<int> inds, int stIndex, int enIndex, int axis) {
		float a = points[inds[stIndex]][axis];
		float b = points[inds[enIndex]][axis];
		int midIndex = (stIndex + enIndex) / 2;
		float m = points[inds[midIndex]][axis];
		
		if (a > b) {
			if (m > a) {
				return stIndex;
			}
			
			if (b > m) {
				return enIndex;
			}
			
			return midIndex;
		} else {
			if (a > m) {
				return stIndex;
			}
			
			if (m > b) {
				return enIndex;
			}
			
			return midIndex;
		}
	}
	

//	Find a new pivot index from the range by splitting the points that fall either side
//	of its plane.
	public int FindPivotIndex(NativeArray<int> inds, int stIndex, int enIndex, int axis) {
		int splitPoint = FindSplitPoint(inds, stIndex, enIndex, axis);

		Vector3 pivot = points[inds[splitPoint]];
		SwapElements(inds, stIndex, splitPoint);

		int currPt = stIndex + 1;
		int endPt = enIndex;
		
		while (currPt <= endPt) {
			Vector3 curr = points[inds[currPt]];
			
			if ((curr[axis] > pivot[axis])) {
				SwapElements(inds, currPt, endPt);
				endPt--;
			} else {
				SwapElements(inds, currPt - 1, currPt);
				currPt++;
			}
		}
		
		return currPt - 1;
	}
	
	
	public NativeArray<int> Iota(int num) {
		NativeArray<int> result = new NativeArray<int>(num, Allocator.Persistent);
		
		for (int i = 0; i < num; i++) {
			result[i] = i;
		}
		
		return result;
	}
	
	
	public int FindNearest2(Vector3 pt) {
		float bestSqDist = 1000000000f;
		int bestIndex = -1;
		
		Search2(pt, ref bestSqDist, ref bestIndex, pivotIndex);
		
		return bestIndex;
	}	

//	Recursively search the tree.
	void Search2(Vector3 pt, ref float bestSqSoFar, ref int bestIndex, int pind) {

		Vector3 pt1 = points[pind];
		int leftChild = leftChilds[pind];
		int rightChild = rightChilds[pind];
		int ax = axies[pind];
		
		
		float mySqDist = (pt1 - pt).sqrMagnitude;
		
		if (mySqDist < bestSqSoFar) {
			bestSqSoFar = mySqDist;
			bestIndex = pind;
		}
		
		
		float planeDist = pt[ax] - pt1[ax];
		
		int selector = planeDist <= 0 ? 0 : 1;
		
		int ichild = -1;
		if(selector == 0){
			ichild = leftChild;
		}
		else if(selector == 1){
			ichild = rightChild;
		}
		
		
		if(ichild > -1){
			Search2(pt, ref bestSqSoFar, ref bestIndex, ichild);
		}
		
		
		
		selector = (selector + 1) % 2;
		
		ichild = -1;
		if(selector == 0){
			ichild = leftChild;
		}
		else if(selector == 1){
			ichild = rightChild;
		}
		
		float sqPlaneDist = planeDist * planeDist;
		
		
		if((ichild > -1) && (bestSqSoFar > sqPlaneDist)){
			Search2(pt, ref bestSqSoFar, ref bestIndex, ichild);
		}
		
	}
	

//	Get a point's distance from an axis-aligned plane.
	float DistFromSplitPlane(Vector3 pt, Vector3 planePt, int axis) {
		return pt[axis] - planePt[axis];
	}
	
	public void DisposeArrays(){
		leftChilds.Dispose();
		rightChilds.Dispose();
		axies.Dispose();
	}
	
}
