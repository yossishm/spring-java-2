"""
OpenTelemetry configuration for the Python Spring Equivalent application.
"""

import logging
from typing import Optional

from opentelemetry import trace
from opentelemetry.exporter.otlp.proto.grpc.trace_exporter import OTLPSpanExporter
from opentelemetry.exporter.otlp.proto.grpc.metric_exporter import OTLPMetricExporter
from opentelemetry.instrumentation.fastapi import FastAPIInstrumentor
from opentelemetry.instrumentation.httpx import HTTPXClientInstrumentor
from opentelemetry.instrumentation.requests import RequestsInstrumentor
from opentelemetry.sdk.metrics import MeterProvider
from opentelemetry.sdk.metrics.export import PeriodicExportingMetricReader
from opentelemetry.sdk.resources import Resource
from opentelemetry.sdk.trace import TracerProvider
from opentelemetry.sdk.trace.export import BatchSpanProcessor

from .config import settings

logger = logging.getLogger(__name__)


def setup_telemetry() -> None:
    """Set up OpenTelemetry tracing and metrics."""
    try:
        # Create resource
        resource = Resource.create({
            "service.name": settings.otel_service_name,
            "service.version": settings.otel_service_version,
            "deployment.environment": "development" if settings.debug else "production"
        })
        
        # Set up tracing
        trace.set_tracer_provider(TracerProvider(resource=resource))
        tracer = trace.get_tracer(__name__)
        
        # Set up OTLP exporter if endpoint is configured
        if settings.otel_exporter_otlp_endpoint:
            otlp_exporter = OTLPSpanExporter(endpoint=settings.otel_exporter_otlp_endpoint)
            span_processor = BatchSpanProcessor(otlp_exporter)
            trace.get_tracer_provider().add_span_processor(span_processor)
            
            logger.info(f"OpenTelemetry tracing configured with OTLP endpoint: {settings.otel_exporter_otlp_endpoint}")
        else:
            logger.info("OpenTelemetry tracing configured without OTLP exporter")
        
        # Set up metrics
        if settings.otel_exporter_otlp_endpoint:
            metric_exporter = OTLPMetricExporter(endpoint=settings.otel_exporter_otlp_endpoint)
            metric_reader = PeriodicExportingMetricReader(metric_exporter, export_interval_millis=10000)
            meter_provider = MeterProvider(resource=resource, metric_readers=[metric_reader])
            
            logger.info(f"OpenTelemetry metrics configured with OTLP endpoint: {settings.otel_exporter_otlp_endpoint}")
        else:
            logger.info("OpenTelemetry metrics configured without OTLP exporter")
        
        # Instrument libraries
        FastAPIInstrumentor.instrument()
        HTTPXClientInstrumentor().instrument()
        RequestsInstrumentor().instrument()
        
        logger.info("OpenTelemetry instrumentation completed")
        
    except Exception as e:
        logger.error(f"Failed to set up OpenTelemetry: {e}")
        # Don't raise the exception to allow the application to continue


def get_tracer(name: str) -> trace.Tracer:
    """Get a tracer instance."""
    return trace.get_tracer(name)


def get_meter(name: str):
    """Get a meter instance."""
    from opentelemetry import metrics
    return metrics.get_meter(name)
