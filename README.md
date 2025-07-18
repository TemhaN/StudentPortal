# 🎓 StudentPortal

**StudentPortal** — десктопное приложение на **C# / WPF**, созданное для управления данными студентов в образовательных учреждениях.  
Работает через **Entity Framework Core**, использует **Material Design**, поддерживает статистику, экспорт в Excel и гибкую фильтрацию.

## ✨ Основные возможности

- 🔐 **Аутентификация**: регистрация, вход, роли (админ / пользователь).
- 👨‍🎓 **Студенты**: добавление, редактирование, удаление, загрузка фото (Base64), поиск, экспорт в Excel.
- 📚 **Курсы и группы**: управление названиями, продолжительностью, годами начала и связями.
- 📝 **Оценки и предметы**: выставление оценок, фильтрация, сортировка по дате.
- 📊 **Статистика**: средние баллы, топ-3 студентов, гистограммы по предметам и группам.
- 🎨 **UI/UX**: Material Design, адаптивная вёрстка, пагинация, плавные анимации.
- 🔍 **Поиск/фильтрация**: по ФИО, группам, курсам, оценкам.

## 📋 Требования

- .NET 8.0+
- SQL Server (или любой совместимый с EF Core)
- Visual Studio 2022+ с поддержкой WPF
- Установленные NuGet-пакеты (см. ниже)

## 🧩 Зависимости

| Пакет                         | Назначение                          |
|------------------------------|-------------------------------------|
| `Microsoft.EntityFrameworkCore`         | ORM для взаимодействия с БД       |
| `Microsoft.EntityFrameworkCore.SqlServer` | Провайдер SQL Server              |
| `MaterialDesignThemes`       | Стили Material Design для WPF       |
| `LiveCharts.Wpf`             | Построение графиков и диаграмм      |
| `ClosedXML`                  | Экспорт в Excel                     |

👉 Полный список в `.csproj`.

## 🚀 Установка и запуск

### 1. Клонируй репозиторий
```bash
git clone https://github.com/YourUsername/StudentPortal.git
cd StudentPortal
````

### 2. Настрой подключение к базе

Открой `appsettings.json` и пропиши строку подключения:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your_server;Database=StudentPortalDb;Trusted_Connection=True;"
  }
}
```

> ⚠️ Замени `your_server` на имя твоего SQL Server.

### 3. Установи зависимости

```bash
dotnet restore
```

### 4. Примени миграции EF Core

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Запусти приложение

```bash
dotnet run
```

Или открой `.sln` файл в Visual Studio и запусти в режиме отладки.

## 🖱️ Как пользоваться

### 👑 Роль: **Администратор**

| Окно             | Возможности                                                       |
| ---------------- | ----------------------------------------------------------------- |
| `MainWindow`     | Вход, регистрация                                                 |
| `WindowStudents` | Панель администратора                                             |
| `Information`    | Управление студентами, пагинация (4 на страницу), экспорт в Excel |
| `CoursePage`     | Управление курсами (название, длительность)                       |
| `GroupPage`      | Управление группами                                               |
| `GradePage`      | Выставление оценок, подсчёт среднего, фильтры                     |
| `StatisticsPage` | Графики, средние баллы, топ-3 студентов, гистограммы              |

> 👤 Учти: логин/пароль по умолчанию для администратора — `admin / admin`

### 🙋 Роль: **Пользователь**

| Окно         | Возможности                                      |
| ------------ | ------------------------------------------------ |
| `UserWindow` | Выбор группы, просмотр оценок, фильтрация, поиск |

### 🧠 Особенности

* Фото студентов хранятся в **Base64**.
* Гибкая **пагинация** и **поиск**.
* Статистика визуализирована через **LiveCharts**.
* **Экспорт** в Excel реализован через **ClosedXML**.

## 📦 Сборка и развёртывание

### 🛠️ Сборка релиза

```bash
dotnet publish -c Release -r win-x64
```

👉 Сборка будет в папке:

```
bin/Release/net8.0-windows/win-x64/publish
```

### 🚀 Развёртывание

1. Скопируй папку `publish` на целевую машину
2. Убедись, что SQL Server доступен
3. Проверь актуальность строки подключения в `appsettings.json`

## 📸 Скриншоты

<div style="display: flex; flex-wrap: wrap; gap: 10px; justify-content: center;">
  <img src="https://github.com/TemhaN/StudentPortal/blob/master/StudentPortal/Screenshots/1.png?raw=true" alt="StudentPortal" width="30%">
  <img src="https://github.com/TemhaN/StudentPortal/blob/master/StudentPortal/Screenshots/2.png?raw=true" alt="StudentPortal" width="30%">
  <img src="https://github.com/TemhaN/StudentPortal/blob/master/StudentPortal/Screenshots/3.png?raw=true" alt="StudentPortal" width="30%">
  <img src="https://github.com/TemhaN/StudentPortal/blob/master/StudentPortal/Screenshots/4.png?raw=true" alt="StudentPortal" width="30%">
  <img src="https://github.com/TemhaN/StudentPortal/blob/master/StudentPortal/Screenshots/5.png?raw=true" alt="StudentPortal" width="30%">
  <img src="https://github.com/TemhaN/StudentPortal/blob/master/StudentPortal/Screenshots/6.png?raw=true" alt="StudentPortal" width="30%">
  <img src="https://github.com/TemhaN/StudentPortal/blob/master/StudentPortal/Screenshots/7.png?raw=true" alt="StudentPortal" width="30%">
  <img src="https://github.com/TemhaN/StudentPortal/blob/master/StudentPortal/Screenshots/8.png?raw=true" alt="StudentPortal" width="30%">
</div>    

## 🧠 Автор

**TemhaN**  
[GitHub профиль](https://github.com/TemhaN)

## 🧾 Лицензия

Проект распространяется под лицензией [MIT License].

## 📬 Обратная связь

Нашли баг или хотите предложить улучшение?
Создайте **issue** или присылайте **pull request** в репозиторий!

## ⚙️ Технологии

* **C# / .NET 8.0** — основа приложения
* **WPF** — интерфейс
* **Entity Framework Core** — ORM
* **SQL Server** — база данных
* **MaterialDesignThemes** — современный UI
* **LiveCharts.Wpf** — графики и статистика
* **ClosedXML** — экспорт в Excel
