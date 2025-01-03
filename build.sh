cd src/Bot

dotnet publish -c release -r linux-arm64 --self-contained true -o ../../build/linux-arm64

dotnet publish -c release -r linux-x64 --self-contained true -o ../../build/linux-x64

dotnet publish -c release -r win-x64 --self-contained true -o ../../build/win-x64