export declare enum LearningProgramStatus {
    Assigned = "assigned",
    InProgress = "inProgress",
    Passed = "passed",
    Failed = "failed",
    Quitted = "quitted"
}
export type LearningProgramStatusProps = {
    status: LearningProgramStatus;
    statusLabel: string;
    dateLabel: string;
    progress: number;
};
export type LearningProgramStatusSectionProps = LearningProgramStatusProps & {
    dataTestId?: string;
};
