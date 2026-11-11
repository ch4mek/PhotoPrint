import { useMemo } from 'react';
import { Column } from 'react-data-grid';

import { SoftSkillCell } from '../components/SoftSkillCell';
import {
	SENIORITY_COLUMN_NAME,
	SOFT_SKILLS_COLUMN_KEYS,
	SOFT_SKILLS_TABLE_COLUMNS_WIDTHS,
	SPECIALIZATION_COLUMN_NAMES,
} from '../constants';

import { getColumnWithTooltipFactory } from '@/components/Grid';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { DEFAULT_LOADER_COLUMN_SPAN_AMOUNT } from '@/constants/defaultValues';
import { SoftSkillRow } from '@/types/softSkills';

export const useSoftSkillsTableColumns = (): Array<Column<SoftSkillRow>> =>
	useMemo(
		() => [
			getColumnWithTooltipFactory({
				key: SOFT_SKILLS_COLUMN_KEYS.name,
				name: SPECIALIZATION_COLUMN_NAMES.section,
				width: getColumnWidth(SOFT_SKILLS_TABLE_COLUMNS_WIDTHS.name).width,
				colSpan(args) {
					return args.type === 'ROW' && args.row.showSkeleton
						? DEFAULT_LOADER_COLUMN_SPAN_AMOUNT
						: 0;
				},
				renderCell: ({ row: { snapshots } }) => {
					const { softSkillName } = snapshots;

					return <SoftSkillCell content={softSkillName as unknown as string} />;
				},
				sortable: false,
			}),
			getColumnWithTooltipFactory({
				key: SOFT_SKILLS_COLUMN_KEYS.seniority,
				name: SENIORITY_COLUMN_NAME,
				width: getColumnWidth(SOFT_SKILLS_TABLE_COLUMNS_WIDTHS.seniority).width,
				sortable: false,
				renderCell: ({ row: { snapshots } }) => {
					const { softSkillSeniority } = snapshots;

					return (
						<SoftSkillCell content={softSkillSeniority as unknown as string} />
					);
				},
			}),
		],
		[]
	);
