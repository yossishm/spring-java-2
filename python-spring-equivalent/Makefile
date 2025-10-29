# Python Spring Equivalent - Makefile

.PHONY: help install test lint format clean run docker-build docker-run docker-compose-up

help: ## Show this help message
	@echo "Python Spring Equivalent - Available Commands:"
	@echo ""
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-20s\033[0m %s\n", $$1, $$2}'

install: ## Install dependencies
	pip install -r requirements.txt

install-dev: ## Install development dependencies
	pip install -r requirements.txt
	pip install -e ".[dev]"

test: ## Run tests
	pytest

test-cov: ## Run tests with coverage
	pytest --cov=python_spring_equivalent --cov-report=html --cov-report=term

test-watch: ## Run tests in watch mode
	pytest-watch

lint: ## Run linting
	flake8 python_spring_equivalent/ tests/
	mypy python_spring_equivalent/
	bandit -r python_spring_equivalent/
	safety check

format: ## Format code
	black python_spring_equivalent/ tests/
	isort python_spring_equivalent/ tests/

format-check: ## Check code formatting
	black --check python_spring_equivalent/ tests/
	isort --check-only python_spring_equivalent/ tests/

clean: ## Clean up build artifacts
	rm -rf build/
	rm -rf dist/
	rm -rf *.egg-info/
	rm -rf .pytest_cache/
	rm -rf htmlcov/
	rm -rf .coverage
	find . -type d -name __pycache__ -delete
	find . -type f -name "*.pyc" -delete

run: ## Run the application
	python run.py

run-dev: ## Run the application in development mode
	DEBUG=true python run.py

docker-build: ## Build Docker image
	docker build -t python-spring-equivalent .

docker-build-distroless: ## Build distroless Docker image
	docker build -f Dockerfile.distroless -t python-spring-equivalent-distroless .

docker-run: ## Run Docker container
	docker run -p 8080:8080 python-spring-equivalent

docker-compose-up: ## Start all services with Docker Compose
	docker-compose up -d

docker-compose-down: ## Stop all services
	docker-compose down

docker-compose-logs: ## View logs
	docker-compose logs -f

setup-dev: install-dev ## Setup development environment
	cp env.example .env
	@echo "Development environment setup complete!"
	@echo "Edit .env file with your configuration"

ci: format-check lint test ## Run CI pipeline

all: clean install-dev format lint test ## Run everything
