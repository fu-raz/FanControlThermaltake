# FanControl Thermaltake Plugin
This is a plugin that let's [Fan Control](https://github.com/Rem0o/FanControl.Releases) connect to a Thermaltake Fan Controller. Right now this is a proof-of-concept thing. The code is sub-optimal and only connects to the controller you get with the Riing Plus fans.

![tt-controller](https://user-images.githubusercontent.com/5355154/179553404-eb8102e8-6ced-4eee-aae5-79912550e278.png)

*Known Issues:*
- It doesn't work when TTRGB is running. You can't run TTRGB and FanControl with this plugin at the same time. Which means you don't get to change the RGB of the fans.
- Setting the fan power is instant, but the read out takes a few seconds to adjust. So it takes a while before the Fan Control app sees the right RPM and Power values.

*Possible Issues:*
- I have added code for checking it there's more than one TT controller in the system. I wasn't able to test it, since I only had one in my development PC. So it might work, it might not.

*Todo:*
- Make it more modular so I can easily add more TT controllers if needed.
- Implement the Thermaltake controller into my [RazerChromaConnect app](https://github.com/fu-raz/Razer-Chroma-WLED-Connect-App) so you have at least one option to control the RGB if you can't use TTRGB.
- Maybe I can have this plugin take precedence over TTRGB when it comes to the fan speeds.
