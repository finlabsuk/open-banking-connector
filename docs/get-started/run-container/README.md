# Run the Open Banking Connector container

Once you have [set up basic configuration](../set-up-basic-configuration/README.md), you are ready to run the Open
Banking Connector Docker container.

Open Banking Connector Docker images are
available [here](https://github.com/finlabsuk/open-banking-connector/pkgs/container/open-banking-connector-web-app). The
Docker images are produced from the Dockerfile in the source code repo and are now multi-architecture Linux images
(support `amd64` and `arm64`). Git tags are used so you can see the exact source code used to create each Docker image.

[Releases](https://github.com/finlabsuk/open-banking-connector/releases) of Open Banking Connector include a link to the
associated image.

You can pull and run the Open Banking Connector container using a command such as:

```bash
# Substitute .env file location and Open Banking Connector version in this command
docker run -dt --env-file "<path to .env file>" -p 50000:8080 --name "open_banking_connector" ghcr.io/finlabsuk/open-banking-connector-web-app:x.y.z # substitute version for x.y.z
```

where a *.env file* is used to supply environment variables (basic configuration).
