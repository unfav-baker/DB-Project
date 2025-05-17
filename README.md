# PRIME_TECH Export Management System

## Overview

This repository contains a modular, enterprise-level management system designed to streamline operations across multiple departments. The project is architected as four tightly integrated sub-projects:

- **Dashboard**
- **Admin**
- **Supplier**
- **Employee**

Each sub-project is managed within its **dedicated Git branch**, allowing for modular development, better collaboration, and separation of concerns.

## Repository Structure

The project is divided into the following branches:

| Branch Name | Description |
|-------------|-------------|
| `dashboard` | Contains the main dashboard interface with analytics and system-wide metrics. |
| `admin`     | Manages administrative tasks such as user roles, permissions, and system settings. |
| `supplier`  | Handles supplier management, including inventory, procurement, and logistics. |
| `employee`  | Manages employee-related operations such as attendance, payroll, and HR functions. |

## Project Integration

All four sub-projects are designed to work **seamlessly together** as a complete system. They share common components and data flows, ensuring consistency and coherence across the application.

Integration includes:
- Shared authorization logic
- Unified database schema and APIs
- Common UI/UX standards and responsive design
- Real-time data sync across modules where applicable

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/unfav-baker/DB-Project.git


To Switch to any of the Branch you can use


git checkout dashboard
# or
git checkout admin
# or
git checkout supplier
# or
git checkout employee
