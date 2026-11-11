import { TaskTableRowItemProps } from '@/components/CollapsibleTable/TaskTable/TaskTableRowItem/types';
import { TasksResponse } from '@/types/tasks';

export const convertToTaskRowType = (
	data: TasksResponse[]
): TaskTableRowItemProps[] =>
	data?.map((item) => ({
		...item,
		trackedInMinutes: item.trackedTimeInMinutes,
	}));
