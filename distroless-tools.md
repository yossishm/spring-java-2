# Distroless Tools & Techniques for Production

## 1. Debugging Distroless Containers

### Debug Image with Shell
```bash
# Create debug version with shell
FROM gcr.io/distroless/java21-debian12:debug
# or
FROM gcr.io/distroless/java21-debian12:nonroot-debug

# Build debug version
docker build -f Dockerfile.distroless.debug -t myapp:distroless-debug .

# Run with shell access
docker run -it --entrypoint=/busybox/sh myapp:distroless-debug
```

### Multi-stage Debug Dockerfile
```dockerfile
# Dockerfile.distroless.debug
FROM eclipse-temurin:21.0.8_9-jre-alpine-3.22 AS builder
WORKDIR /build
COPY target/gs-spring-boot-docker-0.2.0.jar /build/app.jar

# Debug stage with shell
FROM gcr.io/distroless/java21-debian12:debug

COPY --from=builder /build/app.jar /app/app.jar
EXPOSE 8080

# Default to debug shell, can override with CMD
ENTRYPOINT ["/busybox/sh"]
CMD ["-c", "java -jar /app/app.jar"]
```

## 2. Health Checks for Distroless

### Using wget (available in debug image)
```dockerfile
# Dockerfile.distroless.health
FROM gcr.io/distroless/java21-debian12:debug

COPY --from=builder /build/app.jar /app/app.jar
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/actuator/health || exit 1

ENTRYPOINT ["java", "-jar", "/app/app.jar"]
```

### Using netcat (nc)
```dockerfile
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD nc -z localhost 8080 || exit 1
```

## 3. Logging & Monitoring

### Structured Logging
```dockerfile
# Dockerfile.distroless.logging
FROM gcr.io/distroless/java21-debian12:nonroot

COPY --from=builder /build/app.jar /app/app.jar
EXPOSE 8080

# Use structured logging
ENTRYPOINT ["java", \
  "-Dlogging.pattern.console=%d{yyyy-MM-dd HH:mm:ss} [%thread] %-5level %logger{36} - %msg%n", \
  "-jar", "/app/app.jar"]
```

### Prometheus Metrics
```dockerfile
# Add Prometheus endpoint
ENV MANAGEMENT_ENDPOINTS_WEB_EXPOSURE_INCLUDE=health,info,metrics,prometheus
ENV MANAGEMENT_ENDPOINT_METRICS_ENABLED=true
```

## 4. Security Hardening

### Security Context
```yaml
# kubernetes-security.yaml
apiVersion: v1
kind: Pod
metadata:
  name: myapp-secure
spec:
  securityContext:
    runAsNonRoot: true
    runAsUser: 65532  # nonroot user in distroless
    runAsGroup: 65532
    fsGroup: 65532
  containers:
  - name: myapp
    image: myapp:distroless
    securityContext:
      allowPrivilegeEscalation: false
      readOnlyRootFilesystem: true
      capabilities:
        drop:
        - ALL
    volumeMounts:
    - name: tmp
      mountPath: /tmp
    - name: logs
      mountPath: /app/logs
  volumes:
  - name: tmp
    emptyDir: {}
  - name: logs
    persistentVolumeClaim:
      claimName: logs-pvc
```

### Read-only Root Filesystem
```dockerfile
# Dockerfile.distroless.readonly
FROM gcr.io/distroless/java21-debian12:nonroot

COPY --from=builder /build/app.jar /app/app.jar

# Create writable directories
RUN mkdir -p /tmp /app/logs && chown -R 65532:65532 /tmp /app/logs

EXPOSE 8080

ENTRYPOINT ["java", "-jar", "/app/app.jar"]
```

## 5. Multi-architecture Support

### Build for Multiple Platforms
```bash
# Build for multiple architectures
docker buildx build --platform linux/amd64,linux/arm64 \
  -f Dockerfile.distroless \
  -t myapp:distroless-multiarch \
  --push .
```

### Platform-specific Dockerfile
```dockerfile
# Dockerfile.distroless.multiarch
FROM --platform=$BUILDPLATFORM eclipse-temurin:21.0.8_9-jre-alpine-3.22 AS builder
WORKDIR /build
COPY target/gs-spring-boot-docker-0.2.0.jar /build/app.jar

FROM --platform=$TARGETPLATFORM gcr.io/distroless/java21-debian12:nonroot
COPY --from=builder /build/app.jar /app/app.jar
EXPOSE 8080
ENTRYPOINT ["java", "-jar", "/app/app.jar"]
```

## 6. Production Deployment Tools

### Docker Compose with Distroless
```yaml
# docker-compose.distroless.yml
version: '3.8'
services:
  app:
    build:
      context: .
      dockerfile: Dockerfile.distroless
    ports:
      - "8080:8080"
    environment:
      - SPRING_PROFILES_ACTIVE=prod
      - JAVA_OPTS=-Xmx512m -Xms256m
    healthcheck:
      test: ["CMD", "wget", "--no-verbose", "--tries=1", "--spider", "http://localhost:8080/actuator/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
    restart: unless-stopped
    security_opt:
      - no-new-privileges:true
    read_only: true
    tmpfs:
      - /tmp
    volumes:
      - logs:/app/logs
      - /etc/localtime:/etc/localtime:ro

volumes:
  logs:
```

