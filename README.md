# SteamVR_HUDCenter
**NuGet Package [OUTDATED]**
---
A NuGet package is available **[here](https://www.nuget.org/packages/SteamVR_HUDCenter/)**

**Notifications Hello World**
---
Here's a sample code to create a quick Overlay to send notifications though OpenVR.
```c#
            HUDCenterController VRController = new HUDCenterController();
            VRController.Init();
            Overlay Dummy = new Overlay("Dummy", 0);
            VRController.RegisterNewItem(Dummy);
            VRController.DisplayNotification("Hello World!", Dummy, EVRNotificationType.Transient, EVRNotificationStyle.Application, new NotificationBitmap_t());
```

Since v0.0.3 the above code can be written as:
```c#
            HUDCenterController.GetInstance().Init();
            Overlay Dummy = new Overlay("Dummy", 0);
            HUDCenterController.GetInstance().RegisterNewItem(Dummy);
            HUDCenterController.GetInstance().DisplayNotification("Hello World!", Dummy, EVRNotificationType.Transient, EVRNotificationStyle.Application, new NotificationBitmap_t());
```
---
Code based on https://github.com/Marlamin work. The final goal of this project is to create a modular API to let C# developer create VR overlays.
