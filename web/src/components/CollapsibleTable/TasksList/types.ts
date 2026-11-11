import { ReactNode } from 'react';

export interface TasksListProps {
	children: ReactNode;
	tasksCount: number;
	projectName: string;
	clientName: string;
	totalTrackedInMinutes: number;
	isDisplayFact?: boolean;
	handleSetRowOpen: () => void;
	isProjectsRowOpen: boolean;
	isLastItem?: boolean;
}
