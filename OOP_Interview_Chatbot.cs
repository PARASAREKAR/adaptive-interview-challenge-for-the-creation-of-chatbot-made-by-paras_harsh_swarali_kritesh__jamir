using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace OOPInterviewApp
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
    // 2. KNOWLEDGE REPOSITORY (18 Curated OOP Interview Topics)
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
                Category = "THE 4 PILLARS",
                Title = "Encapsulation vs Abstraction",
                Keywords = new string[] { "encapsulation", "abstraction", "data hiding", "access modifiers", "private", "public", "hide complexity" },
                ShortSummary = "Encapsulation bundles state and methods together while protecting invariants via access modifiers. Abstraction simplifies complexity by revealing only essential interfaces.",
                CoreExplanation = "• Encapsulation is about PACKAGING and PROTECTION.\nIt bundles attributes and methods together into a cohesive class while restricting direct access to internal fields using access specifiers (private, protected). This ensures invariants cannot be corrupted from the outside.\n\n• Abstraction is about SIMPLIFICATION.\nIt hides implementation details and exposes only high-level interfaces to callers. It answers WHAT an object does rather than HOW it does it.\n\nReal-World Analogy (Automobile):\n- Abstraction: The steering wheel, accelerator, and brake pedals. You press the pedal to accelerate without needing to understand fuel injection ratios or transmission fluid pressure.\n- Encapsulation: The engine and transmission are sealed under the hood. You cannot directly reach in and tweak the fuel-to-air ratio variable while driving; you interact exclusively through controlled interfaces.",
                CodeExample = "// C# Example of Encapsulation & Abstraction:\npublic interface IBankAccount // Abstraction: What it does\n{\n    void Deposit(decimal amount);\n    bool Withdraw(decimal amount);\n    decimal Balance { get; }\n}\n\npublic class BankAccount : IBankAccount\n{\n    // Encapsulation: State is private & protected from invalid mutations\n    private decimal _balance;\n\n    public decimal Balance { get { return _balance; } }\n\n    public void Deposit(decimal amount)\n    {\n        if (amount <= 0) throw new ArgumentException(\"Deposit must be positive\");\n        _balance += amount;\n    }\n\n    public bool Withdraw(decimal amount)\n    {\n        if (amount <= 0 || amount > _balance) return false;\n        _balance -= amount;\n        return true;\n    }\n}",
                TradeoffsAndGotchas = "Gotcha: Do not create classes with public getters and setters for every private field. Trivial getters/setters for everything is an anti-pattern known as an 'Anemic Domain Model' that breaks true encapsulation.",
                InterviewerProTip = "Senior Interview Tip: Mention that Encapsulation preserves domain invariants, while Abstraction decouples consumers from changes in concrete implementations.",
                SpokenSummary = "Encapsulation bundles data and protects state with access modifiers, while Abstraction simplifies complexity by exposing clean interfaces. Think of a car: the pedals are an abstraction for driving, while the sealed engine encapsulates the mechanical state."
            });

            Topics.Add(new OOPTopic
            {
                Id = "overload_vs_override",
                Category = "THE 4 PILLARS",
                Title = "Method Overloading vs Overriding",
                Keywords = new string[] { "overloading", "overriding", "compile time", "runtime", "static polymorphism", "dynamic polymorphism", "vtable" },
                ShortSummary = "Overloading is compile-time (static) polymorphism with the same method name but different parameter signatures. Overriding is runtime (dynamic) polymorphism where a subclass provides a specific implementation of a virtual method.",
                CoreExplanation = "• Method Overloading (Static / Compile-Time Polymorphism):\nOccurs within the same class (or across hierarchy). Multiple methods share the same name but have DIFFERENT parameter types, counts, or order. The compiler determines which method to invoke during compilation based on the call signature.\n\n• Method Overriding (Dynamic / Run-Time Polymorphism):\nOccurs across inheritance boundaries. A subclass provides its own specific implementation of a method declared in its superclass. The signature (name, return type, parameters) must match. The method to execute is resolved at runtime based on the actual object on the heap via dynamic dispatch (vtables).",
                CodeExample = "public class Calculator\n{\n    // Overloading: Same name, different parameters (Compile-Time)\n    public int Add(int a, int b) { return a + b; }\n    public double Add(double a, double b) { return a + b; }\n    public int Add(int a, int b, int c) { return a + b + c; }\n}\n\npublic class Animal\n{\n    // Overriding: Virtual method in base class\n    public virtual void MakeSound() { Console.WriteLine(\"Generic animal sound\"); }\n}\n\npublic class Dog : Animal\n{\n    // Overriding: Run-Time dynamic dispatch\n    public override void MakeSound() { Console.WriteLine(\"Bark! Bark!\"); }\n}",
                TradeoffsAndGotchas = "Gotcha: In Java and C#, static and private methods cannot be overridden because they are resolved via early binding at compile-time without dynamic dispatch.",
                InterviewerProTip = "Top Interview Tip: Always connect overloading to early binding and overriding to late binding / dynamic dispatch.",
                SpokenSummary = "Method overloading happens at compile time when methods share the same name but take different parameters. Method overriding happens at runtime when a subclass replaces a virtual method from its parent class using dynamic dispatch."
            });

            Topics.Add(new OOPTopic
            {
                Id = "dynamic_dispatch_vtable",
                Category = "THE 4 PILLARS",
                Title = "Dynamic Dispatch & vtable",
                Keywords = new string[] { "dynamic dispatch", "vtable", "vptr", "virtual table", "under the hood", "function pointer", "virtual keyword" },
                ShortSummary = "Dynamic dispatch is the runtime mechanism that resolves overridden virtual methods using class vtables and object vptrs.",
                CoreExplanation = "How dynamic polymorphism works under the hood in C++ and Java:\n\n1. vtable (Virtual Table):\nFor every class that declares or overrides a virtual method, the compiler constructs a static array of function pointers called the vtable. There is only ONE vtable per class, stored in read-only memory.\n\n2. vptr (Virtual Pointer):\nEvery instance (object) allocated on the heap contains an implicit hidden pointer, the vptr, placed at the start of its memory layout. The vptr points to the vtable of the object's actual concrete runtime type.\n\n3. Execution Mechanics:\nWhen your code calls animal.MakeSound(), the CPU performs an indirect call:\n  Step 1: Read the object's vptr address.\n  Step 2: Follow vptr to the class's vtable.\n  Step 3: Read the function pointer at the specific offset reserved for MakeSound().\n  Step 4: Jump to that function address.",
                CodeExample = "// Memory Layout Conceptual Diagram:\n// [Object Dog in Heap] -> [vptr] ----> [Dog vtable in Read-Only Memory]\n//                         [name]       [0]: &Dog.MakeSound()\n//                         [age]        [1]: &Dog.ToString()\n\nAnimal a = new Dog();\na.MakeSound(); // CPU follows a.vptr -> Dog vtable -> executes Dog.MakeSound()",
                TradeoffsAndGotchas = "Performance Trade-offs: Calling a virtual method incurs a small pointer indirection cost (potential cache miss) and inhibits the compiler from inlining the function call.",
                InterviewerProTip = "Staff Engineer Tip: Mention that Modern branch predictors and JIT devirtualization optimize away most vtable overhead when an object type remains stable.",
                SpokenSummary = "Dynamic dispatch is powered by virtual method tables and pointers. Every class with virtual methods has a static lookup table of function pointers, and each object stores a hidden pointer to its class table. At runtime, the computer looks up the pointer to execute the correct subclass implementation."
            });

            Topics.Add(new OOPTopic
            {
                Id = "abstract_vs_interface",
                Category = "THE 4 PILLARS",
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
                Category = "SOLID PRINCIPLES",
                Title = "Single Responsibility Principle (SRP)",
                Keywords = new string[] { "srp", "single responsibility", "reason to change", "actor", "stakeholder", "cohesion", "god class" },
                ShortSummary = "A class should have one, and only one, reason to change — meaning it should be responsible to only one actor or stakeholder.",
                CoreExplanation = "Robert C. Martin clarified that a 'reason to change' maps directly to an ACTOR or STAKEHOLDER who requests modifications:\n\n• Classic Violation (God Class):\nAn Employee class containing:\n  1. CalculatePay() -> Requested by Finance / CFO.\n  2. ReportHours() -> Requested by HR / COO.\n  3. SaveToDb() -> Requested by DBA / System Admin.\n\nIf Finance requests an update to overtime calculation, that logic change could inadvertently alter HR's timesheet report.\n\n• The Fix:\nSeparate responsibilities into distinct classes:\n  - Employee: Pure domain entity holding employee data.\n  - PayCalculator: Financial logic for Finance.\n  - TimesheetReporter: Time tracking & formatting for HR.\n  - EmployeeRepository: Persistence logic for the database.",
                CodeExample = "// Refactored to SRP:\npublic class Employee { public int Id; public string Name; public decimal HourlyRate; }\n\npublic class PayCalculator \n{\n    public decimal CalculateWeeklyPay(Employee emp, double hours) { return emp.HourlyRate * (decimal)hours; }\n}\n\npublic class EmployeeRepository \n{\n    public void Save(Employee emp) { /* Database persistence */ }\n}",
                TradeoffsAndGotchas = "Gotcha: SRP does not mean a class should only have a single method! It means all methods in the class should belong to one cohesive responsibility.",
                InterviewerProTip = "Interview Tip: Explain that violating SRP creates high merge conflict risk and regression cascades when multiple teams work on the same file.",
                SpokenSummary = "The Single Responsibility Principle states that a class should have only one reason to change, meaning it serves only one stakeholder. Instead of a single class that handles business calculations, database saving, and email notifications, break each task into its own cohesive module."
            });

            Topics.Add(new OOPTopic
            {
                Id = "ocp",
                Category = "SOLID PRINCIPLES",
                Title = "Open/Closed Principle (OCP)",
                Keywords = new string[] { "ocp", "open closed", "open for extension", "closed for modification", "polymorphism", "avoid switch" },
                ShortSummary = "Software entities should be open for extension (new features added via new code), but closed for modification (existing tested code left untouched).",
                CoreExplanation = "• Closed for Modification: Once a class is tested and released into production, you should avoid modifying its source code to add new variants or features.\n\n• Open for Extension: You should be able to introduce new behavior simply by writing new classes that implement an abstraction.\n\n• How to Achieve OCP:\nUse Polymorphism, Interfaces, or the Strategy Pattern instead of switch or if-else ladders.\n\nExample:\nIn an e-commerce checkout, instead of a switch(paymentType) that gets edited every time Apple Pay, PayPal, or Crypto is added, define an IPaymentMethod interface. To add Crypto, simply write a CryptoPayment class without touching existing checkout logic.",
                CodeExample = "public interface IPaymentMethod\n{\n    void Process(decimal amount);\n}\n\npublic class CreditCardPayment : IPaymentMethod\n{\n    public void Process(decimal amount) { Console.WriteLine(\"Charged card: \" + amount); }\n}\n\npublic class PayPalPayment : IPaymentMethod\n{\n    public void Process(decimal amount) { Console.WriteLine(\"Charged PayPal: \" + amount); }\n}\n\n// Adding BitcoinPayment requires ZERO changes to CheckoutService!\npublic class CheckoutService\n{\n    public void CompleteOrder(IPaymentMethod method, decimal total)\n    {\n        method.Process(total); // Never needs modification!\n    }\n}",
                TradeoffsAndGotchas = "Gotcha: Do not prematurely abstract everything on day one. Follow the Rule of Three: write concrete code first, and introduce OCP abstractions once a third variant arrives.",
                InterviewerProTip = "Interview Tip: Highlight that OCP drastically reduces regression testing scope because existing binaries are untouched.",
                SpokenSummary = "The Open-Closed Principle means your software should be open for extension but closed for modification. When you need to add a new payment method or reporting format, you introduce a new class implementing an interface, rather than adding another branch to an existing if statement."
            });

            Topics.Add(new OOPTopic
            {
                Id = "lsp",
                Category = "SOLID PRINCIPLES",
                Title = "Liskov Substitution Principle (LSP)",
                Keywords = new string[] { "lsp", "liskov", "behavioral subtyping", "square rectangle", "invariants", "preconditions", "postconditions" },
                ShortSummary = "Subtypes must be substitutable for their supertypes without altering the correctness of the program (Behavioral Subtyping).",
                CoreExplanation = "Barbara Liskov's principle states that inheritance must preserve behavioral contracts, not just method signatures.\n\n• The Classic Square-Rectangle Violation:\nIn mathematics, a Square is a Rectangle. But in OOP with mutable state:\nIf class Square inherits Rectangle, setting Square.Width also changes Height to keep sides equal.\nIf a caller expects a Rectangle and runs:\n  rect.SetWidth(5);\n  rect.SetHeight(4);\n  assert(rect.Area() == 20);\nIf passed a Square, Area() returns 16! The subclass silently broke the caller's invariant that changing height leaves width unaffected.\n\n• Formal Rules of LSP:\n1. Preconditions cannot be strengthened in a subtype (subclass cannot demand stricter input).\n2. Postconditions cannot be weakened in a subtype (subclass cannot guarantee less).\n3. Invariants of the supertype must be preserved.\n4. Subtypes must not throw unexpected exceptions (e.g. NotImplementedException).",
                CodeExample = "// LSP Compliant Design: Separate into read-only abstractions\npublic interface IShape\n{\n    double CalculateArea();\n}\n\npublic class Rectangle : IShape\n{\n    public double Width { get; set; }\n    public double Height { get; set; }\n    public double CalculateArea() { return Width * Height; }\n}\n\npublic class Square : IShape\n{\n    public double Side { get; set; }\n    public double CalculateArea() { return Side * Side; }\n}",
                TradeoffsAndGotchas = "Gotcha: Just because something is an 'IS-A' relationship in the real world does not mean it qualifies as an inheritance relationship in object-oriented software.",
                InterviewerProTip = "Top Tip: Mention 'Design by Contract' and explain that throwing NotImplementedException in a subclass is an automatic LSP violation.",
                SpokenSummary = "The Liskov Substitution Principle requires that any subclass must be usable wherever its parent class is expected, without breaking caller assumptions. For example, making a Square inherit from a Rectangle fails because setting a square's width unexpectedly mutates its height, breaking the rectangle contract."
            });

            Topics.Add(new OOPTopic
            {
                Id = "isp",
                Category = "SOLID PRINCIPLES",
                Title = "Interface Segregation Principle (ISP)",
                Keywords = new string[] { "isp", "interface segregation", "fat interface", "polluted interface", "lean interface", "notimplementedexception", "role interface" },
                ShortSummary = "Clients should not be forced to depend on interfaces they do not use. Split fat interfaces into smaller, cohesive role-based interfaces.",
                CoreExplanation = "• The Core Principle:\nClients should never be forced to implement methods they do not need. When an interface becomes a bloated 'fat interface', implementing classes end up writing dummy stub methods or throwing NotImplementedException.\n\n• The Consequences of Fat Interfaces:\n1. Stub Smells: Classes have zombie methods that throw exceptions, breaking Liskov Substitution Principle.\n2. Recompilation Cascades: When an interface method changes, all implementing classes must be updated and re-tested, even those that never consume that method.\n3. Tight Coupling: Blurs boundaries between distinct responsibilities.\n\n• Solution (Role Interfaces):\nDecompose monolithic interfaces into small, cohesive, role-specific contracts (e.g., IPrinter, IScanner, IFaxMachine instead of ISmartDevice). Classes can implement multiple fine-grained interfaces as needed.",
                CodeExample = "// Bad: Fat interface forces dummy implementations\npublic interface ISmartDevice\n{\n    void Print();\n    void Scan();\n    void Fax();\n}\n\n// Good: Segregated role interfaces\npublic interface IPrinter { void Print(); }\npublic interface IScanner { void Scan(); }\npublic interface IFax { void Fax(); }\n\npublic class BasicPrinter : IPrinter\n{\n    public void Print() { Console.WriteLine(\"Printing document...\"); }\n}\n\npublic class OfficeAllInOne : IPrinter, IScanner, IFax\n{\n    public void Print() { /* ... */ }\n    public void Scan() { /* ... */ }\n    public void Fax() { /* ... */ }\n}",
                TradeoffsAndGotchas = "Gotcha: Don't take ISP to the extreme of creating single-method interfaces for every trivial operation unless that method represents a genuine distinct capability.",
                InterviewerProTip = "Interview Tip: Connect ISP directly to C#'s interface inheritance or Java's multi-interface implementation as the architectural remedy against interface pollution.",
                SpokenSummary = "The Interface Segregation Principle states that clients should never be forced to depend on methods they do not use. Instead of one fat interface with twenty methods, break it down into focused role interfaces so classes only implement what they actually need."
            });

            Topics.Add(new OOPTopic
            {
                Id = "dip",
                Category = "SOLID PRINCIPLES",
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
                Category = "DESIGN PATTERNS",
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
                Id = "factory_patterns",
                Category = "DESIGN PATTERNS",
                Title = "Factory Method vs Abstract Factory",
                Keywords = new string[] { "factory", "factory method", "abstract factory", "creational patterns", "families of objects", "virtual constructor" },
                ShortSummary = "Factory Method uses inheritance to instantiate a single product. Abstract Factory uses composition to create families of related products.",
                CoreExplanation = "• Factory Method (Inheritance-based, Single Product):\n- Defines an abstract creation method in a base class or interface, letting derived classes decide which concrete class to instantiate.\n- Often referred to as a 'virtual constructor'.\n- Focuses on producing ONE product family member (e.g. Document.CreateReport() returning PdfReport or HtmlReport).\n\n• Abstract Factory (Composition-based, Families of Products):\n- Defines an interface with multiple factory methods that create an entire FAMILY of related or dependent objects without specifying concrete classes.\n- Guarantees compatibility across related components (e.g., MacUIFactory creates MacButton and MacCheckbox, while WinUIFactory creates WinButton and WinCheckbox).\n- The client holds a reference to the abstract factory interface and calls methods to assemble the suite.",
                CodeExample = "// Abstract Factory Pattern:\npublic interface IGUIFactory\n{\n    IButton CreateButton();\n    ICheckbox CreateCheckbox();\n}\n\npublic class WindowsFactory : IGUIFactory\n{\n    public IButton CreateButton() { return new WinButton(); }\n    public ICheckbox CreateCheckbox() { return new WinCheckbox(); }\n}\n\npublic class MacFactory : IGUIFactory\n{\n    public IButton CreateButton() { return new MacButton(); }\n    public ICheckbox CreateCheckbox() { return new MacCheckbox(); }\n}",
                TradeoffsAndGotchas = "Gotcha: Adding a new product to an Abstract Factory requires changing the factory interface and updating every concrete factory implementation.",
                InterviewerProTip = "Staff Engineer Tip: Summarize clearly: Factory Method creates a single object via subclassing; Abstract Factory creates families of related objects via object composition.",
                SpokenSummary = "Factory Method uses inheritance to let subclasses decide which single product to instantiate. Abstract Factory uses object composition to produce entire families of related products, like matching buttons and checkboxes for different operating systems."
            });

            Topics.Add(new OOPTopic
            {
                Id = "strategy_vs_state",
                Category = "DESIGN PATTERNS",
                Title = "Strategy Pattern vs State Pattern",
                Keywords = new string[] { "strategy pattern", "state pattern", "finite state machine", "intent", "swapping algorithms" },
                ShortSummary = "Strategy and State share similar UML class structures, but Strategy swaps independent algorithms while State models an internal finite state machine.",
                CoreExplanation = "• Strategy Pattern:\n- Intent: Provide interchangeable algorithms or business policies.\n- Client Role: The client or configuration selects the concrete strategy (e.g., QuickSort vs MergeSort, or FedEx vs UPS shipping).\n- Awareness: Concrete strategies are completely oblivious and independent of each other.\n\n• State Pattern:\n- Intent: Allow an object to alter its behavior when its internal state changes (Finite State Machine).\n- Client Role: The client treats the context as a single entity; state transitions happen automatically based on events.\n- Awareness: Concrete state classes often know about other states to drive transitions (e.g., Draft -> InReview -> Published).",
                CodeExample = "// Strategy Pattern: Configurable algorithm\npublic interface ICompressionStrategy { void Compress(string file); }\n\n// State Pattern: Transitions driven by state\npublic interface IOrderState { void Next(OrderContext ctx); }\npublic class PendingState : IOrderState \n{\n    public void Next(OrderContext ctx) { ctx.State = new PaidState(); }\n}",
                TradeoffsAndGotchas = "Gotcha: If your states don't change based on events and don't trigger transitions, you probably need a Strategy pattern, not a State pattern.",
                InterviewerProTip = "Interview Tip: State patterns encapsulate transitions so you can eliminate long switch(currentState) statements.",
                SpokenSummary = "Although Strategy and State look identical in UML diagrams, their purpose is completely different. Strategy lets a client swap independent algorithms, while State models internal transitions in a finite state machine where states trigger transitions automatically."
            });

            Topics.Add(new OOPTopic
            {
                Id = "observer_pattern",
                Category = "DESIGN PATTERNS",
                Title = "Observer Pattern & Weak References",
                Keywords = new string[] { "observer", "pub sub", "lapsed listener", "memory leak", "weak reference", "event", "delegates" },
                ShortSummary = "Defines a one-to-many subscription model. Mitigates the classic 'Lapsed Listener' memory leak using explicit unsubscription or WeakReferences.",
                CoreExplanation = "• Mechanics of Observer Pattern:\nDefines a one-to-many dependency where a Subject (Observable) notifies registered Observers whenever state changes, usually via an Update() method.\n\n• The 'Lapsed Listener' Memory Leak:\n1. When an observer subscribes to a long-lived subject, the subject stores a STRONG reference to the observer in its listener list.\n2. When the caller is done using the observer (e.g., a modal window closes), if it forgets to explicitly unsubscribe, the Garbage Collector CANNOT reclaim it because an active reference path exists from the subject root.\n3. This leads to hidden memory leaks and phantom event handlers firing in the background.\n\n• Prevention Strategies:\n1. Deterministic Cleanup: Implement IDisposable and pair Subscribe() with Unsubscribe().\n2. Weak References: The Subject stores WeakReference tokens so the GC can reclaim subscribers when no other strong references exist.",
                CodeExample = "public interface IObserver { void Update(string message); }\n\npublic class Subject\n{\n    // Using WeakReference to avoid lapsed listener memory leaks\n    private readonly List<WeakReference> _observers = new List<WeakReference>();\n\n    public void Subscribe(IObserver obs) { _observers.Add(new WeakReference(obs)); }\n\n    public void Notify(string msg)\n    {\n        for (int i = _observers.Count - 1; i >= 0; i--)\n        {\n            var target = _observers[i].Target as IObserver;\n            if (target != null)\n                target.Update(msg);\n            else\n                _observers.RemoveAt(i); // Clean up collected observer\n        }\n    }\n}",
                TradeoffsAndGotchas = "Gotcha: Weak reference iteration requires checking Target != null and removing dead references during notification loops.",
                InterviewerProTip = "Interview Tip: Mentioning the Lapsed Listener problem immediately demonstrates real-world production experience with event-driven architectures.",
                SpokenSummary = "The Observer pattern establishes a one-to-many subscription model. In production, you must watch out for the lapsed listener memory leak, where a long-lived subject holds strong references to short-lived observers, preventing garbage collection. Using weak references or explicit disposable lifecycles solves this."
            });

            Topics.Add(new OOPTopic
            {
                Id = "decorator_pattern",
                Category = "DESIGN PATTERNS",
                Title = "Decorator Pattern vs Inheritance",
                Keywords = new string[] { "decorator", "wrapper", "open closed", "middleware", "stream", "composable behavior" },
                ShortSummary = "Attaches additional responsibilities to an object dynamically at runtime without subclassing, avoiding class explosion.",
                CoreExplanation = "• The Problem with Subclassing for Variations:\nIf you have a Coffee class and want to support Milk, Sugar, Whip, and Caramel:\nUsing inheritance creates 16 subclasses (CoffeeWithMilk, CoffeeWithMilkAndSugar, etc.).\n\n• How Decorator Solves This:\n1. The Decorator implements the SAME interface as the component it wraps.\n2. It holds a private reference to another component instance (composition).\n3. It intercepts calls, performs its own behavior (e.g., adding $0.50 for milk or encrypting bytes in a stream), and delegates to the inner component.\n4. Decorators can be nested arbitrarily: new Whip(new Milk(new SimpleCoffee())).",
                CodeExample = "public interface ICoffee { decimal GetCost(); string GetDescription(); }\n\npublic class SimpleCoffee : ICoffee\n{\n    public decimal GetCost() { return 2.00m; }\n    public string GetDescription() { return \"Simple Coffee\"; }\n}\n\npublic abstract class CoffeeDecorator : ICoffee\n{\n    protected readonly ICoffee _decorated;\n    public CoffeeDecorator(ICoffee decorated) { _decorated = decorated; }\n    public virtual decimal GetCost() { return _decorated.GetCost(); }\n    public virtual string GetDescription() { return _decorated.GetDescription(); }\n}\n\npublic class MilkDecorator : CoffeeDecorator\n{\n    public MilkDecorator(ICoffee coffee) : base(coffee) { }\n    public override decimal GetCost() { return base.GetCost() + 0.50m; }\n    public override string GetDescription() { return base.GetDescription() + \", Milk\"; }\n}",
                TradeoffsAndGotchas = "Gotcha: Decorators can introduce many small objects and make debugging stack traces deeper.",
                InterviewerProTip = "Interview Tip: Classic production example: Java's BufferedInputStream(new GZIPInputStream(new FileInputStream(...))) and C# CryptoStream.",
                SpokenSummary = "The Decorator pattern lets you dynamically add features to an object by wrapping it, without altering existing code or exploding the inheritance tree. Think of streaming pipes like encrypting and compressing data streams on the fly."
            });

            // -----------------------------------------------------------------
            // ADVANCED ARCHITECTURE
            // -----------------------------------------------------------------
            Topics.Add(new OOPTopic
            {
                Id = "composition_over_inheritance",
                Category = "ADVANCED ARCHITECTURE",
                Title = "Composition Over Inheritance",
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
                Category = "ADVANCED ARCHITECTURE",
                Title = "The Diamond Problem & C3 MRO",
                Keywords = new string[] { "diamond problem", "multiple inheritance", "mro", "c3 linearization", "virtual inheritance", "ambiguity" },
                ShortSummary = "The Diamond Problem arises when a class inherits from two parents that share a common ancestor. Solved via C++ virtual inheritance, Java interface disambiguation, or Python C3 MRO.",
                CoreExplanation = "• The Diamond Problem:\nClass D inherits from B and C, both of which inherit from A:\n      A (defines Foo)\n     / \\\n    B   C (both override Foo)\n     \\ /\n      D (calls Foo -> Which one runs?)\n\nAmbiguities:\n1. Method Ambiguity: Does D call B.Foo() or C.Foo()?\n2. State Duplication: Without special handling, D contains two separate copies of A's member variables.\n\n• How Different Languages Solve It:\n1. C++: Supports virtual base classes (class B : virtual public A). Ensures only one shared instance of A exists. Ambiguities must be resolved with scope resolution (B::Foo()).\n2. Java: Forbade multiple class inheritance entirely. For interface default methods, Java forces the subclass to explicitly disambiguate.\n3. Python: Supports multiple inheritance using Method Resolution Order (MRO) with the C3 Linearization algorithm (Class.__mro__), guaranteeing consistent, deterministic order.",
                CodeExample = "# Python C3 MRO Resolution:\nclass A:\n    def speak(self): print('A')\nclass B(A):\n    def speak(self): print('B')\nclass C(A):\n    def speak(self): print('C')\nclass D(B, C):\n    pass\n\n# D().__class__.__mro__ -> (D, B, C, A, object)",
                TradeoffsAndGotchas = "Gotcha: Avoid deep multiple inheritance graphs even in languages that support them. Prefer multiple interface implementation.",
                InterviewerProTip = "Interview Tip: Highlight that Python's super() does not simply call the parent class; it calls the next class in the C3 MRO chain.",
                SpokenSummary = "The Diamond Problem occurs in multiple inheritance when a class inherits from two classes with a common ancestor, creating ambiguity over which method and state copy to invoke. C++ solves this with virtual inheritance, Java forbids multiple class inheritance, and Python uses C3 linearization to order the method resolution chain."
            });

            Topics.Add(new OOPTopic
            {
                Id = "immutability_value_objects",
                Category = "ADVANCED ARCHITECTURE",
                Title = "Immutability & Value Objects (DDD)",
                Keywords = new string[] { "immutability", "value object", "entity", "domain driven design", "thread safety", "hashcode", "defensive copying" },
                ShortSummary = "Value Objects are immutable domain concepts defined solely by their attributes without persistent IDs. They provide inherent thread safety and hashcode stability.",
                CoreExplanation = "• Why Immutability is Crucial in OOP:\n1. Thread Safety by Default: Immutable instances can be freely shared across concurrent threads without locks, mutexes, or race conditions.\n2. Eliminates Temporal Coupling: Methods cannot produce mysterious bugs based on execution order because state never mutates.\n3. Hashcode Stability: Mutating an object that acts as a key in a Dictionary or HashSet corrupts the hash bucket permanently. Immutable keys guarantee index integrity.\n\n• Entity vs Value Object (Domain-Driven Design):\n- Entity: Defined by a persistent unique identity (e.g., CustomerId, OrderId). Attributes may change over time, but identity remains continuous.\n- Value Object: Defined purely by the totality of its attributes (e.g., Money(50, 'USD'), DateRange, GeoLocation). Has NO independent identity. Two Value Objects with identical attributes are considered completely equal.",
                CodeExample = "// Immutable Value Object in C#:\npublic sealed class Money : IEquatable<Money>\n{\n    public decimal Amount { get; private set; }\n    public string Currency { get; private set; }\n\n    public Money(decimal amount, string currency)\n    {\n        if (amount < 0) throw new ArgumentException(\"Amount cannot be negative\");\n        if (currency == null) throw new ArgumentNullException(\"currency\");\n        Amount = amount;\n        Currency = currency;\n    }\n\n    // Operations return brand-new immutable instances\n    public Money Add(Money other)\n    {\n        if (this.Currency != other.Currency) throw new InvalidOperationException(\"Currency mismatch\");\n        return new Money(this.Amount + other.Amount, this.Currency);\n    }\n\n    public bool Equals(Money other) { return other != null && Amount == other.Amount && Currency == other.Currency; }\n    public override bool Equals(object obj) { return Equals(obj as Money); }\n    public override int GetHashCode() { return Amount.GetHashCode() ^ Currency.GetHashCode(); }\n}",
                TradeoffsAndGotchas = "Gotcha: Never mutate a Value Object in-place. Any transformation must return a brand-new instance with the updated values.",
                InterviewerProTip = "Staff Engineer Tip: Emphasize that Value Objects enforce domain invariants at creation time inside their constructor, ensuring invalid states cannot exist in memory.",
                SpokenSummary = "Immutability makes objects thread-safe by default and prevents subtle side effects. In Domain-Driven Design, Value Objects like money, dates, and coordinates are defined entirely by their attributes without unique IDs, and any operation returns a new immutable instance rather than mutating the original."
            });

            Topics.Add(new OOPTopic
            {
                Id = "gc_memory_roots",
                Category = "ADVANCED ARCHITECTURE",
                Title = "GC, Object Lifecycles & Roots",
                Keywords = new string[] { "garbage collection", "memory", "heap", "stack", "generations", "gc roots", "finalizer", "dispose" },
                ShortSummary = "Managed runtimes use generational garbage collection (Gen 0, 1, 2) tracing from GC Roots to automatically reclaim unreachable heap memory.",
                CoreExplanation = "• Generational Garbage Collection Hypothesis:\nMost objects die young (temporary loop variables, strings). Long-lived objects tend to stay alive for the lifetime of the application.\n- Gen 0: Brand new allocations. Smallest, collected very frequently (sub-millisecond).\n- Gen 1: Buffer zone between short-lived and long-lived objects.\n- Gen 2: Long-lived objects (singletons, caches, static references). Collected infrequently during full GC sweeps.\n\n• What constitutes a GC Root?\n1. Stack variables and CPU registers currently executing in active call stacks.\n2. Static variables declared on classes.\n3. Active interop / COM / pinned handles.\n\n• The IDisposable Pattern:\nGC only reclaims managed memory. Unmanaged resources (file handles, database connections, sockets) MUST be released explicitly using Dispose() and 'using' statements.",
                CodeExample = "public class ResourceHolder : IDisposable\n{\n    private IntPtr _nativeHandle;\n    private bool _disposed = false;\n\n    public void Dispose()\n    {\n        Dispose(true);\n        GC.SuppressFinalize(this); // Prevent finalizer overhead!\n    }\n\n    protected virtual void Dispose(bool disposing)\n    {\n        if (!_disposed)\n        {\n            if (disposing) { /* Release managed objects */ }\n            // Close unmanaged handles\n            _disposed = true;\n        }\n    }\n}",
                TradeoffsAndGotchas = "Gotcha: Never resurrect dead objects in finalizers, and always call GC.SuppressFinalize in Dispose() to save GC cycles.",
                InterviewerProTip = "Senior Architect Tip: Static collections and uncleaned event subscriptions are the #1 cause of managed memory leaks in enterprise systems.",
                SpokenSummary = "Managed runtimes use generational garbage collection tracing from roots like active stack frames and static fields. While the collector manages memory automatically, you must implement the disposable pattern to release unmanaged handles like database connections and network sockets."
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

                if (q.Contains(titleLower) || titleLower.Contains(q)) score += 60;

                foreach (var kw in t.Keywords)
                {
                    if (q.Contains(kw.ToLower())) score += 20;
                }

                string[] tokens = q.Split(new char[] { ' ', '?', ',', '.', '-', '(', ')', '/' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var tok in tokens)
                {
                    if (tok.Length > 2)
                    {
                        if (titleLower.Contains(tok)) score += 10;
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

        public static string EscapeJson(string s)
        {
            if (s == null) return "";
            var sb = new StringBuilder();
            foreach (char c in s)
            {
                switch (c)
                {
                    case '\\': sb.Append("\\\\"); break;
                    case '\"': sb.Append("\\\""); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    default:
                        if (c < 32) sb.AppendFormat("\\u{0:X4}", (int)c);
                        else sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
    }

    // =========================================================================
    // 3. NATURAL VOICE SYNTHESIS ENGINE (Windows OneCore)
    // =========================================================================
    public static class NaturalVoiceEngine
    {
        private static dynamic _voice;
        private static dynamic _oneCoreTokens;
        private static dynamic _sapiTokens;
        public static string CurrentVoiceName = "Microsoft Mark - English (United States)";

        public static void Initialize()
        {
            try
            {
                _voice = Activator.CreateInstance(Type.GetTypeFromProgID("SAPI.SpVoice"));

                try
                {
                    dynamic cat = Activator.CreateInstance(Type.GetTypeFromProgID("SAPI.SpObjectTokenCategory"));
                    cat.SetId(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech_OneCore\Voices", false);
                    _oneCoreTokens = cat.EnumerateTokens();
                }
                catch { }

                try { _sapiTokens = _voice.GetVoices(); } catch { }

                SelectVoice("Mark");
                _voice.Rate = -1; // Natural conversational cadence
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
                        string clean = Regex.Replace(text, @"[`*_#\-\\]", " ");
                        clean = Regex.Replace(clean, @"```[\s\S]*?```", " ");
                        clean = Regex.Replace(clean, @"\{[\s\S]*?\}", " ");
                        clean = Regex.Replace(clean, @"\s+", " ").Trim();
                        _voice.Speak(clean, 1); // 1 = SVSFlagsAsync
                    }
                }
                catch { }
            });
        }

        public static void Stop()
        {
            try
            {
                if (_voice != null) _voice.Speak("", 2); // 2 = SVSFPurgeBeforeSpeak
            }
            catch { }
        }
    }

    // =========================================================================
    // 4. JAVASCRIPT INTEROP BRIDGE
    // =========================================================================
    [ComVisible(true)]
    public class ChatScriptBridge
    {
        private readonly Form _parentForm;

        public ChatScriptBridge(Form parent)
        {
            _parentForm = parent;
        }

        public string GetInitialTopicsJson()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < OOPKnowledgeRepository.Topics.Count; i++)
            {
                var t = OOPKnowledgeRepository.Topics[i];
                if (i > 0) sb.Append(",");
                sb.Append("{");
                sb.AppendFormat("\"id\":\"{0}\",", OOPKnowledgeRepository.EscapeJson(t.Id));
                sb.AppendFormat("\"category\":\"{0}\",", OOPKnowledgeRepository.EscapeJson(t.Category));
                sb.AppendFormat("\"title\":\"{0}\",", OOPKnowledgeRepository.EscapeJson(t.Title));
                sb.AppendFormat("\"summary\":\"{0}\"", OOPKnowledgeRepository.EscapeJson(t.ShortSummary));
                sb.Append("}");
            }
            sb.Append("]");
            return sb.ToString();
        }

        public string AskQuestion(string query, bool isVoiceEnabled)
        {
            string q = (query ?? "").Trim();
            if (string.IsNullOrEmpty(q)) return "{}";

            NaturalVoiceEngine.Stop();

            var topic = OOPKnowledgeRepository.FindBestMatch(q);

            if (isVoiceEnabled && !string.IsNullOrEmpty(topic.SpokenSummary))
            {
                NaturalVoiceEngine.Speak(topic.SpokenSummary);
            }

            var sb = new StringBuilder();
            sb.Append("{");
            sb.AppendFormat("\"id\":\"{0}\",", OOPKnowledgeRepository.EscapeJson(topic.Id));
            sb.AppendFormat("\"category\":\"{0}\",", OOPKnowledgeRepository.EscapeJson(topic.Category));
            sb.AppendFormat("\"title\":\"{0}\",", OOPKnowledgeRepository.EscapeJson(topic.Title));
            sb.AppendFormat("\"summary\":\"{0}\",", OOPKnowledgeRepository.EscapeJson(topic.ShortSummary));
            sb.AppendFormat("\"explanation\":\"{0}\",", OOPKnowledgeRepository.EscapeJson(topic.CoreExplanation));
            sb.AppendFormat("\"tradeoffs\":\"{0}\",", OOPKnowledgeRepository.EscapeJson(topic.TradeoffsAndGotchas));
            sb.AppendFormat("\"proTip\":\"{0}\",", OOPKnowledgeRepository.EscapeJson(topic.InterviewerProTip));
            sb.AppendFormat("\"code\":\"{0}\",", OOPKnowledgeRepository.EscapeJson(topic.CodeExample));
            sb.AppendFormat("\"spokenSummary\":\"{0}\"", OOPKnowledgeRepository.EscapeJson(topic.SpokenSummary));
            sb.Append("}");

            return sb.ToString();
        }

        public void SetVoice(string voiceName)
        {
            NaturalVoiceEngine.SelectVoice(voiceName);
        }

        public void StopVoice()
        {
            NaturalVoiceEngine.Stop();
        }

        public void SpeakText(string text)
        {
            NaturalVoiceEngine.Speak(text);
        }

        public void CopyToClipboard(string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    Clipboard.SetText(text);
                }
            }
            catch { }
        }
    }

    // =========================================================================
    // 5. CHATGPT REPLICA UI FORM
    // =========================================================================
    public class MainForm : Form
    {
        private WebBrowser _webBrowser;
        private ChatScriptBridge _bridge;

        public MainForm()
        {
            this.Text = "ChatGPT - OOP Technical Assistant";
            this.Size = new Size(1280, 860);
            this.MinimumSize = new Size(980, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(23, 23, 23);
            this.Icon = SystemIcons.Application;

            this.FormClosing += (s, e) => {
                try { NaturalVoiceEngine.Stop(); } catch { }
            };
            this.FormClosed += (s, e) => {
                try { NaturalVoiceEngine.Stop(); } catch { }
                try {
                    if (_webBrowser != null) {
                        _webBrowser.Dispose();
                        _webBrowser = null;
                    }
                } catch { }
                try { Application.Exit(); } catch { }
                try { System.Diagnostics.Process.GetCurrentProcess().Kill(); } catch { }
                Environment.Exit(0);
            };

            _bridge = new ChatScriptBridge(this);

            _webBrowser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                IsWebBrowserContextMenuEnabled = false,
                WebBrowserShortcutsEnabled = true,
                ScriptErrorsSuppressed = true,
                ObjectForScripting = _bridge
            };

            this.Controls.Add(_webBrowser);

            NaturalVoiceEngine.Initialize();

            LoadChatGptUiHtml();
        }

        private void LoadChatGptUiHtml()
        {
            string html = @"<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='utf-8'/>
<meta http-equiv='X-UA-Compatible' content='IE=edge'/>
<title>ChatGPT - OOP Assistant</title>
<style>
* { box-sizing: border-box; margin: 0; padding: 0; user-select: text; }
html, body { height: 100%; width: 100%; font-family: Söhne, ui-sans-serif, system-ui, -apple-system, 'Segoe UI', Roboto, Ubuntu, Cantarell, 'Noto Sans', sans-serif, 'Helvetica Neue', Arial; background: #212121; color: #ECECEC; overflow: hidden; }

/* Custom Scrollbar */
::-webkit-scrollbar { width: 8px; height: 8px; }
::-webkit-scrollbar-track { background: transparent; }
::-webkit-scrollbar-thumb { background: #424242; border-radius: 4px; }
::-webkit-scrollbar-thumb:hover { background: #565656; }

/* App Layout */
#app-layout { display: flex; height: 100vh; width: 100vw; }

/* ==========================================================================
   SIDEBAR (260px)
   ========================================================================== */
#sidebar {
  width: 260px;
  min-width: 260px;
  background: #171717;
  display: flex;
  flex-direction: column;
  border-right: 1px solid rgba(255, 255, 255, 0.05);
  height: 100%;
}

/* Sidebar Top Header */
.sidebar-header {
  padding: 14px 14px 10px 14px;
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.sidebar-logo {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 16px;
  font-weight: 600;
  color: #ECECEC;
  cursor: pointer;
}
.sidebar-logo svg {
  width: 22px;
  height: 22px;
  fill: #ECECEC;
}
.sidebar-header-icons {
  display: flex;
  align-items: center;
  gap: 6px;
}
.icon-btn {
  background: transparent;
  border: none;
  color: #B4B4B4;
  padding: 6px;
  border-radius: 8px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 15px;
}
.icon-btn:hover {
  background: #212121;
  color: #ECECEC;
}

/* Sidebar Nav Links */
.sidebar-nav {
  padding: 4px 10px;
  display: flex;
  flex-direction: column;
  gap: 2px;
}
.nav-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 8px 10px;
  border-radius: 8px;
  font-size: 13.5px;
  color: #ECECEC;
  cursor: pointer;
  transition: background 0.15s ease;
}
.nav-item:hover {
  background: #212121;
}
.nav-item svg {
  width: 17px;
  height: 17px;
  fill: currentColor;
  opacity: 0.8;
}

/* Sidebar Recents Section */
.sidebar-section-title {
  padding: 16px 14px 6px 14px;
  font-size: 11.5px;
  font-weight: 600;
  color: #737373;
}
.recents-list {
  flex: 1;
  overflow-y: auto;
  padding: 2px 10px 10px 10px;
  display: flex;
  flex-direction: column;
  gap: 2px;
}
.recent-item {
  padding: 8px 10px;
  border-radius: 8px;
  font-size: 13px;
  color: #C5C5D2;
  cursor: pointer;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  display: block;
}
.recent-item:hover {
  background: #212121;
  color: #ECECEC;
}
.recent-item.active {
  background: #212121;
  color: #FFFFFF;
}

/* Sidebar Bottom Profile */
.sidebar-footer {
  padding: 10px;
  border-top: 1px solid rgba(255, 255, 255, 0.05);
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.profile-row {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 6px 8px;
  border-radius: 8px;
  cursor: pointer;
}
.profile-row:hover {
  background: #212121;
}
.avatar-badge {
  width: 28px;
  height: 28px;
  background: #10A37F;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: 700;
  color: #FFFFFF;
}
.profile-info {
  display: flex;
  flex-direction: column;
}
.profile-name {
  font-size: 13px;
  font-weight: 600;
  color: #ECECEC;
}
.profile-plan {
  font-size: 11px;
  color: #737373;
}

.claim-offer-btn {
  width: 100%;
  background: #212121;
  border: 1px solid rgba(255, 255, 255, 0.1);
  color: #ECECEC;
  padding: 8px;
  border-radius: 20px;
  font-size: 12.5px;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
}
.claim-offer-btn:hover {
  background: #2A2A2A;
}

/* ==========================================================================
   MAIN CHAT AREA
   ========================================================================== */
#main-area {
  flex: 1;
  display: flex;
  flex-direction: column;
  height: 100%;
  background: #212121;
  position: relative;
}

/* Top Navigation Bar */
.top-nav {
  height: 54px;
  display: flex;
  align-items: center;
  justify-content: flex-end;
  padding: 0 18px;
  gap: 10px;
}
.voice-picker-pill {
  background: #2A2A2A;
  border: 1px solid rgba(255, 255, 255, 0.1);
  color: #ECECEC;
  padding: 5px 12px;
  border-radius: 20px;
  font-size: 12.5px;
  outline: none;
  cursor: pointer;
}
.offer-btn {
  background: #1E3A8A;
  color: #93C5FD;
  border: none;
  padding: 6px 14px;
  border-radius: 20px;
  font-size: 13px;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 6px;
  cursor: pointer;
}
.offer-btn:hover {
  background: #1D4ED8;
}
.share-btn {
  background: transparent;
  color: #ECECEC;
  border: 1px solid rgba(255, 255, 255, 0.15);
  padding: 6px 12px;
  border-radius: 8px;
  font-size: 13px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 6px;
}
.share-btn:hover {
  background: #2A2A2A;
}

/* Chat Scroll Container */
#chat-scroll {
  flex: 1;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 20px 20px 100px 20px;
}
.chat-centered-column {
  width: 100%;
  max-width: 780px;
  display: flex;
  flex-direction: column;
  gap: 28px;
}

/* Hero Screen When Empty */
#hero-screen {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  margin-top: 12vh;
  gap: 24px;
  width: 100%;
  max-width: 780px;
}
.hero-title {
  font-size: 30px;
  font-weight: 700;
  color: #ECECEC;
  letter-spacing: -0.5px;
  text-align: center;
}
.hero-cards-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
  width: 100%;
}
.hero-card {
  background: #2F2F2F;
  border: 1px solid rgba(255, 255, 255, 0.05);
  border-radius: 14px;
  padding: 16px;
  cursor: pointer;
  transition: all 0.15s ease;
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.hero-card:hover {
  background: #383838;
  border-color: rgba(255, 255, 255, 0.15);
  transform: translateY(-1px);
}
.hero-card-title {
  font-size: 14px;
  font-weight: 600;
  color: #ECECEC;
}
.hero-card-desc {
  font-size: 12.5px;
  color: #949494;
  line-height: 1.4;
}

/* Messages */
.message-row {
  display: flex;
  width: 100%;
}
.user-row {
  justify-content: flex-end;
}
.user-bubble {
  background: #2F2F2F;
  color: #ECECEC;
  padding: 12px 18px;
  border-radius: 22px;
  max-width: 75%;
  font-size: 15px;
  line-height: 1.5;
  word-break: break-word;
}

.assistant-row {
  justify-content: flex-start;
}
.assistant-bubble {
  width: 100%;
  color: #ECECEC;
  font-size: 15px;
  line-height: 1.6;
}

/* Structured Answer Sections */
.answer-tag {
  display: inline-block;
  background: rgba(56, 189, 248, 0.12);
  color: #38BDF8;
  font-size: 11px;
  font-weight: 700;
  padding: 3px 8px;
  border-radius: 6px;
  margin-bottom: 8px;
  text-transform: uppercase;
}
.answer-title {
  font-size: 20px;
  font-weight: 700;
  color: #F8FAFC;
  margin-bottom: 12px;
}
.answer-summary {
  font-size: 15px;
  color: #E2E8F0;
  margin-bottom: 16px;
  line-height: 1.6;
}
.answer-explanation {
  font-size: 14.5px;
  color: #CBD5E1;
  white-space: pre-wrap;
  margin-bottom: 16px;
  line-height: 1.6;
}

/* Gotcha Box */
.gotcha-box {
  background: rgba(245, 158, 11, 0.08);
  border-left: 3px solid #F59E0B;
  padding: 12px 14px;
  border-radius: 0 8px 8px 0;
  font-size: 13.5px;
  color: #FDE68A;
  margin-bottom: 16px;
  line-height: 1.5;
}

/* Pro-Tip Box */
.protip-box {
  background: rgba(192, 132, 252, 0.08);
  border-left: 3px solid #C084FC;
  padding: 12px 14px;
  border-radius: 0 8px 8px 0;
  font-size: 13.5px;
  color: #E9D5FF;
  margin-bottom: 16px;
  line-height: 1.5;
}

/* Code Container */
.code-container {
  background: #0D0D0D;
  border: 1px solid #303030;
  border-radius: 8px;
  overflow: hidden;
  margin-top: 14px;
  margin-bottom: 16px;
}
.code-header {
  background: #181818;
  border-bottom: 1px solid #303030;
  padding: 7px 14px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 12px;
  font-weight: 600;
  color: #8E8E8E;
}
.code-copy-btn {
  background: transparent;
  border: none;
  color: #B4B4B4;
  font-size: 12px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 4px;
}
.code-copy-btn:hover {
  color: #FFFFFF;
}
.code-body {
  padding: 14px;
  font-family: 'Consolas', 'Courier New', monospace;
  font-size: 13px;
  color: #34D399;
  line-height: 1.5;
  overflow-x: auto;
  white-space: pre;
}

/* ChatGPT Action Bar Row Under Message */
.msg-actions-row {
  display: flex;
  align-items: center;
  gap: 6px;
  margin-top: 12px;
  padding-top: 6px;
}
.action-icon-btn {
  background: transparent;
  border: none;
  color: #737373;
  padding: 6px;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
}
.action-icon-btn:hover {
  background: #2F2F2F;
  color: #ECECEC;
}

/* ==========================================================================
   FLOATING BOTTOM INPUT CAPSULE
   ========================================================================== */
#bottom-dock {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  background: linear-gradient(180deg, rgba(33, 33, 33, 0) 0%, #212121 40%);
  padding: 10px 20px 16px 20px;
  display: flex;
  flex-direction: column;
  align-items: center;
}
.input-capsule-container {
  width: 100%;
  max-width: 780px;
  background: #2F2F2F;
  border: 1px solid #383838;
  border-radius: 28px;
  padding: 8px 12px 8px 14px;
  display: flex;
  align-items: center;
  gap: 10px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
}
.plus-btn {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: #3B3B3B;
  border: none;
  color: #B4B4B4;
  font-size: 18px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  flex-shrink: 0;
}
.plus-btn:hover {
  background: #474747;
  color: #ECECEC;
}
input.chat-input {
  flex: 1;
  background: transparent;
  border: none;
  color: #ECECEC;
  font-size: 15px;
  outline: none;
  font-family: inherit;
}
input.chat-input::placeholder {
  color: #8E8E8E;
}

.capsule-actions {
  display: flex;
  align-items: center;
  gap: 8px;
}
.think-btn {
  background: transparent;
  border: 1px solid #4D4D4D;
  border-radius: 16px;
  padding: 4px 10px;
  font-size: 12px;
  color: #B4B4B4;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 4px;
}
.think-btn:hover {
  background: #383838;
  color: #ECECEC;
}
.mic-btn {
  background: transparent;
  border: none;
  color: #B4B4B4;
  font-size: 16px;
  cursor: pointer;
  padding: 6px;
  border-radius: 50%;
}
.mic-btn:hover {
  color: #ECECEC;
}

/* Blue Audio Waveform Circle (Like ChatGPT's Voice Mode) */
.voice-wave-circle {
  width: 34px;
  height: 34px;
  border-radius: 50%;
  background: #2563EB;
  border: none;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #FFFFFF;
  cursor: pointer;
  transition: all 0.15s ease;
  flex-shrink: 0;
}
.voice-wave-circle:hover {
  background: #1D4ED8;
  transform: scale(1.05);
}
.wave-bars {
  display: flex;
  align-items: center;
  gap: 2px;
  height: 14px;
}
.wave-bar {
  width: 2px;
  background: #FFFFFF;
  border-radius: 1px;
}
.wb-1 { height: 6px; }
.wb-2 { height: 12px; }
.wb-3 { height: 16px; }
.wb-4 { height: 8px; }

/* Disclaimer */
.disclaimer-text {
  font-size: 11.5px;
  color: #737373;
  margin-top: 8px;
  text-align: center;
}
</style>
</head>
<body>

<div id='app-layout'>

  <!-- ========================================================================
       SIDEBAR
       ======================================================================== -->
  <div id='sidebar'>
    <div class='sidebar-header'>
      <div class='sidebar-logo' onclick='startNewChat()'>
        <!-- ChatGPT SVG Logo -->
        <svg viewBox='0 0 24 24'>
          <path d='M22.28 12.18a7.99 7.99 0 0 0-.6-5.83 8 8 0 0 0-7.85-4.35 8 8 0 0 0-4.8 2.05A7.99 7.99 0 0 0 4.1 6.84a8 8 0 0 0 .8 8.94 8 8 0 0 0 .6 5.83 8 8 0 0 0 7.85 4.35 8 8 0 0 0 4.8-2.05 7.99 7.99 0 0 0 4.93-2.79 8 8 0 0 0-.8-8.94Zm-8.48 9.32a6.38 6.38 0 0 1-3.87-.27 6.46 6.46 0 0 1 .84-2.8l2.96 1.71a.8.8 0 0 0 .8 0l4.24-2.45a6.43 6.43 0 0 1-4.97 3.81Zm-5.46-3.26a6.47 6.47 0 0 1-.84-2.8 6.47 6.47 0 0 1 2.37-5.07l.02.01 2.96 1.71a.8.8 0 0 0 .8 0l4.24-2.45v4.9a6.43 6.43 0 0 1-9.55 3.7Zm-1.8-6.93a6.38 6.38 0 0 1 .5-3.84 6.46 6.46 0 0 1 2.5 1.55l-.01.02v3.42a.8.8 0 0 0 .4.69l4.24 2.45-4.24 2.45a6.43 6.43 0 0 1-3.39-6.74Zm11.13-2.61-4.24-2.45a6.43 6.43 0 0 1 4.97-3.81 6.38 6.38 0 0 1 3.87.27 6.46 6.46 0 0 1-.84 2.8l-2.96-1.71a.8.8 0 0 0-.8 0Zm2.96 4.91a6.47 6.47 0 0 1-2.37 5.07l-.02-.01-2.96-1.71a.8.8 0 0 0-.8 0l-4.24 2.45v-4.9a6.43 6.43 0 0 1 9.55-3.7l.84 2.8Zm1.8 6.93a6.38 6.38 0 0 1-.5 3.84 6.46 6.46 0 0 1-2.5-1.55l.01-.02v-3.42a.8.8 0 0 0-.4-.69l-4.24-2.45 4.24-2.45a6.43 6.43 0 0 1 3.39 6.74Z'/>
        </svg>
        <span>ChatGPT</span>
      </div>
      <div class='sidebar-header-icons'>
        <button class='icon-btn' title='Search' onclick='focusSearch()'>🔍</button>
        <button class='icon-btn' title='New chat' onclick='startNewChat()'>◫</button>
      </div>
    </div>

    <!-- Navigation Items -->
    <div class='sidebar-nav'>
      <div class='nav-item' onclick='startNewChat()'>
        <span>✏️</span>
        <span>New chat</span>
      </div>
      <div class='nav-item' onclick='filterRecents(""PILLARS"")'>
        <span>💡</span>
        <span>The 4 Pillars</span>
      </div>
      <div class='nav-item' onclick='filterRecents(""SOLID"")'>
        <span>📐</span>
        <span>SOLID Principles</span>
      </div>
      <div class='nav-item' onclick='filterRecents(""PATTERNS"")'>
        <span>🧩</span>
        <span>Design Patterns</span>
      </div>
      <div class='nav-item' onclick='filterRecents(""ARCHITECTURE"")'>
        <span>🏗️</span>
        <span>Architecture & GC</span>
      </div>
    </div>

    <!-- Recents -->
    <div class='sidebar-section-title'>RECENTS</div>
    <div id='recentsContainer' class='recents-list'>
      <!-- Populated via JavaScript -->
    </div>

    <!-- Sidebar Footer -->
    <div class='sidebar-footer'>
      <div class='profile-row'>
        <div class='avatar-badge'>AS</div>
        <div class='profile-info'>
          <div class='profile-name'>asura</div>
          <div class='profile-plan'>Free Plan</div>
        </div>
      </div>
      <button class='claim-offer-btn' onclick='toggleVoiceState()'>
        <span>🎁</span>
        <span id='voiceStatusLabel'>Voice: Mark (Active)</span>
      </button>
    </div>
  </div>

  <!-- ========================================================================
       MAIN CHAT AREA
       ======================================================================== -->
  <div id='main-area'>

    <!-- Top Bar -->
    <div class='top-nav'>
      <select id='voiceSelectTop' class='voice-picker-pill' onchange='handleVoiceChange()'>
        <option value='Mark'>🎙️ Natural: Mark (Male)</option>
        <option value='Zira'>🎙️ Natural: Zira (Female)</option>
        <option value='David'>🎙️ Natural: David (Male)</option>
        <option value='MUTED'>🔇 Voice Muted</option>
      </select>
      <button class='offer-btn' onclick='window.external.StopVoice()'>
        <span>🎁</span>
        <span>Stop Speech</span>
      </button>
      <button class='share-btn' onclick='copyEntireChat()'>
        <span>↗</span>
        <span>Share</span>
      </button>
    </div>

    <!-- Chat Messages Scroll Container -->
    <div id='chat-scroll'>

      <!-- Empty / Hero Screen -->
      <div id='hero-screen'>
        <div class='hero-title'>What can I help with?</div>
        <div class='hero-cards-grid'>
          <div class='hero-card' onclick='submitQuestion(""Encapsulation vs Abstraction"")'>
            <div class='hero-card-title'>Encapsulation vs Abstraction</div>
            <div class='hero-card-desc'>Data hiding, invariants, access modifiers, and real-world car analogies</div>
          </div>
          <div class='hero-card' onclick='submitQuestion(""Dynamic Dispatch & vtable"")'>
            <div class='hero-card-title'>Dynamic Dispatch & vtable</div>
            <div class='hero-card-desc'>Under-the-hood function pointers, virtual method tables, and memory layout</div>
          </div>
          <div class='hero-card' onclick='submitQuestion(""Liskov Substitution Principle (LSP)"")'>
            <div class='hero-card-title'>Liskov Substitution (LSP)</div>
            <div class='hero-card-desc'>Behavioral subtyping, square-rectangle problem, preconditions and contracts</div>
          </div>
          <div class='hero-card' onclick='submitQuestion(""Composition Over Inheritance"")'>
            <div class='hero-card-title'>Composition Over Inheritance</div>
            <div class='hero-card-desc'>The fragile base class problem, tight coupling, and runtime flexibility</div>
          </div>
        </div>
      </div>

      <!-- Chat History Container -->
      <div id='chatColumn' class='chat-centered-column' style='display: none;'>
        <!-- Appended dynamically -->
      </div>

    </div>

    <!-- Floating Bottom Input Dock -->
    <div id='bottom-dock'>
      <div class='input-capsule-container'>
        <button class='plus-btn' title='Add attachment'>+</button>
        <input type='text' id='promptInput' class='chat-input' placeholder='Ask ChatGPT...' onkeydown='handleKeyDown(event)' autocomplete='off'/>
        <div class='capsule-actions'>
          <button class='think-btn'>
            <span>🧠</span>
            <span>Think</span>
          </button>
          <button class='mic-btn' title='Read latest' onclick='readLatestMessage()'>🎙</button>
          <button class='voice-wave-circle' title='Send question or speak' onclick='handleSend()'>
            <div class='wave-bars'>
              <div class='wave-bar wb-1'></div>
              <div class='wave-bar wb-2'></div>
              <div class='wave-bar wb-3'></div>
              <div class='wave-bar wb-4'></div>
            </div>
          </button>
        </div>
      </div>
      <div class='disclaimer-text'>ChatGPT can make mistakes. Check important info.</div>
    </div>

  </div>

</div>

<script>
var allTopics = [];
var lastAiResponseText = '';

window.onload = function() {
    try {
        var jsonStr = window.external.GetInitialTopicsJson();
        allTopics = JSON.parse(jsonStr);
        renderRecents(allTopics);

        var voiceSelect = document.getElementById('voiceSelectTop');
        if (voiceSelect.value !== 'MUTED') {
            window.external.SpeakText('Welcome! Ask me any question regarding Object Oriented Programming, or click any topic on the left sidebar.');
        }
    } catch (e) { }
};

function renderRecents(topics) {
    var container = document.getElementById('recentsContainer');
    container.innerHTML = '';
    for (var i = 0; i < topics.length; i++) {
        var t = topics[i];
        var item = document.createElement('div');
        item.className = 'recent-item';
        item.innerText = t.title;
        item.onclick = (function(topic) {
            return function() { submitQuestion(topic.title); };
        })(t);
        container.appendChild(item);
    }
}

function filterRecents(category) {
    var filtered = [];
    for (var i = 0; i < allTopics.length; i++) {
        if (allTopics[i].category.indexOf(category) >= 0) {
            filtered.push(allTopics[i]);
        }
    }
    renderRecents(filtered.length > 0 ? filtered : allTopics);
}

function focusSearch() {
    var q = prompt('Search OOP Topics:');
    if (q) submitQuestion(q);
}

function startNewChat() {
    document.getElementById('hero-screen').style.display = 'flex';
    document.getElementById('chatColumn').style.display = 'none';
    document.getElementById('chatColumn').innerHTML = '';
    document.getElementById('promptInput').value = '';
    window.external.StopVoice();
}

function handleKeyDown(e) {
    if (e.keyCode === 13) {
        e.preventDefault();
        handleSend();
    }
}

function handleSend() {
    var input = document.getElementById('promptInput');
    var q = input.value.trim();
    if (!q) return;
    input.value = '';
    submitQuestion(q);
}

function submitQuestion(q) {
    // Hide hero screen, show chat column
    document.getElementById('hero-screen').style.display = 'none';
    var chatCol = document.getElementById('chatColumn');
    chatCol.style.display = 'flex';

    // 1. Append User Message
    appendUserMessage(q);

    // 2. Query C# backend
    var voiceSelect = document.getElementById('voiceSelectTop');
    var isVoiceEnabled = (voiceSelect.value !== 'MUTED');
    var jsonStr = window.external.AskQuestion(q, isVoiceEnabled);
    var data = JSON.parse(jsonStr);

    // 3. Append Assistant Message
    appendAssistantMessage(data);

    // 4. Scroll smoothly to bottom
    var scrollContainer = document.getElementById('chat-scroll');
    scrollContainer.scrollTop = scrollContainer.scrollHeight;
}

function appendUserMessage(text) {
    var chatCol = document.getElementById('chatColumn');
    var row = document.createElement('div');
    row.className = 'message-row user-row';
    row.innerHTML = '<div class=""user-bubble"">' + escapeHtml(text) + '</div>';
    chatCol.appendChild(row);
}

function appendAssistantMessage(data) {
    var chatCol = document.getElementById('chatColumn');
    var row = document.createElement('div');
    row.className = 'message-row assistant-row';

    var fullTextToCopy = data.title + ' (' + data.category + ')\n\n' +
                         data.summary + '\n\n' +
                         data.explanation + '\n\n' +
                         (data.tradeoffs ? 'TRADEOFFS:\n' + data.tradeoffs + '\n\n' : '') +
                         (data.proTip ? 'PRO-TIP:\n' + data.proTip + '\n\n' : '') +
                         (data.code ? 'CODE:\n' + data.code : '');

    lastAiResponseText = fullTextToCopy;

    var html = '<div class=""assistant-bubble"">';
    
    // Category Tag & Title
    html += '<span class=""answer-tag"">' + escapeHtml(data.category) + '</span>';
    html += '<div class=""answer-title"">' + escapeHtml(data.title) + '</div>';

    // Summary
    html += '<div class=""answer-summary"">' + escapeHtml(data.summary) + '</div>';

    // Explanation
    html += '<div class=""answer-explanation"">' + escapeHtml(data.explanation) + '</div>';

    // Gotcha Box
    if (data.tradeoffs) {
        html += '<div class=""gotcha-box""><b>⚠️ PITFALLS & GOTCHAS:</b><br/>' + escapeHtml(data.tradeoffs) + '</div>';
    }

    // Senior Pro-Tip Box
    if (data.proTip) {
        html += '<div class=""protip-box""><b>🏆 SENIOR ARCHITECT INTERVIEW PRO-TIP:</b><br/>' + escapeHtml(data.proTip) + '</div>';
    }

    // Code Block
    if (data.code) {
        html += '<div class=""code-container"">' +
                '  <div class=""code-header"">' +
                '    <span>csharp</span>' +
                '    <button class=""code-copy-btn"" onclick=""copyCodeText(' + JSON.stringify(data.code) + ')"">📋 Copy code</button>' +
                '  </div>' +
                '  <div class=""code-body"">' + escapeHtml(data.code) + '</div>' +
                '</div>';
    }

    // ChatGPT Action Buttons Row: [Copy] [Thumbs Up] [Thumbs Down] [Read Aloud] [Share] [Regenerate] [More]
    html += '<div class=""msg-actions-row"">' +
            '  <button class=""action-icon-btn"" title=""Copy"" onclick=""copyCodeText(' + JSON.stringify(fullTextToCopy) + ')"">⧉</button>' +
            '  <button class=""action-icon-btn"" title=""Good response"">👍</button>' +
            '  <button class=""action-icon-btn"" title=""Bad response"">👎</button>' +
            '  <button class=""action-icon-btn"" title=""Read aloud"" onclick=""speakAloudText(' + JSON.stringify(data.spokenSummary || data.summary) + ')"">🔊</button>' +
            '  <button class=""action-icon-btn"" title=""Share"" onclick=""copyCodeText(' + JSON.stringify(fullTextToCopy) + ')"">📤</button>' +
            '  <button class=""action-icon-btn"" title=""Regenerate"" onclick=""submitQuestion(' + JSON.stringify(data.title) + ')"">🔄</button>' +
            '  <button class=""action-icon-btn"" title=""More options"">⋯</button>' +
            '</div>';

    html += '</div>';
    row.innerHTML = html;
    chatCol.appendChild(row);
}

function handleVoiceChange() {
    var val = document.getElementById('voiceSelectTop').value;
    if (val === 'MUTED') {
        window.external.StopVoice();
        document.getElementById('voiceStatusLabel').innerText = 'Voice: Muted';
    } else {
        window.external.SetVoice(val);
        document.getElementById('voiceStatusLabel').innerText = 'Voice: ' + val + ' (Active)';
    }
}

function toggleVoiceState() {
    var sel = document.getElementById('voiceSelectTop');
    if (sel.value === 'MUTED') {
        sel.value = 'Mark';
    } else {
        sel.value = 'MUTED';
    }
    handleVoiceChange();
}

function speakAloudText(text) {
    window.external.SpeakText(text);
}

function readLatestMessage() {
    if (lastAiResponseText) {
        window.external.SpeakText(lastAiResponseText);
    }
}

function copyCodeText(text) {
    window.external.CopyToClipboard(text);
    alert('Copied to clipboard!');
}

function copyEntireChat() {
    if (lastAiResponseText) {
        window.external.CopyToClipboard(lastAiResponseText);
        alert('Conversation copied to clipboard!');
    }
}

function escapeHtml(str) {
    if (!str) return '';
    return str.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/""/g, '&quot;').replace(/'/g, '&#039;');
}
</script>
</body>
</html>";

            _webBrowser.DocumentText = html;
        }
    }

    // =========================================================================
    // 6. APPLICATION ENTRY POINT
    // =========================================================================
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new MainForm());
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
