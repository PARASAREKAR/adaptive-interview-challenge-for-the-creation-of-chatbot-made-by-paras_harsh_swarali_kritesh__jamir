using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Threading;

namespace OOPChatbotWpf
{
    // =========================================================================
    // 1. DATA MODEL
    // =========================================================================
    public class OOPTopic
    {
        public string Id { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string[] Keywords { get; set; }
        public string ShortSummary { get; set; }
        public string CoreExplanation { get; set; }
        public string CodeExample { get; set; }
        public string TradeoffsAndGotchas { get; set; }
        public string InterviewerProTip { get; set; }
        public string SpokenSummary { get; set; }
    }

    // =========================================================================
    // 2. KNOWLEDGE REPOSITORY
    // =========================================================================
    public static class OOPKnowledgeRepository
    {
        public static readonly List<OOPTopic> Topics = new List<OOPTopic>();

        static OOPKnowledgeRepository()
        {
            // -----------------------------------------------------------------
            // THE 4 PILLARS
            // -----------------------------------------------------------------
            Topics.Add(new OOPTopic
            {
                Id = "encap_vs_abstract",
                Category = "🏛️ THE 4 PILLARS",
                Title = "Encapsulation vs Abstraction",
                Keywords = new string[] { "encapsulation", "abstraction", "data hiding", "access modifiers", "private", "public", "hide complexity" },
                ShortSummary = "Encapsulation packages and protects data via access modifiers (data hiding). Abstraction simplifies complexity by revealing only essential interfaces.",
                CoreExplanation = "• Encapsulation is about PACKAGING and PROTECTION.\nIt bundles attributes and methods together into a cohesive class while restricting direct access to internal fields using access specifiers (private, protected). This ensures invariants cannot be corrupted from the outside.\n\n• Abstraction is about SIMPLIFICATION.\nIt hides implementation details and exposes only high-level interfaces to callers. It answers WHAT an object does rather than HOW it does it.\n\nReal-World Analogy (Automobile):\n- Abstraction: The steering wheel, accelerator, and brake pedals. You press the pedal to accelerate without needing to understand fuel injection ratios or transmission fluid pressure.\n- Encapsulation: The engine and transmission are sealed under the hood. You cannot directly reach in and tweak the fuel-to-air ratio variable while driving; you interact exclusively through controlled interfaces.",
                CodeExample = "// C# Example of Encapsulation & Abstraction:\npublic interface IBankAccount // Abstraction: What it does\n{\n    void Deposit(decimal amount);\n    bool Withdraw(decimal amount);\n    decimal Balance { get; }\n}\n\npublic class BankAccount : IBankAccount\n{\n    // Encapsulation: State is private & protected from invalid mutations\n    private decimal _balance;\n\n    public decimal Balance => _balance;\n\n    public void Deposit(decimal amount)\n    {\n        if (amount <= 0) throw new ArgumentException(\"Deposit must be positive\");\n        _balance += amount;\n    }\n\n    public bool Withdraw(decimal amount)\n    {\n        if (amount <= 0 || amount > _balance) return false;\n        _balance -= amount;\n        return true;\n    }\n}",
                TradeoffsAndGotchas = "Gotcha: Do not create classes with public getters and setters for every private field. Trivial getters/setters for everything is an anti-pattern known as an 'Anemic Domain Model' that breaks true encapsulation.",
                InterviewerProTip = "Senior Interview Tip: Mention that Encapsulation preserves domain invariants, while Abstraction decouples consumers from changes in concrete implementations.",
                SpokenSummary = "Encapsulation is about bundling data and protecting state with access modifiers, while Abstraction simplifies complexity by exposing clean interfaces. Think of a car: the pedals are an abstraction for driving, while the sealed engine under the hood encapsulates the mechanical state."
            });

            Topics.Add(new OOPTopic
            {
                Id = "overload_vs_override",
                Category = "🏛️ THE 4 PILLARS",
                Title = "Method Overloading vs Method Overriding",
                Keywords = new string[] { "overloading", "overriding", "compile time", "runtime", "static polymorphism", "dynamic polymorphism", "vtable" },
                ShortSummary = "Overloading is compile-time (static) polymorphism with the same method name but different parameter signatures. Overriding is runtime (dynamic) polymorphism where a subclass provides a specific implementation of a virtual method.",
                CoreExplanation = "• Method Overloading (Static / Compile-Time Polymorphism):\nOccurs within the same class (or across hierarchy). Multiple methods share the same name but have DIFFERENT parameter types, counts, or order. The compiler determines which method to invoke during compilation based on the call signature.\n\n• Method Overriding (Dynamic / Run-Time Polymorphism):\nOccurs across inheritance boundaries. A subclass provides its own specific implementation of a method declared in its superclass. The signature (name, return type, parameters) must match. The method to execute is resolved at runtime based on the actual object on the heap via dynamic dispatch (vtables).",
                CodeExample = "public class Calculator\n{\n    // Overloading: Same name, different parameters (Compile-Time)\n    public int Add(int a, int b) => a + b;\n    public double Add(double a, double b) => a + b;\n    public int Add(int a, int b, int c) => a + b + c;\n}\n\npublic class Animal\n{\n    // Overriding: Virtual method in base class\n    public virtual void MakeSound() => Console.WriteLine(\"Generic animal sound\");\n}\n\npublic class Dog : Animal\n{\n    // Overriding: Run-Time dynamic dispatch\n    public override void MakeSound() => Console.WriteLine(\"Bark! Bark!\");\n}",
                TradeoffsAndGotchas = "Gotcha: In Java and C#, static and private methods cannot be overridden because they are resolved via early binding at compile-time without dynamic dispatch.",
                InterviewerProTip = "Top Interview Tip: Always connect overloading to early binding and overriding to late binding / dynamic dispatch.",
                SpokenSummary = "Method overloading happens at compile time when methods share the same name but take different parameters. Method overriding happens at runtime when a subclass replaces a virtual method from its parent class using dynamic dispatch."
            });

            Topics.Add(new OOPTopic
            {
                Id = "dynamic_dispatch_vtable",
                Category = "🏛️ THE 4 PILLARS",
                Title = "Dynamic Dispatch & Virtual Tables (vtable)",
                Keywords = new string[] { "dynamic dispatch", "vtable", "vptr", "virtual table", "under the hood", "function pointer", "virtual keyword" },
                ShortSummary = "Dynamic dispatch is the runtime mechanism that resolves overridden virtual methods using class vtables and object vptrs.",
                CoreExplanation = "How dynamic polymorphism works under the hood in C++ and Java:\n\n1. vtable (Virtual Table):\nFor every class that declares or overrides a virtual method, the compiler constructs a static array of function pointers called the vtable. There is only ONE vtable per class, stored in read-only memory.\n\n2. vptr (Virtual Pointer):\nEvery instance (object) allocated on the heap contains an implicit hidden pointer, the vptr, placed at the start of its memory layout. The vptr points to the vtable of the object's actual concrete runtime type.\n\n3. Execution Mechanics:\nWhen your code calls animal->MakeSound(), the CPU performs an indirect call:\n  Step 1: Read the object's vptr address.\n  Step 2: Follow vptr to the class's vtable.\n  Step 3: Read the function pointer at the specific offset reserved for MakeSound().\n  Step 4: Jump to that function address.",
                CodeExample = "// Memory Layout Conceptual Diagram:\n// [Object Dog in Heap] -> [vptr] ----> [Dog vtable in Read-Only Memory]\n//                         [name]       [0]: &Dog.MakeSound()\n//                         [age]        [1]: &Dog.ToString()\n\nAnimal a = new Dog();\na.MakeSound(); // CPU follows a.vptr -> Dog vtable -> executes Dog.MakeSound()",
                TradeoffsAndGotchas = "Performance Trade-offs: Calling a virtual method incurs a small pointer indirection cost (potential cache miss) and inhibits the compiler from inlining the function call.",
                InterviewerProTip = "Staff Engineer Tip: Mention that Modern branch predictors and JIT devirtualization optimize away most vtable overhead when an object type remains stable.",
                SpokenSummary = "Dynamic dispatch is powered by virtual method tables and pointers. Every class with virtual methods has a static lookup table of function pointers, and each object stores a hidden pointer to its class table. At runtime, the computer looks up the pointer to execute the correct subclass implementation."
            });

            Topics.Add(new OOPTopic
            {
                Id = "abstract_vs_interface",
                Category = "🏛️ THE 4 PILLARS",
                Title = "Abstract Class vs Interface",
                Keywords = new string[] { "abstract class", "interface", "default methods", "multiple inheritance", "is a", "can do", "when to use" },
                ShortSummary = "Abstract classes define core identity ('IS-A') with shared state/constructors. Interfaces define behavioral contracts ('CAN-DO') across unrelated classes.",
                CoreExplanation = "• Choose an Abstract Class when:\n1. Shared State: You need to define instance fields, mutable properties, or internal state shared across derived classes.\n2. Constructors: You need initialization logic enforced across subclasses.\n3. Core Identity ('IS-A'): The classes form a tight family (e.g., AbstractVehicle -> Car, Truck).\n4. Template Method: You want to provide a concrete execution algorithm with protected overridable hook points.\n\n• Choose an Interface when:\n1. Behavioral Contract ('CAN-DO'): Defining capabilities across completely unrelated classes (e.g., IComparable, ISerializable, IFlyable for both Bird and Airplane).\n2. Multiple Contracts: A class needs to adhere to multiple distinct behaviors.\n3. Loose Coupling & Dependency Injection: Mocking components in unit tests.\n\nModern Note: While modern C# and Java interfaces support default implementation methods, interfaces STILL CANNOT declare instance state (instance variables) or constructors.",
                CodeExample = "// Abstract Class (Identity + State)\npublic abstract class Vehicle\n{\n    protected string _vin;\n    public Vehicle(string vin) { _vin = vin; } // Constructor allowed!\n    public abstract void StartEngine();\n}\n\n// Interface (Capability contract across unrelated types)\npublic interface IFlyable\n{\n    void Fly();\n    void Land();\n}",
                TradeoffsAndGotchas = "Gotcha: Default methods in interfaces were added for backward-compatible API evolution, not to replace abstract classes.",
                InterviewerProTip = "Interview Tip: Highlight that an abstract class is about identity while an interface is about capability.",
                SpokenSummary = "Use an abstract class when you need shared instance state, constructors, and core identity. Use an interface when you are defining capabilities and contracts across unrelated classes without tying them to an inheritance tree."
            });

            // -----------------------------------------------------------------
            // SOLID PRINCIPLES
            // -----------------------------------------------------------------
            Topics.Add(new OOPTopic
            {
                Id = "srp",
                Category = "🛡️ SOLID PRINCIPLES",
                Title = "Single Responsibility Principle (SRP)",
                Keywords = new string[] { "srp", "single responsibility", "reason to change", "actor", "stakeholder", "cohesion", "god class" },
                ShortSummary = "A class should have one, and only one, reason to change — meaning it should be responsible to only one actor or stakeholder.",
                CoreExplanation = "Robert C. Martin clarified that a 'reason to change' maps directly to an ACTOR or STAKEHOLDER who requests modifications:\n\n• Classic Violation (God Class):\nAn Employee class containing:\n  1. CalculatePay() -> Requested by Finance / CFO.\n  2. ReportHours() -> Requested by HR / COO.\n  3. SaveToDb() -> Requested by DBA / System Admin.\n\nIf Finance requests an update to overtime calculation, that logic change could inadvertently alter HR's timesheet report.\n\n• The Fix:\nSeparate responsibilities into distinct classes:\n  - Employee: Pure domain entity holding employee data.\n  - PayCalculator: Financial logic for Finance.\n  - TimesheetReporter: Time tracking & formatting for HR.\n  - EmployeeRepository: Persistence logic for the database.",
                CodeExample = "// Refactored to SRP:\npublic class Employee { public int Id; public string Name; public decimal HourlyRate; }\n\npublic class PayCalculator \n{\n    public decimal CalculateWeeklyPay(Employee emp, double hours) => emp.HourlyRate * (decimal)hours;\n}\n\npublic class EmployeeRepository \n{\n    public void Save(Employee emp) { /* Database persistence */ }\n}",
                TradeoffsAndGotchas = "Gotcha: SRP does not mean a class should only have a single method! It means all methods in the class should belong to one cohesive responsibility.",
                InterviewerProTip = "Interview Tip: Explain that violating SRP creates high merge conflict risk and regression cascades when multiple teams work on the same file.",
                SpokenSummary = "The Single Responsibility Principle states that a class should have only one reason to change, meaning it serves only one stakeholder. Instead of a single class that handles business calculations, database saving, and email notifications, break each task into its own cohesive module."
            });

            Topics.Add(new OOPTopic
            {
                Id = "ocp",
                Category = "🛡️ SOLID PRINCIPLES",
                Title = "Open/Closed Principle (OCP)",
                Keywords = new string[] { "ocp", "open closed", "open for extension", "closed for modification", "polymorphism", "avoid switch" },
                ShortSummary = "Software entities should be open for extension (new features added via new code), but closed for modification (existing tested code left untouched).",
                CoreExplanation = "• Closed for Modification: Once a class is tested and released into production, you should avoid modifying its source code to add new variants or features.\n\n• Open for Extension: You should be able to introduce new behavior simply by writing new classes that implement an abstraction.\n\n• How to Achieve OCP:\nUse Polymorphism, Interfaces, or the Strategy Pattern instead of switch or if-else ladders.\n\nExample:\nIn an e-commerce checkout, instead of a switch(paymentType) that gets edited every time Apple Pay, PayPal, or Crypto is added, define an IPaymentMethod interface. To add Crypto, simply write a CryptoPayment class without touching existing checkout logic.",
                CodeExample = "public interface IPaymentMethod\n{\n    void Process(decimal amount);\n}\n\npublic class CreditCardPayment : IPaymentMethod\n{\n    public void Process(decimal amount) => Console.WriteLine($\"Charged credit card: {amount}\");\n}\n\npublic class PayPalPayment : IPaymentMethod\n{\n    public void Process(decimal amount) => Console.WriteLine($\"Charged PayPal: {amount}\");\n}\n\n// Adding BitcoinPayment requires ZERO changes to CheckoutService!\npublic class CheckoutService\n{\n    public void CompleteOrder(IPaymentMethod method, decimal total)\n    {\n        method.Process(total); // Never needs modification!\n    }\n}",
                TradeoffsAndGotchas = "Gotcha: Do not prematurely abstract everything on day one. Follow the Rule of Three: write concrete code first, and introduce OCP abstractions once a third variant arrives.",
                InterviewerProTip = "Interview Tip: Highlight that OCP drastically reduces regression testing scope because existing binaries are untouched.",
                SpokenSummary = "The Open-Closed Principle means your software should be open for extension but closed for modification. When you need to add a new payment method or reporting format, you introduce a new class implementing an interface, rather than adding another branch to an existing if statement."
            });

            Topics.Add(new OOPTopic
            {
                Id = "lsp",
                Category = "🛡️ SOLID PRINCIPLES",
                Title = "Liskov Substitution Principle (LSP)",
                Keywords = new string[] { "lsp", "liskov", "behavioral subtyping", "square rectangle", "invariants", "preconditions", "postconditions" },
                ShortSummary = "Subtypes must be substitutable for their supertypes without altering the correctness of the program (Behavioral Subtyping).",
                CoreExplanation = "Barbara Liskov's principle states that inheritance must preserve behavioral contracts, not just method signatures.\n\n• The Classic Square-Rectangle Violation:\nIn mathematics, a Square is a Rectangle. But in OOP with mutable state:\nIf class Square inherits Rectangle, setting Square.Width also changes Height to keep sides equal.\nIf a caller expects a Rectangle and runs:\n  rect.SetWidth(5);\n  rect.SetHeight(4);\n  assert(rect.Area() == 20);\nIf passed a Square, Area() returns 16! The subclass silently broke the caller's invariant that changing height leaves width unaffected.\n\n• Formal Rules of LSP:\n1. Preconditions cannot be strengthened in a subtype (subclass cannot demand stricter input).\n2. Postconditions cannot be weakened in a subtype (subclass cannot guarantee less).\n3. Invariants of the supertype must be preserved.\n4. Subtypes must not throw unexpected exceptions (e.g. NotImplementedException).",
                CodeExample = "// LSP Compliant Design: Separate into read-only abstractions\npublic interface IShape\n{\n    double CalculateArea();\n}\n\npublic class Rectangle : IShape\n{\n    public double Width { get; set; }\n    public double Height { get; set; }\n    public double CalculateArea() => Width * Height;\n}\n\npublic class Square : IShape\n{\n    public double Side { get; set; }\n    public double CalculateArea() => Side * Side;\n}",
                TradeoffsAndGotchas = "Gotcha: Just because something is an 'IS-A' relationship in the real world does not mean it qualifies as an inheritance relationship in object-oriented software.",
                InterviewerProTip = "Top Tip: Mention 'Design by Contract' and explain that throwing NotImplementedException in a subclass is an automatic LSP violation.",
                SpokenSummary = "The Liskov Substitution Principle requires that any subclass must be usable wherever its parent class is expected, without breaking caller assumptions. For example, making a Square inherit from a Rectangle fails because setting a square's width unexpectedly mutates its height, breaking the rectangle contract."
            });

            Topics.Add(new OOPTopic
            {
                Id = "dip",
                Category = "🛡️ SOLID PRINCIPLES",
                Title = "Dependency Inversion Principle (DIP)",
                Keywords = new string[] { "dip", "dependency inversion", "dependency injection", "inversion of control", "high level modules", "abstractions" },
                ShortSummary = "High-level modules should not depend on low-level modules; both should depend on abstractions.",
                CoreExplanation = "• The Principle (DIP):\nHigh-level business policies (e.g. OrderProcessingService) should not import or instantiate low-level infrastructure details (e.g. SqlDatabaseDriver, SmtpMailClient). Both should depend on abstractions (interfaces).\n\n• Inversion of Control (IoC) - The Paradigm:\nThe architectural flow of control is inverted. Instead of your custom code calling libraries and controlling execution, a framework manages the lifecycle and calls your code.\n\n• Dependency Injection (DI) - The Mechanism:\nA practical technique where dependencies are supplied ('injected') from the outside, typically via Constructor Injection, rather than instantiated internally using the 'new' keyword.",
                CodeExample = "public interface IMessageSender\n{\n    void Send(string recipient, string message);\n}\n\npublic class NotificationService\n{\n    private readonly IMessageSender _sender;\n\n    // Injected dependency via constructor (DIP & DI compliant!)\n    public NotificationService(IMessageSender sender)\n    {\n        _sender = sender;\n    }\n\n    public void AlertUser(string user, string msg)\n    {\n        _sender.Send(user, msg);\n    }\n}",
                TradeoffsAndGotchas = "Gotcha: Do not confuse DIP (the architectural principle), IoC (the framework paradigm), and DI (the constructor pattern). They are three different tiers of the same concept.",
                InterviewerProTip = "Staff Engineer Tip: Explain how DIP enables Clean Architecture and Hexagonal Architecture by placing domain entities at the center of the dependency graph.",
                SpokenSummary = "The Dependency Inversion Principle states that business logic should never directly depend on low-level database or network drivers. Both must depend on abstractions. Using constructor dependency injection allows you to easily swap components and mock dependencies during unit testing."
            });

            // -----------------------------------------------------------------
            // DESIGN PATTERNS
            // -----------------------------------------------------------------
            Topics.Add(new OOPTopic
            {
                Id = "singleton_thread_safe",
                Category = "📐 DESIGN PATTERNS",
                Title = "Thread-Safe Singleton Pattern",
                Keywords = new string[] { "singleton", "thread safe", "double checked locking", "anti pattern", "volatile", "global state" },
                ShortSummary = "Ensures a class has only one instance while providing global access. Must be implemented with thread safety, but watch out for anti-pattern risks.",
                CoreExplanation = "• Thread-Safe Implementation (Double-Checked Locking):\n1. Make the constructor private so external callers cannot use 'new'.\n2. Declare a private static volatile instance variable (volatile prevents CPU instruction reordering).\n3. In GetInstance(), check if instance == null. If null, acquire a lock and check if instance == null a second time before constructing.\n\n• Why Singleton is frequently criticized as an Anti-Pattern:\n1. Hidden Dependencies: Callers call Database.Instance internally rather than declaring dependencies explicitly via constructors.\n2. Global State: Violates encapsulation by introducing mutable global state.\n3. Prevents Unit Testing: Cannot be mocked or swapped with test doubles.\n4. Violates Single Responsibility: It manages its own lifecycle AND executes domain logic.",
                CodeExample = "public sealed class DatabaseConnection\n{\n    private static volatile DatabaseConnection _instance;\n    private static readonly object _padlock = new object();\n\n    private DatabaseConnection() { /* Private constructor */ }\n\n    public static DatabaseConnection Instance\n    {\n        get\n        {\n            if (_instance == null) // First check\n            {\n                lock (_padlock)\n                {\n                    if (_instance == null) // Second check\n                    {\n                        _instance = new DatabaseConnection();\n                    }\n                }\n            }\n            return _instance;\n        }\n    }\n}",
                TradeoffsAndGotchas = "Gotcha: In modern development, configure classes as Singleton Lifecycles inside a Dependency Injection container rather than writing manual Singleton classes.",
                InterviewerProTip = "Interview Tip: Always mention the volatile keyword in double-checked locking and explain why CPU memory barrier reordering can cause race conditions without it.",
                SpokenSummary = "A thread-safe singleton uses a private constructor and double-checked locking to ensure only one instance is ever created across multiple threads. While useful for hardware loggers or driver connections, it can become an anti-pattern if treated as global state because it makes isolated unit testing difficult."
            });

            Topics.Add(new OOPTopic
            {
                Id = "strategy_vs_state",
                Category = "📐 DESIGN PATTERNS",
                Title = "Strategy Pattern vs State Pattern",
                Keywords = new string[] { "strategy pattern", "state pattern", "finite state machine", "intent", "swapping algorithms" },
                ShortSummary = "Strategy and State share similar UML class structures, but Strategy swaps independent algorithms while State models an internal finite state machine.",
                CoreExplanation = "• Strategy Pattern:\n- Intent: Provide interchangeable algorithms or business policies.\n- Client Role: The client or configuration selects the concrete strategy (e.g., QuickSort vs MergeSort, or FedEx vs UPS shipping).\n- Awareness: Concrete strategies are completely oblivious and independent of each other.\n\n• State Pattern:\n- Intent: Allow an object to alter its behavior when its internal state changes (Finite State Machine).\n- Client Role: The client treats the context as a single entity; state transitions happen automatically based on events.\n- Awareness: Concrete state classes often know about other states to drive transitions (e.g., Draft -> InReview -> Published).",
                CodeExample = "// Strategy Pattern: Configurable algorithm\npublic interface ICompressionStrategy { void Compress(string file); }\n\n// State Pattern: Transitions driven by state\npublic interface IOrderState { void Next(OrderContext ctx); }\npublic class PendingState : IOrderState \n{\n    public void Next(OrderContext ctx) => ctx.State = new PaidState(); // Knows next state!\n}",
                TradeoffsAndGotchas = "Gotcha: If your states don't change based on events and don't trigger transitions, you probably need a Strategy pattern, not a State pattern.",
                InterviewerProTip = "Interview Tip: State patterns encapsulate transitions so you can eliminate long switch(currentState) statements.",
                SpokenSummary = "Although Strategy and State look identical in UML diagrams, their purpose is completely different. Strategy lets a client swap independent algorithms, while State models internal transitions in a finite state machine where states trigger transitions automatically."
            });

            Topics.Add(new OOPTopic
            {
                Id = "composition_over_inheritance",
                Category = "⚡ ADVANCED ARCHITECTURE",
                Title = "Favor Composition Over Inheritance",
                Keywords = new string[] { "composition over inheritance", "has a", "is a", "fragile base class", "tight coupling", "delegation" },
                ShortSummary = "Inheritance creates tight compile-time coupling and breaks encapsulation. Composition creates flexible, loosely coupled systems with runtime swappability.",
                CoreExplanation = "• The Problems with Inheritance (IS-A / White-Box Reuse):\n1. Breaks Encapsulation: Subclasses depend on internal implementation details of superclasses.\n2. Compile-Time Inflexibility: An object cannot change its superclass at runtime.\n3. Class Explosion: Creating deep hierarchies for every combination of features.\n4. The Fragile Base Class Problem: An innocent update to a base class method can silently break subclasses that override dependent methods.\n\n• The Virtues of Composition (HAS-A / Black-Box Reuse):\n1. Strong Encapsulation: Objects interact strictly through public interfaces.\n2. Runtime Flexibility: Composed strategies can be swapped dynamically.\n3. Testability: Dependencies can be mocked effortlessly.",
                CodeExample = "// Bad: Inheritance class explosion (FlyingSwimmingRobot, WalkingRobot...)\n// Good: Composition\npublic class Robot\n{\n    private readonly IMovementBehavior _movement;\n    private readonly IToolBehavior _tool;\n\n    public Robot(IMovementBehavior movement, IToolBehavior tool)\n    {\n        _movement = movement;\n        _tool = tool;\n    }\n\n    public void Action() { _movement.Move(); _tool.Operate(); }\n}",
                TradeoffsAndGotchas = "Gotcha: Use inheritance only when true subtyping applies (LSP holds) and polymorphic substitutability is genuinely required.",
                InterviewerProTip = "Staff Engineer Tip: Recite the classic Fragile Base Class example: HashSet.AddAll calling Add internally, causing custom counting subclasses to double-count elements.",
                SpokenSummary = "Favor composition over inheritance is a core design maxim because inheritance tightly couples classes and exposes internal base class quirks, known as the fragile base class problem. Composition delegates behavior to interfaces, keeping classes modular, testable, and swappable at runtime."
            });

            Topics.Add(new OOPTopic
            {
                Id = "diamond_problem_mro",
                Category = "⚡ ADVANCED ARCHITECTURE",
                Title = "The Diamond Problem & C3 MRO",
                Keywords = new string[] { "diamond problem", "multiple inheritance", "mro", "c3 linearization", "virtual inheritance", "ambiguity" },
                ShortSummary = "The Diamond Problem arises when a class inherits from two parents that share a common ancestor. Solved via C++ virtual inheritance, Java interface disambiguation, or Python C3 MRO.",
                CoreExplanation = "• The Diamond Problem:\nClass D inherits from B and C, both of which inherit from A:\n      A (defines Foo)\n     / \\\n    B   C (both override Foo)\n     \\ /\n      D (calls Foo -> Which one runs?)\n\nAmbiguities:\n1. Method Ambiguity: Does D call B.Foo() or C.Foo()?\n2. State Duplication: Without special handling, D contains two separate copies of A's member variables.\n\n• How Different Languages Solve It:\n1. C++: Supports virtual base classes (class B : virtual public A). Ensures only one shared instance of A exists. Ambiguities must be resolved with scope resolution (B::Foo()).\n2. Java: Forbade multiple class inheritance entirely. For interface default methods, Java forces the subclass to explicitly disambiguate.\n3. Python: Supports multiple inheritance using Method Resolution Order (MRO) with the C3 Linearization algorithm (Class.__mro__), guaranteeing consistent, deterministic order.",
                CodeExample = "# Python C3 MRO Resolution:\nclass A:\n    def speak(self): print('A')\nclass B(A):\n    def speak(self): print('B')\nclass C(A):\n    def speak(self): print('C')\nclass D(B, C):\n    pass\n\n# D().__class__.__mro__ -> (D, B, C, A, object)",
                TradeoffsAndGotchas = "Gotcha: Avoid deep multiple inheritance graphs even in languages that support them. Prefer multiple interface implementation.",
                InterviewerProTip = "Interview Tip: Highlight that Python's super() does not simply call the parent class; it calls the next class in the C3 MRO chain.",
                SpokenSummary = "The Diamond Problem occurs in multiple inheritance when a class inherits from two classes with a common ancestor, creating ambiguity over which method and state copy to invoke. C++ solves this with virtual inheritance, Java forbids multiple class inheritance, and Python uses C3 linearization to order the method resolution chain."
            });
        }

