using UnityEngine;
using UnityEngine.UI;

[RequireComponent ( typeof ( InputField ) )]
public class PlayerPrefText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start ()
    {
        GetComponent<InputField> ().text = PlayerPrefs.GetString ( "IPAddressJoin" );
    }

    public void SetText ()
    {
        PlayerPrefs.SetString ( "IPAddressJoin", GetComponent<InputField> ().text );
    }
}
