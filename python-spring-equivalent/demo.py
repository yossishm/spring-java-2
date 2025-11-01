#!/usr/bin/env python3
"""
Demo script for Python Spring Equivalent application.
This script demonstrates the key features and endpoints.
"""

import asyncio
import json
import time
from typing import Dict, Any

import httpx


class PythonSpringEquivalentDemo:
    """Demo class for Python Spring Equivalent application."""
    
    def __init__(self, base_url: str = "http://localhost:8081"):
        self.base_url = base_url
        self.client = httpx.AsyncClient(timeout=30.0)
        self.auth_token = None
    
    async def close(self):
        """Close the HTTP client."""
        await self.client.aclose()
    
    async def create_token(self, username: str = "admin", password: str = "password") -> str:
        """Create a JWT token."""
        print(f"🔐 Creating JWT token for user: {username}")
        
        token_request = {
            "username": username,
            "password": password,
            "permissions": ["CACHE_READ", "CACHE_WRITE", "CACHE_DELETE"],
            "roles": ["ADMIN"],
            "auth_level": "AAL2",
            "identity_provider": "enterprise"
        }
        
        response = await self.client.post(
            f"{self.base_url}/api/jwt/create",
            json=token_request
        )
        
        if response.status_code == 200:
            data = response.json()
            self.auth_token = data["access_token"]
            print(f"✅ Token created successfully")
            print(f"   Token type: {data['token_type']}")
            print(f"   Expires in: {data['expires_in']} seconds")
            return self.auth_token
        else:
            print(f"❌ Failed to create token: {response.status_code}")
            print(f"   Response: {response.text}")
            return None
    
    async def test_home_endpoint(self):
        """Test the home endpoint."""
        print("\n🏠 Testing home endpoint...")
        
        response = await self.client.get(f"{self.base_url}/")
        
        if response.status_code == 200:
            data = response.json()
            print(f"✅ Home endpoint working")
            print(f"   Message: {data['message']}")
            print(f"   Version: {data['version']}")
            print(f"   Service: {data['service']}")
        else:
            print(f"❌ Home endpoint failed: {response.status_code}")
    
    async def test_cache_operations(self):
        """Test cache operations."""
        print("\n💾 Testing cache operations...")
        
        if not self.auth_token:
            print("❌ No auth token available")
            return
        
        headers = {"Authorization": f"Bearer {self.auth_token}"}
        
        # Test PUT operation
        print("   Testing PUT operation...")
        response = await self.client.put(
            f"{self.base_url}/api/v1/cacheServices/putObject?id=demo123",
            headers=headers
        )
        
        if response.status_code == 200:
            data = response.json()
            print(f"   ✅ PUT successful: {data['message']}")
        else:
            print(f"   ❌ PUT failed: {response.status_code}")
        
        # Test GET operation
        print("   Testing GET operation...")
        response = await self.client.get(
            f"{self.base_url}/api/v1/cacheServices/getObject?id=demo123",
            headers=headers
        )
        
        if response.status_code == 200:
            data = response.json()
            print(f"   ✅ GET successful: {data['message']}")
            if data['data']:
                print(f"   📦 Retrieved data: {data['data']}")
        else:
            print(f"   ❌ GET failed: {response.status_code}")
        
        # Test DELETE operation
        print("   Testing DELETE operation...")
        response = await self.client.delete(
            f"{self.base_url}/api/v1/cacheServices/deleteObject?id=demo123",
            headers=headers
        )
        
        if response.status_code == 200:
            data = response.json()
            print(f"   ✅ DELETE successful: {data['message']}")
        else:
            print(f"   ❌ DELETE failed: {response.status_code}")
    
    async def test_health_checks(self):
        """Test health check endpoints."""
        print("\n🏥 Testing health checks...")
        
        # Test readiness probe
        print("   Testing readiness probe...")
        response = await self.client.get(f"{self.base_url}/health/ready")
        
        if response.status_code == 200:
            data = response.json()
            print(f"   ✅ Readiness: {data['status']}")
            print(f"   📊 Uptime: {data['uptime']:.2f} seconds")
        else:
            print(f"   ❌ Readiness check failed: {response.status_code}")
        
        # Test liveness probe
        print("   Testing liveness probe...")
        response = await self.client.get(f"{self.base_url}/health/live")
        
        if response.status_code == 200:
            data = response.json()
            print(f"   ✅ Liveness: {data['status']}")
        else:
            print(f"   ❌ Liveness check failed: {response.status_code}")
        
        # Test Spring Actuator compatible health check
        print("   Testing Spring Actuator health...")
        response = await self.client.get(f"{self.base_url}/actuator/health")
        
        if response.status_code == 200:
            data = response.json()
            print(f"   ✅ Actuator health: {data['status']}")
            if 'components' in data:
                for component, info in data['components'].items():
                    print(f"   📋 {component}: {info['status']}")
        else:
            print(f"   ❌ Actuator health check failed: {response.status_code}")
    
    async def test_metrics(self):
        """Test metrics endpoints."""
        print("\n📊 Testing metrics...")
        
        # Test general metrics
        print("   Testing general metrics...")
        response = await self.client.get(f"{self.base_url}/metrics/")
        
        if response.status_code == 200:
            data = response.json()
            metrics = data['metrics']
            print(f"   ✅ Metrics available")
            print(f"   📈 Request count: {metrics.get('request_count', 0)}")
            print(f"   📈 Error count: {metrics.get('error_count', 0)}")
            print(f"   📈 Uptime: {metrics.get('uptime_seconds', 0):.2f} seconds")
        else:
            print(f"   ❌ Metrics failed: {response.status_code}")
        
        # Test Prometheus metrics
        print("   Testing Prometheus metrics...")
        response = await self.client.get(f"{self.base_url}/metrics/prometheus")
        
        if response.status_code == 200:
            content = response.text
            if "python_spring_equivalent_requests_total" in content:
                print(f"   ✅ Prometheus metrics available")
            else:
                print(f"   ⚠️ Prometheus metrics format unexpected")
        else:
            print(f"   ❌ Prometheus metrics failed: {response.status_code}")
    
    async def test_jwt_operations(self):
        """Test JWT operations."""
        print("\n🔑 Testing JWT operations...")
        
        if not self.auth_token:
            print("❌ No auth token available")
            return
        
        # Test token verification
        print("   Testing token verification...")
        verify_request = {"token": self.auth_token}
        response = await self.client.post(
            f"{self.base_url}/api/jwt/verify",
            json=verify_request
        )
        
        if response.status_code == 200:
            data = response.json()
            print(f"   ✅ Token verification: {data['valid']}")
            print(f"   👤 Username: {data['username']}")
            print(f"   🔐 Permissions: {data['permissions']}")
            print(f"   👥 Roles: {data['roles']}")
        else:
            print(f"   ❌ Token verification failed: {response.status_code}")
        
        # Test token decoding
        print("   Testing token decoding...")
        response = await self.client.get(
            f"{self.base_url}/api/jwt/decode?token={self.auth_token}"
        )
        
        if response.status_code == 200:
            data = response.json()
            print(f"   ✅ Token decoded successfully")
            print(f"   📋 Algorithm: {data['header'].get('alg', 'unknown')}")
            print(f"   📋 Token type: {data['header'].get('typ', 'unknown')}")
        else:
            print(f"   ❌ Token decoding failed: {response.status_code}")
    
    async def test_vulnerable_endpoints(self):
        """Test vulnerable endpoints (for security testing)."""
        print("\n⚠️ Testing vulnerable endpoints (for security testing)...")
        
        # Test vulnerable token creation
        print("   Testing vulnerable token creation...")
        vulnerable_request = {
            "user": "hacker",
            "role": "admin",
            "algorithm": "HS256"
        }
        
        response = await self.client.post(
            f"{self.base_url}/api/jwt/create-vulnerable",
            json=vulnerable_request
        )
        
        if response.status_code == 200:
            data = response.json()
            print(f"   ⚠️ Vulnerable token created: {data['vulnerable']}")
            print(f"   🔓 Algorithm: {data['algorithm']}")
        else:
            print(f"   ❌ Vulnerable token creation failed: {response.status_code}")
    
    async def run_demo(self):
        """Run the complete demo."""
        print("🚀 Python Spring Equivalent - Demo Script")
        print("=" * 50)
        
        try:
            # Test basic connectivity
            await self.test_home_endpoint()
            
            # Create authentication token
            await self.create_token()
            
            # Test JWT operations
            await self.test_jwt_operations()
            
            # Test cache operations
            await self.test_cache_operations()
            
            # Test health checks
            await self.test_health_checks()
            
            # Test metrics
            await self.test_metrics()
            
            # Test vulnerable endpoints
            await self.test_vulnerable_endpoints()
            
            print("\n" + "=" * 50)
            print("✅ Demo completed successfully!")
            print("\n📚 Available endpoints:")
            print("   • API Documentation: http://localhost:8080/docs")
            print("   • Health Check: http://localhost:8080/health/ready")
            print("   • Metrics: http://localhost:8080/metrics/")
            print("   • Prometheus: http://localhost:8080/metrics/prometheus")
            
        except Exception as e:
            print(f"\n❌ Demo failed with error: {e}")
            print("Make sure the application is running on http://localhost:8080")
        
        finally:
            await self.close()


async def main():
    """Main function."""
    demo = PythonSpringEquivalentDemo()
    await demo.run_demo()


if __name__ == "__main__":
    asyncio.run(main())
