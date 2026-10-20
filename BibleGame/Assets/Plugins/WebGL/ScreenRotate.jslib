mergeInto(LibraryManager.library, {
    RotateToLandscape: function () {
        // Prevent saving the landscape dimensions again.
        if (window.unityLandscapeState) return;

        var canvas = Module.canvas;
        var container = document.getElementById("unity-container");

        var state = {
            canvasStyle: canvas.style.cssText,
            containerStyle: container ? container.style.cssText : null
        };

        state.resize = function () {
            // Fit a 16:9 canvas inside the itch.io frame.
            var width = Math.min(
                window.innerWidth,
                window.innerHeight * 16 / 9
            );

            var height = width * 9 / 16;

            if (container) {
                container.style.setProperty(
                    "width", width + "px", "important"
                );
                container.style.setProperty(
                    "height", height + "px", "important"
                );
            }

            canvas.style.setProperty(
                "width", width + "px", "important"
            );
            canvas.style.setProperty(
                "height", height + "px", "important"
            );
        };

        window.unityLandscapeState = state;

        window.addEventListener("resize", state.resize);
        state.resize();

        // Optional physical orientation lock on supported mobile browsers.
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

            // Restore the original portrait layout.
            canvas.style.cssText = state.canvasStyle;

            if (container) {
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
    }
});