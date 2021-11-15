using UnityEngine;
using HFPS.Player;

namespace HFPS.Systems
{
    public class DamageOverTime : MonoBehaviour
    {
        public int Damage = 10;
        public float DamageEvery = 2f;

        private HealthManager healthManager;
        private float time;

        /*void Awake()
        {
            healthManager = PlayerController.Instance.GetComponent<HealthManager>();
        }*/

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                healthManager = other.GetComponentInChildren<HealthManager> ();

                if ( healthManager )
                {
                    if ( time <= 0 )
                    {
                        healthManager.ApplyDamage ( Damage );
                        time = 2f;
                    }
                    else
                    {
                        time -= Time.deltaTime;
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                time = 0f;
        }
    }
}