initEvents.push(init);

function init() {
    console.log("websocketIO Init");
    webBridgeEvent["SocketIO"] = socketIOEvent;
}

function socketIOEvent(values) {
    switch (values[0]) {
        case "ConnectSocketIO":
            connectSocketIO(values[1], values[2]);
            break;
        case "AddListener":
            addListener(values[1], values[2]);
            break;
        case "SendMessage":
            sendMessage(values[1], values[2], values[3]);
            break;
        case "SendMessageWithCallback":
            sendMessageWithCallback(values[1], values[2], values[3]);
            break;
    }
}
const sockets = [];

function connectSocketIO(url, id) {
    // 确保 io 可用
    if (typeof io !== "function") {
        console.error("Socket.IO 客户端 (io) 未找到，请先引入 socket.io 客户端库。");
        return;
    }
    try {
        sockets[id] = io(url);
    } catch (e) {
        console.error("connectSocketIO 出错：", e);
    }
}

function addListener(eventName, id) {
    const sock = sockets[id];
    if (!sock) {
        console.warn(`addListener: socket id ${id} 不存在。`);
        return;
    }
    sock.on(eventName, (data) => {
        sendMessageToUnity("SocketIOMessage", data, eventName, id);
    });
}

function sendMessage(eventName, c, id) {
    const sock = sockets[id];
    if (!sock) {
        console.warn(`sendMessage: socket id ${id} 不存在。`);
        return;
    }
    let payload;
    try {
        payload = JSON.parse(c);
    } catch (e) {
        console.error("sendMessage: JSON.parse 失败，发送原始字符串。错误：", e);
        payload = c;
    }
    try {
        sock.emit(eventName, payload);
    } catch (e) {
        console.error("sendMessage emit 出错：", e);
    }
}

function sendMessageWithCallback(eventName, c, id) {
    const sock = sockets[id];
    if (!sock) {
        console.warn(`sendMessageWithCallback: socket id ${id} 不存在。`);
        return;
    }
    let payload;
    try {
        payload = JSON.parse(c);
    } catch (e) {
        console.error("sendMessageWithCallback: JSON.parse 失败，发送原始字符串。错误：", e);
        payload = c;
    }
    try {
        sock.emit(eventName, payload, (callback) => {
            sendMessageToUnity("SocketIOMessage", callback, eventName, id);
        });
    } catch (e) {
        console.error("sendMessageWithCallback emit 出错：", e);
    }
}
