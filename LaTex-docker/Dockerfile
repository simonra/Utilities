FROM debian:latest
RUN apt-get update \
    && apt-get install -y \
        texlive-full \
        python3-pygments \
        gnuplot \
        make \
        git \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /data
VOLUME ["/data"]
