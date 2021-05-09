# Raspberry Pi Smart Thermostat

This is the software for my RPi smart thermostat project.

## Project Summary

- `core`: Common types and constants used across all projects.
- `db`: Database schema (used for logging temperature data)
- `rpi-stat`: Executable and hardware drivers which run on the RPi and interface with the temperature sensor and transmitter
- `rpi-stat-server`: Central Websocket (SignalR) server which all appliances running `rpi-stat` will connect to
- `rpi-stat-ui`: An HTML user interface for the server

## Hardware

- [Raspberry Pi Model A](https://shop.pimoroni.com/products/raspberry-pi-3-a-plus)
- [MCP9808 Temp Sensor](https://shop.pimoroni.com/products/mcp9808-high-accuracy-i2c-temperature-sensor-breakout-board)
- [Energenie ENER314 RF Transmitter & Sockets](https://shop.pimoroni.com/products/pi-mote-control-starter-kit-with-2-sockets)
- [Pico Hat Hacker](https://shop.pimoroni.com/products/pico-hat-hacker)
- [GPIO Hammer Header](https://shop.pimoroni.com/products/gpio-hammer-header?variant=35643317962)
- [11mm GPIO Header](https://shop.pimoroni.com/products/2x20-pin-gpio-header-for-raspberry-pi-2-b-a?variant=1132812269)

## Device Setup

### OS Install

1. Download [Balena Etcher](https://www.balena.io/etcher/)
1. Flash [2019-09-26-raspbian-buster-lite](https://downloads.raspberrypi.org/raspbian_lite/images/raspbian_lite-2019-09-30/2019-09-26-raspbian-buster-lite.zip) to SD Card
1. Open `boot` SD partition and copy two files to it:
   - `ssh`: empty file
   - `wpa_supplicant`: see [example](#example-wpa_supplicant) below

### Passwordless SSH Setup

#### Windows

Run these commands to generate a public/private key pair, then copy the public key to the RPi.

```bash
ssh-keygen -t rsa

cat ~/.ssh/id_rsa.pub | ssh pi@192.168.0.X 'mkdir -p ~/.ssh && cat >> ~/.ssh/authorized_keys'
```

You'll be prompted for the `pi` user password when you run the copy command.

**IMPORTANT NOTE**: If your key file isn't called `id_rsa`, it won't be automatically picked up by `sshd` on the Pi, so you'll have to manually add it with `ssh-add`.

### Initial RPi Setup

1. Change the password to stop the OS warning you every time you log in
1. Run `touch ~/.hushlogin` to stop the Debian login message being displayed every time you log in
1. Run `sudo apt-get update` then `sudo apt-get dist-upgrade` to get the latest OS updates
1. Run `sudo raspi-config` to enable the I2C interface (required for the `MCP9808` temp sensor

### .NET Core Build / Publishing

#### Windows

Publish the `rpi-stat` project directly to the Pi:

```bash
dotnet publish -r linux-arm

scp -r .\bin\Debug\netcoreapp3.1\linux-arm\publish pi@192.168.0.X:/home/pi/rpi-stat
```

There are scripts in the `rpi-stat` project to publish the whole project (all binaries) and just the executable.

#### Raspberry Pi

We need to give the published executable execute permissions on the Pi.

```bash
cd ~/rpi-stat

sudo chmod +x rpi-stat

./rpi-stat
```

### Server SSL Setup

#### Windows

1. Download the binary `mkcert` package [for Windows](https://github.com/FiloSottile/mkcert/releases); it'll be named something like `mkcert-v1.4.1-windows-amd64.exe`
1. Rename the downloaded file to `mkcert.exe` and (optionally) add the path to your `PATH` environment variable
1. Create the CA with `mkcert -install`
1. Create a certificate for your desired server hostname:  `mkcert -pkcs12 rpi-stat`
1. Import the certificate using the Server Certificates IIS Manager snap-in
1. Create a site and binding in IIS, using the new certificate for the `https` binding

#### Raspberry Pi

Copy the `mkcert` root CA cert to the Pi:  

```bash
scp -r C:\Users\me\AppData\Local\mkcert\rootCA.pem pi@192.168.78.11:/home/pi/ca
```

Then change to your home directory and install `mkcert` for Linux ARM:

```
cd ~/

sudo apt install libnss3-tools

wget -O mkcert https://github.com/FiloSottile/mkcert/releases/download/v1.4.1/mkcert-v1.4.1-linux-arm

chmod +x  mkcert

sudo mv mkcert /usr/local/bin

CAROOT=~/ca mkcert -install

sudo nano /etc/hosts
```

Add a hosts line for IP address of server e.g. 

`192.168.0.X   rpi-stat`

Finally, test with `wget https://rpi-stat`.


- https://medium.com/@aweber01/locally-trusted-development-certificates-with-mkcert-and-iis-e09410d92031

### Example `wpa_supplicant`

```
country=GB
update_config=1
ctrl_interface=/var/run/wpa_supplicant

network={
    ssid="YOUR-NETWORK"
    psk="YOUR-PASSWORD"
}

```

## Running (as of now)

### First run

- Publish `rpi-stat-server` with `dotnet publish -c Debug /p:EnvironmentName=Development` (there is a `publish.ps1` script for ongoing dev)
- Point the IIS site you created earlier (bound to `https://rpi-stat`) at the publish output

### First _and_ subsequent runs

Make sure the Energenie socket is plugged into an outlet.

You should be able to browse to `https://rpi-stat/home/test` and see an `OK` response (this is a SignalR hub, so there is no response from `/`)

- Fire up the PI and SSH in, then run `./rpi-stat/rpi-stat` to start the stat (you'll see some status messages)

  If you see errors at this point, edit the hosts file on the Pi (`sudo nano /etc/hosts`) and make sure the IP address for `rpi-stat` still points to the correct place for `https://rpi-stat`
- Open a shell in the `rpi-stat-ui` project folder, then `dotnet watch run`
- Navigate to `https://localhost:5000` to see the control panel

There we go! Setting the temp setting lower than the current temp reading should turn the remote socket off, setting it higher should turn the socket back on.

## Reference

Reading material for the various parts of the system.

### IOT/Device Drivers

- https://github.com/dotnet/iot
- https://learn.sparkfun.com/tutorials/i2c
- https://github.com/dwelch67/raspberrypi
- https://learn.sparkfun.com/tutorials/serial-communication
- https://blog.athrunen.dev/learning-hardware-programming-as-a-software-engineer/
- https://downloads.raspberrypi.org/raspbian_lite/images/

### Security

- https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-3.1
- https://wildermuth.com/2018/04/10/Using-JwtBearer-Authentication-in-an-API-only-ASP-NET-Core-Project
- https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api
- https://www.verypossible.com/blog/best-practices-for-securing-iot-devices
- https://docs.microsoft.com/en-us/aspnet/core/security/authentication/certauth?view=aspnetcore-3.1

### SignalR / Websockets

- https://docs.microsoft.com/en-gb/aspnet/core/tutorials/signalr-typescript-webpack?view=aspnetcore-3.1&tabs=visual-studio
- https://stackoverflow.blog/2019/12/18/websockets-for-fun-and-profit/

### PWA / UI

- https://dev.to/thisdotmedia/intro-to-pwa-and-service-workers-15d4
- https://bootstrapbay.github.io/lazy-kit/

### .NET Core

- https://blog.technitium.com/2019/01/quick-and-easy-guide-to-install-net.html


