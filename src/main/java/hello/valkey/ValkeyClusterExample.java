package hello.valkey;

import java.time.Duration;
import java.util.HashSet;
import java.util.Set;

import io.valkey.ConnectionPoolConfig;
import io.valkey.HostAndPort;
import io.valkey.JedisCluster;

/**
 * Minimal example that shows how to create a Valkey cluster-aware client using the official
 * {@code org.valkey:valkey-java} dependency. The explicit {@link JedisCluster} type ensures MOVED
 * redirects are handled transparently by the client.
 */
public final class ValkeyClusterExample {

    private static final String DEFAULT_COMMAND_TIMEOUT_MILLIS = "2000";
    private static final String DEFAULT_MAX_ATTEMPTS = "5";

    private ValkeyClusterExample() {
        // utility class
    }

    /**
     * Creates a {@link JedisCluster} instance backed by a connection pool. In a real application,
     * replace the hard-coded host/port entries and optionally inject credentials from your secrets
     * manager or IAM token provider.
     *
     * @return configured {@link JedisCluster} ready to issue commands against the Valkey cluster.
     */
    public static JedisCluster createClusterClient() {
        ConnectionPoolConfig poolConfig = new ConnectionPoolConfig();
        poolConfig.setMaxTotal(Integer.parseInt(
                System.getenv().getOrDefault("VALKEY_POOL_MAX_TOTAL", "32")));
        poolConfig.setMaxIdle(Integer.parseInt(
                System.getenv().getOrDefault("VALKEY_POOL_MAX_IDLE", "16")));
        poolConfig.setMinIdle(Integer.parseInt(
                System.getenv().getOrDefault("VALKEY_POOL_MIN_IDLE", "4")));

        String host = System.getenv().getOrDefault("VALKEY_PRIMARY_ENDPOINT", "127.0.0.1");
        int port = Integer.parseInt(System.getenv().getOrDefault("VALKEY_PRIMARY_PORT", "6379"));
        int timeoutMillis = Integer.parseInt(
                System.getenv().getOrDefault("VALKEY_COMMAND_TIMEOUT_MILLIS", DEFAULT_COMMAND_TIMEOUT_MILLIS));
        int maxAttempts = Integer.parseInt(
                System.getenv().getOrDefault("VALKEY_MAX_ATTEMPTS", DEFAULT_MAX_ATTEMPTS));
        boolean useTls = Boolean.parseBoolean(
                System.getenv().getOrDefault("VALKEY_ENABLE_TLS", "false"));

        Set<HostAndPort> startupNodes = new HashSet<>();
        startupNodes.add(new HostAndPort(host, port));

        return new JedisCluster(
                startupNodes,
                timeoutMillis,
                timeoutMillis,
                maxAttempts,
                System.getenv().getOrDefault("VALKEY_USERNAME", "default"),
                System.getenv("VALKEY_AUTH_TOKEN"), // password or IAM token if authentication is enabled
                poolConfig,
                useTls);
    }
}

