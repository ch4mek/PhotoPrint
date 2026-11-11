import {
	COLLAPSIBLE_TABLE_FIRST_COLUMN_WIDTH,
	COLLAPSIBLE_TABLE_LAST_COLUMN_WIDTH_LARGE,
	COLLAPSIBLE_TABLE_LAST_COLUMN_WIDTH_SMALL,
	COLLAPSIBLE_TABLE_TASK_COLUMN_WIDTH,
	TASK_ROW_LAST_COLUMN_WIDTH_LARGE,
	TASK_ROW_LAST_COLUMN_WIDTH_SMALL,
} from './constants';
import { GridTemplateColumnsForCollapsibleTableType } from './types';

export const getGridTemplateColumnsForCollapsibleTable = ({
	isDisplayFact,
	isTaskRow,
}: GridTemplateColumnsForCollapsibleTableType) => {
	if (isTaskRow) {
		return `${COLLAPSIBLE_TABLE_TASK_COLUMN_WIDTH}px 1fr ${
			isDisplayFact
				? TASK_ROW_LAST_COLUMN_WIDTH_LARGE
				: TASK_ROW_LAST_COLUMN_WIDTH_SMALL
		}px`;
	}

	return `${COLLAPSIBLE_TABLE_FIRST_COLUMN_WIDTH}px 1fr ${
		isDisplayFact
			? COLLAPSIBLE_TABLE_LAST_COLUMN_WIDTH_LARGE
			: COLLAPSIBLE_TABLE_LAST_COLUMN_WIDTH_SMALL
	}px`;
};
