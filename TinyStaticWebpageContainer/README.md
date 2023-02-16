A very tiny image to host some static content!
Based on [BusyBox's](https://busybox.net/) implementation of [httpd](https://httpd.apache.org/).
Inspired by [Florin Lipan's attempt of making "The smallest Docker image to serve static websites"](https://lipanski.com/posts/smallest-docker-image-static-website) ([repo](https://github.com/lipanski/docker-static-website)).

Main changes are adding a build layer that gzips all content for you, and building based on all assets beyond `httpd.conf` being in the `WebsiteFiles` directory.

To build:

```bash
docker build \
  --file Busybox.Httpd.Dockerfile \
  -t static-website \
  .
```

(Build context is directory with dockerfile.)

To run:

```bash
docker run \
  -it \
  --name static-website-ctnr \
  --rm \
  -p 8081:8080 \
  --init \
  static-website
```

Notes on running to save me a quick refresher should I again forget in 5 months:
- `-it`: Make it interactive and allocate tty so that we can connect
- `--name`: Give the container a name, so that it becomes easier to interact with (now you can for instance do `docker kill static-website-ctnr` instead of fishing out the ID from `docker ps`)
- `--rm`: Automatically remove the container when it exits. Because this is a stateless static site, why would you keep an extra copy in the form of a container around if you've got the image?
- `-p 8081:8080`: Bind port 8081 on your host to 8080 in the container, so that you can brows the site on localhost:8081
- `--init`: Needed for the process to properly handle signals such as `ctrl+c`. Doesn't really make sense to me yet, but it doesn't work without. Probably best to just drop if you intend to host it?
