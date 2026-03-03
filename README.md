# Chat.Server

A real-time messaging application with a modern web-based chat interface and a WPF desktop client. Built for LAB01 assignment.

## 📌 Purpose

This application demonstrates a full-stack real-time chat solution with:
- **Web Server**: ASP.NET Core Razor Pages with SignalR for real-time messaging
- **Desktop Client**: WPF (Windows Presentation Foundation) application for local chat access
- **Authentication**: Supabase-based user registration and login
- **File Sharing**: Support for uploading and sharing files with progress tracking
- **Modern UI**: Messenger-style chat interface with emoji picker and responsive design

## 🏗️ Project Structure

## 🛠️ Tech Stack

### Backend (Chat.Server)
- **Framework**: ASP.NET Core 8.0
- **Real-time Communication**: SignalR
- **Authentication**: Cookie-based authentication + Supabase
- **Database**: Supabase PostgreSQL (for user management)
- **File Handling**: Chunked file upload with streaming
- **Web Server**: Kestrel (supports up to 1.5 GB file uploads)

### Frontend (Web)
- **Markup**: Razor Pages (.cshtml)
- **Styling**: Custom CSS with dark theme (CSS variables)
- **Client Script**: Vanilla JavaScript + SignalR Client
- **UI Features**:
  - Messenger-style chat bubbles (left/right alignment)
  - Emoji picker with categorized emojis
  - Image preview and rendering
  - File upload progress bar
  - Responsive sidebar with online users list
  - Private messaging support

### Desktop Client (Chat.Client)
- **Framework**: WPF (Windows Presentation Foundation)
- **Language**: C# with XAML
- **Communication**: SignalR client library
- **UI Components**: 
  - User login panel
  - Online users list
  - Chat message display
  - Message input area

## ✨ Key Features

### Authentication & User Management
- User registration with Supabase integration
- Secure login with cookie-based sessions
- Online/offline user status tracking

### Real-time Messaging
- Instant message delivery via SignalR
- Private/Direct messaging between users
- System messages for user join/leave events
- Message timestamps and sender identification

### File Sharing
- Multi-file upload with progress tracking
- Large file support (up to 1.5 GB)
- File preview and download links in chat
- Image inline preview with lightbox modal

### Modern UI/UX
- Dark theme with blue accent colors
- Animations and transitions
- Emoji picker integration
- Responsive layout
- Messenger-like chat bubbles

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Visual Studio 2022 or VS Code
- Supabase account (for authentication)

### Running the Server
```bash
cd Chat.Server
dotnet restore
dotnet run
# Server runs on http://localhost:5000
