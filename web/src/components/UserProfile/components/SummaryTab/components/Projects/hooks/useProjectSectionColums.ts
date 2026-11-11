import { useMemo } from 'react';
import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import { projectSectionColumns } from '../constants';
import { ProjectSectionColumnWidths } from '../types';

import {
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	getLogicalTextColumnFactory,
} from '@/components/Grid';
import { ProjectTableRow } from '@/components/UserProfile/types';

export const useProjectSectionColumns = (
	columnWidths: ProjectSectionColumnWidths,
	isEmployee: boolean
) => {
	const { t } = useTranslation('profile', { keyPrefix: 'projects' });

	return useMemo(() => {
		const columns: Array<Column<ProjectTableRow> | null> = [
			getColumnWithTooltipFactory<ProjectTableRow>({
				...projectSectionColumns.projectName,
				name: t('projectName'),
				width: columnWidths.projectName,
			}),
			isEmployee
				? null
				: getColumnWithTooltipFactory<ProjectTableRow>({
						...projectSectionColumns.clientName,
						name: t('clientName'),
						width: columnWidths.clientName,
				  }),
			isEmployee
				? null
				: getLogicalTextColumnFactory<ProjectTableRow>({
						...projectSectionColumns.billabillity,
						name: t('billability'),
						width: columnWidths.billability,
				  }),
			getColumnWithTooltipFactory<ProjectTableRow>({
				...projectSectionColumns.startDate,
				name: t('startDate'),
				width: columnWidths.startDate,
			}),
			getColumnWithTooltipFactory<ProjectTableRow>({
				...projectSectionColumns.endDate,
				name: t('endDate'),
				width: columnWidths.endDate,
			}),
			getEditColumnFactory<ProjectTableRow>({
				key: '',
				name: '',
				displayFilterButton: false,
				sortable: false,
				frozen: true,
				disabled: true,
			}),
		];

		return columns.filter((column) => column !== null);
	}, [isEmployee, columnWidths]);
};
