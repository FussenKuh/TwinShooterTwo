ReadOnlyAttribute adds a script attribute to Unity that allows you to mark class variables as 'ReadOnly.' 

Once included in your project, simply add the [ReadOnly] attribute tag in front of any of your public or serializefield class variables to make them show up in the Unity Inspector as read only labels.

Example:

public class MyClass : MonoBehaviour 
{
   [ReadOnly] public float _myFloat;
   [ReadOnly] [SerializedField] int _myInt;
}