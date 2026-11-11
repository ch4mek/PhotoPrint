import { css, Theme } from '@emotion/react';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStyles,
} from '@/components/Grid';
import { WIDGET_GRID_HEIGHT } from '@/components/Grid/constants';
import { GridColumnWidthsConfiguration } from '@/types/grid';

export const overrideWidgetGridStyles =
	(gridColumnWidths: GridColumnWidthsConfiguration) => (theme: Theme) => {
		const templateColumns =
			getGridColumnTemplateFromColumnWidths(gridColumnWidths);

		return css`
			${overrideGridStyles(false, templateColumns, true)(theme).styles};
			height: ${`${WIDGET_GRID_HEIGHT}px`};
		`;
	};
