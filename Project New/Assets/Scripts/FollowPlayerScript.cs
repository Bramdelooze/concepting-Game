using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerScript : MonoBehaviour {

    public GameObject player;

	// Update is called once per frame
	void Update() {
        int direction;
        if (transform.rotation.y == 0)
        {
            direction = -1;
        }
        else direction = 1;
        if(player == null)
        {
            return;
        }
        Vector3 offset = new Vector3(direction, 0, 0);
        transform.position = player.transform.position + offset;
        transform.rotation = player.transform.rotation;
	}
}
