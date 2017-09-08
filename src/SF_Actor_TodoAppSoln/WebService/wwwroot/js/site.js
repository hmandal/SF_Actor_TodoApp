/* This function calls the ActorBackendController's HTTP GET method to get the number of actors in the ActorBackendService */
function getAllDevices() {
    var http = new XMLHttpRequest();
    http.onreadystatechange = function () {
        if (http.readyState === 4) {
            end = new Date().getTime();
            if (http.status < 400) {
                returnData = JSON.parse(http.responseText);
                if (returnData) {
                    updateFooter(http, (end - start));
                    countDisplay.innerHTML = returnData.devicesInfo[0].device.id
                        + returnData.devicesInfo[1].device.id;
                }
            } else {
                updateFooter(http, (end - start));
            }
        }
    };
    start = new Date().getTime();
    http.open("GET", "/api/ActorBackendService/GetAllDevicesAsync/?c=" + start);
    http.send();
}

/* This function calls the ActorBackendController's HTTP POST method to create a new actor in the ActorBackendService */
function addNewDevice() {
    var devActorId = getDeviceActorId();

    var http = new XMLHttpRequest();
    http.onreadystatechange = function () {
        if (http.readyState === 4) {
            end = new Date().getTime();
            if (http.status < 400) {
                returnData = JSON.parse(http.responseText);
                if (returnData) {
                    updateFooter(http, (end - start));
                    //countDisplay.innerHTML = returnData.id + ((((returnData || {}).devErrInfo || {}).errorInfo || {}).msg || " \<NoErrors\>");
                    debugger;
                    countDisplay.innerHTML = '<button class="btn btn- primary btn- block" onclick="addNewDevice()" type="button" id="addNewDevice" tabindex="1">' + returnData.id + '</button>';
                }
            } else {
                updateFooter(http, (end - start));
            }
        }
    };
    start = new Date().getTime();
    http.open("POST", "/api/ActorBackendService/AddNewDeviceAsync/" + devActorId + "/?c=" + start);
    http.send();
}

/* This function calls the ActorBackendController's HTTP POST method to create a new actor in the ActorBackendService */
function renameFirstDevice() {
    var http = new XMLHttpRequest();
    http.onreadystatechange = function () {
        if (http.readyState === 4) {
            end = new Date().getTime();
            if (http.status < 400) {
                returnData = JSON.parse(http.responseText);
                if (returnData) {
                    updateFooter(http, (end - start));
                    countDisplay.innerHTML = returnData.id + ((((returnData || {}).devErrInfo || {}).errorInfo || {}).msg || " \<NoErrors\>");
                }
            } else {
                updateFooter(http, (end - start));
            }
        }
    };
    start = new Date().getTime();
    http.open("POST", "/api/ActorBackendService/RenameFirstDeviceAsync/?c=" + start);
    http.send();
}

function stubFn() {
    var devActorId = getDeviceActorId();

    var http = new XMLHttpRequest();
    http.onreadystatechange = function () {
        if (http.readyState === 4) {
            end = new Date().getTime();
            if (http.status < 400) {
                returnData = JSON.parse(http.responseText);
                if (returnData) {
                    updateFooter(http, (end - start));
                    countDisplay.innerHTML = returnData;
                }
            } else {
                updateFooter(http, (end - start));
            }
        }
    };
    start = new Date().getTime();
    http.open("POST", "/api/ActorBackendService/StubEndpointAsync/" + devActorId + "/?c=" + start);
    http.send();
}

/**
 * Returns a random integer between min (inclusive) and max (inclusive)
 * Using Math.round() will give you a non-uniform distribution!
 * @returns {number} Device Actor ID.
 */
function getDeviceActorId() {
    return getDeviceActorId.deviceActorId++;
}

getDeviceActorId.deviceActorId = 1000;

/* This function highlights the current nav tab */
function navTab() {
    toggleFooter(0);
    var pathName = document.location.pathname.substring(6);
    switch (pathName) {
        case "":
            document.getElementById('navHome').className = "active";
            break;
        case "Stateless":
            document.getElementById('navStateless').className = "active";
            break;
        case "Stateful":
            document.getElementById('navStateful').className = "active";
            break;
        case "Actor":
            document.getElementById('navActor').className = "active";
            break;
        case "Guest":
            document.getElementById('navGuest').className = "active";
            break;
    }
}

/*This function hides the footer*/
function toggleFooter(option) {
    var footer = document.getElementById('footer');
    switch (option) {
        case 0:
            footer.hidden = true;
            break;
        case 1:
            footer.hidden = false;
            break;
    }
}

/*This function puts HTTP result in the footer */
function updateFooter(http, timeTaken) {
    toggleFooter(1);
    if (http.status < 299) {
        statusPanel.className = 'panel panel-success';
        statusPanelHeading.innerHTML = http.status + ' ' + http.statusText;
        statusPanelBody.innerHTML = 'Result returned in ' + timeTaken.toString() + ' ms from ' + http.responseURL;
    }
    else {
        statusPanel.className = 'panel panel-danger';
        statusPanelHeading.innerHTML = http.status + ' ' + http.statusText;
        statusPanelBody.innerHTML = http.responseText;
    }
}

function handleEnter() {
    var pathName = document.location.pathname.substring(6);
    switch (pathName) {
        case "Stateless":
            onkeyup = function (e) {
                if (e.keyCode === 13) {
                    getStatelessBackendCount();
                    return false;
                }
            };
            break;
        case "Stateful":
            keyInput.onkeyup = function (e) {
                if (e.keyCode === 13) {
                    addStatefulBackendServiceKeyValuePair();
                    return false;
                }
            };
            valueInput.onkeyup = function (e) {
                if (e.keyCode === 13) {
                    addStatefulBackendServiceKeyValuePair();
                    return false;
                }
            };
            break;
    }
}