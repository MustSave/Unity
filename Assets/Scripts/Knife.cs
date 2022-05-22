using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    Transform tr;
    Transform mesh;
    Rigidbody rb;
    Collider col;
    public float rotSpeed = 30;
    public float moveSpeed = 5;
    public float distance = 1;

    private void Awake() 
    {
        tr = GetComponent<Transform>();
        mesh = tr.GetChild(0).GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        col = GetComponentInChildren<Collider>();

        rb.velocity = tr.forward * moveSpeed;
        rb.maxAngularVelocity = 100;
        rb.angularVelocity = tr.right * rotSpeed;

        StartCoroutine("SelfDestroy");
    }

    IEnumerator SelfDestroy()
    {
        yield return new WaitForSecondsRealtime(distance/moveSpeed);
        rb.velocity = rb.angularVelocity = Vector3.zero;
        col.enabled = false;

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
