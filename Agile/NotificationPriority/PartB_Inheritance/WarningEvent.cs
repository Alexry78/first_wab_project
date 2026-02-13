namespace NotificationPriority.PartB_Inheritance
{
    public class WarningEvent : Event
    {
        public WarningEvent() : base(3) // База 3
        {
        }

        public override int GetPriority(NotifyContext context)
        {
            int priority = BasePriority;
            return ApplyModifiers(priority, context);
        }
    }
}