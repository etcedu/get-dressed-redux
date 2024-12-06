using System.Collections;
using UnityEngine;

namespace Simcoach.SkillArcade
{

    public class WebViewController : MonoBehaviour
    {
        /*
        WebViewObject webViewObject;
        public TweenAlpha_SA panelTween;
        public UnityEngine.UI.Text pageTitle;

        public RectTransform header;
        public RectTransform safeArea;
        public Canvas webViewCanvas;

        public bool gameIsHorizontal = true;
        bool closedWebview;

        string currentURL;

        IEnumerator Start()
        {
            webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
            DontDestroyOnLoad(GameObject.Find("WebViewObject").gameObject);
            webViewObject.Init(
                cb: (msg) =>
                {
                    Debug.Log(string.Format("CallFromJS[{0}]", msg));
                },
                err: (msg) =>
                {
                    Debug.Log(string.Format("CallOnError[{0}]", msg));
                },
                ld: (msg) =>
                {
                    Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
#if !UNITY_ANDROID
                webViewObject.EvaluateJS(@"
                  window.Unity = {
                    call: function(msg) {
                      var iframe = document.createElement('IFRAME');
                      iframe.setAttribute('src', 'unity:' + msg);
                      document.documentElement.appendChild(iframe);
                      iframe.parentNode.removeChild(iframe);
                      iframe = null;
                    }
                  }
                ");
#endif
                if (!webViewObject.GetVisibility() && !closedWebview)
                        webViewObject.SetVisibility(true);

                },
                enableWKWebView: false);

            //webViewObject.SetMargins(0, Screen.height - Mathf.RoundToInt(Screen.height * 0.88f), 0, 0);
             webViewObject.SetMargins(0, (int)(header.rect.height * webViewCanvas.scaleFactor) + (int)(webViewCanvas.pixelRect.height - (safeArea.anchorMax.y * webViewCanvas.pixelRect.height)), 0, 0);

            yield break;
        }

        public void CloseWebButton_OnClick()
        {
            webViewObject.SetVisibility(false);
            panelTween.PlayReverse();
            closedWebview = true;

            currentURL = "";

            if (gameIsHorizontal)
            {
                Screen.autorotateToLandscapeLeft = true;
                Screen.autorotateToLandscapeRight = true;

#if UNITY_IOS
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            StartCoroutine(delayAllowAutoRotate());
#else
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                Screen.orientation = ScreenOrientation.AutoRotation;
#endif
            }

        }

        IEnumerator delayAllowAutoRotate()
        {
            yield return new WaitForSeconds(1.0f);
            Screen.orientation = ScreenOrientation.AutoRotation;
        }

        public void OpenURL(string url, string title)
        {
            pageTitle.text = title;
            webViewObject.LoadURL(url.Replace(" ", "%20"));
            currentURL = url.Replace(" ", "%20");
            panelTween.PlayForward();
            closedWebview = false;
            //webViewObject.SetVisibility(true);

            if (gameIsHorizontal)
            {
                Screen.autorotateToLandscapeLeft = true;
                Screen.autorotateToLandscapeRight = true;
                Screen.orientation = ScreenOrientation.AutoRotation;
            }

             webViewObject.SetMargins(0, (int)(header.rect.height * webViewCanvas.scaleFactor) + (int)(webViewCanvas.pixelRect.height - (safeArea.anchorMax.y * webViewCanvas.pixelRect.height)), 0, 0);

        }

        public void OpenURLInBrowser()
        {
            Application.OpenURL(currentURL);
        }

        private void Update()
        {
            if (webViewObject.GetVisibility() && closedWebview)
                webViewObject.SetVisibility(false);
        }
        */
    }
        
}