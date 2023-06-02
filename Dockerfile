#FROM openjdk:11-jre-slim
#FROM openjdk:17-ea-22-jdk-oracle
#FROM registry.access.redhat.com/openjdk/openjdk-11-rhel7:1.1-14
#FROM registry.redhat.io/openjdk/openjdk-11-rhel8
#FROM adoptopenjdk/openjdk8:ubi-minimal
#ARG JAR_FILE=target/*.jar
#COPY ${JAR_FILE} /app.jar
#ENTRYPOINT ["java","-jar","/app.jar"]
FROM eclipse-temurin:17-alpine
RUN mkdir /opt/app
ARG JAR_FILE
ADD target/gs-spring-boot-docker-0.1.0.jar /opt/app/app.jar
CMD ["java", "-jar", "/opt/app/app.jar"]