#FROM eclipse-temurin:21.0.6_7-jre-alpine-3.21
#FROM eclipse-temurin:21.0.7_6-jre-alpine-3.21
FROM eclipse-temurin:21.0.8_9-jre-alpine-3.22
RUN mkdir /opt/app
ARG JAR_FILE
COPY target/gs-spring-boot-docker-0.2.0.jar /opt/app/app.jar
ENTRYPOINT [ "java", "-jar", "/opt/app/app.jar"]
