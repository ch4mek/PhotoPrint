import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column, RenderCellProps } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import {
	FILLER_CELL_CLASS_NAME,
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
import { isResetDisabled } from '@/components/UserProfile/components/SoftSkillsTab/utils/isResetDisabled';
import { DATE_FORMAT } from '@/constants/date';
import { arePrimitiveValuesEqual } from '@/helpers/areValuesEqual';
import dayjs from '@/lib/dayjs';
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
	snapshotDates: SoftSkillsSnapshotDates[],
	softSkills?: UserSoftSkill[]
): Array<Column<SoftSkillRow>> => {
	const { t } = useTranslation('profile', { keyPrefix: 'softSkillsTab' });

	return useMemo(() => {
		if (!softSkills) {
			return [];
		}

		return [
			getColumnWithTooltipFactory({
				...softSillsColumnProps,
				name: t('softSkill'),
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
			...snapshotDates.map(({ snapshotDate }, index) => {
				const isMarkAsPast = index !== 0;

				return getColumnWithTooltipFactory({
					width: softSkillsColumnsWidths.snapshot.width,
					key: snapshotDate,
					name: dayjs(snapshotDate).format(DATE_FORMAT),
					renderHeaderCell: (props) =>
						headerWithEditButton(props, false, undefined, isMarkAsPast),
					renderCell: (props: RenderCellProps<SoftSkillRow, unknown>) =>
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
		];
	}, [snapshotDates, filters, softSkills, t]);
};
