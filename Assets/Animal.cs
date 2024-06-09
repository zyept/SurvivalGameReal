using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public string animalName;
    public bool playerInRange;

    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;
    
    [Header("Sounds")]
    [SerializeField] AudioSource soundChannel;
    [SerializeField] AudioClip rabbitHitAndScream;
    [SerializeField] AudioClip rabbitHitAndDie;

    private Animator animator;
    public bool isDead;
    enum AnimalType
    {
        Rabbit,
        Lion,
        Snake
    }
    [SerializeField] AnimalType thisAnimalType;

    private void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (isDead==false)
        {
            if (currentHealth <= 0)
            {
                PlayDyingSound();

                animator.SetTrigger("DIE");
                GetComponent<AI_Movement>().enabled = false;

                isDead = true;
            }
            else
            {
                PlayHitSound();

            } 
        }
    }

    private void PlayDyingSound()
    {

        switch(thisAnimalType)
        {
            case  AnimalType.Rabbit:
                soundChannel.PlayOneShot(rabbitHitAndDie);
                break;
            case AnimalType.Lion:
               // soundChannel.PlayOneShot();// Lion Sound
                break;
            default :
                break;
               // 
        }
       
    }

    private void PlayHitSound()
    {
        switch(thisAnimalType)
        {
            case  AnimalType.Rabbit:
                soundChannel.PlayOneShot(rabbitHitAndScream);
                break;
            case AnimalType.Lion:
               // soundChannel.PlayOneShot();// Lion Sound
                break;
            default :
                break;
               // 
        }
       
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        { playerInRange = false; }
    }



}
