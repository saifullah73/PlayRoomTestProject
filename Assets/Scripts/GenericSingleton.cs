using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericSingleton<T> : MonoBehaviour where T : Component {
	private static T _instance = null;
	public static T Instance => GetInstance();

	protected virtual void Awake() {
		// this.EnableLogging();
		if (_instance == null) {
			GetInstance();
			DontDestroyOnLoad(this.gameObject);
		}
		else {
			if (FindObjectsOfType<T>().Length >= 2) Destroy(gameObject);
		}
	}

	private static T GetInstance() {
		if (_instance == null) {
			if (!Application.isPlaying) return null;
			_instance = FindObjectOfType<T>();
			if (_instance == null) {
				GameObject obj = new GameObject();
				obj.name = typeof(T).Name;
				_instance = obj.AddComponent<T>();
			}
		}
		return _instance;
	}

}
