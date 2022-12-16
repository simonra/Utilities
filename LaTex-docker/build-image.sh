# Find path to script and dir: https://stackoverflow.com/a/1638397/2890086
# Absolute path to this script, e.g. /home/user/bin/foo.sh
SCRIPT=$(readlink -f "$0")
# Absolute path this script is in, thus /home/user/bin
SCRIPTPATH=$(dirname "$SCRIPT")

docker build --file $SCRIPTPATH/Dockerfile --tag localhost/latex:latest .
