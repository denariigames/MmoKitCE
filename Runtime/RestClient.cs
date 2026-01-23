using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Insthync.UnityRestClient
{
    public class RestClient : MonoBehaviour
    {
        public static bool DoNotCountNextRequest { get; set; }
        public static int PostLoadCount { get; private set; }
        public static int PatchLoadCount { get; private set; }
        public static int PutLoadCount { get; private set; }
        public static int DeleteLoadCount { get; private set; }
        public static int GetLoadCount { get; private set; }

        public static int LoadCount
        {
            get { return PostLoadCount + PatchLoadCount + PutLoadCount + DeleteLoadCount + GetLoadCount; }
        }

        public static readonly string JsonContentType = "application/json";

        public static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new DefaultContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
        };

        public struct AuthHeaderSettings
        {
            public string Header { get; set; }
            public string Prefix { get; set; }
        }

        public static readonly AuthHeaderSettings BearerAuthHeaderSettings = new AuthHeaderSettings()
        {
            Header = "Authorization",
            Prefix = "Bearer ",
        };

        public static readonly AuthHeaderSettings ApiKeyAuthHeaderSettings = new AuthHeaderSettings()
        {
            Header = "x-api-key",
            Prefix = "",
        };

        public struct RequestContent
        {
            public string Type { get; set; }
            public string Data { get; set; }
        }

        public static void SetUserAgent(UnityWebRequest webRequest)
        {
            webRequest.SetRequestHeader("User-Agent",
                $"{Application.identifier}/{Application.version} (Unity {Application.unityVersion}; {Application.platform})");
        }

        public static void SetHeaders(UnityWebRequest webRequest, Dictionary<string, string> headers)
        {
            if (headers == null || headers.Count == 0)
                return;
            foreach (var header in headers)
            {
                webRequest.SetRequestHeader(header.Key, header.Value);
            }
        }

        public static RequestContent GetJsonContent(object data)
        {
            if (data == null)
            {
                return new RequestContent()
                {
                    Type = JsonContentType,
                    Data = "{}",
                };
            }

            return new RequestContent()
            {
                Type = JsonContentType,
                Data = JsonConvert.SerializeObject(data, JsonSerializerSettings),
            };
        }

        private static uint s_debugIdCounter = uint.MinValue;

        private static uint GetNextDebugId()
        {
            if (s_debugIdCounter == uint.MaxValue)
                s_debugIdCounter = uint.MinValue;
            s_debugIdCounter = s_debugIdCounter + 1;
            return s_debugIdCounter;
        }

        public static string GetQueryString(Dictionary<string, object> queries)
        {
            StringBuilder queryStringBuilder = new StringBuilder();
            int i = 0;
            foreach (var query in queries)
            {
                if (string.IsNullOrEmpty(query.Key) || query.Value == null)
                    continue;
                if (i == 0)
                    queryStringBuilder.Append('?');
                else
                    queryStringBuilder.Append('&');
                if (query.Value.GetType().IsArray ||
                    query.Value.GetType().IsAssignableFrom(typeof(IList)))
                {
                    int j = 0;
                    foreach (object value in query.Value as IEnumerable)
                    {
                        if (j > 0)
                            queryStringBuilder.Append('&');
                        queryStringBuilder.Append(query.Key);
                        queryStringBuilder.Append("=");
                        queryStringBuilder.Append(value);
                        ++j;
                    }
                }
                else
                {
                    queryStringBuilder.Append(query.Key);
                    queryStringBuilder.Append('=');
                    queryStringBuilder.Append(query.Value);
                }

                ++i;
            }

            return queryStringBuilder.ToString();
        }

        public static async Task<Result<TResponse>> Get<TResponse>(string url)
        {
            return await Get<TResponse>(url, new Dictionary<string, object>(), string.Empty);
        }

        public static async Task<Result<TResponse>> Get<TResponse>(string url, Dictionary<string, object> queries)
        {
            return await Get<TResponse>(url, queries, string.Empty);
        }

        public static async Task<Result<TResponse>> Get<TResponse>(string url, string authorizationToken)
        {
            return await Get<TResponse>(url, new Dictionary<string, object>(), authorizationToken);
        }

        public static async Task<Result<TResponse>> Get<TResponse>(string url, string authorizationToken,
            AuthHeaderSettings authHeaderSettings)
        {
            return await Get<TResponse>(url, new Dictionary<string, object>(), authorizationToken, authHeaderSettings);
        }

        public static async Task<Result<TResponse>> Get<TResponse>(string url, Dictionary<string, object> queries,
            string authorizationToken)
        {
            Result result = await Get(url, queries, authorizationToken);
            return new Result<TResponse>(result);
        }

        public static async Task<Result<TResponse>> Get<TResponse>(string url, Dictionary<string, object> queries,
            string authorizationToken, AuthHeaderSettings authHeaderSettings)
        {
            Result result = await Get(url, queries, authorizationToken, authHeaderSettings);
            return new Result<TResponse>(result);
        }

        public static async Task<Result<TResponse>> Delete<TResponse>(string url)
        {
            return await Delete<TResponse>(url, new Dictionary<string, object>(), string.Empty);
        }

        public static async Task<Result<TResponse>> Delete<TResponse>(string url, Dictionary<string, object> queries)
        {
            return await Delete<TResponse>(url, queries, string.Empty);
        }

        public static async Task<Result<TResponse>> Delete<TResponse>(string url, string authorizationToken)
        {
            return await Delete<TResponse>(url, new Dictionary<string, object>(), authorizationToken);
        }

        public static async Task<Result<TResponse>> Delete<TResponse>(string url, string authorizationToken,
            AuthHeaderSettings authHeaderSettings)
        {
            return await Delete<TResponse>(url, new Dictionary<string, object>(), authorizationToken,
                authHeaderSettings);
        }

        public static async Task<Result<TResponse>> Delete<TResponse>(string url, Dictionary<string, object> queries,
            string authorizationToken)
        {
            Result result = await Delete(url, queries, authorizationToken);
            return new Result<TResponse>(result);
        }

        public static async Task<Result<TResponse>> Delete<TResponse>(string url, Dictionary<string, object> queries,
            string authorizationToken, AuthHeaderSettings authHeaderSettings)
        {
            Result result = await Delete(url, queries, authorizationToken, authHeaderSettings);
            return new Result<TResponse>(result);
        }

        public static async Task<Result<TResponse>> Post<TForm, TResponse>(string url, TForm data)
        {
            return await Post<TForm, TResponse>(url, data, string.Empty);
        }

        public static async Task<Result<TResponse>> Post<TForm, TResponse>(string url, TForm data,
            string authorizationToken)
        {
            Result result = await Post(url, data, authorizationToken);
            return new Result<TResponse>(result);
        }

        public static async Task<Result<TResponse>> Post<TForm, TResponse>(string url, TForm data,
            string authorizationToken, AuthHeaderSettings authHeaderSettings)
        {
            Result result = await Post(url, data, authorizationToken, authHeaderSettings);
            return new Result<TResponse>(result);
        }

        public static async Task<Result<TResponse>> Post<TResponse>(string url, string authorizationToken)
        {
            Result result = await Post(url, GetJsonContent(null), authorizationToken, BearerAuthHeaderSettings);
            return new Result<TResponse>(result);
        }

        public static async Task<Result<TResponse>> Post<TResponse>(string url, string authorizationToken,
            AuthHeaderSettings authHeaderSettings)
        {
            Result result = await Post(url, GetJsonContent(null), authorizationToken, authHeaderSettings);
            return new Result<TResponse>(result);
        }

        public static async Task<Result> Post(string url, string authorizationToken)
        {
            return await Post(url, GetJsonContent(null), authorizationToken, BearerAuthHeaderSettings);
        }

        public static async Task<Result> Post(string url, string authorizationToken,
            AuthHeaderSettings authHeaderSettings)
        {
            return await Post(url, GetJsonContent(null), authorizationToken, authHeaderSettings);
        }

        public static async Task<Result<TResponse>> Patch<TForm, TResponse>(string url, TForm data)
        {
            return await Patch<TForm, TResponse>(url, data, string.Empty);
        }

        public static async Task<Result<TResponse>> Patch<TForm, TResponse>(string url, TForm data,
            string authorizationToken)
        {
            Result result = await Patch(url, data, authorizationToken);
            return new Result<TResponse>(result);
        }

        public static async Task<Result<TResponse>> Patch<TForm, TResponse>(string url, TForm data,
            string authorizationToken, AuthHeaderSettings authHeaderSettings)
        {
            Result result = await Patch(url, data, authorizationToken, authHeaderSettings);
            return new Result<TResponse>(result);
        }

        public static async Task<Result<TResponse>> Patch<TResponse>(string url, string authorizationToken)
        {
            Result result = await Patch(url, GetJsonContent(null), authorizationToken, BearerAuthHeaderSettings);
            return new Result<TResponse>(result);
        }

        public static async Task<Result<TResponse>> Patch<TResponse>(string url, string authorizationToken,
            AuthHeaderSettings authHeaderSettings)
        {
            Result result = await Patch(url, GetJsonContent(null), authorizationToken, authHeaderSettings);
            return new Result<TResponse>(result);
        }

        public static async Task<Result> Patch(string url, string authorizationToken)
        {
            return await Patch(url, GetJsonContent(null), authorizationToken, BearerAuthHeaderSettings);
        }

        public static async Task<Result> Patch(string url, string authorizationToken,
            AuthHeaderSettings authHeaderSettings)
        {
            return await Patch(url, GetJsonContent(null), authorizationToken, authHeaderSettings);
        }

        public static async Task<Result<TResponse>> Put<TForm, TResponse>(string url, TForm data)
        {
            return await Put<TForm, TResponse>(url, data, string.Empty);
        }

        public static async Task<Result<TResponse>> Put<TForm, TResponse>(string url, TForm data,
            string authorizationToken)
        {
            Result result = await Put(url, data, authorizationToken);
            return new Result<TResponse>(result);
        }

        public static async Task<Result<TResponse>> Put<TForm, TResponse>(string url, TForm data,
            string authorizationToken, AuthHeaderSettings authHeaderSettings)
        {
            Result result = await Put(url, data, authorizationToken, authHeaderSettings);
            return new Result<TResponse>(result);
        }

        public static async Task<Result<TResponse>> Put<TResponse>(string url, string authorizationToken)
        {
            Result result = await Put(url, GetJsonContent(null), authorizationToken, BearerAuthHeaderSettings);
            return new Result<TResponse>(result);
        }

        public static async Task<Result<TResponse>> Put<TResponse>(string url, string authorizationToken,
            AuthHeaderSettings authHeaderSettings)
        {
            Result result = await Put(url, GetJsonContent(null), authorizationToken, authHeaderSettings);
            return new Result<TResponse>(result);
        }

        public static async Task<Result> Put(string url, string authorizationToken)
        {
            return await Put(url, GetJsonContent(null), authorizationToken, BearerAuthHeaderSettings);
        }

        public static async Task<Result> Put(string url, string authorizationToken,
            AuthHeaderSettings authHeaderSettings)
        {
            return await Put(url, GetJsonContent(null), authorizationToken, authHeaderSettings);
        }

        public static async Task<Result> Get(string url)
        {
            return await Get(url, string.Empty, BearerAuthHeaderSettings);
        }

        public static async Task<Result> Get(string url, Dictionary<string, object> queries)
        {
            return await Get(url, queries, string.Empty);
        }

        public static async Task<Result> Get(string url, Dictionary<string, object> queries, string authorizationToken)
        {
            return await Get(url + GetQueryString(queries), authorizationToken, BearerAuthHeaderSettings);
        }

        public static async Task<Result> Get(string url, Dictionary<string, object> queries, string authorizationToken,
            AuthHeaderSettings authHeaderSettings)
        {
            return await Get(url + GetQueryString(queries), authorizationToken, authHeaderSettings);
        }

        public static async Task<Result> Get(string url, string authorizationToken,
            AuthHeaderSettings authHeaderSettings)
        {
            return await Get(url, authorizationToken, authHeaderSettings, null);
        }

        public static async Task<Result> Get(string url, string authorizationToken,
            AuthHeaderSettings authHeaderSettings, Dictionary<string, string> headers)
        {
#if UNITY_EDITOR || DEBUG_REST_CLIENT
            uint id = GetNextDebugId();
            bool errorLogged = false;
            Debug.Log($"Get request {id} {url} {authHeaderSettings.Header} {authHeaderSettings.Prefix} {authorizationToken}");
#endif
            bool doNotCountNextRequest = DoNotCountNextRequest;
            long responseCode = -1;
            bool isHttpError = true;
            bool isNetworkError = true;
            string stringContent = string.Empty;
            string error = string.Empty;
            if (!doNotCountNextRequest)
                GetLoadCount++;
            string method = UnityWebRequest.kHttpVerbGET;
            using (UnityWebRequest webRequest = new UnityWebRequest(url, method))
            {
                webRequest.certificateHandler = new SimpleWebRequestCert();
                SetUserAgent(webRequest);
                SetHeaders(webRequest, headers);
                if (!string.IsNullOrEmpty(authorizationToken))
                {
                    webRequest.SetRequestHeader(authHeaderSettings.Header,
                        authHeaderSettings.Prefix + authorizationToken);
                }

                webRequest.downloadHandler = new DownloadHandlerBuffer();
                try
                {
                    UnityWebRequestAsyncOperation ayncOp = webRequest.SendWebRequest();
                    while (!ayncOp.isDone)
                    {
                        await Task.Yield();
                    }
                }
                catch (Exception ex)
                {
#if UNITY_EDITOR || DEBUG_REST_CLIENT
                    Debug.LogError($"Get error {id} catched {ex}");
                    errorLogged = true;
#else
                    Debug.LogException(ex);
#endif
                }

                responseCode = webRequest.responseCode;
#if UNITY_2020_2_OR_NEWER
                isHttpError = webRequest.result == UnityWebRequest.Result.ProtocolError;
                isNetworkError = webRequest.result == UnityWebRequest.Result.ConnectionError;
#else
                isHttpError = webRequest.isHttpError;
                isNetworkError = webRequest.isNetworkError;
#endif
                if (!isNetworkError)
                    stringContent = webRequest.downloadHandler.text;
                else
                    error = webRequest.error;
#if UNITY_EDITOR || DEBUG_REST_CLIENT
                if ((isHttpError || isNetworkError) && !errorLogged)
                    Debug.LogError($"Get error {id} {stringContent}");
                else
                    Debug.Log($"Get success {id} {responseCode} {stringContent}");
#endif
            }

            if (!doNotCountNextRequest)
                GetLoadCount--;
            if (doNotCountNextRequest)
                DoNotCountNextRequest = false;
            return new Result(url, method, default, authorizationToken, authHeaderSettings, responseCode, isHttpError,
                isNetworkError, stringContent, error);
        }

        public static async Task<Result> Delete(string url)
        {
            return await Delete(url, string.Empty, BearerAuthHeaderSettings);
        }

        public static async Task<Result> Delete(string url, Dictionary<string, object> queries)
        {
            return await Delete(url, queries, string.Empty);
        }

        public static async Task<Result> Delete(string url, Dictionary<string, object> queries,
            string authorizationToken)
        {
            return await Delete(url + GetQueryString(queries), authorizationToken, BearerAuthHeaderSettings);
        }

        public static async Task<Result> Delete(string url, Dictionary<string, object> queries,
            string authorizationToken, AuthHeaderSettings authHeaderSettings)
        {
            return await Delete(url + GetQueryString(queries), authorizationToken, authHeaderSettings);
        }

        public static async Task<Result> Delete(string url, string authorizationToken,
            AuthHeaderSettings authHeaderSettings)
        {
            return await Delete(url, authorizationToken, authHeaderSettings, null);
        }

        public static async Task<Result> Delete(string url, string authorizationToken,
            AuthHeaderSettings authHeaderSettings, Dictionary<string, string> headers)
        {
#if UNITY_EDITOR || DEBUG_REST_CLIENT
            uint id = GetNextDebugId();
            bool errorLogged = false;
            Debug.Log($"Delete request {id} {url} {authHeaderSettings.Header} {authHeaderSettings.Prefix} {authorizationToken}");
#endif
            bool doNotCountNextRequest = DoNotCountNextRequest;
            long responseCode = -1;
            bool isHttpError = true;
            bool isNetworkError = true;
            string stringContent = string.Empty;
            string error = string.Empty;
            if (!doNotCountNextRequest)
                DeleteLoadCount++;
            string method = UnityWebRequest.kHttpVerbDELETE;
            using (UnityWebRequest webRequest = new UnityWebRequest(url, method))
            {
                webRequest.certificateHandler = new SimpleWebRequestCert();
                SetUserAgent(webRequest);
                SetHeaders(webRequest, headers);
                if (!string.IsNullOrEmpty(authorizationToken))
                {
                    webRequest.SetRequestHeader(authHeaderSettings.Header,
                        authHeaderSettings.Prefix + authorizationToken);
                }

                webRequest.downloadHandler = new DownloadHandlerBuffer();
                try
                {
                    UnityWebRequestAsyncOperation ayncOp = webRequest.SendWebRequest();
                    while (!ayncOp.isDone)
                    {
                        await Task.Yield();
                    }
                }
                catch (Exception ex)
                {
#if UNITY_EDITOR || DEBUG_REST_CLIENT
                    Debug.LogError($"Delete error {id} catched {ex}");
                    errorLogged = true;
#else
                    Debug.LogException(ex);
#endif
                }

                responseCode = webRequest.responseCode;
#if UNITY_2020_2_OR_NEWER
                isHttpError = webRequest.result == UnityWebRequest.Result.ProtocolError;
                isNetworkError = webRequest.result == UnityWebRequest.Result.ConnectionError;
#else
                isHttpError = webRequest.isHttpError;
                isNetworkError = webRequest.isNetworkError;
#endif
                if (!isNetworkError)
                    stringContent = webRequest.downloadHandler.text;
                else
                    error = webRequest.error;
#if UNITY_EDITOR || DEBUG_REST_CLIENT
                if ((isHttpError || isNetworkError) && !errorLogged)
                    Debug.LogError($"Delete error {id} {stringContent}");
                else
                    Debug.Log($"Delete success {id} {responseCode} {stringContent}");
#endif
            }

            if (!doNotCountNextRequest)
                DeleteLoadCount--;
            if (doNotCountNextRequest)
                DoNotCountNextRequest = false;
            return new Result(url, method, default, authorizationToken, authHeaderSettings, responseCode, isHttpError,
                isNetworkError, stringContent, error);
        }

        public static async Task<Result> Post<TForm>(string url, TForm data)
        {
            return await Post(url, GetJsonContent(data), string.Empty, BearerAuthHeaderSettings);
        }

        public static async Task<Result> Post<TForm>(string url, TForm data, string authorizationToken)
        {
            return await Post(url, GetJsonContent(data), authorizationToken, BearerAuthHeaderSettings);
        }

        public static async Task<Result> Post<TForm>(string url, TForm data, string authorizationToken,
            AuthHeaderSettings authHeaderSettings)
        {
            return await Post(url, GetJsonContent(data), authorizationToken, authHeaderSettings);
        }

        public static async Task<Result> Post(string url, RequestContent content, string authorizationToken,
            AuthHeaderSettings authHeaderSettings)
        {
            return await Post(url, content, authorizationToken, authHeaderSettings, null);
        }

        public static async Task<Result> Post(string url, RequestContent content, string authorizationToken,
            AuthHeaderSettings authHeaderSettings, Dictionary<string, string> headers)
        {
#if UNITY_EDITOR || DEBUG_REST_CLIENT
            uint id = GetNextDebugId();
            bool errorLogged = false;
            Debug.Log($"Post request {id} {url} {content.Data} {authHeaderSettings.Header} {authHeaderSettings.Prefix} {authorizationToken}");
#endif
            bool doNotCountNextRequest = DoNotCountNextRequest;
            long responseCode = -1;
            bool isHttpError = true;
            bool isNetworkError = true;
            string stringContent = string.Empty;
            string error = string.Empty;
            if (!doNotCountNextRequest)
                PostLoadCount++;
            string method = UnityWebRequest.kHttpVerbPOST;
            using (UnityWebRequest webRequest = new UnityWebRequest(url, method))
            {
                webRequest.certificateHandler = new SimpleWebRequestCert();
                SetUserAgent(webRequest);
                SetHeaders(webRequest, headers);
                if (!string.IsNullOrEmpty(authorizationToken))
                {
                    webRequest.SetRequestHeader(authHeaderSettings.Header,
                        authHeaderSettings.Prefix + authorizationToken);
                }

                webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(content.Data.ToCharArray()));
                webRequest.uploadHandler.contentType = content.Type;
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                try
                {
                    UnityWebRequestAsyncOperation ayncOp = webRequest.SendWebRequest();
                    while (!ayncOp.isDone)
                    {
                        await Task.Yield();
                    }
                }
                catch (Exception ex)
                {
#if UNITY_EDITOR || DEBUG_REST_CLIENT
                    Debug.LogError($"Post error {id} catched {ex}");
                    errorLogged = true;
#else
                    Debug.LogException(ex);
#endif
                }

                responseCode = webRequest.responseCode;
#if UNITY_2020_2_OR_NEWER
                isHttpError = webRequest.result == UnityWebRequest.Result.ProtocolError;
                isNetworkError = webRequest.result == UnityWebRequest.Result.ConnectionError;
#else
                isHttpError = webRequest.isHttpError;
                isNetworkError = webRequest.isNetworkError;
#endif
                if (!isNetworkError)
                    stringContent = webRequest.downloadHandler.text;
                else
                    error = webRequest.error;
#if UNITY_EDITOR || DEBUG_REST_CLIENT
                if ((isHttpError || isNetworkError) && !errorLogged)
                    Debug.LogError($"Post error {id} {stringContent}");
                else
                    Debug.Log($"Post success {id} {responseCode} {stringContent}");
#endif
            }

            if (!doNotCountNextRequest)
                PostLoadCount--;
            if (doNotCountNextRequest)
                DoNotCountNextRequest = false;
            return new Result(url, method, content, authorizationToken, authHeaderSettings, responseCode, isHttpError,
                isNetworkError, stringContent, error);
        }

        public static async Task<Result> Patch<TForm>(string url, TForm data)
        {
            return await Patch(url, GetJsonContent(data), string.Empty, BearerAuthHeaderSettings);
        }

        public static async Task<Result> Patch<TForm>(string url, TForm data, string authorizationToken)
        {
            return await Patch(url, GetJsonContent(data), authorizationToken, BearerAuthHeaderSettings);
        }

        public static async Task<Result> Patch<TForm>(string url, TForm data, string authorizationToken,
            AuthHeaderSettings authHeaderSettings)
        {
            return await Patch(url, GetJsonContent(data), authorizationToken, authHeaderSettings);
        }

        public static async Task<Result> Patch(string url, RequestContent content, string authorizationToken,
            AuthHeaderSettings authHeaderSettings)
        {
            return await Patch(url, content, authorizationToken, authHeaderSettings, null);
        }

        public static async Task<Result> Patch(string url, RequestContent content, string authorizationToken,
            AuthHeaderSettings authHeaderSettings, Dictionary<string, string> headers)
        {
#if UNITY_EDITOR || DEBUG_REST_CLIENT
            uint id = GetNextDebugId();
            bool errorLogged = false;
            Debug.Log($"Patch request {id} {url} {content.Data} {authHeaderSettings.Header} {authHeaderSettings.Prefix} {authorizationToken}");
#endif
            bool doNotCountNextRequest = DoNotCountNextRequest;
            long responseCode = -1;
            bool isHttpError = true;
            bool isNetworkError = true;
            string stringContent = string.Empty;
            string error = string.Empty;
            if (!doNotCountNextRequest)
                PatchLoadCount++;
            string method = "PATCH";
            using (UnityWebRequest webRequest = new UnityWebRequest(url, method))
            {
                webRequest.certificateHandler = new SimpleWebRequestCert();
                SetUserAgent(webRequest);
                SetHeaders(webRequest, headers);
                if (!string.IsNullOrEmpty(authorizationToken))
                {
                    webRequest.SetRequestHeader(authHeaderSettings.Header,
                        authHeaderSettings.Prefix + authorizationToken);
                }

                webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(content.Data.ToCharArray()));
                webRequest.uploadHandler.contentType = content.Type;
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                try
                {
                    UnityWebRequestAsyncOperation ayncOp = webRequest.SendWebRequest();
                    while (!ayncOp.isDone)
                    {
                        await Task.Yield();
                    }
                }
                catch (Exception ex)
                {
#if UNITY_EDITOR || DEBUG_REST_CLIENT
                    Debug.LogError($"Patch error {id} catched {ex}");
                    errorLogged = true;
#else
                    Debug.LogException(ex);
#endif
                }

                responseCode = webRequest.responseCode;
#if UNITY_2020_2_OR_NEWER
                isHttpError = webRequest.result == UnityWebRequest.Result.ProtocolError;
                isNetworkError = webRequest.result == UnityWebRequest.Result.ConnectionError;
#else
                isHttpError = webRequest.isHttpError;
                isNetworkError = webRequest.isNetworkError;
#endif
                if (!isNetworkError)
                    stringContent = webRequest.downloadHandler.text;
                else
                    error = webRequest.error;
#if UNITY_EDITOR || DEBUG_REST_CLIENT
                if ((isHttpError || isNetworkError) && !errorLogged)
                    Debug.LogError($"Patch error {id} {stringContent}");
                else
                    Debug.Log($"Patch success {id} {responseCode} {stringContent}");
#endif
            }

            if (!doNotCountNextRequest)
                PatchLoadCount--;
            if (doNotCountNextRequest)
                DoNotCountNextRequest = false;
            return new Result(url, method, content, authorizationToken, authHeaderSettings, responseCode, isHttpError,
                isNetworkError, stringContent, error);
        }

        public static async Task<Result> Put<TForm>(string url, TForm data)
        {
            return await Put(url, GetJsonContent(data), string.Empty, BearerAuthHeaderSettings);
        }

        public static async Task<Result> Put<TForm>(string url, TForm data, string authorizationToken)
        {
            return await Put(url, GetJsonContent(data), authorizationToken, BearerAuthHeaderSettings);
        }

        public static async Task<Result> Put<TForm>(string url, TForm data, string authorizationToken,
            AuthHeaderSettings authHeaderSettings)
        {
            return await Put(url, GetJsonContent(data), authorizationToken, authHeaderSettings);
        }

        public static async Task<Result> Put(string url, RequestContent content, string authorizationToken,
            AuthHeaderSettings authHeaderSettings)
        {
            return await Put(url, content, authorizationToken, authHeaderSettings, null);
        }

        public static async Task<Result> Put(string url, RequestContent content, string authorizationToken,
            AuthHeaderSettings authHeaderSettings, Dictionary<string, string> headers)
        {
#if UNITY_EDITOR || DEBUG_REST_CLIENT
            uint id = GetNextDebugId();
            bool errorLogged = false;
            Debug.Log($"Put request {id} {url} {content.Data} {authHeaderSettings.Header} {authHeaderSettings.Prefix} {authorizationToken}");
#endif
            bool doNotCountNextRequest = DoNotCountNextRequest;
            long responseCode = -1;
            bool isHttpError = true;
            bool isNetworkError = true;
            string stringContent = string.Empty;
            string error = string.Empty;
            if (!doNotCountNextRequest)
                PutLoadCount++;
            string method = UnityWebRequest.kHttpVerbPUT;
            using (UnityWebRequest webRequest = new UnityWebRequest(url, method))
            {
                webRequest.certificateHandler = new SimpleWebRequestCert();
                SetUserAgent(webRequest);
                SetHeaders(webRequest, headers);
                if (!string.IsNullOrEmpty(authorizationToken))
                {
                    webRequest.SetRequestHeader(authHeaderSettings.Header,
                        authHeaderSettings.Prefix + authorizationToken);
                }

                webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(content.Data.ToCharArray()));
                webRequest.uploadHandler.contentType = content.Type;
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                try
                {
                    UnityWebRequestAsyncOperation ayncOp = webRequest.SendWebRequest();
                    while (!ayncOp.isDone)
                    {
                        await Task.Yield();
                    }
                }
                catch (Exception ex)
                {
#if UNITY_EDITOR || DEBUG_REST_CLIENT
                    Debug.LogError($"Put error {id} catched {ex}");
                    errorLogged = true;
#else
                    Debug.LogException(ex);
#endif
                }

                responseCode = webRequest.responseCode;
#if UNITY_2020_2_OR_NEWER
                isHttpError = webRequest.result == UnityWebRequest.Result.ProtocolError;
                isNetworkError = webRequest.result == UnityWebRequest.Result.ConnectionError;
#else
                isHttpError = webRequest.isHttpError;
                isNetworkError = webRequest.isNetworkError;
#endif
                if (!isNetworkError)
                    stringContent = webRequest.downloadHandler.text;
                else
                    error = webRequest.error;
#if UNITY_EDITOR || DEBUG_REST_CLIENT
                if ((isHttpError || isNetworkError) && !errorLogged)
                    Debug.LogError($"Put error {id} {stringContent}");
                else
                    Debug.Log($"Put success {id} {responseCode} {stringContent}");
#endif
            }

            if (!doNotCountNextRequest)
                PutLoadCount--;
            if (doNotCountNextRequest)
                DoNotCountNextRequest = false;
            return new Result(url, method, content, authorizationToken, authHeaderSettings, responseCode, isHttpError,
                isNetworkError, stringContent, error);
        }

        public static string GetQueryString(params KeyValuePair<string, string>[] parameters)
        {
            string queryString = string.Empty;
            for (int i = 0; i < parameters.Length; ++i)
            {
                if (string.IsNullOrEmpty(parameters[i].Key) ||
                    string.IsNullOrEmpty(parameters[i].Value))
                    continue;
                if (!string.IsNullOrEmpty(queryString))
                    queryString += "&";
                else
                    queryString += "?";
                queryString += $"{parameters[i].Key}={parameters[i].Value}";
            }

            return queryString;
        }

        public static string GetUrl(string apiUrl, string action)
        {
            if (apiUrl.EndsWith("/"))
                apiUrl = apiUrl.Substring(0, apiUrl.Length - 1);
            if (action.StartsWith("/"))
                action = action.Substring(1);
            return $"{apiUrl}/{action}";
        }

        public static string GetNetworkErrorMessage(long responseCode)
        {
            switch (responseCode)
            {
                case 400:
                    return "Bad Request";
                case 401:
                    return "Unauthorized";
                case 402:
                    return "Payment Required";
                case 403:
                    return "Forbidden";
                case 404:
                    return "Not Found";
                case 405:
                    return "Method Not Allowed";
                case 406:
                    return "Not Acceptable";
                case 407:
                    return "Proxy Authentication Required";
                case 408:
                    return "Request Timeout";
                case 409:
                    return "Conflict";
                case 410:
                    return "Gone";
                case 411:
                    return "Length Required";
                case 412:
                    return "Precondition Failed";
                case 413:
                    return "Request Entity Too Large";
                case 414:
                    return "Request-url Too Long";
                case 415:
                    return "Unsupported Media Type";
                case 416:
                    return "Requested Range Not Satisfiable";
                case 417:
                    return "Expectation Failed";
                case 500:
                    return "Internal Server Error";
                case 501:
                    return "Not Implemented";
                case 502:
                    return "Bad Gateway";
                case 503:
                    return "Service Unavailable";
                case 504:
                    return "Gateway Timeout";
                case 505:
                    return "HTTP Version Not Supported";
                default:
                    if (responseCode >= 400 && responseCode < 500)
                        return "Client Error";
                    if (responseCode >= 500 && responseCode < 600)
                        return "Server Error";
                    return "Unknow Error";
            }
        }

        public static string GetNetworkErrorMessage(IResult result)
        {
            if (result.IsNetworkError)
                return "Network Error";
            try
            {
                Dictionary<string, object> objs =
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(result.StringContent,
                        JsonSerializerSettings);
                object tempObject;
                if (objs.TryGetValue("error", out tempObject))
                    return tempObject.ToString();
                if (objs.TryGetValue("Error", out tempObject))
                    return tempObject.ToString();
                if (objs.TryGetValue("message", out tempObject))
                    return tempObject.ToString();
                if (objs.TryGetValue("Message", out tempObject))
                    return tempObject.ToString();
                if (objs.TryGetValue("response", out tempObject))
                    return tempObject.ToString();
                if (objs.TryGetValue("Response", out tempObject))
                    return tempObject.ToString();
            }
            catch
            {
                return GetNetworkErrorMessage(result.ResponseCode);
            }

            return "Unknow Error";
        }

        public interface IResult
        {
            long ResponseCode { get; }
            bool IsHttpError { get; }
            bool IsNetworkError { get; }
            string StringContent { get; }
            string Error { get; }
        }

        public class Result : IResult
        {
            public string Url { get; protected set; }
            public string Method { get; protected set; }
            public RequestContent RequestContent { get; protected set; }
            public string AuthorizationToken { get; protected set; }
            public AuthHeaderSettings AuthHeaderSettings { get; protected set; }
            public long ResponseCode { get; protected set; }
            public bool IsHttpError { get; protected set; }
            public bool IsNetworkError { get; protected set; }
            public string StringContent { get; protected set; }
            public string Error { get; protected set; }

            public Result(string url, string method, RequestContent requestContent, string authorizationToken,
                AuthHeaderSettings authHeaderSettings, long responseCode, bool isHttpError, bool isNetworkError,
                string stringContent, string error)
            {
                Url = url;
                Method = method;
                RequestContent = requestContent;
                AuthorizationToken = authorizationToken;
                AuthHeaderSettings = authHeaderSettings;
                ResponseCode = responseCode;
                IsHttpError = isHttpError;
                IsNetworkError = isNetworkError;
                StringContent = stringContent;
                Error = error;
                UpdateContent(stringContent);
                if (IsHttpError && string.IsNullOrEmpty(Error))
                    Error = GetNetworkErrorMessage(this);
            }

            public Result(Result src) :
                this(src.Url, src.Method, src.RequestContent, src.AuthorizationToken, src.AuthHeaderSettings,
                    src.ResponseCode, src.IsHttpError, src.IsNetworkError, src.StringContent, src.Error)
            {
            }

            protected virtual void UpdateContent(string stringContent)
            {
            }
        }

        public class Result<T> : Result
        {
            public T Content { get; private set; }

            public Result(string url, string method, RequestContent requestContent, string authorizationToken,
                AuthHeaderSettings authHeaderSettings, long responseCode, bool isHttpError, bool isNetworkError,
                string stringContent, string error) :
                base(url, method, requestContent, authorizationToken, authHeaderSettings, responseCode, isHttpError,
                    isNetworkError, stringContent, error)
            {
            }

            public Result(Result src) :
                this(src.Url, src.Method, src.RequestContent, src.AuthorizationToken, src.AuthHeaderSettings,
                    src.ResponseCode, src.IsHttpError, src.IsNetworkError, src.StringContent, src.Error)
            {
            }

            protected override void UpdateContent(string stringContent)
            {
                Content = default;
                if (!IsNetworkError)
                {
                    try
                    {
                        Content = JsonConvert.DeserializeObject<T>(stringContent, JsonSerializerSettings);
                    }
                    catch (Exception ex)
                    {
                        // It may not able to deserialize
                        Debug.LogError(
                            $"Can't deserialize content: {stringContent}, from {Url} ({Method}), content: {RequestContent.Data} ({RequestContent.Type}), {ex}");
                        IsHttpError = true;
                        Error = "Wrong content format, unable to deserialize";
                    }
                }
            }
        }
    }

    public static class RestClientResultExtensions
    {
        public static bool IsError(this RestClient.IResult result)
        {
            return result.IsHttpError || result.IsNetworkError;
        }
    }
}