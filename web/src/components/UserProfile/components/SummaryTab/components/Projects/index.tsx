import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { overrideSectionGridStyles } from '../../utils';
import { SectionGridContainer } from '../SectionGridContainer';

import { getGridColumnTemplateFromColumnWidths } from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import {
	COMMON_GRID_PROPS,
	displayMinimalGridRowsProp,
} from '@/components/Grid/constants';
import { MAX_SECTION_ROWS_COUNT } from '@/components/UserProfile/constansts';
import { UserProfileTabsId } from '@/components/UserProfile/types';
import {
	convertUserProjectsToRows,
	getRowKey,
} from '@/components/UserProfile/utils';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { isRegularEmployee } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';

import {
	columnsWidth,
	columnsWidthForEmployee,
	RESET_FILTER_COLUMN_INDEX,
} from './constants';
import { useProjectSectionColumns } from './hooks/useProjectSectionColums';
import { ProjectsProps } from './types';

export function Projects({ projects }: ProjectsProps) {
	const { role } = useAuth();
	const isUserEmployee = isRegularEmployee(role);
	const { t } = useTranslation('profile', { keyPrefix: 'projects' });

	const actualColumnsWidth = isUserEmployee
		? columnsWidthForEmployee
		: columnsWidth;

	const columns = useProjectSectionColumns(actualColumnsWidth, isUserEmployee);

	const rows = useMemo(() => {
		if (!projects) {
			return [];
		}

		return convertUserProjectsToRows(projects);
	}, [projects]);

	const overrideGridStyles = useMemo(() => {
		const gridColumnsTemplate = getGridColumnTemplateFromColumnWidths(
			actualColumnsWidth,
			true
		);

		return overrideSectionGridStyles(
			false,
			RESET_FILTER_COLUMN_INDEX,
			gridColumnsTemplate
		);
	}, [actualColumnsWidth]);

	return (
		<SectionGridContainer
			tabId={UserProfileTabsId.PROJECTS}
			headerTitle={t('sectionHeader')}
		>
			<DataGridHoverRowWrapper defineHeight={false}>
				<DataGrid
					{...COMMON_GRID_PROPS}
					enableVirtualization
					columns={columns}
					rows={rows}
					rowKeyGetter={getRowKey}
					renderers={noRowsGridFallback}
					overrideDataGridStyles={overrideGridStyles}
					scrollToTopOnRowsChange={false}
					{...displayMinimalGridRowsProp}
					maxRowsCount={MAX_SECTION_ROWS_COUNT}
				/>
			</DataGridHoverRowWrapper>
		</SectionGridContainer>
	);
}
