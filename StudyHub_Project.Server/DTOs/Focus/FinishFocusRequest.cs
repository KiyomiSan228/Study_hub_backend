using StudyHub_Project.Server.Models;

namespace StudyHub_Project.Server.DTOs.Focus;

public class FinishFocusRequest
{
    public FocusSessionStatus Status { get; set; }

    public int FocusedSeconds { get; set; }
}