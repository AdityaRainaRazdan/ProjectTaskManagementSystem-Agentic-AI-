window.support = {};

let connection;
let peer;
let localStream;
let targetConnectionId;

window.support.initialize = async function () {

    if (connection && connection.state === "Connected") {
        console.log("SupportHub already connected.");
        return;
    }

    console.log("Initializing Support SignalR...");

    connection = new signalR.HubConnectionBuilder()
        .withUrl("/supporthub")
        .withAutomaticReconnect()
        .build();

    connection.on("ReceiveOffer", async (fromId, offerJson) => {

        console.log("Offer received");

        targetConnectionId = fromId;

        const offer = JSON.parse(offerJson);

        if (!peer) {
            peer = new RTCPeerConnection({
                iceServers: [{ urls: "stun:stun.l.google.com:19302" }]
            });

            peer.ontrack = e => {
                document.getElementById("adminVideo").srcObject = e.streams[0];
            };

            peer.onicecandidate = async e => {
                if (e.candidate) {
                    await connection.invoke("SendIce",
                        targetConnectionId,
                        JSON.stringify(e.candidate));
                }
            };
        }

        await peer.setRemoteDescription(new RTCSessionDescription(offer));

        const answer = await peer.createAnswer();
        await peer.setLocalDescription(answer);

        await connection.invoke("SendAnswer",
            fromId,
            JSON.stringify(answer));
    });
    connection.on("ReceiveAnswer", async (answerJson) => {

        console.log("Answer received");

        const answer = JSON.parse(answerJson);
        await peer.setRemoteDescription(new RTCSessionDescription(answer));
    });
    connection.on("ReceiveIce", async (candidateJson) => {

        const candidate = JSON.parse(candidateJson);
        await peer.addIceCandidate(new RTCIceCandidate(candidate));
    });

    connection.on("SupportRequested", (connId, username) => {

        console.log("Support request received from:", username);

        const list = document.getElementById("supportList");
        if (!list) return;

        const li = document.createElement("li");
        li.innerHTML = `
            <strong>${username}</strong>
            <button onclick="support.join('${connId}')">
                Join
            </button>
        `;
        
        list.appendChild(li);
    });

    //// ============================
    //// USER: HANDLE CONTROL REQUEST
    //// ============================

    //connection.on("ControlRequested", async (adminId) => {

    //    const allow = confirm("Admin is requesting control of your screen. Allow?");

    //    if (allow) {
    //        targetConnectionId = adminId;
    //        await connection.invoke("SendControlPermission", adminId, true);
    //    }
    //});

    await connection.start();

    console.log("SupportHub Connected:", connection.connectionId);
};


window.support.registerAdmin = async function () {

    if (!connection || connection.state !== "Connected") {
        console.error("Connection not ready for RegisterAdmin");
        return;
    }

    await connection.invoke("RegisterAdmin");
    console.log("Admin registered in Admins group");
};


window.support.requestHelp = async function () {

    if (!connection || connection.state !== "Connected") {
        console.error("SignalR not connected on user side.");
        return;
    }

    peer = new RTCPeerConnection({
        iceServers: [{ urls: "stun:stun.l.google.com:19302" }]
    });

    localStream = await navigator.mediaDevices.getDisplayMedia({
        video: true
    });

    localStream.getTracks().forEach(track =>
        peer.addTrack(track, localStream));

    await connection.invoke("RequestSupport");

    peer.onicecandidate = async e => {
        if (e.candidate) {
            await connection.invoke("SendIce",
                targetConnectionId,
                JSON.stringify(e.candidate));
        }
    };
};



window.support.join = async function (connId) {

    targetConnectionId = connId;

    peer = new RTCPeerConnection({
        iceServers: [{ urls: "stun:stun.l.google.com:19302" }]
    });

    // 🔥 THIS LINE IS THE FIX
    peer.addTransceiver("video", { direction: "recvonly" });

    peer.ontrack = e => {
        console.log("Track received");
        document.getElementById("adminVideo").srcObject = e.streams[0];
    };

    peer.onicecandidate = async e => {
        if (e.candidate) {
            await connection.invoke("SendIce",
                targetConnectionId,
                JSON.stringify(e.candidate));
        }
    };

    const offer = await peer.createOffer();
    await peer.setLocalDescription(offer);

    await connection.invoke("SendOffer",
        connId,
        JSON.stringify(offer));

    console.log("Offer sent to user");

    // Request control permission
    await connection.invoke("RequestControl", targetConnectionId);
};

document.addEventListener("DOMContentLoaded", async () => {

    if (window.location.pathname.includes("admin-support")) {
        await window.support.initialize();
        await window.support.registerAdmin();
    }
    else {
        await window.support.initialize();
    }

});