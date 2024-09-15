using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 100;

    // Update is called once per frame
    void Update()
    {
        //fly forwardfor 2 seconds and then destroy
        transform.localPosition += transform.forward * Time.deltaTime * bulletSpeed;
        
        Destroy(gameObject, 2);
    }
}
