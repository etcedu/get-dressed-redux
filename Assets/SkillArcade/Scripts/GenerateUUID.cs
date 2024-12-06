using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace Simcoach.Net
{
    public static class GenerateUUID
    {
        public static string uuid = "";

        public static void Init()
        {
            if (!PlayerPrefs.HasKey("firstInstall"))
            {
                PlayerPrefs.SetInt("firstInstall", 1);
                Guid guid = Guid.NewGuid();
                uuid = guid.ToString();
                PlayerPrefs.SetString("uuid", uuid);
                Debug.Log(uuid);
            }
            else
            {
                Debug.Log("No UUID generated as not firstInstall");
            }
        }

        public static string GetUUID()
        {
            return PlayerPrefs.GetString("uuid");
        }
    }
}
