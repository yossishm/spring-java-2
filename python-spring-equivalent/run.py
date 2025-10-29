#!/usr/bin/env python3
"""
Startup script for Python Spring Equivalent application.
"""

import sys
import uvicorn
from python_spring_equivalent.config import settings

if __name__ == "__main__":
    uvicorn.run(
        "python_spring_equivalent.main:app",
        host=settings.host,
        port=settings.port,
        reload=settings.debug,
        log_level="info",
        access_log=True
    )
