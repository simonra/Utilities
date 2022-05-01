# Install proton version modded by GloriousEggroll for wine, wine-ge-custom, found at https://github.com/GloriousEggroll/wine-ge-custom
# Installation instructions: https://github.com/GloriousEggroll/wine-ge-custom#installation
# Releases: https://github.com/GloriousEggroll/wine-ge-custom/releases

# tar -xvf /home/$USER/Downloads/wine-lutris-GE-Proton7-10-x86_64.tar.xz -C /home/$USER/.local/share/lutris/runners/wine

# tar --extract --verbose --file /home/$USER/Downloads/wine-lutris-GE-Proton7-10-x86_64.tar.xz --directory=/home/$USER/.local/share/lutris/runners/wine

# Main process:
# - Download latest release
# - Extract and put in lutrises wine runners folder.
# - Restart lutris.

tar --extract --file /home/$USER/Downloads/wine-lutris-GE-Proton7-10-x86_64.tar.xz --directory=/home/$USER/.local/share/lutris/runners/wine
