namespace NotificationPriority.PartB_Inheritance
{
    public class CriticalEvent : Event
    {
        public CriticalEvent() : base(8) // База 8
        {
        }

        public override int GetPriority(NotifyContext context)
        {
            int priority = BasePriority;
            return ApplyModifiers(priority, context);
        }
    }
}