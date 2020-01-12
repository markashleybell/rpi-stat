dotnet publish -r linux-arm
scp -r ./bin/Debug/netcoreapp3.1/linux-arm/publish/rpi-stat* pi@192.168.78.11:/home/pi/rpi-stat
