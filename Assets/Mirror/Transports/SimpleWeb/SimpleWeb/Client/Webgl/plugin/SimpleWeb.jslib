// this will create a global object
<<<<<<< HEAD
const SimpleWeb =
{
    webSockets: [],
    next: 1,
    GetWebSocket: function (index)
    {
        return SimpleWeb.webSockets[index]
    },
    AddNextSocket: function (webSocket)
    {
=======
const SimpleWeb = {
    webSockets: [],
    next: 1,
    GetWebSocket: function (index) {
        return SimpleWeb.webSockets[index]
    },
    AddNextSocket: function (webSocket) {
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
        var index = SimpleWeb.next;
        SimpleWeb.next++;
        SimpleWeb.webSockets[index] = webSocket;
        return index;
    },
<<<<<<< HEAD
    RemoveSocket: function (index)
    {
=======
    RemoveSocket: function (index) {
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
        SimpleWeb.webSockets[index] = undefined;
    },
};

<<<<<<< HEAD
function IsConnected(index)
{
    var webSocket = SimpleWeb.GetWebSocket(index);
    if (webSocket)
        return webSocket.readyState === webSocket.OPEN;
    else
        return false;
}

function Connect(addressPtr, openCallbackPtr, closeCallBackPtr, messageCallbackPtr, errorCallbackPtr)
{
    // fix for unity 2021 because unity bug in .jslib
    if (typeof Runtime === "undefined")
    {
        // if unity doesn't create Runtime, then make it here
        // dont ask why this works, just be happy that it does
        var Runtime = { dynCall: dynCall }
=======
function IsConnected(index) {
    var webSocket = SimpleWeb.GetWebSocket(index);
    if (webSocket) {
        return webSocket.readyState === webSocket.OPEN;
    }
    else {
        return false;
    }
}

function Connect(addressPtr, openCallbackPtr, closeCallBackPtr, messageCallbackPtr, errorCallbackPtr) {
    // fix for unity 2021 because unity bug in .jslib
    if (typeof Runtime === "undefined") {
        // if unity doesn't create Runtime, then make it here
        // dont ask why this works, just be happy that it does
        Runtime = {
            dynCall: dynCall
        }
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
    }

    const address = UTF8ToString(addressPtr);
    console.log("Connecting to " + address);
<<<<<<< HEAD

    // Create webSocket connection.
    var webSocket = new WebSocket(address);
    webSocket.binaryType = 'arraybuffer';

    const index = SimpleWeb.AddNextSocket(webSocket);

    // Connection opened
    webSocket.addEventListener('open', function (event)
    {
        console.log("Connected to " + address);
        Runtime.dynCall('vi', openCallbackPtr, [index]);
    });
    webSocket.addEventListener('close', function (event)
    {
=======
    // Create webSocket connection.
    webSocket = new WebSocket(address);
    webSocket.binaryType = 'arraybuffer';
    const index = SimpleWeb.AddNextSocket(webSocket);

    // Connection opened
    webSocket.addEventListener('open', function (event) {
        console.log("Connected to " + address);
        Runtime.dynCall('vi', openCallbackPtr, [index]);
    });
    webSocket.addEventListener('close', function (event) {
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
        console.log("Disconnected from " + address);
        Runtime.dynCall('vi', closeCallBackPtr, [index]);
    });

    // Listen for messages
<<<<<<< HEAD
    webSocket.addEventListener('message', function (event)
    {
        if (event.data instanceof ArrayBuffer)
        {
=======
    webSocket.addEventListener('message', function (event) {
        if (event.data instanceof ArrayBuffer) {
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
            // TODO dont alloc each time
            var array = new Uint8Array(event.data);
            var arrayLength = array.length;

            var bufferPtr = _malloc(arrayLength);
            var dataBuffer = new Uint8Array(HEAPU8.buffer, bufferPtr, arrayLength);
            dataBuffer.set(array);

            Runtime.dynCall('viii', messageCallbackPtr, [index, bufferPtr, arrayLength]);
            _free(bufferPtr);
        }
<<<<<<< HEAD
        else
        {
=======
        else {
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
            console.error("message type not supported")
        }
    });

<<<<<<< HEAD
    webSocket.addEventListener('error', function (event)
    {
        console.error('Socket Error', event);
=======
    webSocket.addEventListener('error', function (event) {
        console.error('Socket Error', event);

>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
        Runtime.dynCall('vi', errorCallbackPtr, [index]);
    });

    return index;
}

function Disconnect(index) {
    var webSocket = SimpleWeb.GetWebSocket(index);
<<<<<<< HEAD
    if (webSocket)
        webSocket.close(1000, "Disconnect Called by Mirror");
=======
    if (webSocket) {
        webSocket.close(1000, "Disconnect Called by Mirror");
    }
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc

    SimpleWeb.RemoveSocket(index);
}

function Send(index, arrayPtr, offset, length) {
    var webSocket = SimpleWeb.GetWebSocket(index);
<<<<<<< HEAD
    if (webSocket)
    {
=======
    if (webSocket) {
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
        const start = arrayPtr + offset;
        const end = start + length;
        const data = HEAPU8.buffer.slice(start, end);
        webSocket.send(data);
        return true;
    }
    return false;
}

<<<<<<< HEAD
const SimpleWebLib =
{
=======

const SimpleWebLib = {
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
    $SimpleWeb: SimpleWeb,
    IsConnected,
    Connect,
    Disconnect,
    Send
};
<<<<<<< HEAD

=======
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
autoAddDeps(SimpleWebLib, '$SimpleWeb');
mergeInto(LibraryManager.library, SimpleWebLib);
