using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace SilverDogGames.Audio
{
    using Dissonance;
    using Dissonance.Audio.Capture;
    using UnityEngine.InputSystem;

    public class MicrophoneController : MonoBehaviour
    {
        [SerializeField] private Key toggleWindowKey = Key.M;
        [SerializeField] private BasicMicrophoneCapture microphoneCapture = null;
        [SerializeField] private DissonanceComms dissonanceComms = null;
        [SerializeField] private MicrophoneData currentDevice;
        [SerializeField] private List<MicrophoneData> microphones = null;
        [SerializeField] private float amplitudeSensitivity = 0.5f;
        [SerializeField] private bool showDebugWindow = true;
        [SerializeField] private Vector2 debugWindowDimensions;
        private Vector2 scrollPosition = Vector2.zero;

        private void Awake()
        {
            Debug.Assert(microphoneCapture != null, "BasicMicrophoneCapture is null.");
            Debug.Assert(dissonanceComms != null, "DissonanceComms is null.");
        }

        // Start is called before the first frame update
        void Start()
        {
            GetMicrophones();
            if (microphones?.Count > 0)
            {
                StartMicrophoneCapture(microphones.First());
            }
        }

        private void Update()
        {
            if (Keyboard.current[toggleWindowKey].wasPressedThisFrame)
            {
                showDebugWindow = !showDebugWindow;
            }
        }

        private void OnGUI()
        {
            if (!showDebugWindow) { return; }
            Rect windowRect = new Rect(Screen.width - debugWindowDimensions.x - 10, Screen.height / 2 - debugWindowDimensions.y / 2, debugWindowDimensions.x, debugWindowDimensions.y);
            using (new GUILayout.AreaScope(windowRect))
            {
                GUILayout.Box("Mic Config");
                GUILayout.Label(string.Format("Current device: {0}", CurrentDevice()));
                DrawMicrophoneInfoGui();

                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
                if (microphones != null && microphones.Count > 0)
                {
                    GUILayout.Label("Select device:");
                    foreach (var device in microphones)
                    {
                        if (GUILayout.Button(device.DeviceName))
                        {
                            StartMicrophoneCapture(device);
                        }
                    }
                }
                GUILayout.EndScrollView();
            }
        }

        public string CurrentDevice() => currentDevice.DeviceName;

        private void StartMicrophoneCapture(MicrophoneData device)
        {
            if (microphoneCapture != null)
            {
                if (microphoneCapture.IsRecording)
                {
                    microphoneCapture.StopCapture();
                }
                microphoneCapture.StartCapture(device.DeviceName);
                currentDevice = device;
            }
        }
        private void GetMicrophones()
        {
            string[] devices = Microphone.devices;
            if (microphones == null)
            {
                microphones = new List<MicrophoneData>(devices.Length);
            }
            microphones.Clear();
            foreach (string device in devices)
            {
                microphones.Add(new MicrophoneData() { DeviceName = device });
            }
        }
        private void DrawMicrophoneInfoGui()
        {
            if (Application.isPlaying && dissonanceComms != null)
            {
                var playerState = dissonanceComms.FindPlayer(dissonanceComms.LocalPlayerName);
                if (playerState != null && playerState.IsConnected)
                {
                    float sliderWidth = debugWindowDimensions.x / 4;

                    // Amplitude
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(string.Format("Amp ({0:n2})", playerState.Amplitude));
                    GUILayout.HorizontalSlider(playerState.Amplitude, 0f, 1f, GUILayout.Width(sliderWidth));
                    GUILayout.EndHorizontal();

                    // Loudness
                    float loudness = Mathf.Sqrt(playerState.Amplitude * amplitudeSensitivity);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(string.Format("V-Loudness ({0:n2})", loudness));
                    GUILayout.HorizontalSlider(loudness, 0f, 1f, GUILayout.Width(sliderWidth));
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}
