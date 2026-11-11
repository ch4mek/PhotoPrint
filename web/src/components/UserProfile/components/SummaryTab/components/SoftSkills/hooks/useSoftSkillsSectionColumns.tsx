import { useMemo } from 'react';
import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import {
	SOFT_SKILLS_COLUMN_KEYS,
	SOFT_SKILLS_TABLE_COLUMNS_WIDTHS,
} from '../constants';
import { SoftSkillsSectionCell } from '../SoftSkillsSectionCell';

import { getColumnWithTooltipFactory } from '@/components/Grid';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { DEFAULT_LOADER_COLUMN_SPAN_AMOUNT } from '@/constants/defaultValues';
import { SoftSkillRow } from '@/types/softSkills';

export const useSoftSkillsTableColumns = (): Array<Column<SoftSkillRow>> => {
	const { t } = useTranslation('profile', { keyPrefix: 'skills' });

	return useMemo(
		() => [
			getColumnWithTooltipFactory({
				key: SOFT_SKILLS_COLUMN_KEYS.name,
				name: t('name'),
				width: getColumnWidth(SOFT_SKILLS_TABLE_COLUMNS_WIDTHS.name).width,
				colSpan(args) {
					return args.type === 'ROW' && args.row.showSkeleton
						? DEFAULT_LOADER_COLUMN_SPAN_AMOUNT
						: 0;
				},
				renderCell: ({ row: { snapshots } }) => {
					const { softSkillName } = snapshots;

					return (
						<SoftSkillsSectionCell
							content={softSkillName as unknown as string}
						/>
					);
				},
				sortable: false,
			}),
			getColumnWithTooltipFactory({
				key: SOFT_SKILLS_COLUMN_KEYS.seniority,
				name: t('seniority'),
				width: getColumnWidth(SOFT_SKILLS_TABLE_COLUMNS_WIDTHS.seniority).width,
				sortable: false,
				renderCell: ({ row: { snapshots } }) => {
					const { softSkillSeniority } = snapshots;

					return (
						<SoftSkillsSectionCell
							content={softSkillSeniority as unknown as string}
						/>
					);
				},
			}),
		],
		[]
	);
};
