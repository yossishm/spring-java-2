# .NET Code Quality Scanning Guide

This guide explains how to use the comprehensive code quality scanning tools set up for your .NET project.

## 🛠️ Available Tools

### 1. **Built-in .NET Analyzers** - Code Quality Analysis
- **Purpose**: Built-in code analysis with security, maintainability, and reliability checks
- **Command**: `dotnet build --configuration Release /p:RunAnalyzersDuringBuild=true`
- **Report**: Integrated into build output

### 2. **Coverlet** - Code Coverage Analysis
- **Purpose**: Measures test coverage and identifies untested code
- **Command**: `dotnet test --collect:"XPlat Code Coverage"`
- **Report**: `TestResults/CoverageReport/index.html`

### 3. **StyleCop.Analyzers** - Code Style and Formatting
- **Purpose**: Enforces C# coding standards and style consistency
- **Command**: `dotnet format --verify-no-changes`
- **Report**: Integrated into build output

### 4. **SonarAnalyzer.CSharp** - Advanced Code Quality
- **Purpose**: Detects potential bugs, code smells, and security vulnerabilities
- **Command**: `dotnet build --configuration Release`
- **Report**: Integrated into build output

### 5. **Trivy** - Security Vulnerability Scanner
- **Purpose**: Finds security vulnerabilities in dependencies and filesystem
- **Command**: `docker run --rm -v $(pwd):/workspace aquasec/trivy:latest fs /workspace`
- **Report**: Console output and SARIF format

### 6. **dotnet list package --vulnerable** - Dependency Security Audit
- **Purpose**: Checks for known vulnerabilities in NuGet packages
- **Command**: `dotnet list package --vulnerable --include-transitive`
- **Report**: Console output

## 🚀 How to Run All Checks

A convenience script `run-dotnet-code-quality.sh` has been created to execute all configured quality checks in one go.

```bash
./run-dotnet-code-quality.sh
```

This script will:
1. Restore dependencies and build the project
2. Run all unit tests and generate code coverage reports
3. Execute security audit for vulnerable packages
4. Run code analysis with built-in analyzers
5. Check code style with StyleCop
6. Run Trivy security scan
7. Print a summary of the results and paths to detailed reports

## 🔒 Security Testing

To run security checks specifically:

```bash
./test-dotnet-security.sh
```

This script will:
1. Run .NET security audit for vulnerable packages
2. Run Trivy security scan using Docker container
3. Display security findings

## 🐳 SonarQube Integration

To run a full SonarQube analysis (which aggregates results from other tools and provides a comprehensive dashboard):

```bash
# Install SonarQube scanner
dotnet tool install -g dotnet-sonarscanner

# Run analysis (requires SONAR_TOKEN and SONAR_HOST_URL)
dotnet sonarscanner begin /k:"dotnet-spring-equivalent" /d:sonar.host.url=$SONAR_HOST_URL /d:sonar.login=$SONAR_TOKEN
dotnet build --configuration Release
dotnet sonarscanner end /d:sonar.login=$SONAR_TOKEN
```

## ⚙️ Configuration Files

- `SpringJavaEquivalent.csproj`: .NET project configuration, including all analyzer packages and quality properties
- `.github/workflows/dotnet-quality-gate.yml`: GitHub Actions workflow for quality gates
- `.github/workflows/dotnet-security-scan.yml`: GitHub Actions workflow for security scanning

## 📊 Quality Gates

The GitHub Actions workflows include the following quality gates:

### Quality Gate Requirements:
- ✅ Build must succeed
- ✅ All tests must pass
- ✅ Code coverage must be ≥ 80%
- ✅ No high/critical security vulnerabilities
- ✅ Code analysis must pass
- ✅ Code style checks must pass

### Security Gate Requirements:
- ✅ No high/critical vulnerabilities in dependencies
- ✅ No high/critical vulnerabilities in filesystem scan
- ✅ Security audit must pass

## 🚨 Email Notifications

The workflows are configured to send email notifications:
- **On Failure**: Immediate notification with failure details
- **On Success**: Daily summary (for scheduled runs)
- **Security Issues**: Immediate notification for security findings

## 📈 Reports and Artifacts

The workflows generate and upload the following artifacts:
- Test results and coverage reports
- Security scan results (SARIF format)
- Build logs and analysis reports

## 🔧 Local Development

For local development, you can run individual commands:

```bash
# Build with analyzers
dotnet build --configuration Release /p:RunAnalyzersDuringBuild=true

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Check code style
dotnet format --verify-no-changes

# Security audit
dotnet list package --vulnerable --include-transitive

# Install and run Trivy
docker run --rm -v $(pwd):/workspace aquasec/trivy:latest fs /workspace
```

## 📝 Best Practices

1. **Run quality checks before committing**: Use the provided scripts
2. **Fix analyzer warnings**: Address code quality issues promptly
3. **Maintain test coverage**: Keep coverage above 80%
4. **Update dependencies**: Regularly check for security updates
5. **Review security reports**: Address vulnerabilities immediately

---

**Note**: Ensure Docker is running before executing security scans that use Trivy.