        public static OOPTopic FindBestMatch(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return Topics[0];

            string q = query.ToLower();
            OOPTopic bestMatch = null;
            int maxScore = 0;

            foreach (var t in Topics)
            {
                int score = 0;
                string titleLower = t.Title.ToLower();

                if (q.Contains(titleLower) || titleLower.Contains(q)) score += 50;

                foreach (var kw in t.Keywords)
                {
                    if (q.Contains(kw.ToLower())) score += 15;
                }

                string[] tokens = q.Split(new char[] { ' ', '?', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var tok in tokens)
                {
                    if (tok.Length > 2)
                    {
                        if (titleLower.Contains(tok)) score += 8;
                        if (t.CoreExplanation.ToLower().Contains(tok)) score += 2;
                    }
                }

                if (score > maxScore)
                {
                    maxScore = score;
                    bestMatch = t;
                }
            }

            return bestMatch ?? Topics[0];
        }
    }

    // =========================================================================
    // 3. NATURAL ONECORE SPEECH ENGINE (MICROSOFT MARK / ZIRA / DAVID)
    // =========================================================================
    public static class NaturalVoiceEngine
    {
        private static dynamic _voice = null;
        private static dynamic _oneCoreTokens = null;
        private static dynamic _sapiTokens = null;
        public static string CurrentVoiceName = "Mark (Natural Male)";

