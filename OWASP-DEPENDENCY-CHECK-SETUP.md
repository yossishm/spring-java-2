# OWASP Dependency Check Setup Guide

## Issue: 403 Forbidden from NVD

The OWASP Dependency Check plugin is getting a 403 Forbidden error when trying to download NVD (National Vulnerability Database) data. This is because NVD now requires API keys for access (as of 2023).

## Solution: Get a Free NVD API Key

### Step 1: Request an API Key

1. Go to: https://nvd.nist.gov/developers/request-an-api-key
2. Fill out the form with:
   - Your email address
   - Organization name
   - Purpose: "Automated security scanning for CI/CD pipeline"
3. Submit the form
4. Check your email for the API key (usually arrives within minutes)

### Step 2: Configure the API Key

**Option A: Environment Variable (Recommended for CI/CD)**

```bash
export DC_NVD_API_KEY=your-api-key-here
```

**Option B: Maven Settings (for local development)**

Add to `~/.m2/settings.xml`:
```xml
<profiles>
  <profile>
    <id>owasp</id>
    <properties>
      <DC_NVD_API_KEY>your-api-key-here</DC_NVD_API_KEY>
    </properties>
  </profile>
</profiles>
```

Then activate: `mvn -Powasp dependency-check:check`

**Option C: GitHub Secrets (for GitHub Actions)**

1. Go to your repository → Settings → Secrets and variables → Actions
2. Add new secret:
   - Name: `DC_NVD_API_KEY`
   - Value: `your-api-key-here`

Then in your workflow:
```yaml
env:
  DC_NVD_API_KEY: ${{ secrets.DC_NVD_API_KEY }}
```

### Step 3: Verify It Works

```bash
mvn dependency-check:check
```

You should see:
- ✅ No 403 errors
- ✅ "Successfully downloaded NVD data"
- ✅ Vulnerability analysis completes

## Current Configuration

The plugin is configured to:
- ✅ Use API key if available (`DC_NVD_API_KEY` environment variable)
- ✅ Retry up to 5 times with 5-second delays
- ✅ Use cached data (valid for 30 days) if API fails
- ✅ Not fail the build on update errors (uses cached data)

## Without API Key

If you don't have an API key, the plugin will:
- ⚠️ Show warnings about unable to update
- ✅ Use cached NVD data (if available)
- ✅ Still perform vulnerability scanning with cached data
- ✅ Build will succeed (but may miss recent vulnerabilities)

## Rate Limits

- **Without API key**: 5 requests per 30 seconds (very restrictive)
- **With API key**: 50 requests per 30 seconds (much better)

## Troubleshooting

### Still Getting 403?

1. Verify API key is set: `echo $DC_NVD_API_KEY`
2. Check API key is valid: Visit https://nvd.nist.gov/developers/request-an-api-key
3. Try regenerating the API key
4. Check network/firewall isn't blocking NVD

### Build Still Failing?

The current configuration has `failOnError>false</failOnError>` which means it won't fail on update errors. If you want it to fail when vulnerabilities are found (but not on update errors), the configuration is already set to `failBuildOnCVSS>7</failBuildOnCVSS>`.

## References

- NVD API Key Request: https://nvd.nist.gov/developers/request-an-api-key
- OWASP Dependency Check Docs: https://jeremylong.github.io/DependencyCheck/
- NVD API Documentation: https://nvd.nist.gov/developers/vulnerabilities

