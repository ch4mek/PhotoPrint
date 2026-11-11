import { ReportsProjectRow } from '../ReportsProjectRow';

import { useOpenCollapsibleRow } from '@/components/CollapsibleTable/hooks/useOpenCollapsibleRow';
import { TaskListHeader } from '@/components/CollapsibleTable/TaskListHeader';
import { UserTableRow } from '@/components/CollapsibleTable/UserTableRow';
import { useGetUserProjects } from '@/queries/useGetUserProjects';
import { ProjectResponse } from '@/types/project';

import type { UserRowProps } from './types';

export function ReportsUserTableRow({ row, queryParams }: UserRowProps) {
	const { totalTimeInMinutes, lastName, firstName, projectsCount, userId } =
		row;
	const userName = `${lastName} ${firstName}`;

	const { isRowOpen: isUserRowOpen, handleSetRowOpen } =
		useOpenCollapsibleRow();

	const { data, isLoading: isProjectsLoading } = useGetUserProjects(
		{ ...queryParams, userId },
		{
			enabled: isUserRowOpen,
		}
	);

	return (
		<UserTableRow
			row={{
				...row,
				countOfProjects: projectsCount,
				totalTrackedInMinutes: totalTimeInMinutes,
				userName,
			}}
			isRowOpen={isUserRowOpen}
			handleSetRowOpen={handleSetRowOpen}
		>
			{isProjectsLoading && <TaskListHeader showSkeleton />}
			{data?.map((project: ProjectResponse, index: number) => (
				<ReportsProjectRow
					key={`${userId} - ${project.projectId}`}
					userId={userId}
					project={project}
					queryParams={queryParams}
					isUserRowOpen={isUserRowOpen}
					isLastItem={index === data.length - 1}
				/>
			))}
		</UserTableRow>
	);
}
