"""
OOP Interview Chatbot - Knowledge Base (v2 - Adaptive Challenge Edition)
Comprehensive Question Bank with Difficulty Tiers, Key Concepts,
Rubrics, Hints, Follow-ups, Model Solutions, and Real-Time Interviewer Curveballs.
"""

TOPICS = {
    "all": "Full Comprehensive OOP Interview (All Topics)",
    "core_pillars": "The 4 Pillars (Encapsulation, Abstraction, Inheritance, Polymorphism)",
    "solid": "SOLID Design Principles",
    "design_patterns": "OOP Design Patterns (Creational, Structural, Behavioral)",
    "advanced_oop": "Advanced Concepts (Composition, Memory, Diamond Problem, Immutability)",
    "code_challenges": "Code Review & Refactoring Challenges"
}

DIFFICULTIES = ["Junior", "Mid-Level", "Senior / Lead", "Staff / Principal"]

QUESTIONS = [
    # -------------------------------------------------------------
    # 1. CORE PILLARS
    # -------------------------------------------------------------
    {
        "id": "pillar_1",
        "topic": "core_pillars",
        "difficulty": "Junior",
        "title": "Encapsulation vs Abstraction",
        "question": "Can you explain the difference between Encapsulation and Abstraction? Please provide a real-world example illustrating both concepts.",
        "code_snippet": None,
        "expected_concepts": [
            "encapsulation", "data hiding", "bundling", "access modifiers", "private", "public",
            "abstraction", "hiding complexity", "essential features", "interface", "implementation details"
        ],
        "rubric": {
            "excellent": "Clearly distinguishes encapsulation (bundling data and methods, data hiding via access specifiers) from abstraction (hiding implementation complexity, exposing only what is necessary). Gives a concrete example like a Car (abstraction: steering wheel/pedals; encapsulation: internal engine state, protected transmission gears).",
            "moderate": "Defines both terms correctly but might blur the boundary between data hiding and complexity hiding, or provides a vague example.",
            "poor": "Confuses encapsulation with inheritance or polymorphism, or fails to define what abstraction hides."
        },
        "hints": [
            "Think of Encapsulation as 'packaging and protecting' your data, and Abstraction as 'simplifying' the interface to the outside world.",
            "Consider a Smartphone or Car dashboard: what do you see versus what is sealed under the hood?"
        ],
        "follow_up": "How do access modifiers like private, protected, and package-private enforce encapsulation in languages like Java or C++?",
        "curveball": "Curveball Twist: If a class has private fields but exposes public getters and setters for all of them, does it still have true encapsulation? Why or why not?",
        "model_answer": """**Encapsulation** is the mechanism of wrapping data (attributes) and the code acting on the data (methods) together as a single unit, while restricting direct access to internal state using access modifiers (private, protected). It ensures *data hiding and integrity*.

**Abstraction** is the process of hiding internal implementation complexity and showing only essential features of an object to callers. It focuses on *what* an object does rather than *how* it does it.

**Real-World Example (Car):**
- **Abstraction:** The steering wheel, accelerator, and brake pedals. You press the pedal without needing to understand hydraulic fluid pressures or caliper friction physics.
- **Encapsulation:** The engine, fuel injection system, and transmission are sealed under the hood. You cannot directly alter the fuel-to-air ratio variable from the driver's seat; you interact through controlled interfaces (the gas pedal)."""
    },
    {
        "id": "pillar_2",
        "topic": "core_pillars",
        "difficulty": "Junior",
        "title": "Method Overloading vs Method Overriding",
        "question": "What is the difference between Method Overloading and Method Overriding? How do they relate to static vs dynamic Polymorphism?",
        "code_snippet": None,
        "expected_concepts": [
            "overloading", "overriding", "compile-time", "runtime", "static polymorphism", "dynamic polymorphism",
            "same method name", "different parameters", "same signature", "inheritance", "virtual method", "dynamic dispatch"
        ],
        "rubric": {
            "excellent": "Identifies overloading as compile-time (static) polymorphism (same name, different parameter signature in the same class) and overriding as runtime (dynamic) polymorphism (same signature in a subclass providing specific implementation). Mentions late binding or vtables.",
            "moderate": "Explains the syntax difference (same name different parameters vs same signature in subclass) but omits the compile-time vs runtime distinction.",
            "poor": "Mixes up the two concepts or cannot describe how subclasses override behavior."
        },
        "hints": [
            "Consider WHEN the method call is resolved: does the compiler know at compile-time, or is it decided at runtime based on the actual object in memory?",
            "One occurs within the same class with different parameters; the other requires an inheritance hierarchy with the exact same signature."
        ],
        "follow_up": "Can you override a static or private method in Java or C++? Why or why not?",
        "curveball": "Curveball Twist: What happens if a derived class introduces a method with the same name and parameter count as a base class method, but with covariant return types?",
        "model_answer": """**Method Overloading (Compile-Time / Static Polymorphism):**
- Occurs when multiple methods in the same class share the same name but have different parameter lists (different types, number, or order of arguments).
- Resolved by the compiler at compile-time based on the method signature.
- *Example:* `calculateArea(int radius)` and `calculateArea(int length, int width)`.

**Method Overriding (Run-Time / Dynamic Polymorphism):**
- Occurs when a subclass provides a specific implementation of a method that is already defined in its superclass.
- The method must have the **exact same name, return type (or covariant), and parameter list**.
- Resolved at runtime based on the actual object type in memory, typically via dynamic dispatch (virtual method table / vtable).
- *Example:* `Animal.makeSound()` overridden by `Dog.makeSound()` to bark instead of a generic sound."""
    },
    {
        "id": "pillar_3",
        "topic": "core_pillars",
        "difficulty": "Mid-Level",
        "title": "Dynamic Dispatch and Virtual Tables (vtable)",
        "question": "How does dynamic polymorphism work under the hood in languages like C++ or Java? What is a vtable and a vptr, and what are the performance trade-offs?",
        "code_snippet": None,
        "expected_concepts": [
            "dynamic dispatch", "vtable", "virtual method table", "vptr", "pointer", "function pointer",
            "runtime lookup", "indirect call", "virtual keyword", "memory overhead", "cache miss"
        ],
        "rubric": {
            "excellent": "Explains that each class with virtual methods has a static vtable containing function pointers to the most-derived method implementations. Each object instance has an implicit pointer (vptr) pointing to its class's vtable. At runtime, calls are dispatched via an indirect pointer dereference. Mentions cache overhead and branch predictor impact.",
            "moderate": "Mentions that runtime checks determine which method to call and mentions vtable/pointers, but explanation of memory layout or dispatch mechanism is vague.",
            "poor": "Does not know how runtime resolution works and guesses without mentioning function pointers or tables."
        },
        "hints": [
            "Think about how the runtime knows which function to jump to when an Animal reference points to a Dog object in heap memory.",
            "Consider memory overhead: does every object store copies of all functions, or just an address pointing to a table?"
        ],
        "follow_up": "What is devirtualization, and how do modern JIT compilers optimize away virtual calls?",
        "curveball": "Curveball Twist: In a performance-critical loop executing 100 million iterations per second, how does replacing dynamic polymorphism with template-based static polymorphism (CRTP in C++) impact L1 instruction cache and branch prediction?",
        "model_answer": """Under the hood, dynamic polymorphism is powered by **Dynamic Dispatch**, typically implemented using **Virtual Method Tables (vtables)** and **Virtual Table Pointers (vptrs)**:

1. **vtable (Virtual Table):** A lookup table of function pointers created by the compiler for every class that declares or inherits at least one virtual method. There is only **one vtable per class** stored in read-only memory.
2. **vptr (Virtual Pointer):** Every instance (object) of that class contains an invisible pointer (the `vptr`) pointing to the vtable of its actual concrete runtime class.
3. **Dispatch Mechanism:**
   - Code: `animalPtr->makeSound()`
   - CPU: Dereferences `animalPtr` -> reads `vptr` -> indexes into `vtable` -> calls function pointer.

**Trade-offs:** Extra indirection (potential cache miss) and prevents compiler inlining, though modern branch predictors mitigate most overhead."""
    },
    {
        "id": "pillar_4",
        "topic": "core_pillars",
        "difficulty": "Mid-Level",
        "title": "Abstract Class vs Interface",
        "question": "When would you choose an Abstract Class over an Interface, and vice-versa? How have modern language features (like default methods in Java 8+ or C# interfaces) changed this distinction?",
        "code_snippet": None,
        "expected_concepts": [
            "abstract class", "interface", "state", "instance variables", "multiple inheritance",
            "is-a relationship", "can-do contract", "default methods", "constructors", "coupling"
        ],
        "rubric": {
            "excellent": "Points out that Abstract Classes represent identity ('IS-A') and can maintain state (instance fields, constructors, non-static fields), while Interfaces represent behavior/contracts ('CAN-DO'). Notes that classes can implement multiple interfaces but usually inherit from only one class. Mentions modern default methods for backwards-compatible API evolution.",
            "moderate": "Lists the standard textbook differences (multiple inheritance vs single inheritance, method bodies) but doesn't clearly explain the architectural design criteria (state vs behavioral contract).",
            "poor": "Says they are completely identical or fails to mention that interfaces can't maintain instance state."
        },
        "hints": [
            "Consider state: can an interface have non-static fields and constructors?",
            "Think about class hierarchy: how many classes can you inherit from versus how many interfaces can you implement?"
        ],
        "follow_up": "If interfaces can now have default methods and private helper methods, what is the single biggest remaining technical difference between them?",
        "curveball": "Curveball Twist: Suppose you need to build an extensible plugin architecture where third-party developers write plugins. Would you publish an Interface or an Abstract Base Class to allow future non-breaking updates?",
        "model_answer": """**Choose an Abstract Class when:**
1. **Shared State:** You need to share instance fields or provide constructors that subclasses initialize.
2. **Core Identity ('IS-A'):** The classes form a tight hierarchy of closely related objects (e.g., `AbstractVehicle` -> `Car`, `Truck`).
3. **Template Method Pattern:** You want to define a non-overridable template algorithm where subclasses only fill in specific protected hooks.

**Choose an Interface when:**
1. **Behavioral Contract ('CAN-DO'):** You are defining capabilities across unrelated classes (e.g., `Comparable`, `Serializable`, `Flyable` applied to both `Bird` and `Airplane`).
2. **Multiple Inheritance:** A class needs to adhere to multiple contracts without multiple inheritance ambiguity.
3. **Loose Coupling & Mocking:** Perfect for dependency injection, test mocks, and modular plugins.

**Impact of Modern Default Methods:**
While default methods allow interfaces to provide default implementation code, **interfaces still cannot declare instance state (instance variables)** and cannot define constructors."""
    },
    {
        "id": "pillar_5",
        "topic": "core_pillars",
        "difficulty": "Junior",
        "title": "Stack vs Heap Memory in OOP",
        "question": "What is the difference between Stack and Heap memory in object-oriented programming? Where do object references, primitive values, and actual object instances live?",
        "code_snippet": None,
        "expected_concepts": [
            "stack", "heap", "reference", "primitive", "garbage collection",
            "memory allocation", "lifespan", "stack overflow", "out of memory"
        ],
        "rubric": {
            "excellent": "Clearly explains that Stack stores function call frames, local primitive variables, and references to objects, with automatic LIFO allocation/deallocation. Heap stores dynamic memory and actual object instances, managed by a Garbage Collector or manual free/delete. Explains pointer indirection and lifecycle differences.",
            "moderate": "Explains that stack is fast/temporary and heap is for objects, but is vague about where the reference variable itself resides.",
            "poor": "Confuses stack and heap or thinks objects live on the stack in languages like Java/C#."
        },
        "hints": [
            "When you declare `MyClass obj = new MyClass();` inside a method, where does `obj` (the variable) live, and where does `new MyClass()` live?",
            "Consider memory lifetime: what happens when the method exits and returns?"
        ],
        "follow_up": "What is the difference between a StackOverflowError and an OutOfMemoryError?",
        "curveball": "Curveball Twist: In C# or Rust, struct types can be allocated directly on the Stack. When does allocating a value object on the Stack instead of the Heap cause 'boxing', and what is the performance cost?",
        "model_answer": """**Stack Memory:**
- Fast, contiguous memory allocated automatically per-thread in Last-In-First-Out (LIFO) order.
- Stores: Primitive local variables, method call frames, and **object reference pointers**.
- Lifetime: Automatically reclaimed when the method scope returns.

**Heap Memory:**
- Large, dynamic memory pool shared across all threads.
- Stores: The **actual object instances**, their internal fields, and array contents.
- Lifetime: Managed by the Garbage Collector (in Java/C#/Python) or manual `delete` (in C++).

**Code Example:**
```java
void createOrder() {
    int id = 42;                 // Value '42' lives directly on the STACK
    Order ord = new Order(id);   // 'ord' reference lives on STACK; 
                                 // 'new Order()' object payload lives on the HEAP
}
```"""
    },

    # -------------------------------------------------------------
    # 2. SOLID PRINCIPLES
    # -------------------------------------------------------------
    {
        "id": "solid_1",
        "topic": "solid",
        "difficulty": "Mid-Level",
        "title": "Single Responsibility Principle (SRP)",
        "question": "What is the Single Responsibility Principle (SRP), and what did Robert C. Martin mean by 'a class should have only one reason to change'?",
        "code_snippet": None,
        "expected_concepts": [
            "single responsibility", "reason to change", "actor", "stakeholder", "cohesion",
            "separation of concerns", "god class", "maintainability"
        ],
        "rubric": {
            "excellent": "Explains that SRP is not just 'doing only one function', but aligning class responsibilities to a single actor or stakeholder. Gives a clear example of separating business calculation, database persistence, and presentation/rendering into distinct classes.",
            "moderate": "Defines SRP as 'a class should do only one thing', but lacks depth regarding actors or reasons to change.",
            "poor": "Confuses SRP with DRY or procedural modularity without discussing class responsibilities."
        },
        "hints": [
            "Who asks for changes in software? Different departments or stakeholders (e.g., Accounting vs Reporting vs DBA).",
            "If an Employee class calculates pay, formats HTML reports, and saves to MySQL, how many stakeholders could demand changes?"
        ],
        "follow_up": "How does violating SRP increase the risk of regressions and merge conflicts in a team setting?",
        "curveball": "Curveball Twist: If we split classes too aggressively into tiny single-method classes, what new architectural problems arise, and where is the line between good SRP and over-engineering?",
        "model_answer": """**Single Responsibility Principle (SRP)** states that a module, class, or function should have **one, and only one, reason to change**.

Robert C. Martin clarified that a 'reason to change' corresponds to an **Actor or Stakeholder** who requests the change:
- If a class serves multiple actors, changes requested by Actor A can inadvertently break functionality relied upon by Actor B.

**Classic Violation Example:**
An `Employee` class containing:
1. `calculatePay()` -> Requested by CFO / Accounting department.
2. `reportHours()` -> Requested by COO / HR department.
3. `saveToDatabase()` -> Requested by Database Administrator (DBA).

**Refactored Design:**
Separate into:
- `Employee` (Core domain entity)
- `PayCalculator` (Financial calculations for Accounting)
- `HourReporter` (Time tracking and formatting for HR)
- `EmployeeRepository` (Database persistence for DBA)."""
    },
    {
        "id": "solid_2",
        "topic": "solid",
        "difficulty": "Mid-Level",
        "title": "Open/Closed Principle (OCP)",
        "question": "Explain the Open/Closed Principle (OCP). How do we write code that is 'open for extension but closed for modification'?",
        "code_snippet": None,
        "expected_concepts": [
            "open for extension", "closed for modification", "abstraction", "polymorphism", "interfaces",
            "strategy pattern", "avoiding switch statements", "extensibility", "regressions"
        ],
        "rubric": {
            "excellent": "Defines OCP precisely: existing code should not be modified to introduce new features, but new behavior is added by creating new classes that implement an abstraction. Demonstrates how to replace switch/if-else ladders with polymorphism or Strategy pattern.",
            "moderate": "States the definition correctly but struggles to show how code can be extended without touching existing files.",
            "poor": "Cannot explain how code can be both open and closed simultaneously."
        },
        "hints": [
            "Think about what happens when you add a new Payment Method (e.g. PayPal, Apple Pay, Crypto) to a Checkout system.",
            "Instead of opening `PaymentProcessor` and adding another `else if`, what abstraction can you introduce?"
        ],
        "follow_up": "Is 100% adherence to OCP always practical, or should you anticipate changes strategically? What is 'premature abstraction'?",
        "curveball": "Curveball Twist: How do you enforce OCP when configuring dependency injection at application startup without editing code every time a new plugin is compiled?",
        "model_answer": """The **Open/Closed Principle (OCP)** states:
> *Software entities (classes, modules, functions) should be open for extension, but closed for modification.*

- **Closed for Modification:** The existing, tested, bug-free source code should remain untouched.
- **Open for Extension:** New behaviors or requirements can be introduced by adding new classes.

**How to achieve OCP:** Through **Polymorphism and Abstraction** (e.g., Interfaces or Abstract Base Classes).

**Example: Payment Processing:**
- Instead of an `if payment_type == "paypal": ... elif payment_type == "apple_pay": ...` ladder inside `PaymentService`, define a `PaymentMethod` interface.
- Each payment channel is an isolated class (`PayPalPayment`, `ApplePayPayment`).
- Introducing a new payment method requires creating a new class with zero edits to `PaymentService`."""
    },
    {
        "id": "solid_3",
        "topic": "solid",
        "difficulty": "Senior / Lead",
        "title": "Liskov Substitution Principle (LSP)",
        "question": "What is the Liskov Substitution Principle (LSP)? Walk me through the classic Square-Rectangle problem or Bird-Penguin problem, and explain why it violates LSP.",
        "code_snippet": None,
        "expected_concepts": [
            "liskov substitution", "subtypes must be substitutable for supertypes", "behavioral subtyping",
            "square inherits rectangle", "preconditions cannot be strengthened", "postconditions cannot be weakened",
            "invariants preserved", "unexpected side effects"
        ],
        "rubric": {
            "excellent": "Quotes Barbara Liskov's concept of behavioral subtyping: subtypes must behave in accordance with supertype contracts without breaking caller expectations. Explains why Square inheriting Rectangle breaks the postcondition that changing width does not alter height. Mentions preconditions, postconditions, and invariants.",
            "moderate": "Explains the Square-Rectangle issue (setting width changes height, breaking tests) but does not formalize it using contracts, preconditions, or behavioral subtyping.",
            "poor": "Fails to see why a Square inheriting from a Rectangle is an issue ('since mathematically a square is a rectangle')."
        },
        "hints": [
            "In mathematics, a square is a rectangle. Why does that mathematical intuition fail in object-oriented programming when setters are involved?",
            "What assumption does a client function make when passed a Rectangle object?"
        ],
        "follow_up": "How does LSP relate to Design by Contract (DbC)? What are the rules regarding preconditions and postconditions?",
        "curveball": "Curveball Twist: If a ReadOnlyRectangle and MutableRectangle hierarchy is introduced, how would you design the contracts so both Square and Rectangle cleanly obey LSP?",
        "model_answer": """**Liskov Substitution Principle (LSP):**
*Objects of a superclass should be replaceable with objects of a subclass without affecting the correctness of the program.* It is about **behavioral subtyping**, not taxonomic classification.

**The Classic Square-Rectangle Problem:**
In geometry, a Square is a Rectangle. But in OOP with mutable objects:
```python
class Rectangle:
    def set_width(self, w): self.width = w
    def set_height(self, h): self.height = h
    def area(self): return self.width * self.height

class Square(Rectangle):
    def set_width(self, w):
        self.width = w
        self.height = w  # Side effect!
    def set_height(self, h):
        self.width = h
        self.height = h
```
Now consider client code expecting a `Rectangle`:
```python
def verify_area(rect: Rectangle):
    rect.set_width(5)
    rect.set_height(4)
    assert rect.area() == 20  # FAILS if passed a Square! Square.area() returns 16!
```
The caller's valid invariant (`changing height leaves width unaffected`) was broken by the subclass.

**Formal Contract Rules:**
1. Subclasses cannot strengthen preconditions (cannot demand stricter inputs).
2. Subclasses cannot weaken postconditions (must fulfill all guarantees).
3. Superclass invariants must be preserved."""
    },
    {
        "id": "solid_4",
        "topic": "solid",
        "difficulty": "Senior / Lead",
        "title": "Dependency Inversion Principle (DIP)",
        "question": "What is the Dependency Inversion Principle (DIP)? How does it differ from Dependency Injection (DI) and Inversion of Control (IoC)?",
        "code_snippet": None,
        "expected_concepts": [
            "dependency inversion", "high-level modules", "low-level modules", "depend on abstractions",
            "inversion of control", "dependency injection", "framework vs library", "decoupling", "architectural boundary"
        ],
        "rubric": {
            "excellent": "Distinguishes DIP (the high-level architectural principle), IoC (the broader software design pattern where the flow of control is inverted), and DI (a concrete technique/mechanism for supplying dependencies via constructor/setter). Explains high-level business rules shouldn't depend on database/HTTP drivers.",
            "moderate": "Explains DIP and DI well, but treats IoC and DI as interchangeable synonyms without explaining the conceptual hierarchy.",
            "poor": "Cannot explain what is being 'inverted' or confuses DIP with simple interface usage."
        },
        "hints": [
            "Think of DIP as the PRINCIPLE (the rule), IoC as the PARADIGM (who controls the flow), and DI as the PATTERN/TECHNIQUE (how you pass dependencies into an object).",
            "What is being inverted? Traditionally, high-level business logic imports low-level database classes. What points where in DIP?"
        ],
        "follow_up": "How does DIP enable Clean Architecture or Hexagonal Architecture (Ports and Adapters)?",
        "curveball": "Curveball Twist: In Hexagonal Architecture (Ports and Adapters), which layer owns the Port interface: the Domain Core (high-level) or the Database Adapter (low-level)? Why is this distinction crucial for dependency inversion?",
        "model_answer": """**1. Dependency Inversion Principle (DIP) - The Principle:**
A high-level architectural rule:
- High-level modules (business rules) should not depend on low-level modules (SQL drivers, file systems, UI). Both should depend on **abstractions**.
- Abstractions should not depend on details; details should depend on abstractions.
- *What is inverted?* The direction of source code dependencies: the high-level domain owns the interface, and low-level infrastructure implements it.

**2. Inversion of Control (IoC) - The Paradigm:**
A broad architectural pattern where the control flow of a program is inverted compared to traditional procedural programming.
- A **framework** controls the lifecycle and calls your code (the 'Hollywood Principle': *Don't call us, we'll call you*).

**3. Dependency Injection (DI) - The Mechanism:**
A design pattern to implement DIP and IoC. Dependencies are supplied (injected) from outside via constructors or setters rather than instantiated internally."""
    },
    {
        "id": "solid_5",
        "topic": "solid",
        "difficulty": "Mid-Level",
        "title": "Interface Segregation Principle (ISP)",
        "question": "What is the Interface Segregation Principle (ISP)? How does having 'fat' or 'polluted' interfaces cause problems in an evolving system?",
        "code_snippet": None,
        "expected_concepts": [
            "interface segregation", "fat interfaces", "client should not be forced to depend",
            "cohesive interfaces", "dummy implementations", "throw notsupportedexception", "polluted interface"
        ],
        "rubric": {
            "excellent": "Explains that no client should be forced to depend on methods it does not use. Gives an example of a bloated 'MultiFunctionPrinter' interface forced onto a simple 'BasicPrinter' that has to throw NotImplementedException. Recommends segregating into Printer, Scanner, and Fax interfaces.",
            "moderate": "Defines ISP as keeping interfaces small, but does not explain the architectural harm of unnecessary recompilations or empty stub methods.",
            "poor": "Confuses ISP with Single Responsibility or Polymorphism."
        },
        "hints": [
            "Think about what happens when a small client only needs `print()`, but the interface demands `print()`, `scan()`, `fax()`, and `staple()`.",
            "What do developers write inside methods they are forced to implement but don't support? `throw new UnsupportedOperationException()` — an obvious code smell!"
        ],
        "follow_up": "How does ISP relate to role interfaces and micro-interfaces (like Java's `Comparable`, `Runnable`, `Closeable`)?",
        "curveball": "Curveball Twist: How do you evolve an existing public library interface with 10,000 active downstream callers without violating ISP when new capabilities are needed?",
        "model_answer": """**Interface Segregation Principle (ISP):**
> *Clients should not be forced to depend upon interfaces that they do not use.*

**The Problem with 'Fat' Interfaces:**
1. **Dummy Implementations:** Classes are forced to write empty stubs or throw `NotImplementedException`, which violates LSP and creates runtime crashes.
2. **Coupling & Recompilation Cascades:** Changes to an unused method signature in the interface force recompilation and testing of classes that never care about that method.

**Example & Fix:**
- Instead of a fat `interface Worker { void work(); void eat(); void sleep(); }`,
- Segregate into focused role interfaces: `interface Workable { void work(); }` and `interface Feedable { void eat(); }`.
- A robot class implements only `Workable`."""
    },

    # -------------------------------------------------------------
    # 3. DESIGN PATTERNS
    # -------------------------------------------------------------
    {
        "id": "pattern_1",
        "topic": "design_patterns",
        "difficulty": "Mid-Level",
        "title": "Singleton Pattern: Implementation & Thread Safety",
        "question": "How do you implement a thread-safe Singleton pattern? Why is Singleton frequently criticized as an anti-pattern?",
        "code_snippet": None,
        "expected_concepts": [
            "singleton", "single instance", "private constructor", "double-checked locking", "volatile",
            "thread safety", "anti-pattern", "global state", "hidden dependencies", "unit testing difficulty", "tight coupling"
        ],
        "rubric": {
            "excellent": "Describes thread-safe implementations (e.g., synchronized method, Double-Checked Locking with volatile, static initialization holder, or enum singleton). Explains why it is criticized: acts as disguised global state, couples components, prevents mocking in unit tests, and violates SRP.",
            "moderate": "Shows a basic singleton implementation but misses thread safety issues or fails to explain why testing is painful with singletons.",
            "poor": "Only gives a naive non-thread-safe singleton and doesn't know any drawbacks."
        },
        "hints": [
            "What happens if two threads call `getInstance()` at the exact same millisecond when instance is null?",
            "Why is testing a class that calls `DatabaseSingleton.getInstance()` hard in isolated unit tests?"
        ],
        "follow_up": "How does an Inversion of Control (IoC) container provide singleton lifetime without the downsides of the Singleton pattern?",
        "curveball": "Curveball Twist: Why is the `volatile` keyword strictly required in Double-Checked Locking in Java and C++? What CPU hardware instruction reordering problem does it prevent?",
        "model_answer": """**Thread-Safe Implementation (Double-Checked Locking):**
```java
public class DatabaseConnection {
    private static volatile DatabaseConnection instance; // volatile prevents instruction reordering

    private DatabaseConnection() { }

    public static DatabaseConnection getInstance() {
        if (instance == null) { // 1st check (avoids lock overhead once initialized)
            synchronized (DatabaseConnection.class) {
                if (instance == null) { // 2nd check (thread safety)
                    instance = new DatabaseConnection();
                }
            }
        }
        return instance;
    }
}
```

**Why Singleton is often an Anti-Pattern:**
1. **Global State:** Acts as a global variable in disguise, breaking encapsulation.
2. **Hidden Dependencies:** Callers invoke `DB.getInstance()` internally instead of declaring dependencies explicitly via constructors.
3. **Impedes Unit Testing:** Hard to mock or swap with test doubles; state leaks between test cases.
4. **Violates SRP:** Manages its own lifecycle *and* performs domain logic."""
    },
    {
        "id": "pattern_2",
        "topic": "design_patterns",
        "difficulty": "Mid-Level",
        "title": "Factory Method vs Abstract Factory Pattern",
        "question": "What is the difference between the Factory Method pattern and the Abstract Factory pattern? When would you use each?",
        "code_snippet": None,
        "expected_concepts": [
            "factory method", "abstract factory", "single product", "family of related products",
            "inheritance vs composition", "creational pattern", "decoupling", "extensibility"
        ],
        "rubric": {
            "excellent": "Clearly differentiates: Factory Method uses inheritance and subclassing to create ONE product; Abstract Factory uses object composition to produce FAMILIES of related or dependent products without specifying concrete classes (e.g., DarkThemeButton + DarkThemeCheckbox vs LightThemeButton + LightThemeCheckbox).",
            "moderate": "Explains both create objects without using 'new', but is fuzzy on the 'family of related products' distinction.",
            "poor": "Confuses Simple Factory with Abstract Factory or cannot explain when to use which."
        },
        "hints": [
            "Focus on the number and relationship of objects being created: is it one product type or a cohesive family of complementary products?",
            "Think of UI themes: Mac vs Windows vs Linux buttons, scrollbars, and checkboxes."
        ],
        "follow_up": "How does the Abstract Factory pattern relate to the Open/Closed Principle when introducing a new product family versus adding a new product type to all families?",
        "curveball": "Curveball Twist: In the Abstract Factory pattern, if you need to add a new product type (e.g. `createSlider()`) to all existing factory families, what happens to the Open/Closed Principle?",
        "model_answer": """**Factory Method Pattern:**
- **Focus:** Defines an interface for creating a **single product**, letting subclasses decide which concrete class to instantiate.
- **Mechanism:** Relies on **Inheritance**. Derived creator classes override the factory method.
- **Example:** `Document.createReport()` where `PdfDocument` returns `PdfReport` and `HtmlDocument` returns `HtmlReport`.

**Abstract Factory Pattern:**
- **Focus:** Provides an interface for creating **families of related or dependent objects** without specifying concrete classes.
- **Mechanism:** Relies on **Composition**. A client is passed a factory object that has multiple creation methods.
- **Example (Cross-Platform UI):**
  - `GUIFactory` defines: `createButton()`, `createCheckbox()`, `createScrollBar()`.
  - Implementations: `MacUIFactory` (produces Mac controls) vs `WindowsUIFactory` (produces Windows controls).
  - Guarantees controls from different platforms are never accidentally mixed."""
    },
    {
        "id": "pattern_3",
        "topic": "design_patterns",
        "difficulty": "Senior / Lead",
        "title": "Strategy Pattern vs State Pattern",
        "question": "Strategy and State patterns have virtually identical UML class diagrams. What is the fundamental difference in their intent and lifecycle?",
        "code_snippet": None,
        "expected_concepts": [
            "strategy pattern", "state pattern", "intent", "transitions", "context",
            "client selects strategy", "states transition automatically", "behavioral pattern", "finite state machine"
        ],
        "rubric": {
            "excellent": "Identifies that while both delegate behavior to a polymorphic interface, Strategy is chosen by the client or configured upfront to swap algorithms independently (no knowledge of other strategies), whereas State represents internal state transitions of an entity where states know about each other and drive transitions (FSM).",
            "moderate": "Mentions Strategy is for algorithms and State is for states, but doesn't explain the difference in transitions or who controls the swap.",
            "poor": "Claims they are the exact same pattern or confuses them with Template Method."
        },
        "hints": [
            "Ask yourself: who switches the concrete implementation, and do the implementations know about each other?",
            "Think of Navigation routing algorithms (Strategy) vs an Order progressing from Pending -> Paid -> Shipped -> Delivered (State)."
        ],
        "follow_up": "In the State pattern, should state transitions be managed inside the concrete State classes or inside the Context class?",
        "curveball": "Curveball Twist: If state transitions are managed inside the concrete State classes, does that introduce circular dependencies between State implementations? How do you resolve this cleanly?",
        "model_answer": """While both patterns share the same structural UML diagram (a `Context` delegating to an interface with multiple concrete subclasses), their **intent and lifecycle** are fundamentally different:

| Feature | Strategy Pattern | State Pattern |
|---|---|---|
| **Primary Intent** | Interchangeable algorithms or business rules | Modeling behavior that changes as internal state changes (FSM) |
| **Who changes it?** | Typically the **Client** configures the strategy | The **Context or States themselves** trigger transitions automatically |
| **Mutual Awareness** | Concrete strategies are completely **independent** | Concrete states often **know about other states** to trigger transitions |
| **Lifecycle** | Usually fixed for the duration of an operation | Dynamic; transitions continuously throughout the object's lifetime |

**Example:**
- **Strategy:** Compression tool using `ZipStrategy` or `GzipStrategy`.
- **State:** Vending machine transitioning between `NoCoinState` -> `HasCoinState` -> `DispensingState`."""
    },
    {
        "id": "pattern_4",
        "topic": "design_patterns",
        "difficulty": "Mid-Level",
        "title": "Observer Pattern & The Lapsed Listener Problem",
        "question": "How does the Observer Pattern work? What is the 'Lapsed Listener' problem, and how do you prevent memory leaks when using observers?",
        "code_snippet": None,
        "expected_concepts": [
            "observer pattern", "subject", "subscribers", "loose coupling", "event driven",
            "memory leak", "lapsed listener", "weak references", "deregistration", "garbage collection"
        ],
        "rubric": {
            "excellent": "Explains Subject/Observer relationship (attach, detach, notify). Identifies the Lapsed Listener problem: the Subject holds strong references to Observers in its listener list, preventing GC from reclaiming them even after the client is done with them. Solves it via explicit unsubscribe/dispose or using WeakReference.",
            "moderate": "Explains how the Observer pattern works but doesn't know what the Lapsed Listener problem is or how it causes memory leaks.",
            "poor": "Confuses Observer with Pub-Sub message queues or doesn't know how notification dispatch works."
        },
        "hints": [
            "If an event publisher stores an observer in a list `self.observers.append(observer)`, who holds a reference to that observer?",
            "What happens if a UI screen registers for system events, the user closes the screen, but forgets to unsubscribe?"
        ],
        "follow_up": "What is the key architectural difference between the classic Gang-of-Four Observer Pattern and a distributed Publish-Subscribe (Pub/Sub) message broker?",
        "curveball": "Curveball Twist: In a multithreaded application where observers register and unregister concurrently while events are being broadcast, how do you prevent ConcurrentModificationException without blocking notification dispatch?",
        "model_answer": """**How the Observer Pattern Works:**
Defines a one-to-many dependency. When the **Subject** changes state, all registered **Observers** are notified automatically (typically via an `update()` callback).

**The 'Lapsed Listener' Problem (Memory Leak):**
1. **The Issue:** When an observer registers with a subject, the subject holds a **strong reference** to the observer in its subscriber collection.
2. If the application finishes using the observer (e.g., closing a UI window) but **fails to explicitly unregister**, the Garbage Collector cannot collect it because an active root path exists from the long-lived Subject.
3. This creates a silent, compounding memory leak.

**Prevention:**
1. **Explicit Lifecycle Cleanup:** Implement `IDisposable` or pair `subscribe()` with `unsubscribe()`.
2. **Weak References:** Store listeners using `WeakReference` in the Subject's list, allowing GC to reclaim them when other references drop."""
    },
    {
        "id": "pattern_5",
        "topic": "design_patterns",
        "difficulty": "Mid-Level",
        "title": "Decorator Pattern vs Subclass Explosion",
        "question": "What is the Decorator Pattern, and how does it prevent the problem of 'Class Explosion' when adding multiple combinable behaviors to an object?",
        "code_snippet": None,
        "expected_concepts": [
            "decorator pattern", "class explosion", "composition over inheritance", "wrapping",
            "same interface", "open closed principle", "dynamic behavior", "stackable"
        ],
        "rubric": {
            "excellent": "Explains that Decorator dynamically attaches responsibilities to objects without subclassing. Contrasts this with subclass explosion where combinations of features (e.g. Beverage with Milk, Mocha, Whip) lead to 2^N subclasses. Explains the decorator implements the same interface as the wrapped object and delegates calls while adding behavior.",
            "moderate": "Explains that decorator wraps an object, but struggles to articulate the class explosion comparison or interface matching.",
            "poor": "Confuses Decorator with Adapter or Proxy."
        },
        "hints": [
            "Consider a Coffee Shop order system: a base Coffee can have Milk, Sugar, Caramel, or Whipped Cream in any combination.",
            "How many classes would you need with inheritance (e.g., `CoffeeWithMilkWhipAndCaramel`) vs wrapping decorators?"
        ],
        "follow_up": "How does Java's `java.io` stream library (`new BufferedReader(new InputStreamReader(new FileInputStream(...)))`) illustrate the Decorator Pattern?",
        "curveball": "Curveball Twist: What is the main drawback of the Decorator Pattern when client code relies on `instanceof` or concrete type checking of the underlying wrapped object?",
        "model_answer": """**The Decorator Pattern:**
Attaches additional responsibilities to an object dynamically at runtime. Decorators provide a flexible alternative to subclassing for extending functionality.

**Preventing Class Explosion:**
If you have a base `Beverage` and 4 optional add-ons (Milk, Sugar, Whip, Caramel):
- **Inheritance Approach:** You would need up to $2^4 = 16$ distinct subclasses (`CoffeeWithMilkWhip`, etc.). With 10 options, that's $1024$ classes!
- **Decorator Approach:** You only create 4 decorator classes (`MilkDecorator`, `WhipDecorator`). You can nest them dynamically:
  ```python
  my_coffee = Caramel(Milk(SimpleCoffee()))
  ```

**How It Works:**
1. The Decorator implements the **exact same interface** as the component it wraps.
2. It holds a reference to the wrapped component (`HAS-A`).
3. It delegates method calls to the wrapped component while injecting pre/post behavior."""
    },

    # -------------------------------------------------------------
    # 4. ADVANCED OOP ARCHITECTURE
    # -------------------------------------------------------------
    {
        "id": "adv_1",
        "topic": "advanced_oop",
        "difficulty": "Mid-Level",
        "title": "Favor Composition over Inheritance",
        "question": "Why is 'Favor Composition over Inheritance' a fundamental object-oriented design maxim? What is the 'Fragile Base Class' problem?",
        "code_snippet": None,
        "expected_concepts": [
            "composition over inheritance", "has-a vs is-a", "fragile base class", "tight coupling",
            "runtime flexibility", "encapsulation breaking", "delegation"
        ],
        "rubric": {
            "excellent": "Explains that inheritance creates tight compile-time coupling ('white-box reuse') where changes to a base class can silently break subclasses (Fragile Base Class problem). Composition ('black-box reuse' / 'HAS-A') allows swapping behaviors dynamically at runtime and maintains strong encapsulation boundaries.",
            "moderate": "Recites HAS-A vs IS-A, but gives a weak explanation of the Fragile Base Class issue.",
            "poor": "Advocates for deep inheritance trees and cannot explain any downsides of subclassing."
        },
        "hints": [
            "Consider what happens when you inherit from a third-party library class and the author updates an internal method call in a minor version bump.",
            "Can you change a class's parent at runtime? Can you swap a composed reference at runtime?"
        ],
        "follow_up": "Can you give an example where inheritance is truly the right choice over composition?",
        "curveball": "Curveball Twist: In Java, `CustomHashSet` extends `HashSet` and overrides `add()` and `addAll()` to count elements. When `addAll()` is called with 3 elements, the count becomes 6! Walk through exactly why this Fragile Base Class bug happens.",
        "model_answer": """**Why Favor Composition over Inheritance?**

1. **Loose vs Tight Coupling:**
   - **Inheritance (IS-A / White-box reuse):** Subclasses are tightly coupled to superclass implementation details. Protected internals are exposed, breaking encapsulation.
   - **Composition (HAS-A / Black-box reuse):** Objects interact exclusively through public interfaces, keeping internals shielded.
2. **Runtime Flexibility:**
   - Inheritance is static and fixed at compile time.
   - Composition allows swapping behavior dynamically at runtime (e.g., swapping strategies).
3. **The Fragile Base Class Problem:**
   - A superclass modification that seems innocent can silently break subclasses that override those methods.
   - *Classic Example:* If `CustomHashSet` extends `HashSet` and overrides `add()` and `addAll()` to count elements, but `HashSet.addAll()` internally calls `add()`, items get double-counted! With composition, this cannot happen."""
    },
    {
        "id": "adv_2",
        "topic": "advanced_oop",
        "difficulty": "Senior / Lead",
        "title": "The Diamond Problem & Multiple Inheritance",
        "question": "What is the Diamond Problem in Multiple Inheritance? How do different programming languages (C++, Java, Python) solve or prevent it?",
        "code_snippet": None,
        "expected_concepts": [
            "diamond problem", "multiple inheritance", "ambiguity", "c++ virtual inheritance",
            "java default methods", "interfaces", "python mro", "c3 linearization", "method resolution order"
        ],
        "rubric": {
            "excellent": "Draws/describes the diamond inheritance graph (A is base, B and C inherit from A, D inherits from both B and C). Explains ambiguity when calling a method defined in A. Compares solutions: C++ uses `virtual` base classes; Java forbids multiple class inheritance (and uses explicit disambiguation for interface default methods); Python uses C3 Linearization (Method Resolution Order / `__mro__`).",
            "moderate": "Explains the diamond shape and the ambiguity problem, but only knows how one language handles it.",
            "poor": "Does not know what the Diamond Problem is or why multiple inheritance causes ambiguity."
        },
        "hints": [
            "Imagine Class A has `display()`. Classes B and C both inherit from A and override `display()`. Class D inherits from both B and C. If D calls `display()`, which one runs?",
            "Look into Python's `super()` and MRO algorithm, or C++'s virtual base classes."
        ],
        "follow_up": "How does Python's C3 Linearization guarantee monotonicity in method resolution order?",
        "curveball": "Curveball Twist: In C++, how does `virtual` inheritance change the memory layout of an object instance, and which constructor in the hierarchy is responsible for initializing the virtual base class?",
        "model_answer": """**The Diamond Problem:**
Occurs in multiple inheritance when class `D` inherits from two classes `B` and `C`, both of which inherit from common ancestor `A`:
```
      A (defines foo())
     / \\
    B   C (both override foo())
     \\ /
      D (calls foo() -> Which one?)
```
Ambiguity: Should `D` execute `B.foo()` or `C.foo()`? Without special handling, `D` would also hold two duplicate copies of `A`'s member fields.

**How Languages Solve It:**
1. **C++ (Virtual Inheritance):** `B` and `C` declare `virtual public A`. Ensures only **one shared instance** of `A` exists in `D`.
2. **Java (Prohibition of Multiple Class Inheritance):** Disallowed multiple class inheritance. With default methods in interfaces, conflicting signatures must be explicitly disambiguated in `D` via `InterfaceB.super.foo()`.
3. **Python (C3 Linearization / MRO):** Uses deterministic Method Resolution Order (`D.__mro__`) via C3 Linearization. Calls to `super()` follow the linear chain."""
    },
    {
        "id": "adv_3",
        "topic": "advanced_oop",
        "difficulty": "Senior / Lead",
        "title": "Immutability & Value Objects in Domain-Driven OOP",
        "question": "Why is Immutability considered a powerful virtue in Object-Oriented Programming? What is the difference between an Entity and a Value Object?",
        "code_snippet": None,
        "expected_concepts": [
            "immutability", "thread safety", "no side effects", "value object", "entity",
            "identity vs equality", "defensive copying", "hashcode consistency", "domain driven design"
        ],
        "rubric": {
            "excellent": "Articulates advantages of immutability (thread safety without locking, hash map key stability, eliminating temporal coupling and hidden side effects). Accurately distinguishes Entities (defined by unique ID/lifecycle, e.g. User, Order) from Value Objects (defined strictly by their attribute values, immutable, e.g. Money, Address, Color).",
            "moderate": "Mentions that immutable objects cannot change and are thread-safe, but has trouble distinguishing Value Objects from Entities.",
            "poor": "Thinks OOP requires everything to have public getters and setters, and cannot justify why immutability is useful."
        },
        "hints": [
            "What happens if you use a mutable object as a key in a Hash Map and then mutate one of its fields?",
            "Think of two $20 bills vs two people with the same name: do you care which specific $20 bill you have if the value is $20?"
        ],
        "follow_up": "How do you implement an immutable class in Java, C#, or Python, and how do you handle modifications without mutating state?",
        "curveball": "Curveball Twist: If a domain requires updating an attribute of an immutable Value Object (e.g. changing the street address on a Customer profile), how do you model the change cleanly without breaking immutability?",
        "model_answer": """**Why Immutability is a Powerful Virtue in OOP:**
1. **Thread Safety by Default:** Immutable objects can be shared across concurrent threads without locks or race conditions.
2. **Predictability:** Eliminates temporal coupling (methods behaving differently depending on call order).
3. **Safe as Collection Keys:** In hash-based collections (`HashMap`, `HashSet`), mutating an object changes its hashcode, corrupting the hash bucket. Immutable objects guarantee stability.

**Difference between Entity and Value Object (DDD):**

| Characteristic | Entity | Value Object |
|---|---|---|
| **Identity** | Defined by unique, persistent **ID** (`userId = 4921`). Attributes may evolve. | Defined purely by its **Attributes / Values**. No conceptual ID. |
| **Equality** | Compared by ID (`user1.id == user2.id`). | Compared by structural equality of all fields (`m1.amount == m2.amount`). |
| **Mutability** | Usually mutable as state changes over time. | **Strictly Immutable**. To 'change', instantiate a new Value Object. |
| **Examples** | `Customer`, `Order`, `BankAccount`. | `Money($50, 'USD')`, `DateRange`, `GeoCoordinate`. |"""
    },
    {
        "id": "adv_4",
        "topic": "advanced_oop",
        "difficulty": "Staff / Principal",
        "title": "Staff Architecture: Thread-Safe LRU Cache Design",
        "question": "Design an Object-Oriented, thread-safe In-Memory LRU (Least Recently Used) Cache. Explain your choice of data structures, encapsulation boundaries, and concurrency strategy.",
        "code_snippet": None,
        "expected_concepts": [
            "hash map", "doubly linked list", "o(1) lookup", "o(1) eviction",
            "thread safety", "read write lock", "concurrent hash map", "encapsulation", "generic types"
        ],
        "rubric": {
            "excellent": "Combines a Hash Map (for O(1) key-to-node lookup) with a Doubly Linked List (for O(1) recency reordering and eviction). Details concurrency strategies: coarse-grained locks vs ReadWriteLock vs lock striping vs Java's ConcurrentLinkedDeque. Encapsulates node management inside the cache class.",
            "moderate": "Mentions map and linked list for O(1) operations, but has weak handling of thread safety or concurrency contention.",
            "poor": "Suggests an array or single list requiring O(N) linear search or eviction."
        },
        "hints": [
            "How do you achieve O(1) get() while also keeping track of which item was least recently accessed in O(1)?",
            "Why is a Doubly Linked List needed instead of a Singly Linked List for O(1) node removal?"
        ],
        "follow_up": "Under high read contention (95% reads, 5% writes), why does an LRU cache still require write locks during get() operations, and how can we mitigate this?",
        "curveball": "Curveball Twist: In a high-traffic production service with 200,000 read QPS, every `get()` operation updates recency order, which causes heavy lock contention. How would you redesign or approximate LRU (e.g., Clock algorithm or W-TinyLFU) to enable lock-free reads?",
        "model_answer": """**Architectural Components:**
1. **Doubly Linked List (`Node<K, V>`):**
   - Keeps track of access recency.
   - Head represents Most Recently Used (MRU); Tail represents Least Recently Used (LRU).
   - O(1) removal and insertion when given a node pointer.
2. **Hash Map (`Map<K, Node<K, V>>`):**
   - Provides O(1) lookup from key to the corresponding DLL Node.

**Concurrency Strategy:**
- In an LRU cache, even a `get()` operation modifies the linked list (moving accessed node to head).
- **Strategy 1:** ReadWriteLock (insufficient alone because read needs to write to the DLL).
- **Strategy 2:** ConcurrentHashMap with a concurrent lock-free buffer ring for eviction ordering (like Caffeine cache's Window TinyLFU).
- **Strategy 3:** Granular mutex around DLL operations with concurrent read map."""
    },
    {
        "id": "challenge_2",
        "topic": "code_challenges",
        "difficulty": "Staff / Principal",
        "title": "System Design: Extensible Notification Dispatcher",
        "question": "Design an Object-Oriented Notification System in code or pseudocode that supports Email, SMS, Slack, and Push notifications. It must handle user preferences, retries, and make adding a new notification channel (e.g., Discord or Webhook) require ZERO changes to existing dispatch code.",
        "code_snippet": None,
        "expected_concepts": [
            "strategy pattern", "open closed principle", "dependency inversion", "factory",
            "channel interface", "notification payload", "loose coupling", "retry decorator", "extensible"
        ],
        "rubric": {
            "excellent": "Outlines a modular design with: 1) `NotificationChannel` interface (`send(Notification payload)`); 2) Concrete channels (`EmailChannel`, `SlackChannel`); 3) Clean `NotificationService` that iterates over active channels (OCP); 4) Decorator or middleware for cross-cutting concerns like retries/logging; 5) Dependency injection for testability.",
            "moderate": "Creates an interface and implementing classes, but leaves the dispatch logic tightly coupled to switch statements or fails to address retries and extensible configuration.",
            "poor": "Writes a single monolithic class with hardcoded methods for each channel."
        },
        "hints": [
            "Define a common interface `NotificationChannel` with a `send(recipient, message)` method.",
            "How can you use the Decorator Pattern to add retry logic or rate-limiting to any channel without rewriting each channel's code?"
        ],
        "follow_up": "How would you handle asynchronous dispatch or backpressure if one of the third-party providers (e.g. Slack API) slows down?",
        "curveball": "Curveball Twist: If a user specifies fallback rules (e.g. 'If Push fails after 2 attempts, fallback to SMS; if SMS fails, send Email'), how do you model this using Chain of Responsibility or Composite Pattern without hardcoding fallbacks?",
        "model_answer": """**Extensible Notification System Architecture:**

```python
from abc import ABC, abstractmethod
from typing import List

# 1. Core Abstraction (DIP & OCP)
class NotificationChannel(ABC):
    @abstractmethod
    def send(self, recipient: str, message: str) -> bool:
        pass

# 2. Concrete Channel Implementations
class EmailChannel(NotificationChannel):
    def send(self, recipient: str, message: str) -> bool:
        print(f"[Email] Sending to {recipient}: {message}")
        return True

class SlackChannel(NotificationChannel):
    def send(self, recipient: str, message: str) -> bool:
        print(f"[Slack] Posting to channel {recipient}: {message}")
        return True

# 3. Decorator for Retries & Cross-Cutting Concerns
class RetryingChannel(NotificationChannel):
    def __init__(self, wrapped: NotificationChannel, max_retries: int = 3):
        self.wrapped = wrapped
        self.max_retries = max_retries

    def send(self, recipient: str, message: str) -> bool:
        for attempt in range(1, self.max_retries + 1):
            try:
                if self.wrapped.send(recipient, message):
                    return True
            except Exception as e:
                print(f"Attempt {attempt} failed: {e}")
        return False

# 4. Orchestrator Service (Open for extension, closed for modification)
class NotificationService:
    def __init__(self, channels: List[NotificationChannel]):
        self.channels = channels

    def notify_all(self, recipient: str, message: str):
        for channel in self.channels:
            channel.send(recipient, message)
```"""
    }
]

def get_question_by_id(qid: str):
    for q in QUESTIONS:
        if q["id"] == qid:
            return q
    return None

def filter_questions(topic="all", difficulty="all"):
    filtered = QUESTIONS
    if topic and topic != "all":
        filtered = [q for q in filtered if q["topic"] == topic]
    if difficulty and difficulty != "all":
        filtered = [q for q in filtered if difficulty.lower() in q["difficulty"].lower()]
    return filtered
