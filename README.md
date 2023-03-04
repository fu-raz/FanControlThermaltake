# FanControl Thermaltake Plugin
This is a plugin that lets [Fan Control](https://github.com/Rem0o/FanControl.Releases) connect to a Thermaltake Fan Controller. Right now this is a proof-of-concept thing, but I've tried to add all the available TT controllers.

![tt-controller](https://user-images.githubusercontent.com/5355154/179553404-eb8102e8-6ced-4eee-aae5-79912550e278.png)

*Known Issues:*
- Running this plugin and TT RGB Plus at the same time is a problem. Even setting the fans to 0% in the TT software doesn't really work
- Setting the fan power is instant, but the read out takes a few seconds to adjust. So it takes a while before the Fan Control app sees the right RPM and Power values.
