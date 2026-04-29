# Kubernetes Auto Scaling Consumers Based on Kafka Lag

## Overview
This project demonstrates how to automatically scale Kafka consumer applications running in Kubernetes based on **consumer lag**.

Instead of relying on CPU or memory metrics, this approach scales consumers dynamically depending on how far behind they are in processing messages. This ensures efficient resource usage while maintaining high throughput.

---

## Problem Statement
Traditional Kubernetes autoscaling (HPA) uses CPU or memory metrics, which are not suitable for event-driven systems like Kafka.

Key issues:
- CPU usage does not reflect message backlog
- Over-scaling or under-scaling consumers
- Inefficient resource utilization
- Increased processing latency

---

## Solution
This project implements **lag-based autoscaling**, where:

- Kafka **consumer lag** is monitored
- Scaling decisions are made based on lag thresholds
- Kubernetes automatically adjusts the number of consumer pods

---

## Architecture

### Components
- Kafka (message broker)
- Consumer application
- Lag monitoring mechanism
- Kubernetes Deployment
- Autoscaling logic (e.g., KEDA / custom scaler)

---

## How It Works

1. Messages are produced to Kafka topics  
2. Consumers process messages  
3. Lag (difference between produced and consumed messages) is calculated  
4. Autoscaler evaluates lag:  
   - High lag → scale up consumers  
   - Low/no lag → scale down consumers  
5. Kubernetes adjusts pod replicas accordingly  

---

## Key Concepts

### Consumer Lag
Consumer lag represents the number of messages waiting to be processed.

### Scaling Strategy
- Scale up when lag exceeds threshold  
- Scale down when lag decreases  
- Prevent scaling beyond partition count  

> Note: Kafka limits active consumers to the number of partitions in a topic.

---

## Tech Stack
- Kubernetes  
- Apache Kafka  
- Docker  
- (Optional) KEDA (Kubernetes Event-Driven Autoscaler)  

---

## Prerequisites
- Kubernetes cluster (local or cloud)  
- Kafka cluster  
- kubectl configured  
- Docker installed  

---

## Setup & Deployment

### 1. Clone the repository
```bash
git clone https://github.com/ashen89/Kubernetes-Auto-Scaling-Consumers-According-KafkaLag.git
cd Kubernetes-Auto-Scaling-Consumers-According-KafkaLag
```

### 2. Build Docker Image
```bash
docker build -t kafka-consumer-app .
```
