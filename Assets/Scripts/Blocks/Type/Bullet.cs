using UnityEngine;
using System.Collections;
public class Bullet : MonoBehaviour
{
    public ParticleSystem hitEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DestroyAfterTime(5f));
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRollingMovement player = other.GetComponent<PlayerRollingMovement>();
            if (player != null)
            {
                player.died = true;
                Debug.Log("Bullet hit player");
                ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                effect.Play();
                StartCoroutine(PlayerDieCoroutine(effect.main.duration, player, effect));
            }
            else
            {
                Debug.LogWarning("PlayerRollingMovement component not found on Player!");
            }
        }
    }
    private IEnumerator PlayerDieCoroutine(float time, PlayerRollingMovement player, ParticleSystem effect)
    {
        yield return new WaitForSeconds(time);

        Destroy(gameObject);
        player.Die();
        Debug.Log("Player died due to bullet");
        Destroy(effect.gameObject);
    }
    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
        Debug.Log("Bullet destroyed after timeout");
    }
}
