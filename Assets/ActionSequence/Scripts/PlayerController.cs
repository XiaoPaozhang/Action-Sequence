using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionSequence
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 8f;
        void Update()
        {
            Move();
        }

        private void Move()
        {
            if(Input.GetKey(KeyCode.A))
            {
                transform.position += Vector3.left * Time.deltaTime * moveSpeed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += Vector3.right * Time.deltaTime * moveSpeed;
            }
        }
    }
}
