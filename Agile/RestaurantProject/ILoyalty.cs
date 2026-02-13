namespace RestaurantProject
{
    public interface ILoyalty
    {
        int LoyaltyPoints { get; set; }
        /// <param name="amount">Количество добавляемых баллов</param>
        void AddPoints(int amount);
    }
}