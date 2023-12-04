using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            float x = Input.GetAxis("Horizontal");

            var deltaPosition = new Vector3(x, 0, 0) * speed * Time.deltaTime;

            transform.position += deltaPosition;
        }
    }
}
