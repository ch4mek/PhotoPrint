import { RateHistoryContext, RatesGridRow } from '../types';

export const getRateHistoryContextFromRows = (
	rowId: string,
	rows: RatesGridRow[]
): RateHistoryContext | undefined => {
	const row = rows.find(({ id }) => id === rowId);

	if (!row?.projectId) {
		return undefined;
	}

	const parentRow = row.isParent
		? row
		: rows.find(({ id }) => id === row.parentId);
	const userId = parentRow?.id;
	const userName = parentRow?.user;
	const project = row.projects[0];

	if (!userId || !userName || !project) {
		return undefined;
	}

	return {
		userId,
		projectId: row.projectId,
		userName,
		projectName: project.name,
		projectColor: project.color,
	};
};
