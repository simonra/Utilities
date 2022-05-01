# Install proton version modded by GloriousEggroll for steam, proton-ge-custom, found at https://github.com/GloriousEggroll/proton-ge-custom
# Installation instructions: https://github.com/GloriousEggroll/proton-ge-custom
# Releases: https://github.com/GloriousEggroll/proton-ge-custom/releases

# Main process:
# - Download latest release
# - Extract and put in steams compatibility tools folder.
# - Restart steam.

# Partially inspired by https://github.com/sirkhancision/update-proton-ge/

echo "Installing latest version of GloriousEggroll Proton for Steam."

# Constants
URL_REPO=https://github.com/GloriousEggroll/proton-ge-custom
LATEST_RELEASE_PATH=releases/latest
URL_LATEST_RELEASE=$URL_REPO/$LATEST_RELEASE_PATH
PROTON_INSTALL_DIR=/home/$USER/.steam/root/compatibilitytools.d

# Get latest version
echo "Downloading name/tag of latest release."
url_latest_release_resolved_version=$(curl -Ls -o /dev/null -w %{url_effective} $URL_LATEST_RELEASE)
version=$(echo $url_latest_release_resolved_version | sed -E "s;^.*/;;")
echo "Latest version is $version"

install_dir=$PROTON_INSTALL_DIR/$version

echo "Checking if already installed."
if [ -d $install_dir ]
then
    echo "The latest version, $version, is alredy installed at $PROTON_INSTALL_DIR/."
    echo "To re-install, delete the existing version and re-run the install latest script."
    echo "To delete: rm -rf $install_dir"
    # exit
fi

echo "Downloading package and checksum."
package_name=$version.tar.gz
checksum_name=$version.sha512sum

url_download_package=$URL_REPO/releases/download/$version/$package_name
url_download_checksum=$URL_REPO/releases/download/$version/$checksum_name

package_file=/home/$USER/Downloads/$package_name
checksum_file=/home/$USER/Downloads/$checksum_name

# echo "URLs:"
# echo "  - package: $url_download_package"
# echo "  - checksum: $url_download_checksum"

curl --location $url_download_package --output $package_file
curl --location $url_download_checksum --output $checksum_file

echo "Download finished."

# BEGIN DEBUG
# package_name=GE-Proton7-10.tar.gz
# checksum_name=GE-Proton7-10.sha512sum
# END DEBUG

echo "Checking that package matches checksum."

# Find path to script and dir: https://stackoverflow.com/a/1638397/2890086
# Absolute path to this script, e.g. /home/user/bin/foo.sh
SCRIPT=$(readlink -f "$0")
# Absolute path this script is in, thus /home/user/bin
SCRIPTPATH=$(dirname "$SCRIPT")

if ! $($SCRIPTPATH/../verify-sha-checksum.sh --file $package_file --sum_file $checksum_file --quiet)
then
    echo "Error! Checksum verification failed. Hash of '$package_file' did not match value found in '$checksum_file'."
    exit 1
fi

echo "Extracting package to install directory."

tar --extract --file $package_file --directory $PROTON_INSTALL_DIR

echo "Done installing GloriousEggroll proton version $version for Steam. You have to restart steam before the new version shows up and you can select it."
echo "To restart steam: pkill -TERM steam && sleep 5s && nohup steam </dev/null &>/dev/null &"
