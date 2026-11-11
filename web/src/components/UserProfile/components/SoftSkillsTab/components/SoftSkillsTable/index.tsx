import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import { getGridColumnTemplateFromColumnWidths } from '@/components/Grid';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import {
	EDIT_COLUMN_DEFAULT_OFFSET,
	EDIT_COLUMN_NO_DATA_OFFSET,
	softSkillsColumnsWidths,
} from '@/components/UserProfile/components/SoftSkillsTab/constants';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import {
	overrideSoftSkillsStyles,
	SoftSillsTableHoverWrapper,
} from '@/pages/TeamBoardUser/components/SoftSkillsTab/components/SoftSkillsTable/styles';
import { GridColumnWidthsConfiguration } from '@/types/grid';
import { SoftSkillsTableProps } from '@/types/softSkills';

export function SoftSkillsTable({
	rows,
	paginationProps,
	columns,
	snapshotsDates,
	handleSortColumnChange,
	sortColumns,
}: SoftSkillsTableProps) {
	const overrideSoftSkillsGridStyles = useMemo(() => {
		const { name, filler } = softSkillsColumnsWidths;
		const widths: GridColumnWidthsConfiguration = { name };

		snapshotsDates.forEach(({ snapshotDate }) => {
			widths[snapshotDate] = softSkillsColumnsWidths.snapshot;
		});

		widths.filler = filler;

		const editColumnOffset = snapshotsDates.length
			? EDIT_COLUMN_DEFAULT_OFFSET
			: EDIT_COLUMN_NO_DATA_OFFSET;

		const editColumnIndex =
			Object.values(widths).length + snapshotsDates.length + editColumnOffset;

		return overrideSoftSkillsStyles(
			getGridColumnTemplateFromColumnWidths(widths, true),
			!!rows.length,
			editColumnIndex
		);
	}, [snapshotsDates, rows.length]);

	return (
		<SoftSillsTableHoverWrapper>
			<DataGrid
				{...COMMON_GRID_PROPS}
				enableVirtualization
				columns={columns}
				rows={rows}
				overrideDataGridStyles={overrideSoftSkillsGridStyles}
				scrollToTopOnRowsChange={false}
				renderers={noRowsGridFallback}
				paginationProps={paginationProps}
				onSortColumnsChange={handleSortColumnChange}
				sortColumns={sortColumns}
			/>
		</SoftSillsTableHoverWrapper>
	);
}
