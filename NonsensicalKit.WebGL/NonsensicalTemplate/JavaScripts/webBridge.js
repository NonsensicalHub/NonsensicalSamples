let webBridgeEvent = new Array();

let initEvents = [];
function onUnityLoadEnd()
{
    sendMessageToUnity("UrlQueryStr", window.location.search);
    sendMessageToUnity("UrlHref", window.location.href);

    for (var i = 0; i < initEvents.length; i++)
    {
        initEvents[i]();
    }
}

function sendMessageToUnity()
{
    var O = [];
    for (var i = 0; i < arguments.length; i++)
        O.push(arguments[i]);
    var str = JSON.stringify(O);
    UnityInstance.SendMessage("WebBridge", "SendMessageToUnity", str);
}

function sendMessageToJS(key, valuestr)
{
    var func = webBridgeEvent[key];
    if (func != null)
    {
        var values = JSON.parse(valuestr);
        func(values);
    }
}
