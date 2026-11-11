import { Theme } from '@emotion/react';

import { overrideGridStylesWithEditColumn } from '@/components/Grid';
import { overrideGridStylesWithDynamicCell } from '@/components/Grid/renderers/cell/dynamicHeightCellRenderer/styles';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { overrideTeamboardSectionTableStyles } from '@/pages/TeamBoardUser/utils';
import { GridColumnWidth } from '@/types/grid';
import { TeamboardTableMode } from '@/types/teamboard';

export const getDevelopmentPlanTableColumnWidth = (
	width: GridColumnWidth | undefined
) => getColumnWidth(width ?? 0).width;

export const overrideDevelopmentPlanGridStyles =
	(
		isPaginationShown: boolean,
		mode: TeamboardTableMode,
		columnIndex?: number,
		templateColumns?: string
	) =>
	(theme: Theme) => {
		const basicStyles =
			mode === TeamboardTableMode.Tab
				? overrideGridStylesWithEditColumn(
						isPaginationShown,
						columnIndex,
						templateColumns
				  )
				: overrideTeamboardSectionTableStyles(
						isPaginationShown,
						mode,
						columnIndex,
						templateColumns
				  );

		return overrideGridStylesWithDynamicCell(basicStyles)(theme);
	};
