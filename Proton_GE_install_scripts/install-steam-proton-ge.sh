# Install proton version modded by GloriousEggroll for steam, proton-ge-custom, found at https://github.com/GloriousEggroll/proton-ge-custom
# Installation instructions: https://github.com/GloriousEggroll/proton-ge-custom
# Releases: https://github.com/GloriousEggroll/proton-ge-custom/releases

# Main process:
# - Download latest release
# - Extract and put in steams compatibility tools folder.
# - Restart steam.

# tar --extract --file /home/$USER/Downloads/GE-Proton7-16.tar.gz --directory=/home/$USER/.steam/root/compatibilitytools.d

# Partially inspired by https://github.com/sirkhancision/update-proton-ge/

# Constants
REPO_URL=https://github.com/GloriousEggroll/proton-ge-custom
LATEST_RELEASE_PATH=releases/latest
LATEST_RELEASE_ENDPOINT=$REPO_URL/$LATEST_RELEASE_PATH
PROTON_INSTALL_DIR=/home/$USER/.steam/root/compatibilitytools.d

# Get latest version
echo "Downloading name/tag of latest release."
release_url_with_resolved_version=$(curl -Ls -o /dev/null -w %{url_effective} $LATEST_RELEASE_ENDPOINT)
version=$(echo $release_url_with_resolved_version | sed -E "s;^.*/;;")
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

download_url_package=$REPO_URL/releases/download/$version/$package_name
download_url_checksum=$REPO_URL/releases/download/$version/$checksum_name

package_file=/home/$USER/Downloads/$package_name
checksum_file=/home/$USER/Downloads/$checksum_name

# echo "URLs:"
# echo "  - package: $download_url_package"
# echo "  - checksum: $download_url_checksum"

curl --location $download_url_package --output $package_file
curl --location $download_url_checksum --output $checksum_file

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

echo "Done."
