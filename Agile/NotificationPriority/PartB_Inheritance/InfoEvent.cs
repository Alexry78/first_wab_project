namespace NotificationPriority.PartB_Inheritance
{
    public class InfoEvent : Event
    {
        public InfoEvent() : base(1) // База 1
        {
        }

        public override int GetPriority(NotifyContext context)
        {
            int priority = BasePriority;
            return ApplyModifiers(priority, context);
        }
    }
}