Manjaro: How to compile and install new kernel version not yet available in the kernel picker:

```sh
sudo pacman -Syu base-devel git --needed
git clone https://gitlab.manjaro.org/packages/core/linux62
cd linux62
updpkgsums
makepkg --syncdeps --install
```

More generally for packages:

```sh
sudo pacman -Syu
git clone <repo>
cd <repo-name>
updpkgsums
makepkg --syncdeps --install
```

# Using more threads

By default as of 2022-12-29, at least in manjaro, package builds are capped to using 2 threads.
To use a bit more of the available power of our modern CPUs, update `/etc/makepkg.conf`, and replace the `-j2` parameter in `MAKEFLAGS` with `-j$(nproc)`, which should automatically determine the number of available threads.
(The full line, which at the time of writing is line 49, might end up looking something like `MAKEFLAGS="-j$(nproc)`.)
