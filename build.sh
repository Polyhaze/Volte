echo "Cleaning last build..."
rm -rf build

echo "Building just the bot..."

cd src/Bot

dotnet publish -c release -r linux-arm64 --self-contained true -o ../../build/linux-arm64/classic

dotnet publish -c release -r linux-x64 --self-contained true -o ../../build/linux-x64/classic

dotnet publish -c release -r win-x64 --self-contained true -o ../../build/win-x64/classic

dotnet publish -c release -r osx-arm64 --self-contained true -o ../../build/osx-arm64/classic

dotnet publish -c release -r osx-x64 --self-contained true -o ../../build/osx-x64/classic

cd ../../src/UI
echo "Switching to the Avalonia project..."

dotnet publish -c release -r linux-arm64 --self-contained true -o ../../build/linux-arm64/ui

dotnet publish -c release -r linux-x64 --self-contained true -o ../../build/linux-x64/ui

dotnet publish -c release -r win-x64 --self-contained true -o ../../build/win-x64/ui

dotnet publish -c release -r osx-arm64 --self-contained true -o ../../build/osx-arm64/ui

dotnet publish -c release -r osx-x64 --self-contained true -o ../../build/osx-x64/ui