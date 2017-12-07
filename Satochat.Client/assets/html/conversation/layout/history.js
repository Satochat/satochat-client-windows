window.onMessageDeliveryStatusChanged = function(message, status) {
    let element = document.getElementById(message);
    if (!element) {
        console.error("Could not find element for message with UUID " + message);
        return;
    }

    if (status === "Delivered") {
        element.classList.add("delivered");
    }
};

window.usersTyping = {};

window.onUserTyping = function (userUuid, nickname) {
    let timeout = window.usersTyping[userUuid];
    if (timeout) {
        clearTimeout(timeout);
        timeout = null;
        window.usersTyping[userUuid] = null;
    }

    let element = document.getElementById(userUuid);
    if (!element) {
        element = document.createElement("div");
        element.id = userUuid;
        let container = document.getElementById("users-typing");
        container.appendChild(element);
    }

    element.innerText = nickname + " is typing...";
    element.style.display = "block";

    window.usersTyping[userUuid] = setTimeout(function () {
        if (!window.usersTyping[userUuid]) {
            return;
        }

        let element = document.getElementById(userUuid);
        if (element) {
            element.style.display = "none";
        }

        window.usersTyping[userUuid] = null;
    }, 3000);
}
