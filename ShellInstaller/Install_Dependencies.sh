echo "Checking .NET Core 2.2"

if [ ! "$(command -v git)" > 0 ]; then
	echo "Installing Donet SDK 2.2"
  	sudo apt-get install dotnet-sdk-2.2
fi


echo "Checking Git"

if [ ! "$(command -v git)" > 0 ]; then
	echo "Installing GIT"
	sudo apt-get install git
fi
