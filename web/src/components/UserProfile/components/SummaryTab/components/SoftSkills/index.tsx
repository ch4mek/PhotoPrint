import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { SectionGridContainer } from '../SectionGridContainer';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import {
	COMMON_GRID_PROPS,
	displayMinimalGridRowsProp,
} from '@/components/Grid/constants';
import { MAX_SECTION_ROWS_COUNT } from '@/components/UserProfile/constansts';
import { UserProfileTabsId } from '@/components/UserProfile/types';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';

import { useSoftSkillsTableColumns } from './hooks/useSoftSkillsSectionColumns';
import { overrideSoftSkillsSectionTableStyles } from './styles';
import { SoftSkillsProps } from './types';
import { getRowKey, mapSoftSkillsLookupToRows } from './utils';

export function SoftSkills({ softSkills }: SoftSkillsProps) {
	const columns = useSoftSkillsTableColumns();
	const { t } = useTranslation('profile', { keyPrefix: 'personalData' });
	const rows = useMemo(() => {
		if (!softSkills) {
			return [];
		}

		return mapSoftSkillsLookupToRows(softSkills);
	}, [softSkills]);

	return (
		<SectionGridContainer
			headerTitle={t('softSkills')}
			tabId={UserProfileTabsId.SOFT_SKILLS}
		>
			<DataGridHoverRowWrapper defineHeight={false}>
				<DataGrid
					{...COMMON_GRID_PROPS}
					enableVirtualization
					columns={columns}
					rows={rows}
					rowKeyGetter={getRowKey}
					overrideDataGridStyles={overrideSoftSkillsSectionTableStyles}
					renderers={noRowsGridFallback}
					scrollToTopOnRowsChange={false}
					{...displayMinimalGridRowsProp}
					maxRowsCount={MAX_SECTION_ROWS_COUNT}
				/>
			</DataGridHoverRowWrapper>
		</SectionGridContainer>
	);
}
