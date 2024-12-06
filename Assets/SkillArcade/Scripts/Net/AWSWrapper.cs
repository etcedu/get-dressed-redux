using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Simcoach.Net
{

    /// <summary>
    /// AWS wrapper v2.0
    /// A wrapper to organize API traffic between Unity and AWS
    /// v2.0 features: added request cache capability, backwards compatible
    /// 
    /// --- Introduction ---
    /// 
    /// This script doesn't need to be attached to any game object in the scene.
    /// Once you invoke its public static function, it will create a
    /// "AWSWrapperGhost" object in the scene to deligate the work.
    /// 
    /// --- Settings ---
    /// 
    /// There are a few items you need to set up before using the wrapper.
    /// a. AWS related settings: API root url, if use API key, and what
    /// is the used API key.
    /// b. Network related settings: connection time out in seconds
    /// c. Local cache settings: name of the cache file, minimum cache retry wait
    /// time, and maximum cache retry wait time
    /// d. Wrapper settings: if use debug log, and the length of the hash indexing
    /// each incoming request
    /// 
    /// --- Usage ---
    /// 
    /// The only function you need to call is ApiCall, which accepts an
    /// AWSRequestModel encapsulating the request, and returns a unique hash code
    /// corresponding to the request. Once the request is done, no matter if it is
    /// successful or failed, the onApiCallFinished handle will fire an event with
    /// an AWSResponse model encapsulating the response. If the request is failed,
    /// it means that either the request is time out, or the network is down. In
    /// both of these cases, the wrapper will automatically cache the request and
    /// try again silently later given the request has isCached tag set to true.
    /// 
    /// --- Helper Classes ---
    /// 
    /// There are two helper classes included in this script: AWSRequestModel and
    /// AWSResponseModel.All the requests and responses can be encapusulated using 
    /// these classes. Please read the class definitions for more information.
    /// 
    /// </summary>
    public class AWSWrapper : MonoBehaviour
    {

        //
        // callback interfaces
        //

        // callback when API call is finished
        public delegate void OnApiCallFinished(AWSResponseModel response);
        public static event OnApiCallFinished onApiCallFinished;

        //
        // static variables
        //

        // application-wise access
        public static AWSWrapper instance;

        //---------- SETTINGS ----------//

        // AWS access related configuration
        public static string rootUrl = "https://api2.simcoachskillarcade.com";
        public static bool isUseApiKey = true;
        public static string apiKey = "LQIehv0yHE5Qc5scxa2h0aUMuD3DKLuF1WMPGT6E";

        // www related configuration
        public static float timeOut = 30f;

        // local cache configuration
        public static bool isCache = false;
        public static string cacheFileName = "request-cache";
        public static float minRetryTime = 1f;
        public static float maxRetryTime = 64f;

        // wrapper related configuration
        public static bool isLog = false;
        public static int apiCallHashLength = 10;

        //---------- SETTINGS ----------//

        // AWS access parameters
        public static string apiKeyAlias = "x-api-key";
        public static string contentTypeAlias = "Content-Type";
        public static string contentType = "application/json";

        //
        // internal variables (need initialization)
        //

        // API request queue
        private Queue<KeyValuePair<string, AWSRequestModel>> apiQueue;
        // current coroutine object
        private Coroutine apiRoutine;
        // cache file path
        private string cacheFilePath;
        // cache retry time
        public float currentRetryTime;
        // cache coroutine
        private Coroutine cacheRoutine;

        //
        // Unity functions
        //

        // Called before start
        void Awake() { }

        // Use this for initialization
        void Start() { }

        // Update is called once per frame
        void Update() { }

        //
        // class interface functions
        //

        /// <summary>
        /// Make API call.
        /// </summary>
        /// <returns>Hash of the API call.</returns>
        /// <param name="request">API request model.</param>
        public static string ApiCall(AWSRequestModel request)
        {
            // initialize
            CreateInstance();
            // get an hash
            string hash = GenerateApiCallHashString();
            // add to the queue
            instance.apiQueue.Enqueue(new KeyValuePair<string, AWSRequestModel>(hash, request));
            // decide whether to excute
            if (instance.apiRoutine == null)
            {
                KeyValuePair<string, AWSRequestModel> task = instance.apiQueue.Dequeue();
                instance.apiRoutine = instance.StartCoroutine(instance.ApiCallRoutine(task.Key, task.Value));
            }
            // return hash for future reference
            return hash;
        }

        //
        // static helper functions (guarantee instance)
        //

        /// <summary>
        /// Create current instance.
        /// Place all initialization code here.
        /// </summary>
        private static void CreateInstance()
        {
            // create instance
            if (instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = "AWSWrapperGhost";
                instance = obj.AddComponent<AWSWrapper>();
                if (instance.apiQueue == null)
                {
                    instance.apiQueue = new Queue<KeyValuePair<string, AWSRequestModel>>();
                }

                // initialize cache file         
                if (isCache)
                {
                    instance.cacheFilePath = Application.persistentDataPath + "/" + cacheFileName + ".json";
                    if (!File.Exists(instance.cacheFilePath))
                    {
                        File.Create(instance.cacheFilePath);
                    }
                }
                // initilaize cache retry time
                instance.currentRetryTime = minRetryTime;
            }
        }

        /// <summary>
        /// Generates unique API call hash string.
        /// </summary>
        /// <returns>API call hash string.</returns>
        private static string GenerateApiCallHashString()
        {
            char[] pool = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
                            'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
                            'u', 'v', 'w', 'x', 'y', 'z',
                            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
                            'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
                            'U', 'V', 'W', 'X', 'Y', 'Z'};
            string hash = "";
            while (true)
            {
                // generate hash
                hash = "";
                for (int i = 0; i < apiCallHashLength; i++)
                {
                    hash += pool[Random.Range(0, pool.Length)];
                }
                // check for duplicate
                foreach (KeyValuePair<string, AWSRequestModel> entry in instance.apiQueue)
                {
                    if (entry.Key == hash)
                    {
                        continue;
                    }
                }
                break;
            }
            return hash;
        }


        /// <summary>
        /// Log message with control.
        /// </summary>
        /// <param name="message">Log message.</param>
        private static void Log(string message)
        {
            if (isLog)
            {
                Debug.Log(message);
            }
        }

        //
        // private methods
        //

        /// <summary>
        /// Write a request to cache file.
        /// </summary>
        /// <param name="request">Unprocessed API request.</param>
        private void CacheRequest(AWSRequestModel request)
        {
            if (isCache)
            {
                string data = JsonUtility.ToJson(request) + "\n";
                StreamWriter sr = new StreamWriter(cacheFilePath, true);
                sr.Write(data);
                sr.Close();
            }
        }

        /// <summary>
        /// Read a request from cache file.
        /// </summary>
        /// <returns>A cached request.</returns>
        private AWSRequestModel DecacheRequest()
        {
            if (!isCache)
            {
                return null;
            }
            StreamReader sr = new StreamReader(cacheFilePath);
            string data = sr.ReadToEnd();
            sr.Close();
            // process data
            string[] calls = data.Split('\n');
            int pos = -1;
            for (int i = 0; i < calls.Length; i++)
            {
                if (calls[i] != "")
                {
                    pos = i;
                    break;
                }
            }
            // process
            if (pos >= 0)
            {
                // get request
                AWSRequestModel request = JsonUtility.FromJson<AWSRequestModel>(calls[pos]);
                // write back
                string file = "";
                for (int i = 0; i < calls.Length; i++)
                {
                    if (i != pos)
                    {
                        file += calls[i] + "\n";
                    }
                }
                StreamWriter sw = new StreamWriter(cacheFilePath);
                sw.Write(file);
                sw.Close();
                // return
                return request;
            }
            else
            {
                // write empty
                StreamWriter sw = new StreamWriter(cacheFilePath);
                sw.Write("");
                sw.Close();
                // calls not found
                return null;
            }
        }

        /// <summary>
        /// Update current retry time.
        /// </summary>
        /// <param name="success">If last call is made successful.</param>
        private void UpdateRetryTime(bool success)
        {
            if (success)
            {
                currentRetryTime = minRetryTime;
            }
            else
            {
                currentRetryTime = Mathf.Min(maxRetryTime, currentRetryTime * 2f);
            }
        }

        /// <summary>
        /// Get status code from response header.
        /// </summary>
        /// <param name="header">Response header.</param>
        /// <returns>Status code</returns>
        private int GetStatusCode(Dictionary<string, string> header)
        {
            if (header != null && header.ContainsKey("STATUS"))
            {
                string status = header["STATUS"];
                status = System.Text.RegularExpressions.Regex.Match(status, "\\d{3}").Value;
                return int.Parse(status);
            }
            else
            {
                return -1;
            }
        }

        //
        // coroutines
        //

        /// <summary>
        /// Synchrinized API call routine.
        /// </summary>
        /// <param name="hash">Hash generated for request.</param>
        /// <param name="request">API request.</param>
        /// <returns>N/A</returns>
        private IEnumerator ApiCallRoutine(string hash, AWSRequestModel request)
        {
            // modify api request
            if (request.apiHeader.dict.ContainsKey(contentTypeAlias))
            {
                request.apiHeader.dict[contentTypeAlias] = contentType;
            }
            else
            {
                request.apiHeader.dict.Add(contentTypeAlias, contentType);
            }
            if (isUseApiKey && !request.apiHeader.dict.ContainsKey(apiKeyAlias))
            {
                request.apiHeader.dict.Add(apiKeyAlias, apiKey);
            }
            if (request.apiPayload == "")
            {
                request.apiPayload = "{}";
            }
            request.shadowHash = hash;
            // make url
            string url = rootUrl + request.apiUrl + request.apiParameters;

            if (request.basicGETOverrideURL != "")
                url = request.basicGETOverrideURL;

            // log this
            Log("AWSWrapper: API call started with hash " + hash + " to " + url);
            // make payload
            byte[] payload = new System.Text.UTF8Encoding().GetBytes(request.apiPayload);
            // start www instance
            float time = 0f;
            WWW www;
            if (request.apiMethod == "GET")
            {
                Debug.Log("Basic Get at: " + url);
                // if request asks for GET
                www = new WWW(url);
            }
            else
            {
                // by default, make post request
                www = new WWW(url, payload, request.apiHeader.dict);
            }
            // check for time out
            bool is_timeout = false;
            while (!www.isDone)
            {
                time += Time.deltaTime;
                if (time > timeOut)
                {
                    is_timeout = true;
                    break;
                }
                else
                {
                    yield return null;
                }
            }
            // log this
            if (is_timeout)
            {
                Log("AWSWrapper: API call with hash " + hash + " timed out");
            }
            // check if request went through
            bool is_through = true;
            if (www.error != null)
            {
                is_through = GetStatusCode(www.responseHeaders) > 0;
            }
            // log this
            if (!is_through)
            {
                Log("AWSWrapper: API call with hash " + hash + " is not through");
            }
            bool is_failed = is_timeout || !is_through;
            // update retry time
            UpdateRetryTime(!is_failed);
            // create response
            AWSResponseModel response;
            if (is_failed)
            {
                // return failed response
                response = new AWSResponseModel(true, hash, null, "", request.isCached);
            }
            else
            {
                // return normal response
                response = new AWSResponseModel(false, hash, www.responseHeaders, www.text, request.isCached);
            }
            // call callback
            if (onApiCallFinished != null)
            {
                onApiCallFinished(response);
            }
            // log this
            if (!is_failed)
            {
                Log("AWSWrapper: API call made with hash " + hash + " successful");
            }
            // decide if to excute next api
            if (instance != null && instance.apiQueue != null)
            {
                if (instance.apiQueue.Count > 0)
                {
                    KeyValuePair<string, AWSRequestModel> task = instance.apiQueue.Dequeue();
                    instance.apiRoutine = instance.StartCoroutine(instance.ApiCallRoutine(task.Key, task.Value));
                }
                else
                {
                    instance.apiRoutine = null;
                }
            }
            // decide if do cache routine
            if (is_failed && request.isCached)
            {
                // cache request
                CacheRequest(request);
            }
            // start cache routine if none exists (start this anyway, no harm)
            if (cacheRoutine == null)
            {
                UpdateRetryTime(true);
                cacheRoutine = StartCoroutine(CacheCallRoutine());
            }
        }

        /// <summary>
        /// Asynchronized cache API call routine.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CacheCallRoutine()
        {
            AWSRequestModel request = DecacheRequest();
            // loop through all the requests
            while (request != null)
            {
                // binary back off first
                yield return new WaitForSeconds(currentRetryTime);
                // modify api request
                if (request.apiHeader.dict.ContainsKey(contentTypeAlias))
                {
                    request.apiHeader.dict[contentTypeAlias] = contentType;
                }
                else
                {
                    request.apiHeader.dict.Add(contentTypeAlias, contentType);
                }
                if (isUseApiKey && !request.apiHeader.dict.ContainsKey(apiKeyAlias))
                {
                    request.apiHeader.dict.Add(apiKeyAlias, apiKey);
                }
                if (request.apiPayload == "")
                {
                    request.apiPayload = "{}";
                }
                // make url
                string url = rootUrl + request.apiUrl + request.apiParameters;
                // make payload
                byte[] payload = new System.Text.UTF8Encoding().GetBytes(request.apiPayload);
                // start www instance
                float time = 0f;
                WWW www;
                if (request.apiMethod == "GET")
                {
                    // if request asks for GET
                    www = new WWW(url);
                }
                else
                {
                    // by default, make post request
                    www = new WWW(url, payload, request.apiHeader.dict);
                }
                // check for time out
                bool is_timeout = false;
                while (!www.isDone)
                {
                    time += Time.deltaTime;
                    if (time > timeOut)
                    {
                        is_timeout = true;
                        break;
                    }
                    else
                    {
                        yield return null;
                    }
                }
                // log this
                if (is_timeout)
                {
                    Log("AWSWrapper: cached API call with hash " + request.shadowHash + " timed out");
                }
                // check if request went through
                bool is_through = true;
                if (www.error != null)
                {
                    is_through = GetStatusCode(www.responseHeaders) > 0;
                }
                // log this
                if (!is_through)
                {
                    Log("AWSWrapper: cached API call with hash " + request.shadowHash + " is not through");
                }
                bool is_failed = is_timeout || !is_through;
                // update retry time
                UpdateRetryTime(!is_failed);
                // log this
                if (!is_failed)
                {
                    Log("AWSWrapper: cached API call with hash " + request.shadowHash + " made successful");
                }
                // cache this request if failed
                if (is_failed)
                {
                    CacheRequest(request);
                }
                // get next request
                request = DecacheRequest();
            }
            cacheRoutine = null;
        }

    }

    /// <summary>
    /// AWS request model (could be extended to a specific API call).
    /// </summary>
    [System.Serializable]
    public class AWSRequestModel : ISerializationCallbackReceiver
    {
        public string apiUrl = "";                                  // url extend root url (starts with /)
        public string apiParameters = "";                           // addtional parameters in url
        public SerializableDictionay<string, string> apiHeader;     // serialized version of header field
        public string apiPayload = "";                              // payload (should be json file)
        public string apiMethod = "";                               // api request method
        public bool isCached = false;                               // if request is cached

        public string apiHeaderShadow = "";                         // shadow for serialization
        public string shadowHash = "";                              // shadow hash assigned to call

        public string basicGETOverrideURL = "";                     //used for GETs to badge assets

        /// <summary>
        /// Initializes a new instance of the <see cref="Simcoach.Net.AWSRequestModel"/> class.
        /// </summary>
        /// <param name="api_url">API url to extend root url (starts with /)</param>
        /// <param name="api_parameters">Additional API parameters after API url (default empty, starts with /)</param>
        /// <param name="api_header">API header (default empty)</param>
        /// <param name="api_payload">API payload in JSON plain text (default empty)</param>
        /// <param name="method">API method, POST or GET</param>
        public AWSRequestModel(string api_url,
                                string api_parameters = "",
                                Dictionary<string, string> api_header = null,
                                string api_payload = "{}",
                                string api_method = "POST",
                                bool is_cached = false,
                                string _basicGETOverrideURL = "")
        {
            apiUrl = api_url;
            apiParameters = api_parameters;
            if (api_header != null)
            {
                apiHeader = new SerializableDictionay<string, string>(api_header);
            }
            else
            {
                apiHeader = new SerializableDictionay<string, string>();
            }
            apiPayload = api_payload;
            apiMethod = api_method;
            isCached = is_cached;
            basicGETOverrideURL = _basicGETOverrideURL;
        }

        public void OnAfterDeserialize()
        {
            apiHeader = JsonUtility.FromJson<SerializableDictionay<string, string>>(apiHeaderShadow);
        }

        public void OnBeforeSerialize()
        {
            apiHeaderShadow = JsonUtility.ToJson(apiHeader);
        }
    }

    /// <summary>
    /// AWS response model (could be extended to a specific API call).
    /// </summary>
    [System.Serializable]
    public class AWSResponseModel
    {

        public bool isTimeout = false;                  // response time out or network error (backward compatible)
        public string apiHash = "";                     // api hash
        public Dictionary<string, string> apiHeader;    // response header
        public string apiResponse = "";                 // response text body
        public bool isCached = false;                   // corresponding request is cached

        /// <summary>
        /// Initializes a new instance of the <see cref="Simcoach.Net.AWSResponseModel"/> class.
        /// </summary>
        /// <param name="is_timeout">Response failed indicator.</param>
        /// <param name="api_hash">Hash assigned to original request.</param>
        /// <param name="api_header">API response header.</param>
        /// <param name="api_response">API response.</param>
        /// <param name="is_cached">If original request is cached.</param>
        public AWSResponseModel(bool is_timeout,
                                 string api_hash,
                                 Dictionary<string, string> api_header,
                                 string api_response,
                                 bool is_cached)
        {
            isTimeout = is_timeout;
            apiHash = api_hash;
            apiHeader = api_header;
            apiResponse = api_response;
        }

        /// <summary>
        /// Get the three-digit status code from response header.
        /// </summary>
        /// <returns>The status code as integer.</returns>
        public int GetStatusCode()
        {
            if (apiHeader != null && apiHeader.ContainsKey("STATUS"))
            {
                string status = apiHeader["STATUS"];
                status = System.Text.RegularExpressions.Regex.Match(status, "\\d{3}").Value;
                return int.Parse(status);
            }
            else
            {
                return -1;
            }
        }

    }

    /// <summary>
    /// Serializable version of Dictionary.
    /// </summary>
    /// <typeparam name="Tkey">Key template.</typeparam>
    /// <typeparam name="Tval">Value template.</typeparam>
    [System.Serializable]
    public class SerializableDictionay<Tkey, Tval> : ISerializationCallbackReceiver
    {
        public Dictionary<Tkey, Tval> dict;     // in house dictionary
        [SerializeField]
        public List<Tkey> keys;                 // list of keys
        [SerializeField]
        public List<Tval> vals;                 // list of values

        /// <summary>
        /// Default constructor
        /// </summary>
        public SerializableDictionay()
        {
            dict = new Dictionary<Tkey, Tval>();
            keys = new List<Tkey>();
            vals = new List<Tval>();
        }

        /// <summary>
        /// Copy constructor from a standard Dictionary.
        /// </summary>
        /// <param name="d">External Dictionary.</param>
        public SerializableDictionay(Dictionary<Tkey, Tval> d)
        {
            dict = new Dictionary<Tkey, Tval>();
            keys = new List<Tkey>();
            vals = new List<Tval>();
            // copy all
            foreach (KeyValuePair<Tkey, Tval> pair in d)
            {
                dict.Add(pair.Key, pair.Value);
                keys.Add(pair.Key);
                vals.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            // build dictionary
            for (int i = 0; i < Mathf.Min(keys.Count, vals.Count); i++)
            {
                dict.Add(keys[i], vals[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            // decompose dictionary
            keys = new List<Tkey>(dict.Keys);
            vals = new List<Tval>(dict.Values);
        }
    }

} // end of namespace Simcoach.Net
