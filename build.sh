if [ $# != 1 ]; then
   echo "This script requires one argument, representing the version to build."
   read -n1 -r -p "Press any key to exit."
   exit 1
fi

echo "Cleaning previous build & packages..."
rm -rf build
rm -rf packages

echo "Building just the bot..."

cd src/Bot

dotnet publish -c release -r linux-arm64 --self-contained true -o ../../build/linux-arm64/classic --p:Version="$1"
dotnet publish -c release -r linux-x64 --self-contained true -o ../../build/linux-x64/classic --p:Version="$1"
dotnet publish -c release -r win-x64 --self-contained true -o ../../build/win-x64/classic --p:Version="$1"
dotnet publish -c release -r win-arm64 --self-contained true -o ../../build/win-arm64/classic --p:Version="$1"
dotnet publish -c release -r osx-arm64 --self-contained true -o ../../build/osx-arm64/classic --p:Version="$1"
dotnet publish -c release -r osx-x64 --self-contained true -o ../../build/osx-x64/classic --p:Version="$1"

cd ../../src/UI
echo "Switching to the Avalonia project..."

dotnet publish -c release -r linux-arm64 --self-contained true -o ../../build/linux-arm64/ui --p:Version="$1"
dotnet publish -c release -r linux-x64 --self-contained true -o ../../build/linux-x64/ui --p:Version="$1"
dotnet publish -c release -r win-x64 --self-contained true -o ../../build/win-x64/ui --p:Version="$1"
dotnet publish -c release -r win-arm64 --self-contained true -o ../../build/win-arm64/ui --p:Version="$1"
dotnet publish -c release -r osx-arm64 --self-contained true -o ../../build/osx-arm64/ui --p:Version="$1"
dotnet publish -c release -r osx-x64 --self-contained true -o ../../build/osx-x64/ui --p:Version="$1"

cd ../../
echo "Packaging builds..."

7z a packages/ui-volte-v$1-win-x64.7z build/win-x64/ui
7z a packages/ui-volte-v$1-win-arm64.7z build/win-arm64/ui
tar -czvf packages/ui-volte-v$1-linux-x64.tar.gz build/linux-x64/ui
tar -czvf packages/ui-volte-v$1-linux-arm64.tar.gz build/linux-arm64/ui
tar -czvf packages/ui-volte-v$1-mac-x64.tar.gz build/osx-x64/ui
tar -czvf packages/ui-volte-v$1-mac-arm64.tar.gz build/osx-arm64/ui

7z a packages/volte-v$1-win-x64.7z build/win-x64/classic
7z a packages/volte-v$1-win-arm64.7z build/win-arm64/classic
tar -czvf packages/volte-v$1-linux-x64.tar.gz build/linux-x64/classic
tar -czvf packages/volte-v$1-linux-arm64.tar.gz build/linux-arm64/classic
tar -czvf packages/volte-v$1-mac-x64.tar.gz build/osx-x64/classic
tar -czvf packages/volte-v$1-mac-arm64.tar.gz build/osx-arm64/classic

