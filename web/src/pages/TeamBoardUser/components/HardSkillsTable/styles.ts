import { css, Theme } from '@emotion/react';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStyles,
} from '@/components/Grid';

import { HARD_SKILLS_TABLE_COLUMNS_WIDTHS } from './constants';

export const overrideHardSkillsSectionTableStyles = (theme: Theme) =>
	css`
		${overrideGridStyles(
			false,
			getGridColumnTemplateFromColumnWidths(HARD_SKILLS_TABLE_COLUMNS_WIDTHS)
		)(theme).styles}

		.rdg-row {
			background-color: ${theme.palette['background-white']};
		}
	`;
