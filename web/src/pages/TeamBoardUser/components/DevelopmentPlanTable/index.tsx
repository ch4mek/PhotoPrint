import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import { MAX_VISIBLE_ROWS_COUNT } from '../SummaryTabLayout/constants';

import { getGridColumnTemplateFromColumnWidths } from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import {
	displayMinimalGridRowsProp,
	GRID_ROW_HEIGHT,
} from '@/components/Grid/constants';
import { useDynamicRowsHeight } from '@/components/Grid/hooks/useDynamicRowsHeight';
import { DEVELOPMENT_PLAN_SECTION_ROW_HEIGHT } from '@/constants/developmentPlan';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { HeightsContext } from '@/context/CellHeightsContext';
import { TeamboardTableMode } from '@/types/teamboard';

import { developmentPlanTableColumnsWidths } from './constants';
import { useDevelopmentPlanColumns } from './hooks/useDevelopmentPlanColumns';
import { DevelopmentPlanTableProps } from './types';
import { overrideDevelopmentPlanGridStyles } from './utils';

export function DevelopmentPlanTable({
	rows,
	setRows,
	mode,
	sortColumns,
	onSortColumnsChange,
	filter,
	setFilter,
	areFiltersApplied,
	pagination,
	isLoading,
}: DevelopmentPlanTableProps) {
	const columns = useDevelopmentPlanColumns(
		mode,
		filter,
		setFilter,
		areFiltersApplied
	);

	const isTableModeTab = mode === TeamboardTableMode.Tab;
	const isSection = mode === TeamboardTableMode.Section;
	const isPopup = mode === TeamboardTableMode.PopUp;

	const { handleUpdateRows, heightsMap, getHeight } = useDynamicRowsHeight({
		setRows,
		defaultRowHeight: isPopup ? GRID_ROW_HEIGHT : undefined,
	});

	const overrideStyles = useMemo(() => {
		const columnsWidth = developmentPlanTableColumnsWidths[mode];
		const gridColumnsTemplate = getGridColumnTemplateFromColumnWidths(
			columnsWidth,
			true
		);
		const editColumnsIndex = Object.values(columnsWidth).length + 2;

		return overrideDevelopmentPlanGridStyles(
			pagination ? !isLoading && !!rows.length : false,
			mode,
			editColumnsIndex,
			gridColumnsTemplate
		);
	}, [mode, pagination, rows.length, isLoading]);

	const restrictionRowHeight =
		rows.length === 0 || isPopup
			? GRID_ROW_HEIGHT
			: DEVELOPMENT_PLAN_SECTION_ROW_HEIGHT;

	return (
		<DataGridHoverRowWrapper defineHeight={false} flexGrow={isTableModeTab}>
			<HeightsContext.Provider value={heightsMap}>
				<DataGrid
					columns={columns}
					rows={rows}
					sortColumns={sortColumns}
					onSortColumnsChange={onSortColumnsChange}
					overrideDataGridStyles={overrideStyles}
					onRowsChange={handleUpdateRows}
					rowHeight={getHeight}
					scrollToTopOnRowsChange={false}
					paginationProps={!isLoading ? pagination : undefined}
					renderers={noRowsGridFallback}
					{...((isSection || isPopup) && displayMinimalGridRowsProp)}
					{...((isSection || isPopup) && {
						maxRowsCount: MAX_VISIBLE_ROWS_COUNT,
						restrictionRowHeight,
					})}
				/>
			</HeightsContext.Provider>
		</DataGridHoverRowWrapper>
	);
}
