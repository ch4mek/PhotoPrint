# time-tracker-api
### **Modsen management system architecture diagrams**:
- **test coverage diagram**

      Developers use this diagram to:

         - Assess test quality: It shows the percentage of code covered by tests, indicating how thoroughly the code has been tested.

         - Identify weak spots: Highlights code areas that lack testing, focusing efforts on testing those parts.

         - Improve code quality: Encourages developers to write more comprehensive tests, thereby enhancing the program's reliability.

    ![test_coverage_diagram.png](src%2FDocs%2FImages%2Ftest_coverage_diagram.png)

    Here's a breakdown of the tests in our application:
    - 20 Functional Tests;
    - 580+ Integration Tests;
    - 950+ Unit Tests.

- **The high-level architecture view**
       
      In a software context, this view might encompass:

         - Components: Identification of major modules or components within the system, showcasing their roles and relationships.

         - Interfaces: Illustration of how different components interact and communicate with each other, often highlighting API endpoints or data flows.

         - Dependencies: Understanding dependencies between various components, libraries, or external services.

         - Deployment: Overview of how the system will be deployed, including server configurations, cloud services, or infrastructure considerations.

         - Scalability and Performance: Considerations about how the architecture supports scalability and performance requirements.
    
    ![high_level_architecture_view.png](src%2FDocs%2FImages%2Fhigh_level_architecture_view.png)

- **The architecture diagram of a framework**

      The architecture diagram of a framework typically illustrates the structural layout of the framework, showcasing its main components, their relationships, and interactions.
      This visual representation aids in understanding how different parts of the framework collaborate to achieve its functionalities.

    ![framework_module_architecture.png](src%2FDocs%2FImages%2Fframework_module_architecture.png)

- **The architecture diagram of infrastructure modules**

      Key elements of an infrastructure modules architecture diagram might include:

         - Networking Components: Displaying routers, switches, load balancers, firewalls, and other network devices that facilitate communication between different parts of the infrastructure.

         - Server Infrastructure: Illustrating physical or virtual servers, their distribution, and their roles (such as web servers, application servers, database servers) within the infrastructure.

         - Storage Systems: Showing storage solutions like databases, file storage, or distributed storage systems and their relationships with other components.

         - Cloud Services: If applicable, showcasing cloud-based services such as IaaS (Infrastructure as a Service), PaaS (Platform as a Service), or SaaS (Software as a Service) integrated into the infrastructure.

         - Security Measures: Depicting security components like encryption mechanisms, authentication systems, and intrusion detection/prevention systems.

         - Monitoring and Management Tools: Indicating tools and systems used for monitoring infrastructure health, performance, and management.

         - Integration Points: Highlighting connections to external services, APIs, or third-party systems that the infrastructure interacts with.

         - Scalability and Redundancy: Illustrating how the infrastructure accommodates scalability requirements and redundancies for fault tolerance.

    ![infrastructure_modules_architecture.png](src%2FDocs%2FImages%2Finfrastructure_modules_architecture.png)

- **The business module architecture diagram**

      This business module architecture diagram aligns with the N-layer model, where the system is structured
         into distinct layers to efficiently manage functionality and ensure modularity.
      Each layer holds distinct functionality and specialization. The interaction among these layers ensures efficient system operation,
         following modular architecture principles and responsibilities segregation.

    ![business_module_architecture.png](src%2FDocs%2FImages%2Fbusiness_module_architecture.png)

### **Architecture**
Each module comprises the following components:

 - Contract: This component facilitates communication between modules and defines interfaces for interaction. It may contain interfaces, classes, or other constructs outlining contracts for inter-module communication.

 - Controller: It houses controllers responsible for handling endpoints and contains input and output models for these controllers. This is where interaction with incoming requests occurs and responses are formulated.

- Abstractions: Includes interfaces and contracts used for abstracting and defining shared functions that must be implemented by other components within the module.

- Npgsql: This holds configurations, contexts, migrations, database entities, seed data, as well as repositories or data access providers. It interacts with the database, providing data access and manipulation.

- Domain: Contains utilities and shared models specific to the module. This might include utility functions, common data structures, and other tools used within the module.

- UseCase: This component houses business rules, data validation, handlers for queries and commands. It's where the core business logic resides, ensuring the execution of essential operations and rules within the module.

- UnitTest: Contains tests for validating handler functionality, business rules, data validation, repositories, and providers within the module. This section is crucial for ensuring the correctness of the module's functionality and components.

### **Environment strategy**
The environment strategy consists of four stages: **dev, qa, staging, and master (production)**. During a sprint, branches are created from **dev**, and the work is directly carried out on these branches. Once all the necessary work on a particular component is completed, a pull request **is made back into dev**. When both the back and front end implementations are finished, **dev is merged into qa** for testing.

If testers discover a bug, they send it back. However, a branch can now be created directly from **qa**, allowing the bug fix to be made in **qa** without affecting **dev**. Changes are only brought back from **qa to dev** after the fix is implemented.

Once the program has been successfully tested, it moves to **staging**, where no direct modifications are allowed. Testers subject the program to extensive tests, emulating production scenarios. After thorough testing and business approval, a pull request is made into the **master branch**. **The master branch represents the final product for users**.

For more detailed information, please follow the [link](https://modsen.atlassian.net/wiki/spaces/TT/pages/33390593/Onboarding+Backend).
