import { useMemo } from 'react';
import { Column, RenderCellProps } from 'react-data-grid';

import { HardSkillCell } from '../components/HardSkillCell';
import {
	SPECIALIZATION_HARD_SKILL_COLUMN_PROPS,
	specializationColumnsWidth,
} from '../constants';
import { SnapshotDateConfig } from '../types';

import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { hardSkillSeniorityCellRenderer } from '@/pages/TeamBoardUser/components/SpecializationTable/renderers/hardSkillSeniorityCellRenderer';
import { isColumnEditable } from '@/pages/TeamBoardUser/components/SpecializationTable/utils/isColumnEditable';
import {
	SpecializationHardSkillRow,
	SpecializationSnapshotsByDates,
} from '@/pages/TeamBoardUser/types';

export const useSpecializationHardSkillsTableColumns = (
	snapshots: SpecializationSnapshotsByDates,
	allDates: Array<SnapshotDateConfig>
): Array<Column<SpecializationHardSkillRow>> =>
	useMemo(() => {
		if (!snapshots || !allDates) {
			return [];
		}

		return [
			{
				...SPECIALIZATION_HARD_SKILL_COLUMN_PROPS,
				renderHeaderCell: () => null,
				renderCell: HardSkillCell,
			},
			...allDates.map(({ snapshotDate: date }, index) => {
				const isEditable = isColumnEditable(index === 0, date);
				const isMarkAsPast = index !== 0;

				return {
					key: date,
					name: date,
					width: getColumnWidth(specializationColumnsWidth.snapshot(isEditable))
						.width,
					renderHeaderCell: () => null,
					renderCell: (props: RenderCellProps<SpecializationHardSkillRow>) =>
						hardSkillSeniorityCellRenderer(props, isMarkAsPast),
				};
			}),
		];
	}, [snapshots, allDates]);
