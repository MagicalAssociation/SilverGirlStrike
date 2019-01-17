using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSetSelectManager : MonoBehaviour
{
    [System.Serializable]
    public class Parameter
    {
        public CursorParam left;
        public CursorParam right;
        public CursorParam up;
        public CursorParam down;
    }
    public Parameter parameter;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
