import { useMemo } from 'react';

import { DEFAULT_GRID_SKELETON_ROWS_AMOUNT } from '@/components/Grid/constants';
import { getRandomColor } from '@/helpers/getRandomColor';
import { getFullName } from '@/helpers/utils';
import { Dayjs } from '@/lib/dayjs';
import { ProjectEntity, ProjectsGridRow } from '@/types/project';

const mockRow: ProjectsGridRow = {
	id: 0,
	projectUsers: [],
	clientName: '',
	name: '',
	factTrackedTime: 0,
	billableTrackedTime: 0,
	isPrivate: false,
	isBillable: false,
	isPublic: false,
	isActive: true,
	showSkeleton: true,
	color: getRandomColor(),
};

export const useProjectsRows = (
	items: Array<ProjectEntity> | undefined,
	dataUpdatedAt: Dayjs | Date | number,
	page: number,
	rowsPerPage: number,
	isFetching: boolean,
	isLoading: boolean,
	handleEdit: (id?: string | number) => () => void
): Array<ProjectsGridRow> => {
	const showSkeleton = isLoading || isFetching;

	return useMemo(
		() =>
			showSkeleton || !items
				? new Array(items?.length ?? DEFAULT_GRID_SKELETON_ROWS_AMOUNT)
						.fill(mockRow)
						.map((row, index) => ({ ...row, id: -99999 - index }))
				: items.map(
						({
							clientName,
							color,
							id,
							isActive,
							isPublic,
							isBillable,
							name,
							projectUsers,
							factTrackedTime,
							billableTrackedTime,
						}): ProjectsGridRow => {
							const accessUsers = isPublic
								? []
								: projectUsers
										.map((user) => getFullName(user.firstName, user.lastName))
										.sort((a, b) => a.localeCompare(b));

							return {
								id,
								projectUsers: accessUsers,
								clientName,
								name,
								factTrackedTime,
								billableTrackedTime,
								isPrivate: !isPublic,
								isPublic,
								isBillable,
								isActive,
								color,
								disabled: !isActive,
								handleEditClick: handleEdit,
							};
						}
				  ),
		[dataUpdatedAt, page, rowsPerPage, isLoading, isFetching, items]
	);
};
