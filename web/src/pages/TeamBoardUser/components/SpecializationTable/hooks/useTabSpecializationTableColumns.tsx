import { Dispatch, SetStateAction, useMemo } from 'react';
import { CalculatedColumn, Column, RenderCellProps } from 'react-data-grid';

import { HardSkillNameRenderer } from '../components/HardSkillNameRenderer';
import {
	EDITABLE_COLUMN_CLASS_NAME,
	FILLER_CELL_CLASS_NAME,
	HARD_SKILLS_CELL_CLASS_NAME,
	SPECIALIZATION_COLUMN_PROPS,
	SPECIALIZATION_HEADER_CELL_CLASS_NAME,
	specializationColumnsWidth,
} from '../constants';
import {
	SnapshotDateConfig,
	SpecializationTableFilters,
	SpecializationTableFilterSetters,
} from '../types';
import { isColumnEditable } from '../utils/isColumnEditable';
import { isResetDisabled } from '../utils/isResetDisabled';

import {
	FreeTextModal,
	getColumnFactory,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
} from '@/components/Grid/constants';
import { headerWithEditButton } from '@/components/Grid/renderers/header/headerWithEditButton';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { DATE_FORMAT } from '@/constants/date';
import { arePrimitiveValuesEqual } from '@/helpers/areValuesEqual';
import dayjs from '@/lib/dayjs';
import { seniorityCellRenderer } from '@/pages/TeamBoardUser/components/SpecializationTable/renderers/seniorityCellRenderer';
import {
	SpecializationRow,
	SpecializationRowType,
} from '@/pages/TeamBoardUser/types';
import { UserSpecializationInfo } from '@/types/teamboard';

export const useTabSpecializationTableColumns = (
	snapshots: Array<UserSpecializationInfo> | undefined,
	allDates: Array<SnapshotDateConfig>,
	filters: SpecializationTableFilters,
	filterSetters: SpecializationTableFilterSetters,
	onEditColumnClick: (
		snapshotId: number
	) => (column: CalculatedColumn<SpecializationRow>) => void
): Array<Column<SpecializationRow>> =>
	useMemo(
		() => [
			getColumnFactory({
				...SPECIALIZATION_COLUMN_PROPS,
				colSpan: (args) => {
					if (args.type === 'ROW') {
						const { type } = args.row;

						if (type === SpecializationRowType.HardSkills) {
							return allDates.length + 1;
						}
					}
				},
				renderCell: HardSkillNameRenderer,
				headerCellClass: SPECIALIZATION_HEADER_CELL_CLASS_NAME,
				cellClass: (row) =>
					row.type === SpecializationRowType.HardSkills
						? HARD_SKILLS_CELL_CLASS_NAME
						: undefined,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<SpecializationRow, string>({
						...props,
						filter: filters.specializationName,
						setFilter: filterSetters.specializationName as Dispatch<
							SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextModal,
						disableFilterIconActiveStateAsDefault: true,
						isFilterApplied: Boolean(filters.specializationName),
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),
			...allDates.map(({ snapshotDate, relatedSnapshotId }, index) => {
				const isEditable = isColumnEditable(index === 0, snapshotDate);
				const isMarkAsPast = index !== 0;

				return getColumnWithTooltipFactory({
					width: getColumnWidth(specializationColumnsWidth.snapshot(isEditable))
						.width,
					headerCellClass: isEditable ? EDITABLE_COLUMN_CLASS_NAME : undefined,
					key: snapshotDate,
					name: dayjs(snapshotDate).format(DATE_FORMAT),
					renderHeaderCell: (props) =>
						headerWithEditButton(
							props,
							isEditable,
							onEditColumnClick(relatedSnapshotId),
							isMarkAsPast
						),
					renderCell: (props: RenderCellProps<SpecializationRow>) =>
						seniorityCellRenderer(props, isMarkAsPast),
				});
			}),
			getColumnFactory({
				key: 'filler',
				name: '',
				headerCellClass: FILLER_CELL_CLASS_NAME,
				cellClass: FILLER_CELL_CLASS_NAME,
				width: 0,
				minWidth: 0,
				maxWidth: 0,
			}),
			getEditColumnFactory({
				key: '',
				name: '',
				disabled: isResetDisabled(filters),
				clearFilter: filterSetters.reset,
				headerCellClass: EDIT_COLUMN_HEADER_CLASS,
				cellClass: EDIT_COLUMN_CELL_CLASS,
				frozen: true,
			}),
		],
		[snapshots, allDates, filters, onEditColumnClick]
	);
