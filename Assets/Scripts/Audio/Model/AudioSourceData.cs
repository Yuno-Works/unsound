namespace SilverDogGames.Audio
{
    using UnityEngine;

    [System.Serializable]
    public struct AudioSourceData
    {
        public string Name;
        [Range(0f, 1f)] public float Loudness;
        public Vector3 Position;

        public AudioSourceData(string name, float loudness, Vector3 position)
        {
            Name = name;
            Loudness = loudness;
            Position = position;
        }
    }
}
