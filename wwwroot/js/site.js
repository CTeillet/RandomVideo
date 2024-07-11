function reloadVideo(videoElement) {
    if (videoElement) {
        videoElement.load();
        videoElement.play();
    }
}
