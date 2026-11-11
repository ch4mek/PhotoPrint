import { useMemo } from 'react';

import { ClientsGridRowType } from '../constants';
import { ClientsGridRow } from '../types';

import { DEFAULT_GRID_SKELETON_ROWS_AMOUNT } from '@/components/Grid/constants';
import { Roles } from '@/constants/roles';
import { Dayjs } from '@/lib/dayjs';
import { ClientsResponse } from '@/types/clients';

export const useClientsRows = (
	data: ClientsResponse | undefined,
	dataUpdatedAt: Dayjs | Date | number,
	page: number,
	rowsPerPage: number,
	isFetching: boolean,
	isLoading: boolean,
	handleEdit: (equipmentId?: number) => () => void,
	role: Roles
) =>
	useMemo(() => {
		const showSkeleton = isLoading || isFetching;

		const mockRow: ClientsGridRow = {
			id: 0,
			color: '',
			name: '',
			registeredLocation: '',
			comments: '',
			handleEditClick: handleEdit,
			showSkeleton,
			disabled: false,
			role,
			type: ClientsGridRowType.CLIENT,
			factTrackedTimeInMinutes: 0,
			billableTrackedTimeInMinutes: 0,
			hasActiveProject: false,
			hasActiveAssignements: null,
			isActive: true,
		};

		const clientRows: Array<ClientsGridRow> = showSkeleton
			? new Array(data?.items?.length ?? DEFAULT_GRID_SKELETON_ROWS_AMOUNT)
					.fill(mockRow)
					.map((row, index) => ({ ...row, id: index }))
			: data?.items?.map(
					({
						name,
						id,
						billableTrackedTimeInMinutes,
						factTrackedTimeInMinutes,
						registeredLocation,
						comments,
						color,
						isActive,
						hasActiveProject,
						hasActiveAssignements,
					}) => ({
						id,
						color,
						name,
						registeredLocation: registeredLocation ?? '',
						billableTrackedTimeInMinutes: billableTrackedTimeInMinutes ?? null,
						factTrackedTimeInMinutes: factTrackedTimeInMinutes ?? null,
						comments: comments ?? '',
						handleEditClick: handleEdit,
						showSkeleton,
						role,
						disabled: !isActive,
						type: ClientsGridRowType.CLIENT,
						showCell: true,
						isActive,
						hasActiveProject,
						hasActiveAssignements,
					})
			  ) || [];

		return clientRows;
	}, [dataUpdatedAt, page, rowsPerPage, isLoading, isFetching, role]);
