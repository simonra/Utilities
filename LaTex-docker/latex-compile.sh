IMAGE=localhost/latex:latest

mkdir -p .latex-temp

exec docker run \
    --rm \
    -i \
    --user="$(id -u):$(id -g)" \
    --net=none \
    --volume "$PWD":/data \
    "$IMAGE" \
    latexmk \
        -cd \
        -aux-directory=.latex-temp \
        -output-directory=.latex-temp \
        -pdf \
        "$@"
