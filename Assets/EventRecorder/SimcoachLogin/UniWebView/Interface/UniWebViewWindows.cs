#if UNITY_EDITOR_WIN || (!UNITY_EDITOR_OSX && !UNITY_STANDALONE_OSX && !UNITY_IOS && !UNITY_ANDROID)

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/* WTP NOTE: This is a custom UniWebViewInterface that supports the bare minimum needed for providing an
 * OAuth code flow through a browser on Windows.
 *
 *  WTP TODO:
 *      - Need to review timeouts. CDM Vision repo example uses .net's CancallationTokens for cancelling async tasks.
 *      - Need to review exception handling.
 *      - Need to ensure that all disposable resources are actually disposed
 */
public class UniWebViewInterface
{
    
    enum BrowserStatus
    {
        Success,
        UserCanceled,
        UnknownError,
    }

    class BrowserResult
    {
        public BrowserStatus Status { get; }
        
        public string RedirectUrl { get; }
        
        public string Error { get; }

        public BrowserResult(BrowserStatus status, string redirectUrl)
        {
            Status = status;
            RedirectUrl = redirectUrl;
        }
        
        public BrowserResult(BrowserStatus status, string redirectUrl, string error)
        {
            Status = status;
            RedirectUrl = redirectUrl;
            Error = error;
        }
    }
    
    class SessionData
    {
        public string name;
        public string baseUrl;
        public string scheme;
        public Dictionary<string, string> parameters;

        public SessionData(string name, string baseUrl, string scheme, Dictionary<string, string> parameters)
        {
            this.name = name;
            this.baseUrl = baseUrl;
            this.scheme = scheme;
            this.parameters = parameters;
        }

        public string GetFullUrl()
        {
            string queryString = UniWebViewAuthenticationUtils.CreateQueryString(parameters);
            return baseUrl + "?" + queryString;
        }
    }

    static readonly string LoginCompletePageHtml; //Loaded from file
    static readonly string LogoutCompletePageHtml;//Loaded from file
    static string PageHtml; //Contains the actual html to display

    static HttpListener LocalServer;
    //static CancellationTokenSource CancellationTokenSource;
    static TaskCompletionSource<BrowserResult> TaskCompletionSource;
    
    /*
     * WTP NOTE: If we need to support multiple auth sessions UniWebView seems to want to use a dictionary that stores
     * all session related information by session name. In our case I think we only actually need to use one for now.
     */
    // static readonly Dictionary<string, SessionData> SessionDict = new();
    // static void AddSession(SessionData sessionData) => SessionDict.Add(sessionData.name, sessionData);
    // static bool TryGetSession(string name, out SessionData sessionData) => SessionDict.TryGetValue(name, out sessionData);
    // static bool DeleteSession(string name) => SessionDict.Remove(name);
    static SessionData CurrentSession; 
    
    //WTP: Static constructor automatically loads html resources
    static UniWebViewInterface()
    {
        LoginCompletePageHtml = Resources.Load<TextAsset>("login_complete_html").ToString();
        LogoutCompletePageHtml = Resources.Load<TextAsset>("logout_complete_html").ToString();
    }
    
    public static bool IsAuthenticationIsSupported() { CheckPlatform(); return true; }
    
    /* WTP: Uniwebview passes us the full URL with query string. While this works well on android / iOS
     * we need to deconstruct the url in order to get the redirect uri. The redirect should be a local host domain that
     * we will listen to with a local server in order to retrieve the OAuth token. Once the token is received the UniWebView
     * authentication flow is used normally.
     */
    public static void AuthenticationInit(string name, string url, string scheme)
    {
        //WTP: Deconstruct the original URL.
        string[] split = url.Split('?');
        string baseUrl = split[0];
        string parameters = split[1];
        Dictionary<string, string> paramDict = new();
        foreach (string s in parameters.Split('&'))
        {
            string[] paramSplit = s.Split('=');
            //WTP: UniWebView passes in an escaped query string. We need to un-escape them in order to use them.
            paramDict.Add(paramSplit[0], UnityWebRequest.UnEscapeURL(paramSplit[1]));
        }

        //WTP: We add each session to a dictionary keyed by session name. This appears to be the way
        //UniWebView would like us to do it because all methods that preform operations on a session only pass in the
        //session name
        // AddSession(new SessionData(name, baseUrl, scheme, paramDict));

        
        //If session data is already present
        if (CurrentSession != null)
        {
            UniWebViewNativeListener nl = UniWebViewNativeListener.GetListener(CurrentSession.name);
            //Kill the session if it is active
            if (nl)
            {
                //WTP TODO: UniWebView is expecting us to pass a json serialized UniWebViewNativeResultPayload. Not sure the proper way to form.
                UniWebViewNativeResultPayload errorPayload = new();
                errorPayload.identifier = CurrentSession.name;
                errorPayload.data = "New auth session was started so this old was was killed.";
                nl.AuthErrorReceived(JsonUtility.ToJson(errorPayload));
            }
        }

        CurrentSession = new SessionData(name, baseUrl, scheme, paramDict);
    }

