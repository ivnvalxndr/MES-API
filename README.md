Тестовый учебный проект - Система учета книг в библиотеке с использованием C# (backend), Vue.js + Vuetify (frontend).

Database (таблицы) ←→ Entity ←→ Repository ←→ Service ←→ DTO ←→ Controller ←→ API Client (frontend)

LibraryManagement 		- Основной проект

LibraryManagement.Core 	- для моделей и DTO

LibraryManagement.Data 	- для репозиториев и работы с БД

LibraryManagement.BPMN 	- для BPMN компонентов

LibraryManagement.Tests - для unit tests 

Entities для базы данных

DTOs для бизнес-логики и API

Client 
  → [Controller] (принимает/возвращает DTO) 
  → [Business Service] (работает с DTO) 
  → [Repository] (работает с Entities) 
  → Database
  
  MES.API/
  Controllers/          ← Используют DTO из Shared

MES.Business/
  Services/             ← Используют Translators из Shared
  Interfaces/

MES.Data/
  Entities/             ← Entities
  Repositories/

MES.Shared/
  DTOs/                 ← Data Transfer Objects
  Translators/          ← Translators (Entity ↔ DTO)
    Interfaces/


EF Identity
-----------------------------------------------------------------------

Controllers 	- работа с HTTP, валидация моделей, возврат ответов, маппинг DTO

Services 		- бизнес-логика, координация работы репозиториев, валидация, права доступа

Repositories 	- работа с базой данных, CRUD операции

-----------------------------------------------------------------------

Использованные средства:

AspNetCore 	- .NET WEB Framework

YamlDotNet 	- для кастомизации Swagger, редактирование yml. Описание API методов.

Npgsql 		- используется PostreSQL.

xunit 		- юнит тестирование.

Dapper 		- прямое использование SQL запросов в коде, без работы с объектами как в ORM.

DbUp 		- (аналог Flyway для C#) готового инструмента по работе с БД миграциями.

NSwag       - инструмент для автоматической генерации клиентского кода и документации API на основе OpenAPI/Swagger спецификации.

zeebe.camunda - Для взаимодействия с BPMN диаграмми на движке Camunda (C#).

https://console.cloud.camunda.io/ - Построение, просмотр BPMN диаграмм.

-----------------------------------------------------------------------

Папка UML содержит диаграммы.

-----------------------------------------------------------------------

DB Schema:

Book -> BookAuthor <- Author
Many-to-Many (Многие-ко-многим) 

Loan -> Book (Many-to-One)

Book -> BookAuthors (One-to-Many)

Author -> BookAuthors (One-to-Many) 

Loan -> Users (Many-to-One)