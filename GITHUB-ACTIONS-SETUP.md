# GitHub Actions Quality Gate & Security Setup

This guide explains how to set up the GitHub Actions workflows for quality gates and security scanning with email notifications.

## 🚀 Quick Setup

### 1. Repository Secrets

Go to your GitHub repository → Settings → Secrets and variables → Actions, and add the following secrets:

#### Required Secrets:
- `EMAIL_USERNAME` - Your Gmail address (e.g., `your-email@gmail.com`)
- `EMAIL_PASSWORD` - Your Gmail App Password (not your regular password)
- `NOTIFICATION_EMAIL` - Email address to receive notifications

#### Optional Secrets (for SonarQube):
- `SONAR_HOST_URL` - Your SonarQube server URL (e.g., `https://sonarcloud.io`)
- `SONAR_TOKEN` - Your SonarQube authentication token

### 2. Gmail App Password Setup

1. Go to [Google Account Settings](https://myaccount.google.com/)
2. Navigate to Security → 2-Step Verification
3. Enable 2-Step Verification if not already enabled
4. Go to App passwords
5. Generate a new app password for "Mail"
6. Use this app password as `EMAIL_PASSWORD` secret

### 3. Workflow Files

The following workflow files are already created:

- `.github/workflows/quality-gate.yml` - Main quality gate workflow
- `.github/workflows/security-scan.yml` - Security scanning workflow (builds and scans .NET Alpine image)

## 🔧 Workflow Features

### Quality Gate Workflow

**Triggers:**
- Push to `main` or `develop` branches
- Pull requests to `main` or `develop` branches
- Daily at 2 AM UTC

**Jobs:**
1. **Quality Gate** - Runs tests, coverage, SpotBugs, Checkstyle, PMD
2. **Security Check** - OWASP dependency check and Trivy security scan
3. **SonarQube Analysis** - Code quality analysis (if secrets configured)
4. **Notifications** - Email notifications on success/failure

### Security Scan Workflow (.NET Alpine)

**Triggers:**
- Push to `main` or `develop` branches
- Pull requests to `main` or `develop` branches
- Daily at 3 AM UTC

**Features:**
- Builds `.NET` image with `dotnet-spring-equivalent/Dockerfile.alpine`
- Trivy image scanning with SARIF upload
- GitHub Security tab integration
- Email notifications on security issues

## 📊 Quality Gates

### Coverage Threshold
- **Minimum Coverage:** 80%
- **Enforcement:** Build fails if coverage below threshold

### Security Thresholds
- **OWASP CVSS Score:** 7.0 (High severity)
- **Build fails on:** High and Critical vulnerabilities

### Quality Checks
- ✅ **Tests:** All tests must pass
- ✅ **SpotBugs:** 0 bugs allowed
- ✅ **Checkstyle:** Style violations (warnings only)
- ⚠️ **PMD:** Code quality issues (warnings only)

## 📧 Email Notifications

### Failure Notifications
- **Trigger:** Any job failure
- **Content:** Detailed failure information with links
- **Recipients:** Configured in `NOTIFICATION_EMAIL` secret

### Success Notifications
- **Trigger:** All jobs pass on push to main/develop
- **Content:** Success summary
- **Recipients:** Configured in `NOTIFICATION_EMAIL` secret

### Security Notifications
- **Trigger:** Security vulnerabilities found
- **Content:** Security scan results
- **Recipients:** Configured in `NOTIFICATION_EMAIL` secret

## 🛠️ Customization

### Modify Quality Gates

Edit `.github/workflows/quality-gate.yml`:

```yaml
# Change coverage threshold
COVERAGE_PERCENT=$(echo "$COVERAGE * 100" | bc -l | cut -d'.' -f1)
if [ "$COVERAGE_PERCENT" -lt 90 ]; then  # Change from 80 to 90
  echo "❌ Coverage below threshold: ${COVERAGE_PERCENT}% < 90%"
  exit 1
fi
```

### Modify Security Thresholds

Edit `pom.xml`:

```xml
<plugin>
    <groupId>org.owasp</groupId>
    <artifactId>dependency-check-maven</artifactId>
    <configuration>
        <failBuildOnCVSS>5</failBuildOnCVSS>  <!-- Change from 7 to 5 -->
    </configuration>
</plugin>
```

### Add More Recipients

Edit the workflow files and add multiple email addresses:

```yaml
to: ${{ secrets.NOTIFICATION_EMAIL }}, ${{ secrets.ADMIN_EMAIL }}
```

## 📈 Monitoring

### GitHub Actions Tab
- View workflow runs and results
- Download artifacts (reports, logs)
- Monitor build history

### GitHub Security Tab
- View security scan results
- Track vulnerability trends
- Manage security alerts

### Email Notifications
- Immediate failure notifications
- Daily security scan summaries
- Success confirmations

## 🔍 Troubleshooting

### Common Issues

1. **Email not sending:**
   - Check Gmail App Password is correct
   - Verify 2-Step Verification is enabled
   - Check email secrets are properly set

2. **SonarQube not working:**
   - Verify `SONAR_HOST_URL` and `SONAR_TOKEN` secrets
   - Check SonarQube server accessibility
   - Verify project key matches

3. **Security scan failures:**
   - Review OWASP suppressions in `owasp-suppressions.xml`
   - Check dependency versions
   - Update vulnerable dependencies

### Debug Mode

Enable debug logging by adding to workflow:

```yaml
env:
  ACTIONS_STEP_DEBUG: true
  ACTIONS_RUNNER_DEBUG: true
```

## 📚 Additional Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [OWASP Dependency Check](https://owasp.org/www-project-dependency-check/)
- [Trivy Security Scanner](https://trivy.dev/)
- [SonarQube Quality Gates](https://docs.sonarqube.org/latest/user-guide/quality-gates/)

## 🎯 Next Steps

1. Set up repository secrets
2. Push code to trigger workflows
3. Monitor email notifications
4. Review security reports
5. Customize quality gates as needed

Your quality gate and security scanning setup is now complete! 🎉
