using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.right * Input.GetAxisRaw("Horizontal");
        transform.position += transform.forward * Input.GetAxisRaw("Vertical");
    }
}
