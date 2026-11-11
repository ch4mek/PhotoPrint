import { Dispatch, SetStateAction, useMemo } from 'react';
import { CalculatedColumn, Column, RenderCellProps } from 'react-data-grid';

import {
	SOFT_SKILL_SENIORITY_KEY,
	SOFT_SKILLS_COLUMNS_KEY,
	softSillsColumnProps,
	softSkillsColumnsWidths,
} from '../constants';

import {
	cellWithTooltipRenderer,
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
import { seniorityCellRenderer } from '@/components/Grid/renderers/cell/seniorityCellRenderer';
import { headerWithEditButton } from '@/components/Grid/renderers/header/headerWithEditButton';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { DATE_FORMAT } from '@/constants/date';
import { arePrimitiveValuesEqual } from '@/helpers/areValuesEqual';
import dayjs from '@/lib/dayjs';
import {
	EDITABLE_COLUMN_CLASS_NAME,
	FILLER_CELL_CLASS_NAME,
} from '@/pages/TeamBoardUser/components/SoftSkillsTab/components/SoftSkillsTable/constants';
import { isResetDisabled } from '@/pages/TeamBoardUser/components/SoftSkillsTab/utils/isResetDisabled';
import { isColumnEditable } from '@/pages/TeamBoardUser/components/SpecializationTable/utils/isColumnEditable';
import {
	SoftSkill,
	SoftSkillRow,
	SoftSkillsFilterSetter,
	SoftSkillsSnapshotDates,
	SoftSkillTableFilters,
	UserSoftSkill,
} from '@/types/softSkills';

const { softSkillName } = SOFT_SKILLS_COLUMNS_KEY;

export const useSoftSkillsTableColumns = (
	filters: SoftSkillTableFilters,
	filterSetter: SoftSkillsFilterSetter,
	onEditColumnClick: (
		snapshotId: number
	) => (column: CalculatedColumn<SoftSkillRow>) => void,
	snapshotDates: SoftSkillsSnapshotDates[],
	softSkills?: UserSoftSkill[]
): Array<Column<SoftSkillRow>> =>
	useMemo(
		() => [
			getColumnWithTooltipFactory({
				...softSillsColumnProps,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<SoftSkillRow, string>({
						...props,
						filter: filters.softSkillName,
						isFilterApplied: !!filters.softSkillName,
						setFilter: filterSetter.softSkillName as Dispatch<
							SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextModal,
						areValuesEquals: arePrimitiveValuesEqual,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						fieldToDisplay: softSkillName,
					}),
			}),
			...snapshotDates.map(({ snapshotDate, snapshotId }, index) => {
				const isEditable = isColumnEditable(index === 0, snapshotDate);
				const isMarkAsPast = index !== 0;

				return getColumnWithTooltipFactory({
					width: getColumnWidth(softSkillsColumnsWidths.snapshot(isEditable))
						.width,
					headerCellClass: isEditable ? EDITABLE_COLUMN_CLASS_NAME : undefined,
					key: snapshotDate,
					name: dayjs(snapshotDate).format(DATE_FORMAT),
					renderHeaderCell: (props) =>
						headerWithEditButton(
							props,
							isEditable,
							onEditColumnClick(snapshotId),
							isMarkAsPast
						),
					renderCell: (props: RenderCellProps<SoftSkillRow>) =>
						seniorityCellRenderer<SoftSkillRow, SoftSkill>(
							props,
							SOFT_SKILL_SENIORITY_KEY,
							isMarkAsPast
						),
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
				clearFilter: filterSetter.reset,
				headerCellClass: EDIT_COLUMN_HEADER_CLASS,
				cellClass: EDIT_COLUMN_CELL_CLASS,
				frozen: true,
				minWidth: 0,
			}),
		],
		[snapshotDates, filters, softSkills]
	);
