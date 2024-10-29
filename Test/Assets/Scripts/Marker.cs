using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public bool view;

    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform); 
        if(view == false)
        {
            gameObject.SetActive(false);
        }
        else
            view = false;
    }
}
