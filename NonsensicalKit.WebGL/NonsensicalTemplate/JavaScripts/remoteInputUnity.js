initEvents.push(init);

function init()
{
    console.log("RemoteInput Init");
    webBridgeEvent["RemoteInput"] = RInputEvent;
}

function RInputEvent(values)
{
    switch (values[0])
    {
        case "Create":
            state.wsAddress = values[1];
            state.wsPort = values[2];
            RemoteInput.configure(state.config);
            RemoteInput.start();
            break;
        case "Stop":
            RemoteInput.stop();
            break;
    }
}
