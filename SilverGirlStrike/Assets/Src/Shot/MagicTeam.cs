using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicTeam : MonoBehaviour
{
    public float speed;
    private void Update()
    {
        this.transform.Rotate(new Vector3(0, 0, speed));
    }
    public void NotActive()
    {
        this.gameObject.SetActive(false);
    }
}
