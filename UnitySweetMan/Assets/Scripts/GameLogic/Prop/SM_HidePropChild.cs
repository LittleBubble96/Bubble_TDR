using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_HidePropChild : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            //Debug.DrawRay(contact.point, contact.normal, Color.white);
            if (collision.gameObject.tag == "Player")
            {
                 SM_HideProp prop = transform.GetComponentInParent<SM_HideProp>();
                 if (prop!=null)
                 {
                     prop.SetHide();
                 }
            }
        }
    }
}
