# SingletoneHelper
An extremely useful class for creating objects using a singleton.

Sometimes you need to use a singleton to create single instances of classes. This project demonstrates a convenient and fast implementation of this pattern.

Q: Why not use lazy<>?
A: It's slow, and it also automatically regulates the object's state.

Q: How do I use it?
A: Now just use the instance created in SingletoneHelper:
 
* SingletonHelper.Instance.GetSingleton <FooClass> ().Bar();


You can also create a separate object that will reference the created class object.
To create objects
 
*  private FooClass fooClass = SingletonHelper.Instance.GetSingleton <FooClass> ();
*  fooClass.Bar();

You can also use a unique ID or rewrite the code for Enum

* SingletonHelper.Instance.GetSingleton <FooClass> (1);
* SingletonHelper.Instance.GetSingleton <FooClass> (2);

You can also receive notifications about new/received/deleted objects using events

*  Program.SingletoneInstance.OnObjectCreateEvent += SingletoneInstance_OnObjectCreateEvent;
*  Program.SingletoneInstance.OnGetInstanceEvent += SingletoneInstance_OnGetInstanceEvent; 
*  Program.SingletoneInstance.OnObjectRemovedEvent += SingletoneInstance_OnObjectRemovedEvent; 



Q: Singleton is an anti-pattern.
A: Don't hammer nails with a microscope. This project isn't designed to solve absolutely every problem in the code or solution.

