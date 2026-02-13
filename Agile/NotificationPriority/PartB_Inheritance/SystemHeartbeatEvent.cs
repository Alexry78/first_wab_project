namespace NotificationPriority.PartB_Inheritance
{
    public class SystemHeartbeatEvent : Event
    {
        public SystemHeartbeatEvent() : base(1) // База 1
        {
        }

        public override int GetPriority(NotifyContext context)
        {
           
            return 1;
        }
    }
}