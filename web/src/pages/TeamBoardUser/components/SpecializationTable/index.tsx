import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { DEFAULT_GRID_ROW_HEIGHT } from '@mms/mms-ui-library/constants/grid';
import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import { FlagIcon } from '@mms/mms-ui-library/icons/base/FlagIcon';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { RowsChangeData } from 'react-data-grid';

import { SpecializationRow, SpecializationRowType } from '../../types';

import { getGridColumnTemplateFromColumnWidths } from '@/components/Grid';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { getSkeletonRows } from '@/components/Grid/utils/getSkeletonRows';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import {
	getRowClass,
	getRowHeight,
	getRowKey,
} from '@/pages/TeamBoardUser/components/SpecializationTable/utils/getGridSettings';
import { isColumnEditable } from '@/pages/TeamBoardUser/components/SpecializationTable/utils/isColumnEditable';
import { GridColumnWidthsConfiguration } from '@/types/grid';

import {
	EDIT_COLUMN_DEFAULT_OFFSET,
	EDIT_COLUMN_NO_DATA_OFFSET,
	SKELETON_SPECIALIZATION_ROW,
	SKELETON_SPECIALIZATION_ROW_AMOUNT,
	specializationColumnsWidth,
} from './constants';
import { SpecializationTableContext } from './context/specializationTableContext';
import {
	overrideSpecializationsStyles,
	SpecializationTableHoverWrapper,
} from './styles';
import { SpecializationTableProps } from './types';

export function SpecializationTable({
	onSpecializationDeleteClick,
	onSpecializationSetPrimaryClick,
	allDates,
	rows,
	columns,
	gridPagination,
}: SpecializationTableProps) {
	const [gridRows, setGridRows] = useState<Array<SpecializationRow>>(
		getSkeletonRows(
			SKELETON_SPECIALIZATION_ROW_AMOUNT,
			SKELETON_SPECIALIZATION_ROW
		)
	);

	useEffect(() => {
		setGridRows(rows);
	}, [rows]);

	const overrideHardSkillsGridStyles = useMemo(() => {
		const { name, filler } = specializationColumnsWidth;
		const widths: GridColumnWidthsConfiguration = { name };

		allDates.forEach(({ snapshotDate }, index) => {
			widths[snapshotDate] = specializationColumnsWidth.snapshot(
				isColumnEditable(index === 0, snapshotDate)
			);
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

	const contextMenuOption = [
		{
			value: 'Set as primary',
			addon: <FlagIcon />,
			onClick: (row: SpecializationRow) => {
				onSpecializationSetPrimaryClick(row);
			},
		},
		{
			value: 'Delete',
			addon: <BasketIcon />,
			onClick: (row: SpecializationRow) => {
				onSpecializationDeleteClick(row);
			},
		},
	];

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

	const contextValue = useMemo(
		() => ({
			contextMenuOption,
			allDates,
		}),
		[contextMenuOption, allDates]
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
