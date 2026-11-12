
mergeInto(LibraryManager.library, {

    RotateToLandscape: function () {
        if (window.unityLandscapeState) return;

        var canvas = Module.canvas;
        var container = document.getElementById("unity-container");

        var state = {
            canvasStyle: canvas.style.cssText,
            containerStyle: container ? container.style.cssText : null
        };

        state.resize = function () {
            var width = Math.min(
                window.innerWidth,
                window.innerHeight * 16 / 9
            );

            var height = width * 9 / 16;

            if (container) {
                container.style.setProperty("width", width + "px", "important");
                container.style.setProperty("height", height + "px", "important");
            }

            canvas.style.setProperty("width", width + "px", "important");
            canvas.style.setProperty("height", height + "px", "important");
        };

        window.unityLandscapeState = state;
        window.addEventListener("resize", state.resize);
        state.resize();

        if (document.fullscreenElement &&
            screen.orientation &&
            screen.orientation.lock) {
            screen.orientation.lock("landscape").catch(function (error) {
                console.log("Landscape lock unavailable:", error);
            });
        }
    },

    RotateToPortrait: function () {
        var state = window.unityLandscapeState;

        if (state) {
            var canvas = Module.canvas;
            var container = document.getElementById("unity-container");

            window.removeEventListener("resize", state.resize);

            canvas.style.cssText = state.canvasStyle;

            if (container && state.containerStyle !== null) {
                container.style.cssText = state.containerStyle;
            }

            delete window.unityLandscapeState;
        }

        if (document.fullscreenElement &&
            screen.orientation &&
            screen.orientation.lock) {
            screen.orientation.lock("portrait").catch(function (error) {
                console.log("Portrait lock unavailable:", error);
            });
        }
    },

    // NEW: Rotate the entire WebGL screen 90 degrees.



RotateScreen90: function () {
    if (window.unityRotationState) return;

    var canvas = Module.canvas;
    var container = document.getElementById("unity-container");

    // Rotate only the actual WebGL display.
    var target = canvas;

    var state = {
        style: target.style.cssText,
        originalWidth: canvas.width,
        originalHeight: canvas.height
    };

    // Prevent Unity from changing canvas resolution
    // when CSS size changes.
    if (typeof unityInstance !== "undefined" &&
        unityInstance.Module &&
        unityInstance.Module.JSEvents) {
        // No resolution changes required.
    }

    target.style.setProperty(
        "position", "absolute", "important"
    );

    target.style.setProperty(
        "left", "50%", "important"
    );

    target.style.setProperty(
        "top", "50%", "important"
    );

    target.style.setProperty(
        "width", state.originalWidth + "px", "important"
    );

    target.style.setProperty(
        "height", state.originalHeight + "px", "important"
    );

    target.style.setProperty(
        "transform-origin", "center center", "important"
    );

    target.style.setProperty(
        "transform",
        "translate(-50%, -50%) rotate(90deg)",
        "important"
    );

    window.unityRotationState = state;
},

ResetScreenRotation: function () {
    var state = window.unityRotationState;
    if (!state) return;

    Module.canvas.style.cssText = state.style;

    delete window.unityRotationState;
}


});
