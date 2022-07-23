# Install proton version modded by GloriousEggroll for wine, wine-ge-custom, found at https://github.com/GloriousEggroll/wine-ge-custom
# Installation instructions: https://github.com/GloriousEggroll/wine-ge-custom#installation
# Releases: https://github.com/GloriousEggroll/wine-ge-custom/releases

# tar -xvf /home/$USER/Downloads/wine-lutris-GE-Proton7-10-x86_64.tar.xz -C /home/$USER/.local/share/lutris/runners/wine

# tar --extract --verbose --file /home/$USER/Downloads/wine-lutris-GE-Proton7-10-x86_64.tar.xz --directory=/home/$USER/.local/share/lutris/runners/wine

# Main process:
# - Download latest release
# - Extract and put in lutrises wine runners folder.
# - Restart lutris.

# tar --extract --file /home/$USER/Downloads/wine-lutris-GE-Proton7-10-x86_64.tar.xz --directory=/home/$USER/.local/share/lutris/runners/wine

echo ":: Installing latest version of GloriousEggroll Proton for Wine for Lutris."

# Constants
URL_REPO=https://github.com/GloriousEggroll/wine-ge-custom
LATEST_RELEASE_PATH=releases/latest
URL_LATEST_RELEASE=$URL_REPO/$LATEST_RELEASE_PATH
PROTON_INSTALL_DIR=/home/$USER/.local/share/lutris/runners/wine

# Get latest version
echo "  Downloading name/tag of latest release."
url_latest_release_resolved_version=$(curl --silent --location --output /dev/null --write-out %{url_effective} --url $URL_LATEST_RELEASE)
version=$(echo $url_latest_release_resolved_version | sed -E "s;^.*/;;")
echo "  Latest version is $version"

install_dir=$PROTON_INSTALL_DIR/lutris-$version-x86_64

echo "  Checking if already installed."
if [ -d $install_dir ]
then
    echo "  The latest version, $version, is alredy installed at $PROTON_INSTALL_DIR/."
    echo "    To re-install, delete the existing version and re-run the install latest script."
    echo "    To delete: rm -rf $install_dir"
    exit
fi

echo "  Downloading package and checksum."
package_name=wine-lutris-$version-x86_64.tar.xz
checksum_name=wine-lutris-$version-x86_64.sha512sum

url_download_package=$URL_REPO/releases/download/$version/$package_name
url_download_checksum=$URL_REPO/releases/download/$version/$checksum_name

package_file=/home/$USER/Downloads/$package_name
checksum_file=/home/$USER/Downloads/$checksum_name

# echo "URLs:"
# echo "  - package: $url_download_package"
# echo "  - checksum: $url_download_checksum"

curl --location --url $url_download_package --output $package_file
curl --location --url $url_download_checksum --output $checksum_file

echo "  Download finished."

# BEGIN DEBUG
# package_name=GE-Proton7-10.tar.gz
# checksum_name=GE-Proton7-10.sha512sum
# END DEBUG

echo "  Checking that package matches checksum."

# Find path to script and dir: https://stackoverflow.com/a/1638397/2890086
# Absolute path to this script, e.g. /home/user/bin/foo.sh
SCRIPT=$(readlink -f "$0")
# Absolute path this script is in, thus /home/user/bin
SCRIPTPATH=$(dirname "$SCRIPT")

if ! $($SCRIPTPATH/../verify-sha-checksum.sh --file $package_file --type sha512 --sum_file $checksum_file --quiet)
then
    echo "Error! Checksum verification failed. Hash of '$package_file' did not match value found in '$checksum_file'."
    exit 1
fi

echo "  Extracting package to install directory."

tar --extract --file $package_file --directory $PROTON_INSTALL_DIR

echo "  Done installing GloriousEggroll proton version $version for Wine for Lutris. It should now be located at $install_dir. You have to restart Lutris before the new version shows up and you can select it."
