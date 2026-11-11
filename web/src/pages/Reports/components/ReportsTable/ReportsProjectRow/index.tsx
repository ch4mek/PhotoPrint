import { keepPreviousData } from '@tanstack/react-query';
import { memo, useMemo } from 'react';

import { useOpenCollapsibleRow } from '@/components/CollapsibleTable/hooks/useOpenCollapsibleRow';
import { TasksList } from '@/components/CollapsibleTable/TasksList';
import { TaskTableRow } from '@/components/CollapsibleTable/TaskTable';
import { useGetUserTasks } from '@/queries/useGetUserTasks';

import type { ProjectRowProps } from './types';
import { convertToTaskRowType } from './utils';

function ReportsProjectRowInner({
	userId,
	queryParams,
	project,
	isUserRowOpen,
	isLastItem,
}: ProjectRowProps) {
	const { year, monthOfYear, trackedTimeType } = queryParams;

	const { projectId } = project;

	const {
		isRowOpen: isProjectsRowOpen,
		handleSetRowOpen: handleSetReportsRowOpen,
	} = useOpenCollapsibleRow();

	const { data, isLoading: isTasksLoading } = useGetUserTasks(
		{
			userId,
			projectId,
			year,
			monthOfYear,
			trackedTimeType,
		},
		{
			enabled: Boolean(isUserRowOpen && isProjectsRowOpen),
			placeholderData: keepPreviousData,
		}
	);

	const convertedData = useMemo(() => convertToTaskRowType(data || []), [data]);

	return (
		<TasksList
			totalTrackedInMinutes={project.totalTimeInMinutes}
			{...project}
			isProjectsRowOpen={isProjectsRowOpen}
			handleSetRowOpen={handleSetReportsRowOpen}
			isLastItem={isLastItem}
		>
			<TaskTableRow
				data={convertedData}
				isLoading={isTasksLoading}
				isTaskRowHeader
			/>
		</TasksList>
	);
}

export const ReportsProjectRow = memo(ReportsProjectRowInner);
