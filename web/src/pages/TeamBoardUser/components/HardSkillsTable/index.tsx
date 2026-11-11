import { DataGrid } from '@mms/mms-ui-library/components/Grids';

import { MAX_VISIBLE_ROWS_COUNT } from '../SummaryTabLayout/constants';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import {
	COMMON_GRID_PROPS,
	displayMinimalGridRowsProp,
} from '@/components/Grid/constants';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { TeamboardTableMode } from '@/types/teamboard';

import { useHardSkillsTableColumns } from './hooks/useHardSkillsTableColumns';
import { overrideHardSkillsSectionTableStyles } from './styles';
import { HardSkillsTableProps } from './types';
import { getRowKey } from './utils';

export function HardSkillsTable({
	rows,
	pagination,
	isLoading,
	mode,
}: HardSkillsTableProps) {
	const columns = useHardSkillsTableColumns();
	const isSection = mode === TeamboardTableMode.Section;

	return (
		<DataGridHoverRowWrapper defineHeight={false}>
			<DataGrid
				{...COMMON_GRID_PROPS}
				enableVirtualization
				columns={columns}
				rows={rows}
				rowKeyGetter={getRowKey}
				overrideDataGridStyles={overrideHardSkillsSectionTableStyles}
				paginationProps={isLoading ? undefined : pagination}
				renderers={noRowsGridFallback}
				scrollToTopOnRowsChange={false}
				{...(isSection && displayMinimalGridRowsProp)}
				{...(isSection && {
					maxRowsCount: MAX_VISIBLE_ROWS_COUNT,
				})}
			/>
		</DataGridHoverRowWrapper>
	);
}
