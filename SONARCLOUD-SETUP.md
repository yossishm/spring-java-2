# SonarCloud Setup Guide for .NET Project

## 🚀 Quick Setup (3 minutes)

### 1. Create SonarCloud Account
1. Go to [sonarcloud.io](https://sonarcloud.io)
2. Sign in with GitHub account (user: `yossishm`)
3. Import organization: `yossishm`

### 2. Import Repository
1. Click "Import an organization" → "From GitHub"
2. Select your repository: `spring-java-2`
3. Choose "Public" visibility (free)

### 3. Generate Token
1. Go to [sonarcloud.io/account/security](https://sonarcloud.io/account/security)
2. Generate token with name: `dotnet-spring-equivalent`
3. Copy the token (save it!)

### 4. Add to GitHub Secrets
1. Go to your GitHub repo → Settings → Secrets and variables → Actions
2. Add new repository secret:
   - Name: `SONAR_TOKEN`
   - Value: `squ_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx` (your token)

### 5. Push Changes
```bash
git add .
git commit -m "Add SonarCloud integration for .NET project"
git push origin main
```

### 6. Check Results
- Go to [sonarcloud.io](https://sonarcloud.io) → Your org (`yossishm`) → `spring-java-2-dotnet`
- View coverage metrics (should show ~78% coverage!)
- Quality gate status
- Code smells, bugs, vulnerabilities

## 📊 Expected Results

After setup, SonarCloud will show:
- ✅ **Coverage**: ~78.8% (716/909 lines)
- ✅ **Quality Gate**: Passed
- ✅ **Bugs**: Minimal
- ✅ **Code Smells**: Analyzed
- ✅ **Security**: Scanned

## 🔧 Configuration Details

### Project Key: `spring-java-2-dotnet`
### Organization: `yossishm`

### Coverage Settings:
- Format: SonarQube generic XML (converted from Cobertura)
- Path: `./TestResults/SonarQube.xml`
- Exclusions: `**/bin/**,**/obj/**,**/TestResults/**`

### Quality Profiles:
- C#: Sonar way (default)
- Security: Enabled
- Coverage threshold: 80%

## 🆘 Troubleshooting

### Coverage Not Showing?
1. Check GitHub Actions logs for "Generate SonarQube coverage report" step
2. Verify `SONAR_TOKEN` secret is set correctly
3. Check if `VisualStudio.xml` file is generated in `./TestResults/`

### Quality Gate Failing?
1. Check test failures in GitHub Actions
2. Verify coverage meets 80% threshold
3. Review code analysis warnings

### Permission Issues?
1. Ensure repository is public OR you have SonarCloud paid plan
2. Check GitHub integration permissions
3. Verify token has "Execute Analysis" permission

## 💰 Pricing

- **Free**: Public repos, unlimited analysis
- **Paid**: $10/month for private repos (100 hours included)
- **Enterprise**: Advanced features, custom rules

Your setup will work perfectly with the free tier for public repositories!

---

# Java SonarCloud Integration

## 🚀 Java Setup (Additional)

The Java project uses the same SonarCloud organization but sends analysis to the main `spring-java-2` project.

### Java Configuration:
- **Project Key**: `spring-java-2` (same as main project)
- **Organization**: `yossishm`
- **Coverage Format**: JaCoCo XML (`target/site/jacoco/jacoco.xml`)
- **Test Reports**: Surefire (`target/surefire-reports`)
- **Exclusions**: `**/target/**,**/build/**,**/*.class,**/node_modules/**,**/logs/**,**/bin/**,**/obj/**,**/TestResults/**`

### Java Quality Tools:
- **SpotBugs**: Static analysis for bugs
- **Checkstyle**: Code style checking
- **PMD**: Code quality analysis
- **OWASP**: Dependency vulnerability scanning
- **Trivy**: Security scanning

### Java Workflow:
- Triggered on changes to `src/**` or `pom.xml`
- Runs quality gate, security checks, and SonarCloud analysis
- Uses existing `SONAR_TOKEN` secret