        public static void Initialize()
        {
            try
            {
                _voice = Activator.CreateInstance(Type.GetTypeFromProgID("SAPI.SpVoice"));

                // Load OneCore high-definition natural voice tokens
                try
                {
                    dynamic cat = Activator.CreateInstance(Type.GetTypeFromProgID("SAPI.SpObjectTokenCategory"));
                    cat.SetId(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech_OneCore\Voices", false);
                    _oneCoreTokens = cat.EnumerateTokens();
                }
                catch { }

                // Fallback SAPI tokens
                try { _sapiTokens = _voice.GetVoices(); } catch { }

                // Default to Microsoft Mark (modern natural male)
                SelectVoice("Mark");
                _voice.Rate = -1; // Natural conversational tempo
                _voice.Volume = 100;
            }
            catch { }
        }

        public static void SelectVoice(string voiceKeyword)
        {
            if (_voice == null) return;
            try
            {
                if (_oneCoreTokens != null)
                {
                    for (int i = 0; i < _oneCoreTokens.Count; i++)
                    {
                        string desc = _oneCoreTokens.Item(i).GetDescription();
                        if (desc.IndexOf(voiceKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            _voice.Voice = _oneCoreTokens.Item(i);
                            CurrentVoiceName = desc;
                            return;
                        }
                    }
                }
                if (_sapiTokens != null)
                {
                    for (int i = 0; i < _sapiTokens.Count; i++)
                    {
                        string desc = _sapiTokens.Item(i).GetDescription();
                        if (desc.IndexOf(voiceKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            _voice.Voice = _sapiTokens.Item(i);
                            CurrentVoiceName = desc;
                            return;
                        }
                    }
                }
            }
            catch { }
        }

        public static void Speak(string text)
        {
            Stop();
            ThreadPool.QueueUserWorkItem((state) =>
            {
                try
                {
                    if (_voice != null)
                    {
                        // Clean text so it sounds like a human speaking
                        string clean = Regex.Replace(text, @"[`*_#\-\\]", "");
                        clean = Regex.Replace(clean, @"```[\s\S]*?```", "");
                        _voice.Speak(clean, 1); // SVSFlagsAsync
                    }
                }
                catch { }
            });
        }

        public static void Stop()
        {
            try
            {
                if (_voice != null) _voice.Speak("", 2); // SVSFPurgeBeforeSpeak
            }
            catch { }
        }
    }

    // =========================================================================
    // 4. MODERN SLICK WPF WINDOW UI
    // =========================================================================
    public class MainWindow : Window
    {
        // Modern Theme Palette (Tailwind Slate-950 aesthetic)
        private readonly SolidColorBrush BrushBgWindow = new SolidColorBrush(Color.FromRgb(11, 15, 25));       // #0B0F19
        private readonly SolidColorBrush BrushBgCard = new SolidColorBrush(Color.FromRgb(17, 24, 39));         // #111827
        private readonly SolidColorBrush BrushBgSidebar = new SolidColorBrush(Color.FromRgb(14, 20, 36));      // #0E1424
        private readonly SolidColorBrush BrushBgInput = new SolidColorBrush(Color.FromRgb(19, 27, 46));        // #131B2E
        private readonly SolidColorBrush BrushBorder = new SolidColorBrush(Color.FromRgb(30, 41, 59));          // #1E293B
        private readonly SolidColorBrush BrushIndigo = new SolidColorBrush(Color.FromRgb(99, 102, 241));       // #6366F1
        private readonly SolidColorBrush BrushEmerald = new SolidColorBrush(Color.FromRgb(52, 211, 153));      // #34D399
        private readonly SolidColorBrush BrushSky = new SolidColorBrush(Color.FromRgb(56, 189, 248));          // #38BDF8
        private readonly SolidColorBrush BrushAmber = new SolidColorBrush(Color.FromRgb(245, 158, 11));        // #F59E0B
        private readonly SolidColorBrush BrushTextLight = new SolidColorBrush(Color.FromRgb(248, 250, 252));   // #F8FAFC
        private readonly SolidColorBrush BrushTextDim = new SolidColorBrush(Color.FromRgb(148, 163, 184));     // #94A3B8

        // Controls
        private ComboBox _voiceCombo;
        private TextBox _searchBox;
        private StackPanel _topicsListPanel;
        private ScrollViewer _chatScrollViewer;
        private StackPanel _chatMessagesPanel;
        private TextBox _inputPrompt;
        private Button _btnSend;
        private string _lastCopiedText = "";

        public MainWindow()
        {
            this.Title = "OOP Technical AI Assistant (Ask Me Anything)";
            this.Width = 1180;
            this.Height = 820;
            this.MinWidth = 920;
            this.MinHeight = 650;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Background = BrushBgWindow;
            this.FontFamily = new FontFamily("Segoe UI");

            this.Closing += (s, e) => {
                try { NaturalVoiceEngine.Stop(); } catch { }
            };
            this.Closed += (s, e) => {
                try { NaturalVoiceEngine.Stop(); } catch { }
                try { if (Application.Current != null) Application.Current.Shutdown(); } catch { }
                try { System.Diagnostics.Process.GetCurrentProcess().Kill(); } catch { }
                Environment.Exit(0);
            };

            BuildModernLayout();
            NaturalVoiceEngine.Initialize();

            // Display introductory greeting
            ShowGreeting();
        }

        private void BuildModernLayout()
        {
            var rootGrid = new Grid();
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(68) }); // Header
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Main Body

            // =================================================================
            // HEADER BAR
            // =================================================================
            var headerBorder = new Border
            {
                Background = BrushBgCard,
                BorderBrush = BrushBorder,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(24, 0, 24, 0)
            };
            Grid.SetRow(headerBorder, 0);

            var headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Left Brand
            var brandPanel = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
            var iconCircle = new Border
            {
                Width = 42,
                Height = 42,
                CornerRadius = new CornerRadius(21),
                Background = new SolidColorBrush(Color.FromArgb(40, 99, 102, 241)),
                BorderBrush = BrushIndigo,
                BorderThickness = new Thickness(1.5),
                Margin = new Thickness(0, 0, 14, 0)
            };
            iconCircle.Child = new TextBlock
            {
                Text = "⚡",
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            brandPanel.Children.Add(iconCircle);

            var titleStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            var titleText = new TextBlock
            {
                Text = "OOP Technical AI Assistant",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = BrushTextLight
            };
            var subtitleText = new TextBlock
            {
                Text = "Ask me anything about Object-Oriented Programming, SOLID, Patterns, & Architecture",
                FontSize = 11,
                Foreground = BrushSky,
                Margin = new Thickness(0, 2, 0, 0)
            };
            titleStack.Children.Add(titleText);
            titleStack.Children.Add(subtitleText);
            brandPanel.Children.Add(titleStack);
            Grid.SetColumn(brandPanel, 0);
            headerGrid.Children.Add(brandPanel);

            // Right Voice & Controls
            var rightControls = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };

            _voiceCombo = new ComboBox
            {
                Width = 220,
                Height = 36,
                Background = BrushBgInput,
                Foreground = BrushTextLight,
                BorderBrush = BrushBorder,
                Padding = new Thickness(8, 6, 8, 6),
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            };
            _voiceCombo.Items.Add("🎙️ Natural: Mark (Male)");
            _voiceCombo.Items.Add("🎙️ Natural: Zira (Female)");
            _voiceCombo.Items.Add("🎙️ Natural: David (Male)");
            _voiceCombo.Items.Add("🔇 Voice Muted (Text Only)");
            _voiceCombo.SelectedIndex = 0;
            _voiceCombo.SelectionChanged += (s, e) =>
            {
                if (_voiceCombo.SelectedIndex == 0) NaturalVoiceEngine.SelectVoice("Mark");
                else if (_voiceCombo.SelectedIndex == 1) NaturalVoiceEngine.SelectVoice("Zira");
                else if (_voiceCombo.SelectedIndex == 2) NaturalVoiceEngine.SelectVoice("David");
                else NaturalVoiceEngine.Stop();
            };
            rightControls.Children.Add(_voiceCombo);

            var btnStop = CreateStyledButton("⏹ Stop Voice", 100, 36, Color.FromRgb(47, 57, 77));
            btnStop.Click += (s, e) => NaturalVoiceEngine.Stop();
            rightControls.Children.Add(btnStop);

            Grid.SetColumn(rightControls, 1);
            headerGrid.Children.Add(rightControls);
            headerBorder.Child = headerGrid;
            rootGrid.Children.Add(headerBorder);

            // =================================================================
            // MAIN BODY: SIDEBAR + CHAT AREA
            // =================================================================
            var bodyGrid = new Grid();
            bodyGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(320) }); // Sidebar
            bodyGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Chat
            Grid.SetRow(bodyGrid, 1);

