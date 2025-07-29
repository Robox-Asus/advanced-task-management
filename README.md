## Goals Or Obective 
This project will allow teams to manage tasks, projects, and users with different roles, offering features like batch operations and comprehensive reporting, which can be computationally intensive and benefit from parallel processing.

## 1. Project Requirements Checklist
This section ensures all functional and non-functional requirements are thoroughly defined and documented, serving as the blueprint for development and testing.

### 5.1. Functional Requirements Checklist

#### User Authentication & Authorization:
--> The system shall allow users to register for a new account.
--> The system shall allow registered users to log in using their credentials.
--> The system shall allow logged-in users to log out.
--> The system shall allow administrators to manage user roles (e.g., assign Admin, Project Manager, Team Member).
--> The system shall restrict access to features based on assigned user roles.

#### Project Management:
--> The system shall allow authorized users (Project Managers, Admins) to create, view, update, and delete projects.
--> The system shall allow Project Managers/Admins to add and remove team members from a project.
--> The system shall display a list of all projects accessible to the user.
--> The system shall display detailed information for a specific project, including associated tasks and team members.

#### Task Management:
--> The system shall allow authorized users to create, view, update, and delete tasks within a project.
--> The system shall allow users to assign tasks to specific team members.
--> The system shall support task statuses (e.g., ToDo, InProgress, Done, Blocked) and priorities (e.g., Low, Medium, High, Critical).
--> The system shall allow users to add and view comments on tasks.
--> The system shall display a list of all tasks accessible to the user.
--> The system shall display detailed information for a specific task, including its comments.

#### Reporting & Analytics:
--> The system shall generate project performance reports (e.g., total tasks, completion rates, overdue tasks).
--> The system shall provide task distribution reports by assignee.
--> The system shall allow authorized users to perform batch updates on task statuses.

### 5.2. Non-Functional Requirements Checklist
#### Performance:
    --> Response Time: (e.g., "All user interactions shall have a response time under 2 seconds").
    --> Throughput: (e.g., "The system shall support 100 concurrent active users").
    --> Scalability: (e.g., "The system shall scale to support 10,000 users within 6 months with minimal architecture changes").
    --> Resource Utilization: (e.g., "CPU utilization shall not exceed 70% under peak load").
#### Security:
    --> Authentication: (e.g., "Implement JWT-based authentication with token expiration and refresh mechanisms").
    --> Authorization: (e.g., "Enforce role-based access control (RBAC) for all sensitive operations").
    --> Data Encryption: (e.g., "All sensitive data at rest and in transit shall be encrypted using AES-256").
    --> Vulnerability Management: (e.g., "Conduct quarterly security audits and penetration tests").
    --> Input Validation: (e.g., "All user inputs shall be validated to prevent common injection attacks").
#### Usability:
    --> Ease of Use: (e.g., "New users shall be able to complete core tasks within 15 minutes without training").
    --> User Interface (UI) Standards: (e.g., "The UI shall adhere to Bootstrap 5 design principles and provide a consistent user experience").
    --> Accessibility: (e.g., "The application shall comply with WCAG 2.1 Level AA guidelines").
#### Reliability:
    --> Availability: (e.g., "The system shall maintain 99.9% uptime per month").
    --> Fault Tolerance: (e.g., "The system shall gracefully handle database connection failures and retry operations").
    --> Backup & Recovery: (e.g., "Daily automated backups shall be performed, with a Recovery Time Objective (RTO) of 4 hours and Recovery Point Objective (RPO) of 24 hours").
#### Maintainability:
    --> Modifiability: (e.g., "New features shall be implementable within 2-week sprints without significant refactoring").
    --> Supportability: (e.g., "All critical errors shall be logged with sufficient detail for diagnosis").
    --> Code Quality: (e.g., "Code shall adhere to C# and TypeScript coding standards and be reviewed via pull requests").
    --> Documentation: (e.g., "API endpoints shall be documented using Swagger/OpenAPI specifications").
#### Portability:
    --> (e.g., "The backend shall be deployable on both Windows and Linux environments").
    --> (e.g., "The database shall support migration between SQL Server and PostgreSQL").
#### Compatibility:
    --> (e.g., "The frontend shall be compatible with the latest two stable versions of Chrome, Firefox, Edge, and Safari").
    --> (e.g., "The API shall integrate with existing corporate LDAP for user synchronization").
#### Legal & Regulatory Compliance:
    --> (e.g., "The system shall comply with GDPR regulations regarding data privacy and user consent").
    --> (e.g., "All financial transactions shall comply with PCI DSS standards").
