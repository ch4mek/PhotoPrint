import { useMemo } from 'react';

import { PROJECT_PERMISSIONS_GRID_SKELETON_ROWS_AMOUNT } from '../../../../grids/constants';

import { mockRow } from './constants';
import {
	BillableWorkingHoursPermissionsRow,
	UseBillableWorkingHoursPermissionsRowsOptions,
} from './types';

export const useBillableWorkingHoursPermissionsRows = ({
	billableWorkingHoursPermissionsRequest,
	page,
	rowsPerPage,
}: UseBillableWorkingHoursPermissionsRowsOptions) => {
	const { data, isLoading, isFetching, dataUpdatedAt } =
		billableWorkingHoursPermissionsRequest;

	const showSkeleton = isLoading || isFetching;

	const rows: BillableWorkingHoursPermissionsRow[] = useMemo(
		() =>
			showSkeleton || !data?.items
				? new Array(
						data?.items?.length ?? PROJECT_PERMISSIONS_GRID_SKELETON_ROWS_AMOUNT
				  )
						.fill(mockRow)
						.map((row, index) => ({ ...row, assignmentId: -99999 - index }))
				: data?.items.map(
						({
							userId,
							userFullName,
							projectId,
							projectName,
							assignmentId,
							userColor: color,
						}) => ({
							userId,
							assignmentId,
							userFullName,
							projectId,
							projectName,
							showSkeleton,
							color,
						})
				  ),
		[dataUpdatedAt, page, rowsPerPage, isLoading, isFetching]
	);

	return { rows };
};
