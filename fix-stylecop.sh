#!/bin/bash

# Script to fix common StyleCop issues in test files

cd /Users/yshmulev/dev/spring-java-2/dotnet-spring-equivalent/SpringJavaEquivalent.Tests

# List of test files to fix
files=(
    "AuthorizationTests.cs"
    "SimpleWorkingTests.cs" 
    "VulnerableJwtControllerTests.cs"
    "JwtServiceTests.cs"
    "ApplicationControllerTests.cs"
    "PermissionHandlerTests.cs"
    "TokenControllerTests.cs"
    "MetricsControllerTests.cs"
    "EnhancedAuthTestControllerAdvancedTests.cs"
    "AuthorizationRequirementTests.cs"
)

for file in "${files[@]}"; do
    if [ -f "$file" ]; then
        echo "Fixing $file..."
        
        # Fix using directives - move them inside namespace
        if grep -q "^using " "$file"; then
            # Create a temporary file
            temp_file=$(mktemp)
            
            # Extract namespace line
            namespace_line=$(grep "^namespace " "$file" | head -1)
            
            # Extract using directives
            using_lines=$(grep "^using " "$file")
            
            # Extract everything after the last using directive
            after_using=$(sed -n '/^using /,$p' "$file" | sed '1,/^namespace /d')
            
            # Reconstruct the file
            echo "$namespace_line" > "$temp_file"
            echo "{" >> "$temp_file"
            echo "$using_lines" | sed 's/^/    /' >> "$temp_file"
            echo "" >> "$temp_file"
            echo "$after_using" | sed 's/^    /        /' >> "$temp_file"
            
            # Replace original file
            mv "$temp_file" "$file"
        fi
        
        # Fix field names - remove underscore prefix
        sed -i '' 's/private readonly \([A-Za-z][A-Za-z0-9]*\) _\([a-z][A-Za-z0-9]*\)/private readonly \1 \2/g' "$file"
        sed -i '' 's/private \([A-Za-z][A-Za-z0-9]*\) _\([a-z][A-Za-z0-9]*\)/private \1 \2/g' "$file"
        
        # Fix field references - add this. prefix
        sed -i '' 's/_\([a-z][A-Za-z0-9]*\)/this.\1/g' "$file"
        
        # Fix trailing whitespace
        sed -i '' 's/[[:space:]]*$//' "$file"
        
        # Fix trailing commas in multi-line initializers
        sed -i '' 's/},$/},/g' "$file"
        sed -i '' 's/},$/},/g' "$file"
        
        # Fix empty strings
        sed -i '' 's/""/string.Empty/g' "$file"
        
        echo "Fixed $file"
    fi
done

echo "StyleCop fixes completed!"
