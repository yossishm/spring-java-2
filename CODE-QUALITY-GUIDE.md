# Code Quality Scanning Guide

This guide explains how to use the comprehensive code quality scanning tools set up for your Spring Java project.

## 🛠️ Available Tools

### 1. **SonarQube** - Comprehensive Code Quality Analysis
- **Purpose**: Complete code quality analysis with security, maintainability, and reliability checks
- **Setup**: `./sonar-setup.sh`
- **Manual**: `mvn sonar:sonar -Dsonar.host.url=http://localhost:9000`

### 2. **JaCoCo** - Code Coverage Analysis
- **Purpose**: Measures test coverage and identifies untested code
- **Command**: `mvn jacoco:report`
- **Report**: `target/site/jacoco/index.html`

### 3. **Checkstyle** - Code Style and Formatting
- **Purpose**: Enforces coding standards and style consistency
- **Command**: `mvn checkstyle:check`
- **Report**: `target/site/checkstyle/index.html`

### 4. **PMD** - Code Quality and Best Practices
- **Purpose**: Detects code quality issues, design problems, and performance issues
- **Command**: `mvn pmd:check`
- **Report**: `target/site/pmd/index.html`

### 5. **SpotBugs** - Static Analysis for Bug Detection
- **Purpose**: Finds potential bugs and security vulnerabilities
- **Command**: `mvn spotbugs:check`
- **Report**: `target/spotbugs/`

### 6. **Pitest** - Mutation Testing
- **Purpose**: Tests the quality of your unit tests by introducing mutations
- **Command**: `mvn org.pitest:pitest-maven:mutationCoverage`
- **Report**: `target/pit-reports/`

## 🚀 Quick Start

### Run All Quality Checks
```bash
./run-code-quality.sh
```

### Run Individual Checks
```bash
# Code coverage
mvn jacoco:report

# Code style
mvn checkstyle:check

# Code quality
mvn pmd:check

# Bug detection
mvn spotbugs:check

# Mutation testing
mvn org.pitest:pitest-maven:mutationCoverage
```

### SonarQube Analysis
```bash
# Setup and run SonarQube (requires Docker)
./sonar-setup.sh

# Or run manually
mvn sonar:sonar -Dsonar.host.url=http://localhost:9000
```

## 📊 Understanding the Reports

### JaCoCo Coverage Report
- **Green**: Covered code
- **Red**: Uncovered code
- **Yellow**: Partially covered code
- **Target**: 80%+ coverage

### Checkstyle Report
- **Violations**: Code style issues
- **Severity**: Error, Warning, Info
- **Categories**: Naming, Imports, Size, Whitespace, etc.

### PMD Report
- **Categories**:
  - **Best Practices**: Coding best practices
  - **Code Style**: Style and formatting
  - **Design**: Design issues
  - **Error Prone**: Potential bugs
  - **Performance**: Performance issues
  - **Security**: Security vulnerabilities

### SpotBugs Report
- **Bug Categories**:
  - **Correctness**: Logic errors
  - **Bad Practice**: Poor coding practices
  - **Performance**: Performance issues
  - **Security**: Security vulnerabilities
  - **Dodgy Code**: Suspicious code patterns

### SonarQube Dashboard
- **Issues**: Code quality problems
- **Measures**: Metrics and coverage
- **Security Hotspots**: Security issues
- **Code Smells**: Maintainability issues

## 🎯 Quality Gates

The project is configured with the following quality gates:

### Coverage Requirements
- **Minimum Coverage**: 80%
- **Enforced by**: JaCoCo plugin

### Code Quality Standards
- **Checkstyle**: Enforces coding standards
- **PMD**: Detects quality issues
- **SpotBugs**: Finds potential bugs
- **SonarQube**: Comprehensive quality analysis

## 🔧 Configuration Files

### `pom.xml`
Contains all Maven plugin configurations for:
- SonarQube scanner
- SpotBugs
- Checkstyle
- PMD
- JaCoCo
- Pitest

### `checkstyle.xml`
Checkstyle configuration with rules for:
- Java coding standards
- Spring-specific rules
- Custom formatting rules

### `sonar-project.properties`
SonarQube project configuration:
- Source paths
- Exclusions
- Coverage reports
- Quality gates

## 📈 Best Practices

### 1. Regular Quality Checks
```bash
# Run before committing
./run-code-quality.sh

# Check specific areas
mvn checkstyle:check
mvn spotbugs:check
```

### 2. CI/CD Integration
Add to your CI pipeline:
```yaml
- name: Code Quality Check
  run: ./run-code-quality.sh
```

### 3. Pre-commit Hooks
Set up Git hooks to run quality checks before commits.

### 4. Team Standards
- Agree on Checkstyle rules
- Set coverage thresholds
- Define quality gate criteria

## 🐛 Troubleshooting

### Common Issues

#### SonarQube Won't Start
```bash
# Check Docker status
docker ps

# Restart SonarQube
docker stop sonarqube
docker rm sonarqube
./sonar-setup.sh
```

#### Checkstyle Failures
```bash
# View detailed report
mvn checkstyle:checkstyle
open target/site/checkstyle/index.html
```

#### Coverage Below Threshold
```bash
# Generate detailed coverage report
mvn jacoco:report
open target/site/jacoco/index.html
```

### Performance Tips
- Run individual tools for faster feedback
- Use `-DskipTests` for style checks only
- Configure exclusions for generated code

## 📚 Additional Resources

- [SonarQube Documentation](https://docs.sonarqube.org/)
- [JaCoCo Documentation](https://www.jacoco.org/jacoco/)
- [Checkstyle Documentation](https://checkstyle.sourceforge.io/)
- [PMD Documentation](https://pmd.github.io/)
- [SpotBugs Documentation](https://spotbugs.github.io/)
- [Pitest Documentation](https://pitest.org/)

## 🎉 Success Metrics

A healthy codebase should have:
- ✅ 80%+ test coverage
- ✅ 0 critical SonarQube issues
- ✅ 0 Checkstyle errors
- ✅ 0 PMD violations
- ✅ 0 SpotBugs high-priority issues
- ✅ 80%+ mutation test score

Happy coding! 🚀
