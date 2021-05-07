using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Flock : MonoBehaviour

{
    public FlockinManager myManager;
    float speed;
    bool turning = false;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(myManager.minSpeed, myManager.maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        //cria um limite de area ate onde o peixe pode nadar
        Bounds b = new Bounds(myManager.transform.position, myManager.swinLimits * 2);
        if (!b.Contains(transform.position))
        {
            turning = true;

        }
        else
        {
            turning = false;
        }

        //move o peixe devolta para o cardume
        if (turning)
        {
            Vector3 direction = myManager.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), myManager.rotationSpeed * Time.deltaTime);
        }
        else
        {
            if(Random.Range(0, 100) < 10)
                speed = Random.Range(myManager.minSpeed, myManager.maxSpeed);
                if (Random.Range(0, 100) < 20)
                    AppRules();
         
            
        }
        transform.Translate(0, 0, Time.deltaTime * speed);
    }

    void AppRules()
    {

        GameObject[] gos;
        gos = myManager.allFish;

        Vector3 vcenter = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        int groupSize = 0;

        foreach (GameObject go in gos)
        {
            //corige a direção do cardume
            if (go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                if (nDistance <= myManager.neighbuorDistance)
                {
                    vcenter += go.transform.position;
                    groupSize++;

                    //evita a colizão dos peixes
                    if (nDistance < 1.0)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }
                    Flock anotherFlok = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlok.speed;
                }

            }
        }
        if(groupSize > 0)
        {
            vcenter = vcenter / groupSize + (myManager.goalPos - this.transform.position);
            speed = gSpeed / groupSize;

            //faz a movimentação e a rotação parecida com um peixe
            Vector3 direction = (vcenter + vavoid) - transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), myManager.rotationSpeed * Time.deltaTime);
            }
        }

    }
}
