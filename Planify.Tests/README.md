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

---

## 5. Chạy test

Test -> Run All Tests (Ctrl + R,A)
hoặc xem
Test -> Test Explorer (Ctrl + E,T)

* Chú ý: Một số test case kiểm tra ngày (ngày bắt đầu phải sau ngày hôm nay) có thể bị sai nếu chạy vì ngày trong test case đúng nếu tính "ngày hôm nay" là ngày chạy test (ngày chạy có trong report)

--------------
Unit Test Project – User Guide
This project contains unit tests for the backend. Follow the instructions below to integrate and run the tests within the backend solution.

1. Open the Backend Solution
Open Visual Studio

Open the backend .sln file (Planify_BackEnd.sln)

2. Add the Unit Test Project to the Solution
Right-click on the Solution > Add > Existing Project...

Navigate to this folder (Planify.Tests)

Select the .csproj file of the test project (Planify.Tests.csproj)

3. Add a Reference to the Main Project
Right-click on the test project > Add > Project Reference...

Check the main backend project (Planify_BackEnd)

Click OK to add the reference

4. Install Required NuGet Packages (if not already installed)
The following packages are used:

NUnit

NUnit.Analyzers

NUnit3TestAdapter

Moq (Install via: Install-Package Moq)

Microsoft.NET.Test.Sdk

Microsoft.EntityFrameworkCore.InMemory

## 5. Run test

Test -> Run All Tests (Ctrl + R,A)

or view:

Test -> Test Explorer (Ctrl + E,T)

* Note: Some test cases that validate dates (e.g., the start date must be after today) may fail if executed on a different day, as the test data assumes that "today" is the day the tests are run (the actual test date is recorded in the report).
