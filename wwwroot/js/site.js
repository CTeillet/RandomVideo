window.reloadVideo = function(videoElement) {
    if (videoElement) {
        videoElement.load();
        videoElement.play();
    }
};
