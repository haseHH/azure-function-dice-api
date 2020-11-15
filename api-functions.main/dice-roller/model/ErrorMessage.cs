namespace dice_roller.model
{
    class ErrorMessage
    {
        public string ErrorType { get; set; }
        public string RequestedToss { get; set; }
        public string Message { get; set; }

        public ErrorMessage(string type, string dice, string mes)
        {
            ErrorType = type;
            RequestedToss = dice;
            Message = mes;
        }
    }
}
