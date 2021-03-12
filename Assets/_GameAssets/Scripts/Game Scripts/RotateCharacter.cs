using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotateCharacter : MonoBehaviour
{
    private RotateCharacter myscript;
    public Vector3 vector;
    public GameObject model;
    private float speed = 125f;
    private void Start()
    {
        myscript = GetComponent<RotateCharacter>();
    }
    void Update()
    {
        
        if (!AwardManager.instance.clickedButton)
        {
            transform.Rotate(0, speed * Time.deltaTime, 0);

        }
    }
    public void Rotatechar()
    {
        transform.DORotate(vector, 1f);
    }
}