### Kubernetes Deployment
```yaml
# k8s-distroless.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: myapp-distroless
spec:
  replicas: 3
  selector:
    matchLabels:
      app: myapp
  template:
    metadata:
      labels:
        app: myapp
    spec:
      securityContext:
        runAsNonRoot: true
        runAsUser: 65532
        fsGroup: 65532
      containers:
      - name: myapp
        image: myapp:distroless
        ports:
        - containerPort: 8080
        env:
        - name: SPRING_PROFILES_ACTIVE
          value: "prod"
        - name: JAVA_OPTS
          value: "-Xmx512m -Xms256m"
        resources:
          requests:
            memory: "512Mi"
            cpu: "250m"
          limits:
            memory: "1Gi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /actuator/health
            port: 8080
          initialDelaySeconds: 60
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /actuator/health
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 5
        securityContext:
          allowPrivilegeEscalation: false
          readOnlyRootFilesystem: true
          capabilities:
            drop:
            - ALL
        volumeMounts:
        - name: logs
          mountPath: /app/logs
        - name: tmp
          mountPath: /tmp
      volumes:
      - name: logs
        persistentVolumeClaim:
          claimName: myapp-logs
      - name: tmp
        emptyDir: {}
```

## 7. Monitoring & Observability

### Prometheus Configuration
```yaml
# prometheus-config.yml
scrape_configs:
  - job_name: 'myapp'
    static_configs:
      - targets: ['myapp:8080']
    metrics_path: '/actuator/prometheus'
    scrape_interval: 15s
```

### Grafana Dashboard
```json
{
  "dashboard": {
    "title": "Spring Boot Distroless App",
    "panels": [
      {
        "title": "JVM Memory",
        "targets": [
          {
            "expr": "jvm_memory_used_bytes{application=\"myapp\"}"
          }
        ]
      },
      {
        "title": "HTTP Requests",
        "targets": [
          {
            "expr": "http_server_requests_seconds_count{application=\"myapp\"}"
          }
        ]
      }
    ]
  }
}
```

## 8. CI/CD Pipeline

### GitHub Actions with Distroless
```yaml
# .github/workflows/distroless.yml
name: Build and Deploy Distroless
on:
  push:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    
    - name: Set up JDK 21
      uses: actions/setup-java@v3
      with:
        java-version: '21'
        distribution: 'temurin'
    
    - name: Build with Maven
      run: ./mvnw clean package -DskipTests
    
    - name: Build Distroless Image
      run: |
        docker build -f Dockerfile.distroless -t myapp:distroless .
        docker tag myapp:distroless ${{ secrets.REGISTRY }}/myapp:distroless-${{ github.sha }}
    
    - name: Security Scan
      run: |
        docker scout quickview myapp:distroless
        docker scout cves myapp:distroless
    
    - name: Push to Registry
      run: |
        echo ${{ secrets.REGISTRY_PASSWORD }} | docker login ${{ secrets.REGISTRY }} -u ${{ secrets.REGISTRY_USERNAME }} --password-stdin
        docker push ${{ secrets.REGISTRY }}/myapp:distroless-${{ github.sha }}
```

## 9. Troubleshooting Commands

### Debug Running Container
```bash
# Check if container is running
docker ps

# View logs
docker logs myapp-container

# Check container stats
docker stats myapp-container

# Inspect container
docker inspect myapp-container

# Copy files from container (if needed)
docker cp myapp-container:/app/app.jar ./app.jar
```

### Debug with Debug Image
```bash
# Run debug version
docker run -it --entrypoint=/busybox/sh myapp:distroless-debug

# Inside debug container
ls -la /app/
ps aux
netstat -tlnp
wget -O- http://localhost:8080/actuator/health
```

## 10. Performance Optimization

### JVM Tuning for Distroless
```dockerfile
# Dockerfile.distroless.optimized
FROM gcr.io/distroless/java21-debian12:nonroot

COPY --from=builder /build/app.jar /app/app.jar
EXPOSE 8080

ENTRYPOINT ["java", \
  "-XX:+UseContainerSupport", \
  "-XX:MaxRAMPercentage=75.0", \
  "-XX:+UseG1GC", \
  "-XX:MaxGCPauseMillis=200", \
  "-Djava.security.egd=file:/dev/./urandom", \
  "-Dfile.encoding=UTF-8", \
  "-jar", "/app/app.jar"]
```

### Resource Limits
```yaml
# k8s-resources.yaml
resources:
  requests:
    memory: "512Mi"
    cpu: "250m"
  limits:
    memory: "1Gi"
    cpu: "500m"
  ephemeral-storage: "100Mi"
```