            // -----------------------------------------------------------------
            // SIDEBAR
            // -----------------------------------------------------------------
            var sidebarBorder = new Border
            {
                Background = BrushBgSidebar,
                BorderBrush = BrushBorder,
                BorderThickness = new Thickness(0, 0, 1, 0),
                Padding = new Thickness(16, 16, 16, 16)
            };
            Grid.SetColumn(sidebarBorder, 0);

            var sidebarGrid = new Grid();
            sidebarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Heading
            sidebarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Search
            sidebarGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Topics list
            sidebarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Action buttons

            var sidebarHeader = new TextBlock
            {
                Text = "💡 EXPLORE FREQUENT TOPICS",
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Foreground = BrushTextDim,
                Margin = new Thickness(4, 0, 0, 10)
            };
            Grid.SetRow(sidebarHeader, 0);
            sidebarGrid.Children.Add(sidebarHeader);

            _searchBox = new TextBox
            {
                Height = 36,
                Background = BrushBgInput,
                Foreground = BrushTextLight,
                BorderBrush = BrushBorder,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10, 8, 10, 8),
                FontSize = 12,
                Margin = new Thickness(0, 0, 0, 12)
            };
            _searchBox.Text = "Search topics (e.g. SOLID, vtable)...";
            _searchBox.GotFocus += (s, e) => { if (_searchBox.Text.StartsWith("Search")) _searchBox.Text = ""; };
            _searchBox.TextChanged += (s, e) => FilterTopics();
            Grid.SetRow(_searchBox, 1);
            sidebarGrid.Children.Add(_searchBox);

