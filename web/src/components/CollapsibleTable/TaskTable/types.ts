import { TaskTableRowItemProps } from './TaskTableRowItem/types';

export interface TaskTableRowProps {
	data: TaskTableRowItemProps[];
	isLoading: boolean;
	isDisplayFact?: boolean;
	isTaskRowHeader?: boolean;
}
