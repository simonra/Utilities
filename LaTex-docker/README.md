# Compile LaTex in docker

## Usage

```sh
Utilities/LaTex-docker/latex-compile.sh path/to/your_file.tex
```

Or, if you feel lazy, you could make a shorter alias in your bashrc like for instance:

```bash
alias latex='Utilities/LaTex-docker/latex-compile.sh'
```

which would make usage as simple as

```sh
latex path/to/your_file.tex
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

(And remember to make the files executalbe if you're not me or havent just cloned the project with git (I think it's preserving the flag that makes it executable?):
`cd Utilities/LaTex-docker && chmod +x *.sh`
)

## Motivation

I occasionally want to make small basic things with latex, but not often enough that I want to maintain a full set of packages in my local install, nor advanced enough that I feel a need for a particularly current or updated collection of packages.
This lets me make a stable working environment for building latex files locally on my machines, whithout much hassle or clutter.

## Generalization of use of image

Note that the docker image is a very basic debian image with packages for latex and some basic utilities.
This means that you could easily use it more directly if your needs differ from what I've set up in the `latex-compile.sh` script.

For instace, if you want to use the image to run arbitrary commands in the directory you are currently in, like `pdflatex main.tex`, and don't fancy running a cryptic command like
`docker run -i --rm --user="$(id -u):$(id -g)" -v "$PWD":/data --net=none "localhost/latex:latest" pdflatex main.tex`
all the time, you could instead make a script file with content like below:

```sh
IMAGE=localhost/latex:latest
exec docker run \
    -i \
    --rm \
    --user="$(id -u):$(id -g)" \
    --volume "$PWD":/data \
    --network none \
    "$IMAGE" \
    "$@"
```

This would allow you to instead do things like
`path/to/script_file_that_you_made.sh pdflatex main.tex`
or
`path/to/script_file_that_you_made.sh latexmk -cd -pdf main.tex`
.

(Or even simpler if you alias the paths to the files: `youralias pdflatex main.tex`.)

A limitation of this approach is that the `--rm` ensures that the container is deleted after every run, which might not be desirable if you're iterating on your work and don't want to waste time and resources on docker constantly starting and stopping the container.

In this case you could make a script that starts the container in the background like in the `latex-compile.sh` script:

```sh
IMAGE=localhost/latex:latest
exec docker run \
    -d \
    -i \
    -t \
    --rm \
    --name latex_daemon \
    --user="$(id -u):$(id -g)" \
    --network none \
    --volume $PWD:/data \
    "$IMAGE" \
    /bin/sh \
        -c "sleep infinity"
```

You could then run commands against it like a regular background container
`docker exec -it latex_daemon pdflatex main.tex`
or, if you put that in a script file like below:

```sh
exec docker exec -it latex_daemon "$@"
```

you could do several iterations like:
`path/to/script_file_that_you_made.sh pdflatex main.tex`

before killing the container with
`docker kill latex_daemon`
once you are done.
(The inelegant use of `docker kill containername` instead of `docker stop containername` is due to using `sleep infinity` for keeping it alive in the background.
`sleep infinity` does not seem to respond to `sigterm` in the 10 seconds grace period it gets by default from docker before it is killed, so you will save yourself some waiting by just killing it outright.)
