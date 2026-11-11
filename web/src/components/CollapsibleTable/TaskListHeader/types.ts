interface TasksListHeaderProps {
	projectTitle: string;
	tasksCount: number;
	totalTrackedInMinutes: number;
	handleSetRowOpen: () => void;
	isRowOpen: boolean;
	isDisplayFact: boolean;
	showSkeleton: boolean;
	isLastItem?: boolean;
}

export type PartialTasksListHeaderProps = Partial<TasksListHeaderProps>;
