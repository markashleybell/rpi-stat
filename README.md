# Raspberry Pi Smart Thermostat

This is the software for my RPi start thermostat project.

## Hardware

- [Raspberry Pi Model A](https://shop.pimoroni.com/products/raspberry-pi-3-a-plus)
- [MCP9808 Temp Sensor](https://shop.pimoroni.com/products/mcp9808-high-accuracy-i2c-temperature-sensor-breakout-board)
- [Energenie ENER314 RF Transmitter & Sockets](https://shop.pimoroni.com/products/pi-mote-control-starter-kit-with-2-sockets)
- [Pico Hat Hacker](https://shop.pimoroni.com/products/pico-hat-hacker)
- [GPIO Hammer Header](https://shop.pimoroni.com/products/gpio-hammer-header?variant=35643317962)
- [11mm GPIO Header](https://shop.pimoroni.com/products/2x20-pin-gpio-header-for-raspberry-pi-2-b-a?variant=1132812269)

## Device Setup

1. Download [Balena Etcher](https://www.balena.io/etcher/)
1. Flash [2019-09-26-raspbian-buster-lite](https://downloads.raspberrypi.org/raspbian_lite/images/raspbian_lite-2019-09-30/2019-09-26-raspbian-buster-lite.zip) to SD Card
1. Open `boot` SD partition and copy two files to it:
   - `ssh`: empty file
   - `wpa_supplicant`: see [example](#example-wpa_supplicant) below


(on win box) set up passwordless SSH login

ssh-keygen -t rsa 

cat ~/.ssh/id_rsa.pub | ssh pi@192.168.78.11 'mkdir -p ~/.ssh && cat >> ~/.ssh/authorized_keys'


IF YOU DON'T CALL THE KEY id_rsa IT WON'T BE PICKED UP BY sshd on pi - you'll have to manually add it


on pi 

change password

sudo apt-get update
sudo apt-get dist-upgrade

touch ~/.hushlogin


.NET Core App

Create project on Windows

dotnet publish -r linux-arm

scp -r .\bin\Debug\netcoreapp3.1\linux-arm\publish pi@192.168.78.8:/home/pi/rpi-test

on pi

SUB rpi-test for your folder and exe names

cd ~/rpi-test

sudo chmod +x rpi-test

./rpi-test


sudo raspi-config to enable I2C


SSL:

on Windows: 

Download binary package:
https://github.com/FiloSottile/mkcert/releases

Rename to mkcert, optionally add to path

Create the CA with mkcert -install

mkcert -pkcs12 rpi-stat (or whatever name)

Create site and binding in IIS

Copy root CA cert to Pi:
scp -r C:\Users\me\AppData\Local\mkcert\rootCA.pem pi@192.168.78.11:/home/pi/ca


on pi:

cd ~/

sudo apt install libnss3-tools

wget -O mkcert https://github.com/FiloSottile/mkcert/releases/download/v1.4.1/mkcert-v1.4.1-linux-arm

chmod +x  mkcert
sudo mv mkcert /usr/local/bin

CAROOT=~/ca mkcert -install

Add host to /etc/hosts (sudo nano /etc/hosts, add line for IP)

Test: wget https://rpi-stat


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


