import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo, useEffect } from 'react';

import { ClientsGridRowType } from '../../constants';
import { ClientsGridRow } from '../../types';

import { getGridColumnTemplateFromColumnWidths } from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { CLIENTS_PROJECT_GRID_ROW_HEIGHT } from '@/components/Grid/constants';
import { calculateMinWidth } from '@/components/Grid/utils/getGridColumnTemplateFromHeader';
import { isHeadOrGroupManager } from '@/constants/roles';
import { useToast } from '@/context/index';
import { useClientProjects } from '@/queries/useClientProjects';

import {
	CLIENT_PROJECTS_COLUMN_WIDTHS,
	HEAD_CLIENT_PROJECTS_COLUMN_WIDTHS,
} from './constants';
import {
	ClientsProjectsGridWrapper,
	overrideClientProjectsGridStyles as overrideGridStyles,
} from './styles';
import { ClientProjectsGridProps } from './types';
import { getClientProjectGridColumns } from './utils';

const getRowKey = (row: ClientsGridRow) => row.id;

export function ClientProjectsGrid({
	clientId,
	role,
	handleSetProjectItemsRowHeight,
	parentColumnsConfig,
}: ClientProjectsGridProps) {
	const toast = useToast();
	const { data, isLoading, isFetching } = useClientProjects(clientId, toast);

	const displayTrackedColumns = useMemo(
		() => !isHeadOrGroupManager(role),
		[role]
	);

	const clientProjectRows = useMemo(() => {
		const showSkeleton = isLoading || isFetching;
		const mockRow: ClientsGridRow = {
			id: 0,
			factTrackedTimeInMinutes: 0,
			billableTrackedTimeInMinutes: 0,
			color: '',
			name: '',
			registeredLocation: '',
			comments: '',
			showSkeleton,
			disabled: false,
			role,
			type: ClientsGridRowType.PROJECT,
			isActive: true,
		};

		const gridRows: Array<ClientsGridRow> = showSkeleton
			? new Array(1).fill(mockRow)
			: data?.map(
					({ name, id, factTrackedTime, billableTrackedTime, isActive }) => ({
						id,
						color: '',
						name,
						factTrackedTimeInMinutes: factTrackedTime,
						billableTrackedTimeInMinutes: billableTrackedTime,
						comments: '',
						showSkeleton,
						role,
						disabled: !isActive,
						isActive,
						isSmall: true,
					})
			  ) || [];

		return gridRows;
	}, [data, isLoading, isFetching]);

	useEffect(() => {
		if (handleSetProjectItemsRowHeight) {
			const height = clientProjectRows.length * CLIENTS_PROJECT_GRID_ROW_HEIGHT;
			handleSetProjectItemsRowHeight(height);
		}
	}, [clientProjectRows.length]);

	const clientProjectGridColumns = useMemo(
		() =>
			displayTrackedColumns
				? CLIENT_PROJECTS_COLUMN_WIDTHS
				: HEAD_CLIENT_PROJECTS_COLUMN_WIDTHS,
		[displayTrackedColumns]
	);

	const overrideClientProjectsGridStyles = useMemo(
		() =>
			overrideGridStyles(
				getGridColumnTemplateFromColumnWidths(
					clientProjectGridColumns,
					undefined,
					calculateMinWidth(Object.values(parentColumnsConfig))
				)
			),
		[clientProjectGridColumns]
	);

	return (
		<DataGridHoverRowWrapper defineHeight={false}>
			<ClientsProjectsGridWrapper>
				<DataGrid
					rows={clientProjectRows}
					columns={getClientProjectGridColumns(displayTrackedColumns)}
					rowKeyGetter={getRowKey}
					enableVirtualization={false}
					scrollToTopOnRowsChange={false}
					rowHeight={CLIENTS_PROJECT_GRID_ROW_HEIGHT}
					headerRowHeight={0}
					overrideDataGridStyles={overrideClientProjectsGridStyles}
				/>
			</ClientsProjectsGridWrapper>
		</DataGridHoverRowWrapper>
	);
}