            var topicScroll = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Margin = new Thickness(0, 0, 0, 12)
            };
            _topicsListPanel = new StackPanel();
            topicScroll.Content = _topicsListPanel;
            Grid.SetRow(topicScroll, 2);
            sidebarGrid.Children.Add(topicScroll);

            var sidebarActions = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
            var btnClear = CreateStyledButton("🗑 Clear Chat", 135, 34, Color.FromRgb(30, 41, 59));
            btnClear.Margin = new Thickness(0, 0, 8, 0);
            btnClear.Click += (s, e) =>
            {
                _chatMessagesPanel.Children.Clear();
                ShowGreeting();
            };
            sidebarActions.Children.Add(btnClear);

            var btnCopy = CreateStyledButton("📋 Copy Answer", 135, 34, Color.FromRgb(30, 41, 59));
            btnCopy.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(_lastCopiedText))
                {
                    Clipboard.SetText(_lastCopiedText);
                    btnCopy.Content = "✔ Copied!";
                    var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                    timer.Tick += (ts, te) => { btnCopy.Content = "📋 Copy Answer"; timer.Stop(); };
                    timer.Start();
                }
            };
            sidebarActions.Children.Add(btnCopy);

            Grid.SetRow(sidebarActions, 3);
            sidebarGrid.Children.Add(sidebarActions);

            sidebarBorder.Child = sidebarGrid;
            bodyGrid.Children.Add(sidebarBorder);

            // -----------------------------------------------------------------
            // CHAT AREA & FLOATING INPUT DOCK
            // -----------------------------------------------------------------
            var chatContainer = new Grid();
            chatContainer.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Chat Stream
            chatContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Input Dock
            Grid.SetColumn(chatContainer, 1);

            _chatScrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Padding = new Thickness(32, 24, 32, 24)
            };
            _chatMessagesPanel = new StackPanel();
            _chatScrollViewer.Content = _chatMessagesPanel;
            Grid.SetRow(_chatScrollViewer, 0);
            chatContainer.Children.Add(_chatScrollViewer);

            // Floating Input Dock
            var inputDock = new Border
            {
                Background = BrushBgCard,
                BorderBrush = BrushBorder,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(14),
                Margin = new Thickness(32, 0, 32, 20),
                Padding = new Thickness(12, 10, 12, 10)
            };
            Grid.SetRow(inputDock, 1);

            var inputGrid = new Grid();
            inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            _inputPrompt = new TextBox
            {
                Background = Brushes.Transparent,
                Foreground = BrushTextLight,
                BorderThickness = new Thickness(0),
                FontSize = 13.5,
                AcceptsReturn = true,
                MaxHeight = 85,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                TextWrapping = TextWrapping.Wrap,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(6, 0, 12, 0)
            };
            _inputPrompt.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter && !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                {
                    e.Handled = true;
                    ProcessUserQuestion(_inputPrompt.Text);
                }
            };
            Grid.SetColumn(_inputPrompt, 0);
            inputGrid.Children.Add(_inputPrompt);

            _btnSend = CreateStyledButton("Ask AI ➔", 100, 42, Color.FromRgb(99, 102, 241));
            _btnSend.Click += (s, e) => ProcessUserQuestion(_inputPrompt.Text);
            Grid.SetColumn(_btnSend, 1);
            inputGrid.Children.Add(_btnSend);

            inputDock.Child = inputGrid;
            chatContainer.Children.Add(inputDock);

            bodyGrid.Children.Add(chatContainer);
            rootGrid.Children.Add(bodyGrid);

            this.Content = rootGrid;

            // Render topics into sidebar
            PopulateTopics(OOPKnowledgeRepository.Topics);
        }

        private Button CreateStyledButton(string text, double width, double height, Color bgColor)
        {
            var btn = new Button
            {
                Content = text,
                Width = width,
                Height = height,
                Background = new SolidColorBrush(bgColor),
                Foreground = Brushes.White,
                FontWeight = FontWeights.SemiBold,
                FontSize = 12,
                Cursor = Cursors.Hand,
                BorderThickness = new Thickness(0)
            };
            return btn;
        }

        private void PopulateTopics(IEnumerable<OOPTopic> topics)
        {
            _topicsListPanel.Children.Clear();
            foreach (var t in topics)
            {
                var card = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(21, 30, 50)),
                    BorderBrush = BrushBorder,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(12, 10, 12, 10),
                    Margin = new Thickness(0, 0, 0, 6),
                    Cursor = Cursors.Hand
                };

                var sp = new StackPanel();
                var catText = new TextBlock
                {
                    Text = t.Category,
                    FontSize = 9.5,
                    FontWeight = FontWeights.Bold,
                    Foreground = t.Category.Contains("SOLID") ? BrushAmber : (t.Category.Contains("PATTERNS") ? BrushIndigo : BrushSky),
                    Margin = new Thickness(0, 0, 0, 3)
                };
                var titleText = new TextBlock
                {
                    Text = t.Title,
                    FontSize = 12,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = BrushTextLight,
                    TextWrapping = TextWrapping.Wrap
                };

                sp.Children.Add(catText);
                sp.Children.Add(titleText);
                card.Child = sp;

                // Hover effects
                card.MouseEnter += (s, e) => { card.Background = new SolidColorBrush(Color.FromRgb(30, 41, 65)); card.BorderBrush = BrushIndigo; };
                card.MouseLeave += (s, e) => { card.Background = new SolidColorBrush(Color.FromRgb(21, 30, 50)); card.BorderBrush = BrushBorder; };

                // Click to ask
                card.MouseDown += (s, e) =>
                {
                    _inputPrompt.Text = t.Title;
                    ProcessUserQuestion(t.Title);
                };

                _topicsListPanel.Children.Add(card);
            }
        }

        private void FilterTopics()
        {
            string q = _searchBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(q) || q.StartsWith("search"))
            {
                PopulateTopics(OOPKnowledgeRepository.Topics);
                return;
            }

            var matches = new List<OOPTopic>();
            foreach (var t in OOPKnowledgeRepository.Topics)
            {
                if (t.Title.ToLower().Contains(q) || t.Category.ToLower().Contains(q) || string.Join(" ", t.Keywords).ToLower().Contains(q))
                {
                    matches.Add(t);
                }
            }
            PopulateTopics(matches);
        }

        private void ShowGreeting()
        {
            var greetBorder = new Border
            {
                Background = BrushBgCard,
                BorderBrush = BrushBorder,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 0, 0, 16)
            };

            var sp = new StackPanel();
            var h = new TextBlock
            {
                Text = "⚡ Welcome to your OOP Technical Assistant!",
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Foreground = BrushSky,
                Margin = new Thickness(0, 0, 0, 10)
            };
            var body = new TextBlock
            {
                Text = "You can ask me any question about Object-Oriented Programming, SOLID principles, design patterns, and software architecture.\n\n" +
                       "👉 Type your question in the box below and press Enter, or click any topic on the left sidebar to explore in-depth!",
                FontSize = 13,
                Foreground = BrushTextLight,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 22
            };
            sp.Children.Add(h);
            sp.Children.Add(body);
            greetBorder.Child = sp;

            _chatMessagesPanel.Children.Add(greetBorder);

            if (_voiceCombo.SelectedIndex != 3)
            {
                NaturalVoiceEngine.Speak("Welcome! Ask me any question regarding Object Oriented Programming, or click any topic in the sidebar.");
            }
        }

        private void ProcessUserQuestion(string query)
        {
            string q = query.Trim();
            if (string.IsNullOrEmpty(q)) return;

            _inputPrompt.Clear();
            NaturalVoiceEngine.Stop();

            // 1. Add User Question Bubble
            var userBubble = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Background = new SolidColorBrush(Color.FromRgb(30, 41, 59)),
                BorderBrush = BrushBorder,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(16, 12, 16, 12),
                Margin = new Thickness(120, 0, 0, 16),
                MaxWidth = 720
            };
            var userText = new TextBlock
            {
                Text = "👤 " + q,
                FontSize = 13.5,
                FontWeight = FontWeights.SemiBold,
                Foreground = BrushTextLight,
                TextWrapping = TextWrapping.Wrap
            };
            userBubble.Child = userText;
            _chatMessagesPanel.Children.Add(userBubble);

            // 2. Lookup best match
            var topic = OOPKnowledgeRepository.FindBestMatch(q);

            // 3. Build Rich AI Response Card
            var aiCard = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = BrushBgCard,
                BorderBrush = BrushBorder,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(14),
                Padding = new Thickness(22, 18, 22, 18),
                Margin = new Thickness(0, 0, 100, 20),
                MaxWidth = 800
            };

            var aiStack = new StackPanel();

            // Category Badge & Title
            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
            var catBadge = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(40, 56, 189, 248)),
                BorderBrush = BrushSky,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(6, 2, 6, 2),
                Margin = new Thickness(0, 0, 10, 0)
            };
            catBadge.Child = new TextBlock { Text = topic.Category, FontSize = 9.5, FontWeight = FontWeights.Bold, Foreground = BrushSky };
            headerPanel.Children.Add(catBadge);

            var titleBlock = new TextBlock
            {
                Text = topic.Title,
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Foreground = BrushTextLight,
                VerticalAlignment = VerticalAlignment.Center
            };
            headerPanel.Children.Add(titleBlock);
            aiStack.Children.Add(headerPanel);

            // Short Summary
            var summaryBlock = new TextBlock
            {
                Text = topic.ShortSummary,
                FontSize = 13,
                Foreground = BrushSky,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 12),
                LineHeight = 20
            };
            aiStack.Children.Add(summaryBlock);

            // Detailed Core Explanation
            var explBlock = new TextBlock
            {
                Text = topic.CoreExplanation,
                FontSize = 12.5,
                Foreground = BrushTextLight,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 14),
                LineHeight = 21
            };
            aiStack.Children.Add(explBlock);

            // Code Snippet Box (If present)
            if (!string.IsNullOrEmpty(topic.CodeExample))
            {
                var codeBox = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(5, 8, 17)),
                    BorderBrush = BrushBorder,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(14),
                    Margin = new Thickness(0, 0, 0, 14)
                };
                var codeText = new TextBox
                {
                    Text = topic.CodeExample,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    Foreground = BrushEmerald,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    IsReadOnly = true,
                    TextWrapping = TextWrapping.Wrap
                };
                codeBox.Child = codeText;
                aiStack.Children.Add(codeBox);
            }

            // Trade-offs & Gotchas (Amber callout)
            if (!string.IsNullOrEmpty(topic.TradeoffsAndGotchas))
            {
                var gotchaBox = new Border
                {
                    Background = new SolidColorBrush(Color.FromArgb(25, 245, 158, 11)),
                    BorderBrush = BrushAmber,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(12, 10, 12, 10),
                    Margin = new Thickness(0, 0, 0, 10)
                };
                var gotchaText = new TextBlock
                {
                    Text = topic.TradeoffsAndGotchas,
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Color.FromRgb(254, 215, 170)),
                    TextWrapping = TextWrapping.Wrap
                };
                gotchaBox.Child = gotchaText;
                aiStack.Children.Add(gotchaBox);
            }

            // Senior Architect Tip (Indigo callout)
            if (!string.IsNullOrEmpty(topic.InterviewerProTip))
            {
                var tipBox = new Border
                {
                    Background = new SolidColorBrush(Color.FromArgb(25, 99, 102, 241)),
                    BorderBrush = BrushIndigo,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(12, 10, 12, 10)
                };
                var tipText = new TextBlock
                {
                    Text = topic.InterviewerProTip,
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Color.FromRgb(224, 231, 255)),
                    TextWrapping = TextWrapping.Wrap
                };
                tipBox.Child = tipText;
                aiStack.Children.Add(tipBox);
            }

            aiCard.Child = aiStack;
            _chatMessagesPanel.Children.Add(aiCard);

            // Cache for clipboard copy
            _lastCopiedText = string.Format("{0}\n\n{1}\n\n{2}\n\nCode:\n{3}", topic.Title, topic.ShortSummary, topic.CoreExplanation, topic.CodeExample);

            // Auto scroll to bottom
            _chatScrollViewer.ScrollToEnd();

            // Speak natural summary
            if (_voiceCombo.SelectedIndex != 3 && !string.IsNullOrEmpty(topic.SpokenSummary))
            {
                NaturalVoiceEngine.Speak(topic.SpokenSummary);
            }
        }
    }

    // =========================================================================
    // 5. APPLICATION ENTRY POINT
    // =========================================================================
    public class App : Application
    {
        [STAThread]
        public static void Main()
        {
            try
            {
                var app = new App();
                app.ShutdownMode = ShutdownMode.OnMainWindowClose;
                var win = new MainWindow();
                app.Run(win);
            }
            finally
            {
                try { NaturalVoiceEngine.Stop(); } catch { }
                try { System.Diagnostics.Process.GetCurrentProcess().Kill(); } catch { }
                Environment.Exit(0);
            }
        }
    }
}
