# Compile LaTex in docker

## Usage

```sh
Utilities/LaTex-docker/latex-compile.sh path/to/your_file.tex
```

## Prerequisites

Docker must be running.

To check if docker is running:

```sh
sudo systemctl status docker
```

If you've not set docker to autostart:

```sh
sudo systemctl start docker
```

If you want the resulting files to belong to you (and not root), you need to be in the docker user group so that you can run docker commands, or you need to chow the files after they've been made.

To add yourself to the docker users group:

```sh
sudo usermod -aG docker $USER
```

Before first time usage you have to build the docker image.
To do so, simply run the build-image.sh script

```sh
Utilities/LaTex-docker/build-image.sh
```

## Motivation

I occasionally want to make small basic things with latex, but not often enough that I want to maintain a full set of packages in my local install.
This lets me make a stable workin environment for building latex files locally on my machines, whithout much hassle and clutter.
