# Kế hoạch Triển khai Dự án Microservice E-commerce Mini với .NET Aspire

## 1. Tổng quan Dự án (Project Overview)
Xây dựng một hệ thống Thương mại điện tử (E-commerce) phân tán thu nhỏ, áp dụng kiến trúc Microservice chuẩn production. Dự án sử dụng **.NET Aspire (.NET 8/9)** làm nền tảng Orchestration trung tâm giúp đơn giản hóa việc quản lý container, cấu hình kết nối, quan sát dữ liệu (Observability) trên Local và dễ dàng triển khai lên Cloud (Azure Container Apps).

## 2. Danh mục Công nghệ & Hạ tầng (Tech Stack Matrix)

| Thành phần (Component) | Công nghệ lựa chọn (Technology) | Vai trò & Mục đích sử dụng |
| :--- | :--- | :--- |
| **Framework Cốt lõi** | .NET 8/.NET 9 (C#) | Xây dựng các Web API dịch vụ độc lập |
| **Orchestration & Monitoring** | .NET Aspire | Quản lý vòng đời container, Service Discovery, Telemetry Dashboard |
| **API Gateway** | YARP | Định tuyến Request, Centralized Authentication & Rate Limiting |
| **Database per Service** | PostgreSQL | Cơ sở dữ liệu quan hệ lưu trữ dữ liệu Auth, Product, Order |
| **In-Memory Data Store** | Redis | Distributed Caching (Product), Cart State Management, API Rate Limiting |
| **Message Broker (Nghiệp vụ)**| RabbitMQ + MassTransit | Xử lý sự kiện bất đồng bộ chính xác (Order -> Notification -> Payment) |
| **Event Streaming (Data Analytics)**| Apache Kafka | Thu thập và xử lý luồng dữ liệu lớn (User Clickstream, Event Tracking) |
| **Cloud Deployment** | Azure Container Apps (ACA) / azd | Triển khai hạ tầng tự động lên đám mây thông qua Infrastructure as Code |

## 3. Kiến trúc Chi tiết Các Sub-Systems (Services Breakdown)

1. **ECommerce.AppHost**: Project Aspire trung tâm định nghĩa hạ tầng, kết nối các container (Postgres, Redis, RabbitMQ, Kafka) và microservices.
2. **ECommerce.ServiceDefaults**: Cấu hình chuẩn cho OpenTelemetry (Tracing, Metrics, Logs), Health Checks, và Service Discovery.
3. **ApiGateway (YARP)**: Điểm vào duy nhất cho Client. Tích hợp Redis Rate Limiting để chống DDOS và kiểm tra JWT Token.
4. **AuthService**: Quản lý tài khoản, phân quyền, phát hành JWT Access Token (Database: PostgreSQL).
5. **ProductService**: Quản lý Catalog sản phẩm & Tồn kho. Tích hợp Redis Distributed Cache cho các truy vấn Read-Heavy.
6. **CartService**: Quản lý Giỏ hàng mua sắm. Lưu trữ dữ liệu trực tiếp trong Redis Key-Value/Hash (không dùng RDBMS).
7. **OrderService**: Xử lý logic đặt hàng. Ghi đơn hàng vào Postgres và bắn event `OrderCreatedEvent` qua RabbitMQ.
8. **NotificationService**: Consumer lắng nghe `OrderCreatedEvent` từ RabbitMQ để phát email/tin nhắn xác nhận.
9. **TrackingService**: Thu thập log hành vi người dùng (xem sản phẩm, thêm giỏ hàng) qua Kafka Topic để phân tích Real-time.

## 4. Lộ trình Triển khai 6 Giai đoạn (6-Phase Implementation Roadmap)

**Tiến trình:** `[Giai đoạn 1] -> [Giai đoạn 2] -> [Giai đoạn 3] -> [Giai đoạn 4] -> [Giai đoạn 5] -> [Giai đoạn 6]`

### Giai đoạn 1: Khởi tạo Nền tảng (Aspire Orchestration & Core)
- [ ] Cài đặt .NET Aspire Workload (`dotnet workload install aspire`).
- [ ] Khởi tạo dự án mẫu Aspire Starter (`ECommerce.AppHost` & `ECommerce.ServiceDefaults`).
- [ ] Thử nghiệm chạy Aspire Dashboard để kiểm tra OpenTelemetry Traces và Metrics.

### Giai đoạn 2: Xây dựng Core Microservices & Database
- [ ] Khai báo tài nguyên PostgreSQL Container trong AppHost (`builder.AddPostgres(...)`).
- [ ] Xây dựng **AuthService**: Viết API Đăng ký, Đăng nhập, Sinh mã JWT.
- [ ] Xây dựng **ProductService**: CRUD Sản phẩm, kết nối Database Postgres riêng.

### Giai đoạn 3: Tối ưu với Redis (Cache, Cart State & Rate Limiting)
- [ ] Cấu hình AppHost: Đăng ký Redis resource (`builder.AddRedis("redis")`).
- [ ] **ProductService**: Tích hợp Redis Cache (`IDistributedCache`) tối ưu API lấy danh sách/chi tiết sản phẩm.
- [ ] **CartService**: Xây dựng API giỏ hàng lưu trữ hoàn toàn trên Redis Data Structure.
- [ ] **ApiGateway**: Cấu hình YARP Gateway kết hợp Redis Rate Limiting middleware.

### Giai đoạn 4: Giao tiếp Bất đồng bộ với RabbitMQ (Async Business Events)
- [ ] Cấu hình AppHost: Thêm resource RabbitMQ (`builder.AddRabbitMQ("messaging")`).
- [ ] **OrderService**: Cài đặt MassTransit, lưu đơn hàng và publish `OrderCreatedEvent` lên RabbitMQ Broker.
- [ ] **NotificationService**: Tạo Consumer đăng ký nhận event từ RabbitMQ và mô phỏng gửi mail xác nhận.

### Giai đoạn 5: Xử lý Luồng Dữ liệu Lớn với Kafka (Event Streaming & Analytics)
- [ ] Cấu hình AppHost: Thêm resource Apache Kafka (`builder.AddKafka("kafka")`).
- [ ] **ApiGateway** / **ProductService**: Phát sự kiện `ProductViewedEvent` lên Kafka topic mỗi khi user xem sản phẩm.
- [ ] **TrackingService**: Xây dựng Kafka Consumer đọc stream dữ liệu liên tục để thống kê danh mục Hot theo thời gian thực.

### Giai đoạn 6: Triển khai Đám mây (Cloud Deployment via Azure Container Apps)
- [ ] Cài đặt Azure Developer CLI (`azd`).
- [ ] Chạy `azd init` để khởi tạo cấu hình Infrastructure as Code (Bicep templates).
- [ ] Ánh xạ các Resource (Postgres, Redis, RabbitMQ, Kafka) sang Managed Services trên Cloud tương ứng.
- [ ] Thực thi lệnh `azd up` để Aspire tự động build Docker images, đẩy lên Container Registry và deploy toàn bộ lên Azure Container Apps (ACA).

## 5. Tiêu chuẩn Hoàn thành (Definition of Done - DoD)
1. **Hiểu rõ vai trò từng công nghệ:**
   - **Redis:** Dùng cho Cache ngắn hạn, Session, Giỏ hàng tạm thời, Rate Limit.
   - **RabbitMQ:** Dùng cho Event nghiệp vụ chính xác (Transactional events, 1-to-1 or 1-to-Many Routing).
   - **Kafka:** Dùng cho Luồng dữ liệu hành vi lớn (High-throughput Streaming Data, Log Analytics).
2. **Khởi chạy Local 1-Click:** Toàn bộ Microservices, Gateways và Databases khởi chạy thành công chỉ qua 1 nút bấm F5 hoặc `dotnet run` từ AppHost.
3. **Observability hoàn chỉnh:** Theo dõi được Distributed Tracing xuyên suốt từ `ApiGateway -> OrderService -> RabbitMQ -> NotificationService` trên Aspire Dashboard.
4. **Sẵn sàng triển khai Cloud:** Có khả năng đóng gói Docker và deploy thành công lên môi trường Cloud (ACA/AWS).
