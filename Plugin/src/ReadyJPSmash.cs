using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using UnityEngine;

namespace JohnPaularatusEnemy;
public class ReadyJPSmash : MonoBehaviour
{
    public ReadyJP mainscript;
    public AudioClip smashSound;
    public AudioSource smashAudioSource;
    private List<PlayerControllerB> playersBeingSmashed = new List<PlayerControllerB>();
    private void OnTriggerEnter(Collider other)
    {
        if (mainscript.meleeAttack && other.CompareTag("Player") && !playersBeingSmashed.Contains(other.GetComponent<PlayerControllerB>()))
        {
            PlayerControllerB player = other.GetComponent<PlayerControllerB>();
            playersBeingSmashed.Add(player);
            StartCoroutine(SmashPlayer(player));
            smashAudioSource.PlayOneShot(smashSound);
            player.DamagePlayer(25, true, true, CauseOfDeath.Bludgeoning, 0, false);
        }
    }

    private IEnumerator SmashPlayer(PlayerControllerB player)
    {
        float duration = 0.5f;
        Vector3 direction = (player.transform.position - this.mainscript.gameObject.transform.position).normalized;
        while (duration > 0) {
            duration -= Time.fixedDeltaTime;
            player.externalForces = direction * 50f;
            yield return new WaitForFixedUpdate();
        }
        playersBeingSmashed.Remove(player);
    }
}