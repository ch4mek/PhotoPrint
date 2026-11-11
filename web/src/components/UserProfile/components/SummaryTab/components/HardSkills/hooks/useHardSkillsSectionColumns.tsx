import { useMemo } from 'react';
import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import { columnsWidth, hardSkillsColumns } from '../constants';
import { HardSkillsSectionCell } from '../HardSkillsSectionCell';
import { HardSkillsSectionRow } from '../types';

import {
	getColumnWithTooltipFactory,
	getEditColumnFactory,
} from '@/components/Grid';

export const useHardSkillsSectionColumns = (): Array<
	Column<HardSkillsSectionRow>
> => {
	const { t } = useTranslation('profile', { keyPrefix: 'skills' });

	return useMemo(
		() => [
			getColumnWithTooltipFactory({
				...hardSkillsColumns.name,
				name: t('name'),
				width: columnsWidth.name,
				renderCell: ({ row: { name, isPrimary } }) => (
					<HardSkillsSectionCell content={name} isPrimary={isPrimary} />
				),
			}),
			getColumnWithTooltipFactory({
				...hardSkillsColumns.seniority,
				name: t('seniority'),
				width: columnsWidth.seniority,
				renderCell: ({ row: { seniority, isPrimary } }) => (
					<HardSkillsSectionCell content={seniority} isPrimary={isPrimary} />
				),
			}),
			getEditColumnFactory({
				key: '',
				name: '',
				displayFilterButton: false,
				sortable: false,
				frozen: true,
				disabled: true,
			}),
		],
		[]
	);
};
