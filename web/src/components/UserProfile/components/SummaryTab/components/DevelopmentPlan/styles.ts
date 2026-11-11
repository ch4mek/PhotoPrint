import { Theme } from '@emotion/react';

import { overrideGridStylesWithDynamicCell } from '@/components/Grid/renderers/cell/dynamicHeightCellRenderer/styles';
import { overrideSectionGridStyles } from '@/components/UserProfile/components/SummaryTab/utils';

export const overrideDevelopmentPlanSectionGridStyles =
	(
		isPaginationShown: boolean,
		columnIndex?: number,
		templateColumns?: string
	) =>
	(theme: Theme) =>
		overrideGridStylesWithDynamicCell(
			overrideSectionGridStyles(isPaginationShown, columnIndex, templateColumns)
		)(theme);
