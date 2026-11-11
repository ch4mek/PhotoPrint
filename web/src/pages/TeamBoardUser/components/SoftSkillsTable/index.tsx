import { DataGrid } from '@mms/mms-ui-library/components/Grids';

import { MAX_VISIBLE_ROWS_COUNT } from '../SummaryTabLayout/constants';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import {
	COMMON_GRID_PROPS,
	displayMinimalGridRowsProp,
} from '@/components/Grid/constants';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { TeamboardTableMode } from '@/types/teamboard';

import { useSoftSkillsTableColumns } from './hooks/useSoftSkillsTableColumns';
import { overrideSoftSkillsSectionTableStyles } from './styles';
import { SoftSkillsTableProps } from './types';
import { getRowKey } from './utils';

export function SoftSkillsTable({
	rows,
	pagination,
	isLoading,
	mode,
}: SoftSkillsTableProps) {
	const columns = useSoftSkillsTableColumns();
	const isSection = mode === TeamboardTableMode.Section;

	return (
		<DataGridHoverRowWrapper defineHeight={false}>
			<DataGrid
				{...COMMON_GRID_PROPS}
				enableVirtualization
				columns={columns}
				rows={rows}
				rowKeyGetter={getRowKey}
				overrideDataGridStyles={overrideSoftSkillsSectionTableStyles}
				paginationProps={isLoading ? undefined : pagination}
				renderers={noRowsGridFallback}
				scrollToTopOnRowsChange={false}
				{...(isSection && displayMinimalGridRowsProp)}
				{...(isSection && { maxRowsCount: MAX_VISIBLE_ROWS_COUNT })}
			/>
		</DataGridHoverRowWrapper>
	);
}
