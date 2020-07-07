dotnet publish -r linux-arm
scp -r ./bin/Debug/netcoreapp3.1/linux-arm/publish/* pi@192.168.78.8:/home/pi/rpi-stat
