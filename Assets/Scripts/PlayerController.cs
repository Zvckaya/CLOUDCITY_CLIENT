using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;

    private bool isMoving;

    private Vector2 input;

    private Animator animator;

    public LayerMask solidObjectsLayer;
    public LayerMask interactablesLayer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");


            if (input.x != 0) input.y = 0;

           
            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if(isWalkable(targetPos))
                    StartCoroutine(Move(targetPos));
            }
        }

        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.Space))
            Interact();
    }

    void Interact()
    {
        Vector3 facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY")); //x,y,값 받아오기 
        Vector3 interactPos = transform.position + facingDir; //벡터 더하기 

        Debug.DrawLine(transform.position, interactPos,Color.red,1f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.2f, interactablesLayer); 
        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }

    private bool isWalkable(Vector3 targetPos)
    {
        if(Physics2D.OverlapCircle(targetPos,0.2f,solidObjectsLayer| interactablesLayer) != null)
        {
            return false;
        }
        return true;
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }

}
