using System.Collections.Generic;

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
        ChangeTicketPriority,
        CompleteTicket,
        ListTickets,
        CreateTicket,
        CreateExpense,
        DeleteExpense,
        EditExpense,
        ListExpenses,
        CreateIncome,
        DeleteIncome,
        EditIncome,
        ListIncome,
        ImportExpenses,
        ExportExpenses,
        ImportIncome,
        ExportIncome
    }
}
