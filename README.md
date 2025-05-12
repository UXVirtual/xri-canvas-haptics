[![npm package](https://img.shields.io/npm/v/com.uxvirtual.xri-canvas-haptics)](https://www.npmjs.com/package/com.uxvirtual.xri-canvas-haptics)
[![openupm](https://img.shields.io/npm/v/com.uxvirtual.xri-canvas-haptics?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.uxvirtual.xri-canvas-haptics/)
![Tests](https://github.com/uxvirtual/xri-canvas-haptics/workflows/Tests/badge.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

# XRI Canvas Haptics

Package for Unity XR Interaction Toolkit v3.x which enables haptics actuation for Unity canvas IPointer UI events via the SimpleHapticFeedback script.

- [How to use](#how-to-use)
- [Install](#install)
  - [via Git URL](#via-git-url)
  - [via npm](#via-npm)
  - [via OpenUPM](#via-openupm)
  - [Tests](#tests)
- [Configuration](#configuration)
  - [Scene Setup](#scene-setup)
  - [Usage](#usage)
- [Contributors](#contributors)

<!-- toc -->

## How to use

## Install

The best way to install the package is via the Unity Package Manager using the Git URL.

### via Git URL

Open `Packages/manifest.json` with your favorite text editor. Add following line to the dependencies block:
```json
{
  "dependencies": {
    "com.uxvirtual.xri-canvas-haptics": "https://github.com/uxvirtual/xri-canvas-haptics.git?path=/com.uxvirtual.xri-canvas-haptics"
  }
}
```

### via npm

Open `Packages/manifest.json` with your favorite text editor. Add a [scoped registry](https://docs.unity3d.com/Manual/upm-scoped.html) and following line to dependencies block:
```json
{
  "scopedRegistries": [
    {
      "name": "npmjs",
      "url": "https://registry.npmjs.org/",
      "scopes": [
        "com.uxvirtual"
      ]
    }
  ],
  "dependencies": {
    "com.uxvirtual.xri-canvas-haptics": "1.0.0"
  }
}
```
Package should now appear in package manager.

### via OpenUPM

The package is also available on the [openupm registry](https://openupm.com/packages/com.uxvirtual.xri-canvas-haptics). You can install it eg. via [openupm-cli](https://github.com/openupm/openupm-cli).

```
openupm add com.uxvirtual.xri-canvas-haptics
```

## Configuration

### Scene Setup

You can open a preconfigured Unity 6 project from `Samples/XRICanvasHapticsTester`, otherwise try the following:

#### Sample Project - Import Haptics Starter Sample Into Existing Unity Project
1. Import *XR Interaction Toolkit v3.x* and all required dependencies.
2. Import the *Starter Assets* sample from the *XR Interaction Toolkit* package.
3. Import the *Haptics Starter* sample from the *XRI Canvas Haptics* package.

#### New Projects - Add XR Interaction Toolkit XR Rig
1. Import *XR Interaction Toolkit v3.x* and all required dependencies.
2. Import the *Starter Assets* sample from the XR Interaction Toolkit package.
3. Create a new Unity scene.
4. Add the XR Origin (XR Rig) prefab from the *Starter Assets* sample to your scene.

##### Add XRI Canvas Haptics Scripts
1. Add the *Manual Haptics Manager* script from the *XRI Canvas Haptics* package to the XR Rig.
2. Add the *Canvas Haptic Manager* script to each canvas that you wish to activate haptic interaction on. This canvas should contain UI elements such Buttons, Sliders, Input fields etc as its children. This will search down multiple levels to find all descendents.
3. Set the Button Container reference in the *Canvas Haptic Manager* script to a GameObject that contains Buttons anywhere in its heirarchy.
4. Set the *Manual Haptics Manager* reference to the script that was placed on your XR Rig. If you don't set this or your XR Rig is instantiated later, you will need to update this reference on each Canvas UI that has *Canvas Haptic Manager* attached.
5. Create a new GameObject called `Event System` and add *Event System* and *XR UI Input Module* components to it. This is required for the *UI Element Haptic Helper* to hook into the Event System when interactors select or hover over Buttons in UI Canvases.
6. (optional) Manually add *UI Element Haptic Helper* script to any Buttons, Sliders, Input fields for fine control over which child UI element GameObjects will activate haptics. This should work with any UI element that implements `IPointerEnterHandler`, `IPointerExitHandler`, `IPointerDownHandler` and `IPointerUpHandler`.

##### Add Hand Tracking Support
1. (optional) Add and configure an additional *SimpleHapticFeedback* script to the Near-Far and Poke interactors on the *Left* and *Right Hand* GameObjects in the *Hands Interaction Demo* sample from the *Unity XR Interaction Toolkit*.

### Usage

1. Start SteamVR and/or connect your XR headset to another OpenXR runtime.
2. Test the scene by starting Play Mode in the Unity Editor, the buttons should have a *Key Haptics Helper* script added to them. This observes any interactions with the Button UI element and triggers haptic effects on the current Near-Far or Poke interactors that have a *SimpleHapticFeedback* script added to the interactor.

## Contributors

See [CONTRIBUTING.md](https://github.com/UXVirtual/xri-canvas-haptics/blob/main/CONTRIBUTING.md) for a guide on how to contribute fixes or features back to this project.

### Tests

The package can optionally be set as *testable*.
In practice this means that tests in the package will be visible in the [Unity Test Runner](https://docs.unity3d.com/2017.4/Documentation/Manual/testing-editortestsrunner.html).

Open `Packages/manifest.json` with your favorite text editor. Add following line **after** the dependencies block:
```json
{
  "dependencies": {
  },
  "testables": [ "com.uxvirtual.xri-canvas-haptics" ]
}
```

## License

MIT License

Copyright © 2025 UXVirtual
