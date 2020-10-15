using UnityEngine;
using System.Collections;

public class AutoDestructTeamRed : MonoBehaviour
{
    public float DestructTime = 2.0f;

    void Start()
    {
        Destroy(gameObject, DestructTime);
    }
}
