using UnityEngine;
using Simcoach.SimpleJSON;

namespace Simcoach.SkillArcade
{
    public static class CurrentSkiller
    {
        public static string emailAddr;
        public static string firstName;
        public static string lastName;
        public static string zipCode;
        public static string zipCodeLocServ;
        public static string skid;

        public static void ParseUserData(string jsonData)
        {
            JSONNode result = JSON.Parse(jsonData);
            emailAddr = result["data"]["email"];
            firstName = result["data"]["firstName"];
            lastName = result["data"]["lastName"];
            zipCode = result["data"]["zipCode"];
            skid = result["data"]["skid"];

            CheckCurrentSkillerData();
        }

        public static string GetInitials()
        {
            string initials = "";
            if (firstName != null && firstName.Length > 0)
                initials += firstName.ToUpper()[0];
            if (lastName != null && lastName.Length > 0)
                initials += lastName.ToUpper()[0];
            return initials;
        }

        public static void ClearSkiller()
        {
            emailAddr = "";
            firstName = "";
            lastName = "";
            zipCode = "";
            skid = "";
        }

        public static string GetSkid()
        {
            return skid;
        }

        public static string GetZipCode()
        {
            zipCodeLocServ = "";// Net.GetZipCode.instance.zipCodeFromLocationServices;

            if (!string.IsNullOrEmpty(zipCodeLocServ) && zipCodeLocServ != "-1")
                return zipCodeLocServ;
            else
                return zipCode;
        }

        public static void CheckCurrentSkillerData()
        {
            if(skid != null || skid != "")
            {
                if(FirstLogin())
                {
                    ClaimExistingProgress();
                }
            }
        }

        public static bool FirstLogin( )
        {
            
            string[] badges = {skid + GameIdentifiers.bronzeBadgeCompletedTag, skid + GameIdentifiers.silverBadgeCompletedTag, skid + GameIdentifiers.goldBadgeCompletedTag };
            bool firstLogin;

            if(PlayerPrefs.HasKey(badges[0]) || PlayerPrefs.HasKey(badges[1]) || PlayerPrefs.HasKey(badges[2]))
            {
                firstLogin = false;
            }
            else
            {
                firstLogin = true;
            }

            return firstLogin;
        }

        static void ClaimExistingProgress()
        {
            PlayerPrefs.SetInt(skid + GameIdentifiers.bronzeBadgeCompletedTag, PlayerPrefs.GetInt(GameIdentifiers.bronzeBadgeCompletedTag));
            PlayerPrefs.SetInt(skid + GameIdentifiers.silverBadgeCompletedTag, PlayerPrefs.GetInt(GameIdentifiers.silverBadgeCompletedTag));
            PlayerPrefs.SetInt(skid + GameIdentifiers.goldBadgeCompletedTag, PlayerPrefs.GetInt(GameIdentifiers.goldBadgeCompletedTag));
                    

            ResetBadge();
        }

        static void ResetBadge()
        {
            PlayerPrefs.SetInt(GameIdentifiers.bronzeBadgeCompletedTag, 0);
            PlayerPrefs.SetInt(GameIdentifiers.silverBadgeCompletedTag, 0);
            PlayerPrefs.SetInt(GameIdentifiers.goldBadgeCompletedTag, 0);

        }
    }
}
