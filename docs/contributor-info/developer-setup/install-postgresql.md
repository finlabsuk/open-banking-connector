# Install PosgreSQL

## macOS

Install [Postgres.app](https://postgresapp.com/).

## Ubuntu Linux

For WSL, do the following (based on https://learn.microsoft.com/en-us/windows/wsl/tutorials/wsl-database#install-postgresql):

```
sudo apt install -y postgresql postgresql-contrib
#sudo systemctl enable postgresql # should work when systemd support enabled?
sudo service postgresql start
```

For WSL, can auto-start by adding this to `/etc/wsl.conf`:
```conf
[boot]
command = "service postgresql start"
```

If not using WSL, install using:

```bash
sudo apt install postgresql
```

You will need to set a password:
```bash
sudo -u postgres psql # then use "\password" to change (set) password
```

## Windows

We suggest to use the WSL2 installation on Windows to avoid a conflict. This can be accessed at `localhost` by tools such as JetBrains DataGrip.