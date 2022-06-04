using Mirror;

namespace SilverDogGames.AI.Goap.Agents
{
    using UnityEngine;
    using ReGoap.Unity;

    [RequireComponent(typeof(NetworkIdentity))]
    public class BaseCreatureAgent : ReGoapAgentAdvanced<string, object>
    {
        protected override void Start()
        {
            if (!GetComponent<NetworkIdentity>().isServer)
            {
                enabled = false;
                return;
            }
            base.Start();
        }
    }
}