    public static void AuthenticationStart(string name)
    {
        // if (!TryGetSession(name, out SessionData sessionData)) //WTP NOTE: If we want to support more than one session at a time
        if (CurrentSession.name != name)
        {
            Debug.LogError($"Could not find session data by key {name}");
            return;
        }
        
        AuthenticateAsync(CurrentSession);
    }

    static async void AuthenticateAsync(SessionData sessionData)
    {
        try
        {
            BrowserResult browserResult = await StartBrowserAsync(sessionData);
            if (browserResult.Status == BrowserStatus.Success)
            {
                UniWebViewNativeListener.GetListener(sessionData.name).AuthFinished(browserResult.RedirectUrl);
                return;
            }

            UniWebViewNativeListener.GetListener(sessionData.name).AuthErrorReceived(browserResult.Error);
        }
        catch (TaskCanceledException e)
        {
          //WTP TODO: Probably need to do something here!   
        }
    }
    
    static async Task<BrowserResult> StartBrowserAsync(SessionData session)
    {
        TaskCompletionSource = new TaskCompletionSource<BrowserResult>();
        
        if(LocalServer == null)
            LocalServer = new HttpListener();
        
        try
        {
            //WTP HACK?: For whatever reason if we are authenticating  / logging out a different redirect param name is used.
            //It is helpful for us at this point in time to determine the operation we are performing though! 
            string prefix = "";
            if (session.parameters.ContainsKey("redirect_uri"))
            {
                prefix = session.parameters["redirect_uri"];
                PageHtml = LoginCompletePageHtml;
            }
            else if (session.parameters.ContainsKey("logout_uri"))
            {
                prefix = session.parameters["logout_uri"];
                PageHtml = LogoutCompletePageHtml;
            }

            if (!LocalServer.IsListening)
            {
                LocalServer.Prefixes.Add(prefix);
                LocalServer.Start();
                LocalServer.BeginGetContext(IncomingHttpRequest, LocalServer);    
            }
            
            Application.OpenURL(session.GetFullUrl());
            return await TaskCompletionSource.Task;
        }
        finally
        {
            LocalServer.Stop();
        }
    }

    static void IncomingHttpRequest(IAsyncResult result)
    {
        HttpListener httpListener = (HttpListener) result.AsyncState;
        HttpListenerContext ctx = httpListener.EndGetContext(result);
        HttpListenerRequest req = ctx.Request;
        using HttpListenerResponse resp = ctx.Response;
        resp.Headers.Set("Content-Type", "text/html");
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(PageHtml);
        resp.ContentLength64 = buffer.Length;
        Stream output = resp.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();

        TaskCompletionSource.SetResult(new BrowserResult(BrowserStatus.Success, req.Url.ToString()));
    }

