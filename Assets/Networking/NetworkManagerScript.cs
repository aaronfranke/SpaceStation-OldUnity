
#if ENABLE_UNET

using System;
using System.Net;

namespace UnityEngine.Networking
{
    [AddComponentMenu("Network/NetworkManagerHUD")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

    public class NetworkManagerScript : NetworkManager
    {
        public string username = "Newbie";

        // Network game settings such as username and IP to connect to.

        public void SetUsername(string s)
        {
            username = s;
            print(s);
            PlayerPrefs.SetString("username", s);
        }

        public void SetAddress(string s)
        {
            networkAddress = s;
            char[] c = { '.' };
            IPAddress result = null;
            if (s.Length > 6 && (
                    IPAddress.TryParse(s, out result) ||
                    s.Split(c, 5).Length == 3 ||
                    s.Equals("localhost")
                ))
            {
                PlayerPrefs.SetString("networkAddress", s);
            }
        }

        public void SetPort(string s)
        {
            networkPort = Int32.Parse(s);
            PlayerPrefs.SetInt("networkPort", networkPort);
        }

        // End network game settings.

        public override void OnServerConnect(NetworkConnection Conn)
        {

            if (Conn.hostId >= 0)
            {
                Debug.Log("New Player has joined");
            }
        }

    }
};

#endif //ENABLE_UNET



