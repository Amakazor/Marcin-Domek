namespace Marcin_Domek_Server.Src.Requests
{
    internal enum RequestType
    {
        Empty,
        Logout,

        ListUsers,
        AddUser,
        DeleteUser,
        ChangeUserPassword,
        ChangeUserType,

        ApplyTicketToSelf,
        ReleaseTicket,
        RejectTicket,
        CompleteTicket,
        ListTickets,
        ListMyTickets,
        ListUnclaimedTickets,
        CreateTicket,

        CreateExpense,
        DeleteExpense,
        EditExpense,
        ListExpenses,
        SearchExpenses,

        CreateIncome,
        DeleteIncome,
        EditIncome,
        ListIncome,
        SearchIncome,

        ImportExpenses,

        ImportIncome,
    }
}
