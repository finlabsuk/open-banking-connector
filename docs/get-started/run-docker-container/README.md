# Run Docker container

Open Banking Connector Docker images (including the latest) are available [here](https://github.com/finlabsuk/open-banking-connector/pkgs/container/open-banking-connector-web-app). The Docker images are produced from the Dockerfile in the source code repo. Git tags are used so you can see the exact source code used to create each Docker image.

You can pull and run the Open Banking Connector Docker image using a command such as:
```bash
# Substitute env file location and image version in this command
docker run -dt --env-file "<path>/docker.env" -p 50000:80 --name "open_banking_connector" ghcr.io/finlabsuk/open-banking-connector-web-app:x.y.z # substitute version for x.y.z
```
where a *.env file* is used to supply environment variables.
