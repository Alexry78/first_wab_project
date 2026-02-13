namespace NotificationPriority.PartB_Inheritance
{
    public class AlertEvent : Event
    {
        public AlertEvent() : base(5) // База 5
        {
        }

        public override int GetPriority(NotifyContext context)
        {
            int priority = BasePriority;
            return ApplyModifiers(priority, context);
        }
    }
}