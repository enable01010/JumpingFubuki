using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clear : MonoBehaviour
{
    [SerializeField] Transform character;
    bool finish = false;
    private void Update()
    {
        Vector3 distance = character.position - transform.position;
        distance.z = 0;
        if (distance.magnitude < 3 && finish == false)
        {
            Debug.Log("‚ ‚½‚Á‚½");
            SceneAnimation.instance.LoadScene(0);
            finish = true;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("‚ ‚½‚Á‚½");
        if (collision.gameObject.tag == "Player")
        {
            SceneAnimation.instance.LoadScene(0);
        }
    }

}
