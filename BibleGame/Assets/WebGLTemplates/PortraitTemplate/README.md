# Unity WebGL Portrait Template

## Install
1. Copy this whole `PortraitTemplate` folder into your Unity project at:
   `Assets/WebGLTemplates/PortraitTemplate/`
   (the folder name becomes the template name Unity shows you)
2. Add a `favicon.ico` and, optionally, `unity-logo-dark.png` / `webgl-logo.png` into `TemplateData/` if you want custom branding (referenced via `background-image` in `style.css` — add those rules if used).
3. In Unity: **Edit > Project Settings > Player > Resolution and Presentation**
   - Set **WebGL Template** to `PortraitTemplate`.
   - Under **Resolution and Presentation**, set **Default Canvas Width/Height** to a portrait ratio, e.g. `1080 x 1920`.
   - Enable **Run In Background** if you want it to keep running when the tab loses focus.
4. Build. Unity will populate `Build/` and `StreamingAssets/` next to `index.html` automatically.

## What it does
- Locks the visual layout to a 9:16 portrait aspect box, letterboxed and centered on any screen size (phone, tablet, desktop).
- Shows a "please rotate your device" overlay if a mobile device is physically turned to landscape, and pauses rendering of the container until it's back in portrait (Unity keeps running underneath — extend the JS if you want to actually pause the instance).
- Standard Unity loading bar / progress UI, fullscreen button, and warning banner, restyled for a dark portrait UI.
- `user-scalable=no` and `touch-action: none` to prevent pinch-zoom/scroll interfering with touch input.

## Customize
- `PORTRAIT_WIDTH` / `PORTRAIT_HEIGHT` constants near the top of the `<script>` block in `index.html` control the aspect ratio used for letterboxing — change these to match your canvas render size (defaults to 1080×1920 / 9:16).
- If you don't want the rotate-to-portrait lock at all (e.g. you want portrait *and* landscape to both just letterbox), delete the `checkOrientation()` call and the `#rotate-overlay` block.
- Colors, loading bar, and footer styling are all in `TemplateData/style.css`.
