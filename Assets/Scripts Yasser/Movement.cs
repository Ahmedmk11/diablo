using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Movement : MonoBehaviour
{
    public Camera camera;
    private RaycastHit hit;
    private NavMeshAgent agent;
    private Animator animator;
    public float speedo;

    private string groundTag = "Ground";
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) )
        {
            //print("mouse down");
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {

                agent.SetDestination(hit.point);
                Vector3 direction = hit.point - transform.position;

                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.5f);

            }
            
        }
        float speed = agent.velocity.magnitude; // Get the current speed of the player
        if (speed > 3)
        {
            speedo = -1;
        }
        else if (speed > 0)
        {
            speedo = 1;
        }
        else
        {
            speedo = 0;
        }
        animator.SetFloat("Speed", speedo);

    }
}