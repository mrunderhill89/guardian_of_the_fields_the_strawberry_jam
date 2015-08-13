using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SortByZ : IComparer<float>{
	public int Compare(float a, float b){
		return 0;
	}
}

public class GameObjectPool : MonoBehaviour {
	public GameObject prefab;
	public int total_instances;
	public Vector3 min_position = new Vector3(0,0,0);
	public Vector3 max_position = new Vector3(0,0,0);

	protected SortedList<float,GameObject> instances;
	protected List<float> search_list;
	protected List<GameObject> object_list;
	public Transform origin = null;
	public float range;
	public float update_distance = 1.0f;

	public bool isVisible(float z){
		float oz = origin.position.z;
		return (z >= (oz - range) && z <= (oz + range));
	}

	Vector3 getNextPosition(){
		return this.transform.position + new Vector3 (
			RandomUtils.random_float (min_position.x, max_position.x),
			RandomUtils.random_float (min_position.y, max_position.y),
			RandomUtils.random_float (min_position.z, max_position.z)
		);
	}

	// Use this for initialization
	void Start () {
		instances = new SortedList<float,GameObject>();
		Vector3 nextPosition; GameObject instance;
		for (int i = 0; i < total_instances; i++) {
			nextPosition = this.getNextPosition();
			instance = GameObject.Instantiate(prefab, nextPosition, Quaternion.identity) as GameObject;
			try{
				instances.Add(
					nextPosition.z,
					instance
	            );
			} catch(ArgumentException err){
				Debug.Log("Hmm... C# says that adding "+instance.ToString()+" didn't work...");
			}
			instance.SetActive(isVisible(nextPosition.z));
		}
		search_list = new List<float>(instances.Keys);
		object_list = new List<GameObject>(instances.Values);
		StartCoroutine(visibility_routine());
	}
	IEnumerator visibility_routine(){
		float oz = 0.0f,head,tail;
		while (true) {
			yield return null;
			if (Math.Abs(oz - origin.position.z) > update_distance){
				oz = origin.position.z;
				head = oz + range;
				tail = oz - range;
				yield return StartCoroutine(yielded_binary_search(search_list, head, visibility_at_index));
				yield return StartCoroutine(yielded_binary_search(search_list, tail, visibility_at_index));
			}
		}
	}
	IEnumerator yielded_binary_search<T>(List<T> list, T target, Func<int, IEnumerator> found_it) where T:IComparable{
		int head = 0, tail = list.Count - 1, mid = head + ((tail - head) / 2), comp = -1;
		T value;
		while (head <= tail && comp != 0) {
			mid = head + ((tail - head) / 2);
			value = list[mid];
			comp = target.CompareTo(value);
			if (comp > 0){
				//Target is greater than the head but less than the middle.
				head = mid +1;
			} else if(comp < 0){
				tail = mid -1;
			}
			yield return null;
		}
		yield return StartCoroutine(found_it(mid));
	}
	IEnumerator visibility_at_index(int index){
		check_visibility(index);
		yield return null;
		for (int up = index+1; up < search_list.Count; up++) {
			if (check_visibility (up))
				break;
			yield return null;
		}
		yield return null;
		for (int down = index-1; down > 0; down--) {
			if (check_visibility (down))
				break;
			yield return null;
		}
	}
	bool check_visibility(int index){
		bool shouldBeVisible, visible;
		GameObject closest;
		shouldBeVisible = isVisible(search_list[index]);
		closest = object_list[index];
		visible = closest.activeSelf;
		closest.SetActive(shouldBeVisible);
		return (visible == shouldBeVisible);
	}
	// Update is called once per frame
	void Update () {
	}
}
