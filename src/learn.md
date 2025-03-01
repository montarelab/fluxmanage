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


# Connect MongoDB to your application

1. Add the following code to your docker-compose.yml file

```yaml
  mongo:
    image: mongo:latest
    container_name: mongo
    ports:
      - "27017:27017"
    volumes:
      - mongo_data:/data/db
    networks:
      - fluxmanage
```

add volume:
```yaml
volumes:
  mongo_data:
    driver: local
```

and network.

2. Install .NET packages:
   - **MongoDB.Bson**
   - **MongoDB.Driver**

3. Add MongoDB connection in your configuration file:

```json
  "MongoDbConfig": {
    "ConnectionString": "mongodb://mongo:27017",
    "Database": "YourDB",
    "Collection": "YourCollection"
  }
```

4. Register services and configs for your .NET app:
```cs
// Register Guid Serializer
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

// Register all of your event classes
BsonClassMap.RegisterClassMap<ProjectCreatedEvent>();
...
    
// Register your configuration
services.Configure<MongoDbConfig>(Configuration.GetSection("MongoDbConfig"));
```

5. Init your collection inside code:
```cs
// Create MongoDb client
var mongoClient = new MongoClient(config.Value.ConnectionString);

// Get database and collection    
var mongoDatabase = mongoClient.GetDatabase(config.Value.Database);
_eventStoreCollection = mongoDatabase.GetCollection<EventModel>(config.Value.Collection); 
```

# Connect Apache Kafka to your application
1. Add the following code to your docker-compose.yml file to connect Kafka and Zookeeper

```yaml
  zookeeper:
    image: bitnami/zookeeper
    restart: always
    ports:
      - "2181:2181"
    volumes:
      - "zookeeper_data:/bitnami"
    environment:
      - ALLOW_ANONYMOUS_LOGIN=yes
    networks:
      - fluxmanage

  kafka:
     image: bitnami/kafka
     ports:
        - "9092:9092"
        - "29092:29092"
     restart: always
     volumes:
        - "kafka_data:/bitnami"
     environment:
        - KAFKA_ZOOKEEPER_CONNECT=zookeeper:2181
        - ALLOW_PLAINTEXT_LISTENER=yes
        - KAFKA_LISTENERS=INTERNAL://0.0.0.0:9092,EXTERNAL://0.0.0.0:29092
        - KAFKA_ADVERTISED_LISTENERS=INTERNAL://kafka:9092,EXTERNAL://localhost:29092
        - KAFKA_LISTENER_SECURITY_PROTOCOL_MAP=INTERNAL:PLAINTEXT,EXTERNAL:PLAINTEXT
        - KAFKA_INTER_BROKER_LISTENER_NAME=INTERNAL
     depends_on:
        - zookeeper
     networks:
        - fluxmanage
```

here:
- **ALLOW_PLAINTEXT_LISTENER** - Allows to use plain text connection
- **KAFKA_LISTENERS** - Defines where Kafka listens for connections.
- **KAFKA_ADVERTISED_LISTENERS** - Defines what Kafka tells clients to connect to

add volume:
```yaml
volumes:
  zookeeper_data:
    driver: local
  kafka_data:
    driver: local
```

and network.

2. Install **Confluent.Kafka** .NET package 

3. Add Kafka connection in your configuration file:

```json
  "ProducerConfig": {
    "BootstrapServers": "kafka:9092"
  }
```

4. Register config for your .NET app:
```cs
// Register your configuration
builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));
```

5. Init your Kafka producer to produce messages inside code:
```cs
using var producer = new ProducerBuilder<string, string>(_config)
    .SetKeySerializer(Serializers.Utf8)
    .SetValueSerializer(Serializers.Utf8)
    .Build();
```