using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DestroyerScript : MonoBehaviour
{
    [SerializeField] string[] objects_to_destroy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (objects_to_destroy.Contains(collision.gameObject.tag))
        {
            Destroy(collision.gameObject);
        }
    }


}