    #region Not Implemented On Windows
    public static void SetLogLevel(int level) { CheckPlatform(); }
    public static void Init(string name, int x, int y, int width, int height) { CheckPlatform(); }
    public static void Destroy(string name) { CheckPlatform(); }
    public static void Load(string name, string url, bool skipEncoding, string readAccessURL) { CheckPlatform(); }
    public static void LoadHTMLString(string name, string html, string baseUrl, bool skipEncoding) { CheckPlatform(); }
    public static void Reload(string name) { CheckPlatform(); }
    public static void Stop(string name) { CheckPlatform(); }
    public static string GetUrl(string name) { CheckPlatform(); return ""; }
    public static void SetFrame(string name, int x, int y, int width, int height) { CheckPlatform(); }
    public static void SetPosition(string name, int x, int y) { CheckPlatform(); }
    public static void SetSize(string name, int width, int height) { CheckPlatform(); }
    public static bool Show(string name, bool fade, int edge, float duration, bool useAsync, string identifier) { CheckPlatform(); return false; }
    public static bool Hide(string name, bool fade, int edge, float duration, bool useAsync, string identifier) { CheckPlatform(); return false; }
    public static bool AnimateTo(string name, int x, int y, int width, int height, float duration, float delay, string identifier) { CheckPlatform(); return false; }
    public static void AddJavaScript(string name, string jsString, string identifier) { CheckPlatform(); }
    public static void EvaluateJavaScript(string name, string jsString, string identifier) { CheckPlatform(); }
    public static void AddUrlScheme(string name, string scheme) { CheckPlatform(); }
    public static void RemoveUrlScheme(string name, string scheme) { CheckPlatform(); }
    public static void AddSslExceptionDomain(string name, string domain) { CheckPlatform(); }
    public static void RemoveSslExceptionDomain(string name, string domain) { CheckPlatform(); }
    public static void SetHeaderField(string name, string key, string value) { CheckPlatform(); }
    public static void SetUserAgent(string name, string userAgent) { CheckPlatform(); }
    public static string GetUserAgent(string name) { CheckPlatform(); return ""; }
    public static void SetAllowAutoPlay(bool flag) { CheckPlatform(); }
    public static void SetAllowInlinePlay(bool flag) { CheckPlatform(); }
    public static void SetAllowJavaScriptOpenWindow(bool flag) { CheckPlatform(); }
    public static void SetForwardWebConsoleToNativeOutput(bool flag) { CheckPlatform(); }
    public static void SetAllowFileAccess(string name, bool flag) { CheckPlatform(); }
    public static void SetAllowFileAccessFromFileURLs(string name, bool flag) { CheckPlatform(); }
    public static void SetAllowUniversalAccessFromFileURLs(bool flag) { CheckPlatform(); }
    public static void SetJavaScriptEnabled(bool flag) { CheckPlatform(); }
    public static void SetLimitsNavigationsToAppBoundDomains(bool enabled) { CheckPlatform(); }
    public static void CleanCache(string name) { CheckPlatform(); }
    public static void ClearCookies() { CheckPlatform(); }
    public static void SetCookie(string url, string cookie, bool skipEncoding) { CheckPlatform(); }
    public static void RemoveCookies(string url, bool skipEncoding) { CheckPlatform(); }
    public static void RemoveCookie(string url, string key, bool skipEncoding) { CheckPlatform(); }
    public static string GetCookie(string url, string key, bool skipEncoding) { CheckPlatform(); return ""; }
    public static void ClearHttpAuthUsernamePassword(string host, string realm) { CheckPlatform(); }
    public static void SetBackgroundColor(string name, float r, float g, float b, float a) { CheckPlatform(); }
    public static void SetWebViewAlpha(string name, float alpha) { CheckPlatform(); }
    public static float GetWebViewAlpha(string name) { CheckPlatform(); return 0.0f; }
    public static void SetShowSpinnerWhileLoading(string name, bool show) { CheckPlatform(); }
    public static void SetSpinnerText(string name, string text) { CheckPlatform(); }
    public static void SetAllowUserDismissSpinnerByGesture(string name, bool flag) { CheckPlatform(); }
    public static void ShowSpinner(string name) { CheckPlatform(); }
    public static void HideSpinner(string name) { CheckPlatform(); }
    public static bool CanGoBack(string name) { CheckPlatform(); return false; }
    public static bool CanGoForward(string name) { CheckPlatform(); return false; }
    public static void GoBack(string name) { CheckPlatform(); }
    public static void GoForward(string name) { CheckPlatform(); }
    public static void SetOpenLinksInExternalBrowser(string name, bool flag) { CheckPlatform(); }
    public static void SetHorizontalScrollBarEnabled(string name, bool enabled) { CheckPlatform(); }
    public static void SetVerticalScrollBarEnabled(string name, bool enabled) { CheckPlatform(); }
    public static void SetBouncesEnabled(string name, bool enabled) { CheckPlatform(); }
    public static void SetZoomEnabled(string name, bool enabled) { CheckPlatform(); }
    public static void SetShowToolbar(string name, bool show, bool animated, bool onTop, bool adjustInset) { CheckPlatform(); }
    public static void SetToolbarDoneButtonText(string name, string text) { CheckPlatform(); }
    public static void SetToolbarGoBackButtonText(string name, string text) { CheckPlatform(); }
    public static void SetToolbarGoForwardButtonText(string name, string text) { CheckPlatform(); }
    public static void SetToolbarTintColor(string name, float r, float g, float b) { CheckPlatform(); }
    public static void SetToolbarTextColor(string name, float r, float g, float b) { CheckPlatform(); }
    public static void SetUserInteractionEnabled(string name, bool enabled) { CheckPlatform(); }
    public static void SetTransparencyClickingThroughEnabled(string name, bool enabled) { CheckPlatform(); }
    public static void SetWebContentsDebuggingEnabled(bool enabled) { CheckPlatform(); }
    public static void SetAllowHTTPAuthPopUpWindow(string name, bool flag) { CheckPlatform(); }
    public static void Print(string name) { CheckPlatform(); }
    public static void CaptureSnapshot(string name, string filename) { CheckPlatform(); }
    public static void SetCalloutEnabled(string name, bool flag) { CheckPlatform(); }
    public static void SetSupportMultipleWindows(string name, bool enabled, bool allowJavaScriptOpening) { CheckPlatform(); }
    public static void SetDragInteractionEnabled(string name, bool flag) { CheckPlatform(); }
    public static void ScrollTo(string name, int x, int y, bool animated) { CheckPlatform(); }
    public static float NativeScreenWidth() { CheckPlatform(); return 0.0f; }
    public static float NativeScreenHeight() { CheckPlatform(); return 0.0f; }
    public static void SafeBrowsingInit(string name, string url) { CheckPlatform(); }
    public static void SafeBrowsingSetToolbarColor(string name, float r, float g, float b) { CheckPlatform(); }
    public static void SafeBrowsingShow(string name) { CheckPlatform(); }
    public static void AuthenticationSetPrivateMode(string name, bool enabled) { CheckPlatform(); }
    public static void SetShowEmbeddedToolbar(string name, bool show) { CheckPlatform(); }
    public static void SetEmbeddedToolbarOnTop(string name, bool top) { CheckPlatform(); }
    public static void SetEmbeddedToolbarDoneButtonText(string name, string text) { CheckPlatform(); }
    public static void SetEmbeddedToolbarGoBackButtonText(string name, string text) { CheckPlatform(); }
    public static void SetEmbeddedToolbarGoForwardButtonText(string name, string text) { CheckPlatform(); }
    public static void SetEmbeddedToolbarTitleText(string name, string text) { CheckPlatform(); }
    public static void SetEmbeddedToolbarBackgroundColor(string name, Color color) { CheckPlatform(); }
    public static void SetEmbeddedToolbarButtonTextColor(string name, Color color) { CheckPlatform(); }
    public static void SetEmbeddedToolbarTitleTextColor(string name, Color color) { CheckPlatform(); }
    public static void SetEmeddedToolbarNavigationButtonsShow(string name, bool show) { CheckPlatform(); }
    public static void StartSnapshotForRendering(string name, string identifier) { CheckPlatform(); }
    public static void StopSnapshotForRendering(string name) { CheckPlatform(); }
    public static byte[] GetRenderedData(string name, int x, int y, int width, int height) { CheckPlatform(); return null; }
    
    private static bool alreadyLoggedWarning = false;
    public static void CheckPlatform() 
    {
        if (!alreadyLoggedWarning)
            alreadyLoggedWarning = true;
        else
            return;

        Debug.LogWarning("WTP: UniWebView only fully supports iOS/Android/macOS Editor. Your current platform " + Application.platform + " is not fully supported.");
    }

    #endregion
}
#endif