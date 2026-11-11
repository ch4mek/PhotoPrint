export const developmentPlanColumnsKeys = {
	status: 'status',
	deadlineDate: 'deadlineDate',
	summary: 'summary',
};

export const columnsWidth: Record<
	keyof typeof developmentPlanColumnsKeys,
	number
> = {
	status: 122,
	deadlineDate: 123,
	summary: 300,
};

export const RESET_FILTER_COLUMN_INDEX =
	Object.values(developmentPlanColumnsKeys).length + 2;
