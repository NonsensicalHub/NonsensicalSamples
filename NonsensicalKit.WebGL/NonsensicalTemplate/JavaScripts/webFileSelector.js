initEvents.push(init);

function init() {
    console.log("webFileSelector Init");
    webBridgeEvent["FileSelector"] = fileSelectorEvent;

    //创建隐形的用于开启文件选择窗口的按钮
    var input = document.createElement("input");
    input.type = "file";
    input.id = "fileSelector";
    input.style.display = "none";
    input.addEventListener('click', function () {  this.value = '';  }, false);
    input.onchange = function () { fileSelect(); };
    input.multiple = "multiple";
    input.accept = ""; 
    document.body.append(input);
}

function fileSelectorEvent(values) {
    openFileSelector(values[1], values[2], values[3]);
}

let id;

function openFileSelector(type, isMultiple, crtID) {
    id = crtID;
    var tempFileLayout = document.getElementById('fileSelector');
    tempFileLayout.accept = type;
    if (isMultiple) {
        tempFileLayout.multiple = "multiple";
    }
    else {
        tempFileLayout.multiple = "none";
    }
    tempFileLayout.click();
}

let fileUrls=[];
function fileSelect() {
    
    for (var i = 0; i < fileUrls.length; i++)
    {
        URL.revokeObjectURL(fileUrls[i]);
    }
    fileUrls=[];   

    var files = document.getElementById('fileSelector').files;
    var array = [];
    for (let i = 0; i < files.length; ++i) {
        var newUrl= URL.createObjectURL(files[i]);
        fileUrls.push(newUrl);
        array.push(files[i].name);
        array.push(newUrl);
    }

    var str = JSON.stringify(array);
    sendMessageToUnity("FileSelected", str, id);
}
