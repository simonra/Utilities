IMAGE=localhost/latex:latest

PATH_TO_MAIN_TEX_FILE="$1"
BASE_NAME_WITHOUT_EXTENSION=$(basename $PATH_TO_MAIN_TEX_FILE .tex)
FINAL_PDF_NAME="${BASE_NAME_WITHOUT_EXTENSION}.pdf"
ORIGINAL_WORKING_DIRECTORY=$(echo "$PWD")
MAIN_TEX_FILE_DIRECTORY=$(dirname $PATH_TO_MAIN_TEX_FILE)

echo "Docker latex maker: Making PDF from $(realpath $PATH_TO_MAIN_TEX_FILE)"

echo "Docker latex maker: Changing working dir to main tex file location"
cd $MAIN_TEX_FILE_DIRECTORY
echo "Docker latex maker: (current working dir is $PWD)"

echo "Docker latex maker: Starting container with latex tooling as background daemon"

# Some notes on what happens below:
# We start the container detatched in the background with the `-d` parameter (it becomes just like a daemon!)
# We tell docker to clean up and delete the container once it's done running with `-rm`.
# We take our current user ID and use that for running inside the docker container with `--user="$(id -u):$(id -g)"`.
# This ensures that the output once the container is done is actually ourselves and not for instance root.
# We disable all networking with `--network none`, because realistically all I need can be done offline.

docker run \
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

echo "Docker latex maker: Making directory \"$PWD/.latex-temp/\" for temp and output files if not already exists"

docker exec -it latex_daemon mkdir -p /data/.latex-temp

echo "Docker latex maker: Compiling PDF for \"${PATH_TO_MAIN_TEX_FILE}\""

docker exec -it latex_daemon \
    latexmk \
        -cd \
        -aux-directory=/data/.latex-temp \
        -output-directory=/data/.latex-temp \
        -pdf \
        "${PATH_TO_MAIN_TEX_FILE}"

# If you instead of using `latexmk` want to for instance just run pdflatex 3 times comment out the lines below
# docker exec -it latex_daemon pdflatex "${PATH_TO_MAIN_TEX_FILE}"
# docker exec -it latex_daemon pdflatex "${PATH_TO_MAIN_TEX_FILE}"
# docker exec -it latex_daemon pdflatex "${PATH_TO_MAIN_TEX_FILE}"

echo "Docker latex maker: Moving produced PDF from temp folder to $PWD"

docker exec -it latex_daemon \
    mv /data/.latex-temp/$FINAL_PDF_NAME /data

echo "Docker latex maker: Stopping background container"

# Just kill the container, because `sleep infinity` does not seem to handle sigterm
docker kill latex_daemon

echo "Docker latex maker: Changing working dir back to original location"
cd $ORIGINAL_WORKING_DIRECTORY
echo "Docker latex maker: (current working dir is $PWD)"

echo "Docker latex maker: Done! Path to produced PDF:"
echo "Docker latex maker: $(realpath $MAIN_TEX_FILE_DIRECTORY/$FINAL_PDF_NAME)"
