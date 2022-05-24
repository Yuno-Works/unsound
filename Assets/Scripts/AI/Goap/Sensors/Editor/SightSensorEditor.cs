using UnityEngine;

namespace SilverDogGames.AI.Goap.Editor
{
    using SilverDogGames.AI.Goap.Sensors;
    using UnityEditor;

    [CustomEditor(typeof(SightSensor))]
    public class SightSensorEditor : Editor
    {
        private void OnSceneGUI()
        {
            SightSensor sensor = target as SightSensor;
            Handles.color = Color.white;
            Handles.DrawWireArc(sensor.transform.position, Vector3.up, Vector3.forward, 360, sensor.ViewRadius);
            Vector3 viewAngleA = sensor.DirFromAngle(-sensor.ViewAngle / 2f, false);
            Vector3 viewAngleB = sensor.DirFromAngle(sensor.ViewAngle / 2f, false);

            Handles.DrawLine(sensor.transform.position, sensor.transform.position + viewAngleA * sensor.ViewRadius);
            Handles.DrawLine(sensor.transform.position, sensor.transform.position + viewAngleB * sensor.ViewRadius);
        }
    }
}
