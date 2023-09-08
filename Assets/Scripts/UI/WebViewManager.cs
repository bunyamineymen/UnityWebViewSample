using Gpm.WebView;

using Ratic_Kit.Scripts;

using System.Collections.Generic;

using UnityEngine;


public class WebViewManager : MonoBehaviour
{

    [SerializeField] private GameObject _closeGO;
    [SerializeField] private WebViewHandler _webViewHandler;

    #region Monobehaviours

    private void Awake()
    {
        Singleton();
    }

    #endregion

    #region Singleton

    public static WebViewManager instance;

    private void Singleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion




    public void OpenWebViewYemeksepeti()
    {
        GpmWebView.ShowUrl("https://ratic.co/yemeksepeti", GetConfiguration(), OnWebViewCallback, new List<string> { "test-scheme" });
    }

    public void OpenWebView(string url)
    {
        if (string.IsNullOrEmpty(url) == false)
        {
            _closeGO.SetActive(true);
            // GpmWebView.ShowUrl(url, GetConfiguration(), OnWebViewCallback, new List<string> { "test-scheme" });
            _webViewHandler.OpenWebViewViaUrl(url);
        }
        else
        {
            Debug.LogError("[SampleWebView] Input url is empty.");
        }
    }


    private void OnWebViewCallback(GpmWebViewCallback.CallbackType callbackType, string data, GpmWebViewError error)
    {
        Debug.Log("OnWebViewCallback: " + callbackType);
        switch (callbackType)
        {
            case GpmWebViewCallback.CallbackType.Open:
                if (error != null)
                {
                    Debug.LogFormat("Fail to open WebView. Error:{0}", error);
                }
                break;
            case GpmWebViewCallback.CallbackType.Close:
                if (error != null)
                {
                    Debug.LogFormat("Fail to close WebView. Error:{0}", error);
                }
                break;
            case GpmWebViewCallback.CallbackType.PageStarted:
                if (string.IsNullOrEmpty(data) == false)
                {
                    Debug.LogFormat("PageStarted Url : {0}", data);
                }
                break;
            case GpmWebViewCallback.CallbackType.PageLoad:
                if (string.IsNullOrEmpty(data) == false)
                {
                    Debug.LogFormat("Loaded Page:{0}", data);
                }
                break;
            case GpmWebViewCallback.CallbackType.MultiWindowOpen:
                Debug.Log("MultiWindowOpen");
                break;
            case GpmWebViewCallback.CallbackType.MultiWindowClose:
                Debug.Log("MultiWindowClose");
                break;
            case GpmWebViewCallback.CallbackType.Scheme:
                Debug.LogFormat("Scheme:{0}", data);
                break;
            case GpmWebViewCallback.CallbackType.GoBack:
                Debug.Log("GoBack");
                break;
            case GpmWebViewCallback.CallbackType.GoForward:
                Debug.Log("GoForward");
                break;
            case GpmWebViewCallback.CallbackType.ExecuteJavascript:
                Debug.LogFormat("ExecuteJavascript data : {0}, error : {1}", data, error);
                break;
        }

    }


    public void Close()
    {
        if (_webViewHandler.webViewObject != null)
        {
            _closeGO.SetActive(false);
            // GpmWebView.Close();
            Destroy(_webViewHandler.webViewObject.gameObject);
        }
    }


    private GpmWebViewRequest.Configuration GetConfiguration()
    {
        GpmWebViewRequest.CustomSchemePostCommand customSchemePostCommand = new GpmWebViewRequest.CustomSchemePostCommand();
        customSchemePostCommand.Close("CUSTOM_SCHEME_POST_CLOSE");

        return new GpmWebViewRequest.Configuration()
        {
            style = GpmWebViewStyle.FULLSCREEN,
            isClearCache = true,
            isClearCookie = true,
            backgroundColor = "#716e7d",

            title = "",
            navigationBarColor = "#716e7d",

            isNavigationBarVisible = true,
            isBackButtonVisible = false,
            isForwardButtonVisible = false,
            supportMultipleWindows = true,


            customSchemePostCommand = customSchemePostCommand,

            position = GetConfigurationPosition(),
            size = GetConfigurationSize(),
            // margins = GetConfigurationMargins(),
            contentMode= GpmWebViewContentMode.MOBILE
#if UNITY_IOS
            contentMode = GpmWebViewContentMode.MOBILE,
            isMaskViewVisible = true,
            isAutoRotation = false
#endif
        };
    }


    #region Helpers


    private GpmWebViewRequest.Position GetConfigurationPosition()
    {
        return new GpmWebViewRequest.Position
        {
            hasValue = true,
            x = (int)(Screen.height * 0.1f),
            y = (int)(Screen.height * 0.1f)
        };
    }

    private GpmWebViewRequest.Size GetConfigurationSize()
    {
        return new GpmWebViewRequest.Size
        {
            hasValue = true,
            width = (int)(Screen.height * 0.8f),
            height = (int)(Screen.height * 0.8f)
        };
    }

    private GpmWebViewRequest.Margins GetConfigurationMargins()
    {
        return new GpmWebViewRequest.Margins
        {
            hasValue = true,
            left = 0,
            top = (int)(Screen.height * 0.15f),
            right = 0,
            bottom = 0
        };
    }


    #endregion


}
