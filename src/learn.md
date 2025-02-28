# Docker volumes

```docker volume ls```
To check all of the volumes

```docker volume inspect <volume_name>```
Inspect docker volume

```docker volume create <volume_name>```
Create a new volume

```docker volume rm <volume_name>```
Remove a volume

### How to create volumes automatically?

Follow this structure in your docker-compose.yml file

```yaml
volumes:
  zookeeper_data:
    driver: local
  kafka_data:
    driver: local
  mongo_data:
    driver: local
```