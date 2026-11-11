import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { DEFAULT_GRID_ROW_HEIGHT } from '@mms/mms-ui-library/constants/grid';
import { useEffect, useMemo, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { SectionGridContainer } from '../SectionGridContainer';

import { getGridColumnTemplateFromColumnWidths } from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import {
	COMMON_GRID_PROPS,
	displayMinimalGridRowsProp,
} from '@/components/Grid/constants';
import { useDynamicRowsHeight } from '@/components/Grid/hooks/useDynamicRowsHeight';
import { MAX_SECTION_ROWS_COUNT } from '@/components/UserProfile/constansts';
import { UserProfileTabsId } from '@/components/UserProfile/types';
import { DEVELOPMENT_PLAN_SECTION_ROW_HEIGHT } from '@/constants/developmentPlan';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { HeightsContext } from '@/context/CellHeightsContext';

import { columnsWidth, RESET_FILTER_COLUMN_INDEX } from './constants';
import { useDevelopmentPlanSectionColumns } from './hooks/useDevelopmentPlanSectionColumns';
import { overrideDevelopmentPlanSectionGridStyles } from './styles';
import { DevelopmentPlanProps, DevelopmentPlanSectionRow } from './types';
import { convertDevelopmentPlanResponseToRows } from './utils';

export function DevelopmentPlan({ developmentPlan }: DevelopmentPlanProps) {
	const [rows, setRows] = useState<DevelopmentPlanSectionRow[]>([]);
	const columns = useDevelopmentPlanSectionColumns();
	const { t } = useTranslation('profile', { keyPrefix: 'developmentPlan' });
	const { handleUpdateRows, heightsMap, getHeight } = useDynamicRowsHeight({
		setRows,
	});

	const overrideGridStyles = useMemo(() => {
		const gridColumnsTemplate = getGridColumnTemplateFromColumnWidths(
			columnsWidth,
			true
		);

		return overrideDevelopmentPlanSectionGridStyles(
			false,
			RESET_FILTER_COLUMN_INDEX,
			gridColumnsTemplate
		);
	}, []);

	useEffect(() => {
		if (developmentPlan?.items) {
			setRows(convertDevelopmentPlanResponseToRows(developmentPlan.items));

			return;
		}

		setRows([]);
	}, [developmentPlan]);

	return (
		<SectionGridContainer
			tabId={UserProfileTabsId.DEVELOPMENT_PLAN}
			headerTitle={t('sectionHeader')}
		>
			<DataGridHoverRowWrapper defineHeight={false}>
				<HeightsContext.Provider value={heightsMap}>
					<DataGrid
						{...COMMON_GRID_PROPS}
						rowHeight={getHeight}
						enableVirtualization
						columns={columns}
						rows={rows}
						onRowsChange={handleUpdateRows}
						renderers={noRowsGridFallback}
						overrideDataGridStyles={overrideGridStyles}
						scrollToTopOnRowsChange={false}
						{...displayMinimalGridRowsProp}
						maxRowsCount={MAX_SECTION_ROWS_COUNT}
						restrictionRowHeight={
							rows.length
								? DEVELOPMENT_PLAN_SECTION_ROW_HEIGHT
								: DEFAULT_GRID_ROW_HEIGHT
						}
					/>
				</HeightsContext.Provider>
			</DataGridHoverRowWrapper>
		</SectionGridContainer>
	);
}
