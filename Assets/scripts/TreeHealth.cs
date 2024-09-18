using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeHealth : MonoBehaviour
{
    public int health;
    // Start is called before the first frame update
    public void TreeFall()
    {
        gameObject.AddComponent<Rigidbody>();
        Rigidbody rig = GetComponent<Rigidbody>();
        rig.isKinematic = false;
        rig.useGravity = true;
        rig.mass = 200;
        rig.constraints = RigidbodyConstraints.FreezeRotationY;
        Destroy(gameObject, 5);
    }
}
