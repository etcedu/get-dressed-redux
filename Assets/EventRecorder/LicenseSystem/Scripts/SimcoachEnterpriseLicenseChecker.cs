using Newtonsoft.Json.Linq;
using SimcoachGames.EventRecorder;
using SimcoachGames.EventRecorder.Login;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SimcoachEnterpriseLicenseChecker : MonoBehaviour
{
    public static SimcoachEnterpriseLicenseChecker Instance;

    public static bool userAuthenticated;
    [SerializeField] EventRecorderEndpointSO endpointSO;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    
    public void GetLicenseStatus(Action<bool, int, string> handleLicenseCheck)
    {
        StartCoroutine(PostRoutine((success, response) =>
        {
            //code is the actual HTTPS response code
            //statusCode is our function specific response used to differentiate the results of a "successful" API call ("code" == 200)
            string message = "None";
            int statusCode = 400;
            try
            {
                int code = (int)JObject.Parse(response)["code"];
                if (code == 400)
                {
                    success = false;
                    response = JObject.Parse(response)["body"].ToString();
                }

                if (success)
                {
                    statusCode = (int)JObject.Parse(JObject.Parse(response)["body"].ToString())["statusCode"];
                    message = JObject.Parse(JObject.Parse(response)["body"].ToString())["message"].ToString();
                }
                else
                {
                    EventRecorderLog.Log("License Check Error: " + response);
                    statusCode = 400;
                    message = response;
                }
            }
            catch (Exception e)
            {
                handleLicenseCheck.Invoke(false, 400, e.Message);
            }

            handleLicenseCheck.Invoke(success, statusCode, message);
        }
        ));
    }

    static IEnumerator PostRoutine(Action<bool, string> handleServerResponse)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(Instance.endpointSO.endpoint))
        {
            string org = SimcoachLoginUserManagement.GetActiveOrganization() != null ? SimcoachLoginUserManagement.GetActiveOrganization() : "None";
            webRequest.method = "POST";
            byte[] bytes = Encoding.UTF8.GetBytes("{ \"organizationID\": \"" + org + "\", \"gameID\": \"" + EventRecorderSettingsSO.GetInstance().eventId + "\" }");
            webRequest.uploadHandler = new UploadHandlerRaw(bytes);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("x-api-key", Instance.endpointSO.apiKey);
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();
            EventRecorderLog.Log("License Check Result: " + webRequest.result);

            if (webRequest.result != UnityWebRequest.Result.Success)
                handleServerResponse.Invoke(false, webRequest.error);
            else
                handleServerResponse.Invoke(true, webRequest.downloadHandler.text);
        }
    }
}
