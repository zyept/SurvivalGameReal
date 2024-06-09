using System.Collections.Generic;
using UnityEngine;

public class PlacebleItem : MonoBehaviour
{
    [SerializeField] bool isGrounded;
    [SerializeField] bool isOverlappingItems;
    public bool isValidToBeBuilt;

    [SerializeField] BoxCollider solidCollider;
    private Outline outline;

    private void Start()
    {
        outline = GetComponent<Outline>();
    }

    void Update()
    {
        if (isGrounded && !isOverlappingItems)
        {
            isValidToBeBuilt = true;
        }
        else
        {
            isValidToBeBuilt = false;
        }

        var boxHeight = transform.lossyScale.y;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit groundHit, boxHeight * 0.5f, LayerMask.GetMask("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground") && PlacementSystem.Instance.inPlacementMode)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                Quaternion newRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                transform.rotation = newRotation;
                isGrounded = true;
            }
        }

        if (other.CompareTag("Tree") || other.CompareTag("pickable"))
        {
            isOverlappingItems = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground") && PlacementSystem.Instance.inPlacementMode)
        {
            isGrounded = false;
        }

        if ((other.CompareTag("Tree") || other.CompareTag("pickable")) && PlacementSystem.Instance.inPlacementMode)
        {
            isOverlappingItems = false;
        }
    }

    public void SetInvalidColor()
    {
        if (outline != null)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.red;
        }
    }

    public void SetValidColor()
    {
        if (outline != null)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.green;
        }
    }

    public void SetDefaultColor()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}
