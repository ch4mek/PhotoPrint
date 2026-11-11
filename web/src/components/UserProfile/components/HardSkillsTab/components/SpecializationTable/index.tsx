import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { DEFAULT_GRID_ROW_HEIGHT } from '@mms/mms-ui-library/constants/grid';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { RowsChangeData } from 'react-data-grid';

import { SpecializationRow, SpecializationRowType } from '../../types';

import { getGridColumnTemplateFromColumnWidths } from '@/components/Grid';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { GridColumnWidthsConfiguration } from '@/types/grid';

import {
	EDIT_COLUMN_DEFAULT_OFFSET,
	EDIT_COLUMN_NO_DATA_OFFSET,
	specializationColumnsWidth,
} from './constants';
import { SpecializationTableContext } from './context/specializationTableContext';
import {
	overrideSpecializationsStyles,
	SpecializationTableHoverWrapper,
} from './styles';
import {
	SpecializationTableContextValues,
	SpecializationTableProps,
} from './types';
import { getRowClass, getRowHeight, getRowKey } from './utils/getGridSettings';

export function SpecializationTable({
	allDates,
	rows,
	columns,
	gridPagination,
}: SpecializationTableProps) {
	const [gridRows, setGridRows] = useState<Array<SpecializationRow>>([]);

	useEffect(() => {
		setGridRows(rows);
	}, [rows]);

	const overrideHardSkillsGridStyles = useMemo(() => {
		const { name, filler } = specializationColumnsWidth;
		const widths: GridColumnWidthsConfiguration = { name };

		allDates.forEach(({ snapshotDate }) => {
			widths[snapshotDate] = specializationColumnsWidth.snapshot;
		});

		widths.filler = filler;

		const editColumnOffset = allDates.length
			? EDIT_COLUMN_DEFAULT_OFFSET
			: EDIT_COLUMN_NO_DATA_OFFSET;

		const editColumnIndex =
			Object.values(widths).length + allDates.length + editColumnOffset;

		return overrideSpecializationsStyles(
			getGridColumnTemplateFromColumnWidths(widths, true),
			editColumnIndex,
			!!rows.length
		);
	}, [allDates, rows.length]);

	const handleRowsChange = useCallback(
		(
			newRows: SpecializationRow[],
			{ indexes }: RowsChangeData<SpecializationRow>
		) => {
			const row = newRows[indexes[0]];

			if (row.isExpanded) {
				if (row.type === SpecializationRowType.Specialization) {
					newRows.splice(indexes[0] + 1, 0, {
						...row,
						id: -row.id,
						type: SpecializationRowType.HardSkills,
						height: row.hardSkills.length * DEFAULT_GRID_ROW_HEIGHT,
					});
				}
			} else {
				const rowNextFromChanged = newRows[indexes[0] + 1];

				if (rowNextFromChanged) {
					if (rowNextFromChanged.type === SpecializationRowType.HardSkills) {
						newRows.splice(indexes[0] + 1, 1);
					}
				}
			}
			setGridRows(newRows);
		},
		[]
	);

	const contextValue: SpecializationTableContextValues = useMemo(
		() => ({
			allDates,
		}),
		[allDates]
	);

	return (
		<SpecializationTableContext.Provider value={contextValue}>
			<SpecializationTableHoverWrapper>
				<DataGrid
					{...COMMON_GRID_PROPS}
					enableVirtualization
					columns={columns}
					rows={gridRows}
					onRowsChange={handleRowsChange}
					rowKeyGetter={getRowKey}
					rowHeight={getRowHeight}
					rowClass={getRowClass}
					overrideDataGridStyles={overrideHardSkillsGridStyles}
					scrollToTopOnRowsChange={false}
					renderers={noRowsGridFallback}
					paginationProps={gridPagination}
				/>
			</SpecializationTableHoverWrapper>
		</SpecializationTableContext.Provider>
	);
}
