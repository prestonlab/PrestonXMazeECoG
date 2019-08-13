using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteObject : MonoBehaviour
{

    public Material spr1;
    public Material spr2;
    public Material spr3;
    public Material spr4;
    public Material spr5;
    public Material spr6;
    public Material spr7;
    public Material spr8;
    public Material spr9;
    public Material spr10;
    public Material spr11;
    public Material spr12;

    void Start()
    {
        this.Sprite(0);
    }

    void Sprite(int num)
    {
        if(num == 0)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        else if(num == 1)
        {
            GetComponent<MeshRenderer>().material = spr1;
            GetComponent<MeshRenderer>().enabled = true;
        }
        else if(num == 2)
        {
            GetComponent<MeshRenderer>().material = spr2;
            GetComponent<MeshRenderer>().enabled = true;
        }
        else if(num == 3)
        {
            GetComponent<MeshRenderer>().material = spr3;
            GetComponent<MeshRenderer>().enabled = true;
        }
        else if(num == 4)
        {
            GetComponent<MeshRenderer>().material = spr4;
            GetComponent<MeshRenderer>().enabled = true;
        }
        else if(num == 5)
        {
            GetComponent<MeshRenderer>().material = spr5;
            GetComponent<MeshRenderer>().enabled = true;
        }
        else if(num == 6)
        {
            GetComponent<MeshRenderer>().material = spr6;
            GetComponent<MeshRenderer>().enabled = true;
        }
        else if (num == 7)
        {
            GetComponent<MeshRenderer>().material = spr7;
            GetComponent<MeshRenderer>().enabled = true;
        }
        else if (num == 8)
        {
            GetComponent<MeshRenderer>().material = spr8;
            GetComponent<MeshRenderer>().enabled = true;
        }
        else if (num == 9)
        {
            GetComponent<MeshRenderer>().material = spr9;
            GetComponent<MeshRenderer>().enabled = true;
        }
        else if (num == 10)
        {
            GetComponent<MeshRenderer>().material = spr10;
            GetComponent<MeshRenderer>().enabled = true;
        }
        else if (num == 11)
        {
            GetComponent<MeshRenderer>().material = spr11;
            GetComponent<MeshRenderer>().enabled = true;
        }
        else if (num == 12)
        {
            GetComponent<MeshRenderer>().material = spr12;
            GetComponent<MeshRenderer>().enabled = true;
        }
        //Debug.Log(name + ", Sprite: " + num);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.SendMessage("GiveReward", gameObject);
        }
    }

}
