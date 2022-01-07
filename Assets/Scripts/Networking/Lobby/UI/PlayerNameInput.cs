using UnityEngine;
using UnityEngine.UI;

namespace SilverDogGames.Mirror.Lobby
{
    public class PlayerNameInput : MonoBehaviour
    {
        [Header ( "UI" )]
        [SerializeField]
        private InputField m_nameInputField = null;
        [SerializeField]
        private Button m_continueButton = null;

        public static string DisplayName { get; private set; }
        private const string PlayerPrefsNameKey = "Player Name";

        // Start is called before the first frame update
        private void Start () => SetUpInputField ();

        private void SetUpInputField ()
        {
            if ( !PlayerPrefs.HasKey ( PlayerPrefsNameKey ) ) { return; }

            string defaultName = PlayerPrefs.GetString ( PlayerPrefsNameKey );
            m_nameInputField.text = defaultName;

            SetPlayerName ( defaultName );
        }

        public void SetPlayerName ( string playerName )
        {
            m_continueButton.interactable = !string.IsNullOrEmpty ( playerName );
        }

        public void SavePlayerName ()
        {
            DisplayName = m_nameInputField.text;
            PlayerPrefs.SetString ( PlayerPrefsNameKey, DisplayName );
        }
    }
}
