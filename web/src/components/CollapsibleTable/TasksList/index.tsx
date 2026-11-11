import { TaskListHeader } from '../TaskListHeader';

import { Collapse } from '@/components/Collapse';

import type { TasksListProps } from './types';

export function TasksList({
	tasksCount,
	projectName,
	clientName,
	totalTrackedInMinutes,
	children,
	isDisplayFact = false,
	handleSetRowOpen,
	isProjectsRowOpen,
	isLastItem,
}: TasksListProps) {
	const projectTitle = `${projectName} - ${clientName}`;

	return (
		<>
			<TaskListHeader
				projectTitle={projectTitle}
				tasksCount={tasksCount}
				totalTrackedInMinutes={totalTrackedInMinutes}
				handleSetRowOpen={handleSetRowOpen}
				isRowOpen={isProjectsRowOpen}
				isDisplayFact={isDisplayFact}
				isLastItem={isLastItem}
			/>
			<Collapse isOpen={isProjectsRowOpen}>{children}</Collapse>
		</>
	);
}
