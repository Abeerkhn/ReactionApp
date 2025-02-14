let mediaRecorder;
let recordedChunks = [];
let player;

// ✅ Load YouTube API
(function () {
    var tag = document.createElement('script');
    tag.src = "https://www.youtube.com/iframe_api";
    var firstScriptTag = document.getElementsByTagName('script')[0];
    firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
})();

// ✅ YouTube API Ready Callback
function onYouTubeIframeAPIReady() {
    console.log("YouTube API Ready");
    player = new YT.Player('videoFrame', {
        events: {
            'onStateChange': onPlayerStateChange
        }
    });
}

function onPlayerStateChange(event) {
    console.log("Player state changed: ", event.data);

    if (event.data === YT.PlayerState.PLAYING && !isRecording) {
        console.log("Video started. Auto-starting recording...");
        startVideoRecording();
    } else if (event.data === YT.PlayerState.ENDED) {
        console.log("Video ended. Stopping recording...");
        stopVideoRecording();
    }
}


// ✅ Start Recording
function startVideoRecording() {
    navigator.mediaDevices.getUserMedia({ video: true, audio: true })
        .then(stream => {
            const videoElement = document.getElementById("cameraFeed");
            videoElement.srcObject = stream;
            videoElement.style.display = "block";

            mediaRecorder = new MediaRecorder(stream);
            recordedChunks = [];

            mediaRecorder.ondataavailable = event => {
                if (event.data.size > 0) {
                    recordedChunks.push(event.data);
                }
            };

            mediaRecorder.start();
            console.log("Recording started...");

            document.getElementById("startRecordingBtn").disabled = true;
            document.getElementById("stopRecordingBtn").disabled = false;
        })
        .catch(err => {
            console.error("Error accessing camera/microphone:", err);
            alert("Please allow camera and microphone access.");
        });
}

// ✅ Stop Recording
function stopVideoRecording() {
    if (mediaRecorder) {
        mediaRecorder.stop();
        console.log("Recording stopped.");

        document.getElementById("startRecordingBtn").disabled = false;
        document.getElementById("stopRecordingBtn").disabled = true;

        const blob = new Blob(recordedChunks, { type: "video/webm" });
        const url = URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = "recorded-video.webm";
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
    }
}
