using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class EquipableItem : MonoBehaviour
{


    public Animator animator;
    public bool swingWait = false;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetMouseButtonDown(0) &&
            InventorySystem.Instance.isOpen == false &&
            CraftingSystem.Instance.isOpen == false &&
            SelectionManager.Instance.handIsVisible == false&& swingWait==false && !ConstructionManager.Instance.inConstructionMode
            ) 
        {
            swingWait = true;
            StartCoroutine(SwingSoundDelay());

            animator.SetTrigger("hit");
            
        }
    }
    public void GetHit()
    {
        GameObject selectedTree = SelectionManager.Instance.selectedTree;
        if (selectedTree != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.chopSound);
            selectedTree.GetComponent<ChoppableTree>().GetHit();
        }
    }
    IEnumerator SwingSoundDelay()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.toolSwingSound);
        swingWait = false;
    }
}
