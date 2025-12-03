# Native Image Build Guide - GraalVM OSS Community Edition

This guide explains how to build and run the Spring Boot application as a native executable using GraalVM Community Edition (OSS).

## Overview

The native image build compiles the Spring Boot application into a standalone executable that:
- Starts much faster (milliseconds vs seconds)
- Uses less memory
- Has a smaller footprint
- Runs without a JVM

## Prerequisites

### Option 1: Local GraalVM Installation

1. **Install GraalVM Community Edition**
   - Download from: https://www.graalvm.org/downloads/
   - Or use SDKMAN:
     ```bash
     sdk install java 21.0.2-graal
     sdk use java 21.0.2-graal
     ```
   - Or use Homebrew (macOS):
     ```bash
     brew install --cask graalvm-jdk
     ```

2. **Install Native Image**
   ```bash
   gu install native-image
   ```

3. **Verify Installation**
   ```bash
   java -version
   native-image --version
   ```

### Option 2: Docker/Podman (No Local Installation Required)

Docker or Podman is the recommended approach as it doesn't require local GraalVM installation. The build script automatically detects and uses whichever is available.

## Building Native Image

### Method 1: Local Build (with GraalVM installed)

```bash
./build-native.sh
```

This will:
- Clean previous builds
- Compile the native executable
- Output: `target/gs-spring-boot-docker`

### Method 2: Container Build (Recommended)

```bash
./build-native-docker.sh
```

This will:
- Automatically detect and use Docker or Podman (whichever is available)
- Build the native image using containers
- Create a container image with Alpine Linux runtime
- No local GraalVM installation needed

**Note**: The script prefers Podman if both are installed, but works with either.

### Method 3: Manual Maven Build

```bash
./mvnw clean package -Pnative -DskipTests
```

## Running the Native Executable

### Local Execution

```bash
./target/gs-spring-boot-docker
```

### Container Execution

**Using Docker:**
```bash
# Build the image
docker build -f Dockerfile.native -t spring-boot-native:latest .

# Run the container
docker run -p 8080:8080 spring-boot-native:latest
```

**Using Podman:**
```bash
# Build the image
podman build -f Dockerfile.native -t spring-boot-native:latest .

# Run the container
podman run -p 8080:8080 spring-boot-native:latest
```

**Note**: The `build-native-docker.sh` script automatically detects and uses the available container runtime.

## Configuration

### Native Image Configuration Files

The project includes native image configuration files in `src/main/resources/META-INF/native-image/`:

- **native-image.properties**: Main configuration with build arguments
- **reflect-config.json**: Reflection configuration for classes that need reflection
- **resource-config.json**: Resource inclusion patterns
- **jni-config.json**: JNI configuration (currently empty)
- **proxy-config.json**: Dynamic proxy configuration (currently empty)

### Build Arguments

Key build arguments configured in `pom.xml`:

- `--verbose`: Detailed build output
- `-H:+ReportExceptionStackTraces`: Better error reporting
- `-H:+AllowVMInspection`: Allow VM inspection tools
- `-H:IncludeResources=.*`: Include all resources
- `--enable-url-protocols=http,https`: Enable HTTP/HTTPS protocols
- `--enable-all-security-services`: Enable security services
- `-H:+StaticExecutableWithDynamicLibC`: Static executable with dynamic libc

### Runtime Image

The Dockerfile uses **Alpine Linux 3.22** for the runtime image:
- Minimal footprint (~5MB base)
- Security-focused
- Uses `gcompat` for glibc compatibility (GraalVM native images use glibc)
- Non-root user execution
- Health check included

## Troubleshooting

### Build Failures

1. **Missing Reflection Configuration**
   - If you get reflection errors, add classes to `reflect-config.json`
   - Spring Boot usually handles this automatically, but custom classes may need manual configuration

2. **Resource Not Found**
   - Add resource patterns to `resource-config.json`
   - Check that resources are included in the build

3. **Alpine Compatibility Issues**
   - If the native executable doesn't run on Alpine, ensure `gcompat` is installed
   - Alternatively, use a fully static build (remove `-H:+StaticExecutableWithDynamicLibC`)

### Runtime Issues

1. **ClassNotFoundException**
   - Add missing classes to reflection configuration
   - Check that all dependencies are compatible with native image

2. **Performance Issues**
   - Native images may have slower warm-up for some operations
   - Consider using `-H:+AllowVMInspection` for profiling

## Performance Characteristics

### Startup Time
- **JVM**: ~2-5 seconds
- **Native**: ~50-200 milliseconds

### Memory Usage
- **JVM**: ~150-300 MB
- **Native**: ~50-100 MB

### Image Size
- **JAR**: ~50-100 MB
- **Native Executable**: ~80-150 MB
- **Docker Image (Alpine)**: ~100-200 MB

## Limitations

1. **Build Time**: Native image compilation takes significantly longer (5-15 minutes)
2. **Reflection**: Requires explicit configuration for reflective access
3. **Dynamic Features**: Some dynamic features may not work (e.g., dynamic class loading)
4. **Debugging**: Debugging native images is more complex than JVM debugging

## Dependencies Compatibility

The following dependencies are configured for native image:

- ✅ Spring Boot 3.5.8 (fully supported)
- ✅ Spring Security (fully supported)
- ✅ MongoDB Driver (reflection configured)
- ✅ Valkey/Redis Client (reflection configured)
- ✅ JWT Libraries (reflection configured)
- ✅ OpenTelemetry (may need additional configuration)
- ✅ SpringDoc OpenAPI (reflection configured)

## Additional Resources

- [GraalVM Native Image Documentation](https://www.graalvm.org/latest/reference-manual/native-image/)
- [Spring Native Image Documentation](https://docs.spring.io/spring-boot/reference/native-image/index.html)
- [GraalVM Community Edition](https://www.graalvm.org/downloads/)

## Security Considerations

- ✅ Non-root user execution
- ✅ Minimal Alpine base image
- ✅ No unnecessary packages
- ✅ Security services enabled
- ✅ Certificate validation enabled

