import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { Hint } from '@mms/mms-ui-library/components/Hint';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import { useMemo } from 'react';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { useTooltipContent } from '@/hooks/useTooltipContent';
import {
	CLEAR_FILTERS_COLUMN_INDEX,
	getRowKey,
} from '@/pages/AdminPanel/components/UsersTab/constants';
import { useUsersColumns } from '@/pages/AdminPanel/components/UsersTab/hooks/useUsersHeaders';

import { UsersGridProps } from './types';

export function UsersGrid({
	rows,
	columnsWidths,
	sortColumns,
	paginationProps,
	options,
	filters,
	setFilters,
	initialFilters,
	handleSortColumnChange,
	handleToggleUserActive,
	handleCrossLinkClick,
	enableCellClick,
}: UsersGridProps) {
	const { tooltipContent, isTextCut, setTooltipContent } = useTooltipContent();
	const isDesktopSWidth = useHasScreenWidth(breakpoints.desktopS);

	const { defaultOptions, positionOptions, departmentOptions } = options;

	const columns = useUsersColumns({
		filters,
		setFilters,
		options: defaultOptions,
		initialFilters,
		positionOptions,
		departmentOptions,
		usersColumnsWidths: columnsWidths,
		setTooltipContent,
		onCellClick: handleCrossLinkClick,
		enableCellClick,
		onToggleUserActive: handleToggleUserActive,
	});

	const overrideDataGridStyles = useMemo(() => {
		const getGridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			columnsWidths,
			true
		);

		return overrideGridStylesWithEditColumn(
			!!rows.length,
			CLEAR_FILTERS_COLUMN_INDEX,
			getGridColumnTemplate
		);
	}, [rows.length, isDesktopSWidth, columnsWidths]);

	return (
		<>
			<DataGridHoverRowWrapper defineHeight={false} flexGrow>
				<DataGrid
					{...COMMON_GRID_PROPS}
					{...{
						columns,
						sortColumns,
						rows,
						paginationProps,
						overrideDataGridStyles,
					}}
					defaultColumnOptions={{
						sortable: true,
					}}
					onSortColumnsChange={handleSortColumnChange}
					renderers={noRowsGridFallback}
					scrollToTopOnRowsChange={false}
					rowKeyGetter={getRowKey}
				/>
			</DataGridHoverRowWrapper>

			{tooltipContent && isTextCut && (
				<Hint
					open
					anchorElement={tooltipContent.anchorElement}
					text={tooltipContent.text}
				/>
			)}
		</>
	);
}
