using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    private Rigidbody rb;
    public float startingSpeed;
    public float mass;
    public float density;
    public float radius = 0.5f;

    void setUpDensity()
    {
        this.radius = Mathf.Pow((this.mass / this.density * 3 / 4 / Mathf.PI), (1f / 3f)) / Mathf.Pow(10, 8);
        float factor = radius * 8;
        this.transform.localScale = new Vector3(factor, factor, factor);
    }

    void merge(GameObject o1, GameObject o2)
    {
        float m1 = o1.GetComponent<Movement>().mass / Mathf.Pow(10, 20);
        float m2 = o2.GetComponent<Movement>().mass / Mathf.Pow(10, 20);
        //Debug.Log(this + " " + m1 + " " + m2);
        if(m1 >= m2)
        {
            //Destroy(o2.GetComponent<Movement>());
            o1.GetComponent<Movement>().mass += m2 * Mathf.Pow(10, 20);
            o1.GetComponent<Movement>().setUpDensity();
            Rigidbody rb2 = o2.GetComponent<Rigidbody>();
            Vector3 v2 = rb2.velocity;
            Vector3 push = v2 * v2.magnitude * m2 / 2;
            float lenght = Mathf.Sqrt(push.magnitude * 2 / (m2 + m1));
            push = push.normalized * lenght / 10000;
            Debug.Log(push.x + " " + o1.GetComponent<Rigidbody>().velocity.x);
            o1.GetComponent<Rigidbody>().velocity += push;
            Debug.Log(push.x + " " + o1.GetComponent<Rigidbody>().velocity.x);
            Destroy(o2);
        }
/*        else
        {
            Destroy(o1.GetComponent<Movement>());
            o2.GetComponent<Movement>().mass += m1;
            o2.GetComponent<Movement>().setUpDensity();
            Rigidbody rb1 = o1.GetComponent<Rigidbody>();
            Vector3 v1 = rb1.velocity;
            Vector3 push = v1 * v1.magnitude * m1 / 2;
            float lenght = Mathf.Sqrt(push.magnitude / (m1 + m2) * 2);
            push = push.normalized * lenght * 0.4f;
            o2.GetComponent<Rigidbody>().velocity += push;
            Destroy(o1);
        }
        */
    }

    Vector3 gravityForce()
    {
        //Vector3 thrustDirection = -this.gameObject.transform.position;
        Vector3 thrustDirection = new Vector3();
        Collider[] tab = Physics.OverlapSphere(rb.position, 10000);
        for (int i = 0; i < tab.Length; i++)
        {
            if (tab[i].tag == "FlyingObject" && !(this.name == tab[i].name))
            {
                //Debug.Log(this + " " + tab[i]);
                float m2 = tab[i].GetComponent<Movement>().mass;
                float r = (tab[i].transform.position - this.transform.position).magnitude * Mathf.Pow(10, 9);
                //Debug.Log(r);
                if(Mathf.Pow(10, -9) * r <= (tab[i].GetComponent<Movement>().radius + this.GetComponent<Movement>().radius))
                {
                    merge(this.gameObject, tab[i].gameObject);
                }
                Vector3 p1 = tab[i].transform.position;
                Vector3 p2 = this.transform.position;
                thrustDirection += (p1 - p2) * m2 / (r * r * r);
                //Debug.Log(this + " " + thrustDirection + " " + tab[i].transform.position);
            }
        }
        //thrustDirection = Vector3.Normalize(thrustDirection);
        return thrustDirection * 100;
    }

    void Start()
    {
        setUpDensity();
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.forward * startingSpeed;
    }

    void FixedUpdate()
    {
        Vector3 g = gravityForce();
        rb.AddForce(g, ForceMode.Acceleration);
        Debug.DrawRay(this.transform.position, g * 2, Color.green);
        
        //Debug.Log(gravityForce() + " " + this);
    }
}
