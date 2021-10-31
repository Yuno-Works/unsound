/*
 * DynamicObject.cs - by ThunderWire Studio
 * DynamicObjectHelper.cs - by Silver Dog Games
 * ver. 2.0
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ThunderWire.Helpers;
using HFPS.Player;

#if TW_LOCALIZATION_PRESENT
using ThunderWire.Localization;
#endif

namespace HFPS.Systems
{
    [RequireComponent ( typeof ( DynamicObject ) )]
    public class DynamicObjectHelper : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> IgnoreObjects = new List<Transform> ();

        private DynamicObject m_dynamicObject = null;
        private List<Collider> m_ignoreColliders = new List<Collider> ();

        private void Awake ()
        {
            // Assign component reference
            m_dynamicObject = GetComponent<DynamicObject> ();

            // Call recursive search method on every transform
            foreach ( Transform ignoreObj in IgnoreObjects )
            {
                AddIgnoreColliders ( ignoreObj );
            }

            // Add ignore colliders to DynamicObject
            m_dynamicObject.IgnoreColliders.AddRange ( m_ignoreColliders );
        }

        /// <summary>
        /// Recursively add all Colliders from <paramref name="ignoreObject"/>.
        /// </summary>
        /// <param name="ignoreObject">Transform to search, including its children.</param>
        private void AddIgnoreColliders ( Transform ignoreObject )
        {
            if ( ignoreObject )
            {
                Collider [] colliders = ignoreObject.GetComponents<Collider> ();
                if ( colliders != null )
                {
                    foreach ( Collider collider in colliders )
                    {
                        m_ignoreColliders.Add ( collider );
                    }
                }
                foreach ( Transform child in ignoreObject )
                {
                    AddIgnoreColliders ( child );
                }
            }
        }
    }
}