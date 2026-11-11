import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column, RenderCellProps } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import { SpecializationRow, SpecializationRowType } from '../../../types';
import { HardSkillNameRenderer } from '../components/HardSkillNameRenderer';
import {
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
import { seniorityCellRenderer } from '@/components/UserProfile/components/HardSkillsTab/components/SpecializationTable/renderers/seniorityCellRenderer';
import { DATE_FORMAT } from '@/constants/date';
import { DEFAULT_LOADER_COLUMN_SPAN_AMOUNT } from '@/constants/defaultValues';
import { arePrimitiveValuesEqual } from '@/helpers/areValuesEqual';
import dayjs from '@/lib/dayjs';
import { UserSpecializationInfo } from '@/types/userProfile';

export const useTabSpecializationTableColumns = (
	snapshots: Array<UserSpecializationInfo> | undefined,
	allDates: Array<SnapshotDateConfig>,
	filters: SpecializationTableFilters,
	filterSetters: SpecializationTableFilterSetters
): Array<Column<SpecializationRow>> => {
	const { t } = useTranslation('profile', { keyPrefix: 'hardSkillsTab' });

	return useMemo(() => {
		if (!snapshots) {
			return [];
		}

		return [
			getColumnFactory({
				...SPECIALIZATION_COLUMN_PROPS,
				name: t('specialization'),
				colSpan: (args) => {
					if (args.type === 'ROW') {
						const { showSkeleton, type } = args.row;

						if (showSkeleton) {
							return DEFAULT_LOADER_COLUMN_SPAN_AMOUNT;
						}

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
			...allDates.map(({ snapshotDate }, index) => {
				const isMarkAsPast = index !== 0;

				return getColumnWithTooltipFactory({
					width: specializationColumnsWidth.snapshot.width,
					key: snapshotDate,
					name: dayjs(snapshotDate).format(DATE_FORMAT),
					renderHeaderCell: (props) =>
						headerWithEditButton(props, false, undefined, isMarkAsPast),
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
		];
	}, [snapshots, allDates, filters]);
};
