import { useMemo } from 'react';
import { Column, RenderCellProps } from 'react-data-grid';

import {
	SpecializationHardSkillRow,
	SpecializationSnapshotsByDates,
} from '../../../types';
import { HardSkillCell } from '../components/HardSkillCell';
import {
	SPECIALIZATION_HARD_SKILL_COLUMN_PROPS,
	specializationColumnsWidth,
} from '../constants';
import { SnapshotDateConfig } from '../types';

import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { hardSkillSeniorityCellRenderer } from '@/components/UserProfile/components/HardSkillsTab/components/SpecializationTable/renderers/hardSkillSeniorityCellRenderer';

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
				const isMarkAsPast = index !== 0;

				return {
					key: date,
					name: date,
					width: getColumnWidth(specializationColumnsWidth.snapshot.width)
						.width,
					renderHeaderCell: () => null,
					renderCell: (props: RenderCellProps<SpecializationHardSkillRow>) =>
						hardSkillSeniorityCellRenderer(props, isMarkAsPast),
				};
			}),
		];
	}, [snapshots, allDates]);
