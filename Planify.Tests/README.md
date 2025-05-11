# Unit Test Project – Hướng dẫn sử dụng

Đây là project chứa các unit test cho backend. Làm theo hướng dẫn bên dưới để tích hợp và chạy các test trong solution backend.

---

## 1. Mở Solution Backend

1. Mở Visual Studio
2. Mở file `.sln` của backend (Planify_BackEnd.sln)

---

## 2. Thêm Project Unit Test vào Solution

1. Click chuột phải vào **Solution** > `Add` > `Existing Project...`
2. Tìm đến thư mục này (Planify.Tests)
3. Chọn file `.csproj` của project test (Planify.Tests.csproj)

---

## 3. Add Reference tới Project Chính

1. Click chuột phải vào project test > `Add` > `Project Reference...`
2. Tick chọn project backend chính (Planify_BackEnd)
3. Nhấn OK để thêm reference

---

## 4. Cài các NuGet Package cần thiết (nếu chưa có)

Các package sử dụng:

NUnit
NUnit.Analyzers
NUnit3TestAdapter
Install-Package Moq
Microsoft.NET.Test.Sdk
Microsoft.EntityFrameworkCore.InMemory

