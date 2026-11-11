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
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';

import { columnsWidth, RESET_FILTER_COLUMN_INDEX } from './constants';
import { useHardSkillsSectionColumns } from './hooks/useHardSkillsSectionColumns';
import { HardSkillsProps } from './types';
import { convertHardSkillsLookupToRows, getRowKey } from './utils';

export function HardSkills({ hardSkills }: HardSkillsProps) {
	const columns = useHardSkillsSectionColumns();
	const { t } = useTranslation('profile', { keyPrefix: 'personalData' });
	const rows = useMemo(() => {
		if (!hardSkills) {
			return [];
		}

		return convertHardSkillsLookupToRows(hardSkills);
	}, [hardSkills]);

	const overrideGridStyles = useMemo(() => {
		const gridColumnsTemplate = getGridColumnTemplateFromColumnWidths(
			columnsWidth,
			true
		);

		return overrideSectionGridStyles(
			false,
			RESET_FILTER_COLUMN_INDEX,
			gridColumnsTemplate
		);
	}, []);

	return (
		<SectionGridContainer
			headerTitle={t('hardSkills')}
			tabId={UserProfileTabsId.HARD_SKILLS}
		>
			<DataGridHoverRowWrapper defineHeight={false}>
				<DataGrid
					{...COMMON_GRID_PROPS}
					enableVirtualization
					columns={columns}
					rows={rows}
					rowKeyGetter={getRowKey}
					overrideDataGridStyles={overrideGridStyles}
					renderers={noRowsGridFallback}
					scrollToTopOnRowsChange={false}
					{...displayMinimalGridRowsProp}
					maxRowsCount={MAX_SECTION_ROWS_COUNT}
				/>
			</DataGridHoverRowWrapper>
		</SectionGridContainer>
	);
}
