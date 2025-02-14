let mediaRecorder;
let recordedChunks = [];

function startVideoRecording() {
    // Ensure the DOM is fully loaded
    document.addEventListener("DOMContentLoaded", () => {
        const videoElement = document.getElementById("cameraFeed");

        if (!videoElement) {
            console.error("Error: Camera feed video element not found.");
            return;
        }

        navigator.mediaDevices.getUserMedia({ video: true, audio: true })
            .then(stream => {
                videoElement.srcObject = stream; // Assign camera stream
                videoElement.style.display = "block"; // Show hidden camera feed

                console.log("Camera and microphone access granted.");

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
    });
}

function stopVideoRecording() {
    if (mediaRecorder) {
        mediaRecorder.stop();
        console.log("Recording stopped.");

        document.getElementById("startRecordingBtn").disabled = false;
        document.getElementById("stopRecordingBtn").disabled = true;

        // Save the recorded video
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
