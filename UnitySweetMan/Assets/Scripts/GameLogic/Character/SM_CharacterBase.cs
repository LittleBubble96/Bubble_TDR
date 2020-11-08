using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_CharacterBase : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public float pushForce;
    [HideInInspector]
    public Vector3 pushDir;
    [HideInInspector]
    public bool isStuned = false;
    [HideInInspector]
    public bool wasStuned = false; //If player was stunned before get stunned another time
    [HideInInspector]
    public bool canMove = true; //If player is not hitted
    [HideInInspector]
    public bool slide = false;
    [HideInInspector]
    public float gravity = 10.0f;

    [HideInInspector] 
    public int Order = -1;
   

    public virtual void HitPlayer(Vector3 velocityF, float time)
    {
        rb.velocity = velocityF;

        pushForce = velocityF.magnitude;
        pushDir = Vector3.Normalize(velocityF);
        StartCoroutine(Decrease(velocityF.magnitude, time));
    }
    
    private IEnumerator Decrease(float value, float duration)
    {
        if (isStuned)
            wasStuned = true;
        isStuned = true;
        canMove = false;

        float delta = 0;
        delta = value / duration;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            yield return null;
            if (!slide) //Reduce the force if the ground isnt slide
            {
                pushForce = pushForce - Time.deltaTime * delta;
                pushForce = pushForce < 0 ? 0 : pushForce;
                //Debug.Log(pushForce);
            }
            rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0)); //Add gravity
        }

        if (wasStuned)
        {
            wasStuned = false;
        }
        else
        {
            isStuned = false;
            canMove = true;
        }
    }
}
