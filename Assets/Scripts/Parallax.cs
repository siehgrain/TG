using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField]private GameObject CameraPlayer;
    [SerializeField]private float lenght,startPos,speedParalax;
    private float ImageHeight;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position.x;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float temp = (CameraPlayer.transform.position.x * (1 - speedParalax));
        float dist = (CameraPlayer.transform.position.x * speedParalax);

        transform.position = new Vector3 (startPos + dist, transform.position.y, transform.position.z);
        if (temp > startPos + lenght)
        {
            startPos += lenght;
        }
        else if (temp < startPos - lenght)
        {
            startPos -= lenght;
        }
    }
}
