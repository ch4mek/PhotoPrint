import { useMemo } from 'react';
import { Column } from 'react-data-grid';

import { HardSkillCell } from '../components/HardSkillCell';
import {
	HARD_SKILLS_COLUMN_KEYS,
	HARD_SKILLS_TABLE_COLUMNS_WIDTHS,
	SENIORITY_COLUMN_NAME,
	SPECIALIZATION_COLUMN_NAMES,
} from '../constants';

import { getColumnWithTooltipFactory } from '@/components/Grid';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { HardSkillRow } from '@/pages/TeamBoardUser/types';

export const useHardSkillsTableColumns = (): Array<Column<HardSkillRow>> =>
	useMemo(
		() => [
			getColumnWithTooltipFactory({
				key: HARD_SKILLS_COLUMN_KEYS.name,
				name: SPECIALIZATION_COLUMN_NAMES.section,
				width: getColumnWidth(HARD_SKILLS_TABLE_COLUMNS_WIDTHS.name).width,
				renderCell: ({ row: { name, isPrimary, showSkeleton } }) => (
					<HardSkillCell
						content={name}
						isPrimary={isPrimary}
						showSkeleton={showSkeleton}
					/>
				),
				sortable: false,
			}),
			getColumnWithTooltipFactory({
				key: HARD_SKILLS_COLUMN_KEYS.seniority,
				name: SENIORITY_COLUMN_NAME,
				width: getColumnWidth(HARD_SKILLS_TABLE_COLUMNS_WIDTHS.seniority).width,
				sortable: false,
				renderCell: ({ row: { seniority, isPrimary, showSkeleton } }) => (
					<HardSkillCell
						content={seniority}
						isPrimary={isPrimary}
						showSkeleton={showSkeleton}
					/>
				),
			}),
		],
		[]
	);
