import { useCallback, useEffect, useMemo, useState } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { ClientProjectsGrid } from '../components/ClientsProjectsGrid';
import {
	CHIEF_ROLES_CLIENT_GRID_WIDTH,
	ClientsGridRowType,
	HEAD_CLIENT_GRID_WIDTH,
} from '../constants';
import { ClientsGridRow } from '../types';

import { ExpandedCell } from '@/components/ExpandedCell';
import { cellWithColorCircleRenderer } from '@/components/Grid';
import { isChiefRole, isHeadOrGroupManager } from '@/constants/roles';

import { CellWrapper } from './styles';

export const ClientExpandableNameCellRenderer = (
	props: RenderCellProps<ClientsGridRow, unknown>
) => {
	const { row, tabIndex, onRowChange } = props;
	const {
		disabled,
		parentId,
		expanded,
		showSkeleton,
		role,
		hasActiveProject,
		hasActiveAssignements,
	} = row;

	const [currentRow, setCurrentRow] = useState(row);

	const displayTrackedColumn = useMemo(() => isChiefRole(role), [role]);

	const columnsWidths = useMemo(
		() =>
			displayTrackedColumn
				? CHIEF_ROLES_CLIENT_GRID_WIDTH
				: HEAD_CLIENT_GRID_WIDTH,
		[displayTrackedColumn]
	);

	useEffect(() => {
		if (currentRow !== row) {
			onRowChange(currentRow);
		}
	}, [currentRow]);

	useEffect(() => {
		setCurrentRow(row);
	}, [showSkeleton]);

	const updateRow = useCallback(
		(newRowParts: Partial<ClientsGridRow>) => {
			setCurrentRow((prevRow) => ({ ...prevRow, ...newRowParts }));
		},
		[onRowChange]
	);

	const handleSetProjectItemsRowHeight = (height: number) => {
		updateRow({ ...row, projectItemsRowHeight: height });
	};

	if (row.type === ClientsGridRowType.PROJECT) {
		return (
			<ClientProjectsGrid
				handleSetProjectItemsRowHeight={handleSetProjectItemsRowHeight}
				clientId={parentId!}
				role={role}
				parentColumnsConfig={columnsWidths}
			/>
		);
	}
	const defaultColoredCell = cellWithColorCircleRenderer(props);

	const shouldCellBeExpanded =
		!disabled &&
		!showSkeleton &&
		hasActiveProject &&
		(isHeadOrGroupManager(role) ? hasActiveAssignements : true);

	return (
		<CellWrapper>
			{defaultColoredCell}
			{shouldCellBeExpanded && (
				<ExpandedCell
					expanded={!!expanded}
					tabIndex={tabIndex}
					onCellExpand={() => {
						updateRow({ expanded: !expanded });
					}}
				/>
			)}
		</CellWrapper>
	);
};
