import { css, Theme } from '@emotion/react';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStyles,
} from '@/components/Grid';

import { SOFT_SKILLS_TABLE_COLUMNS_WIDTHS } from './constants';

export const overrideSoftSkillsSectionTableStyles = (theme: Theme) =>
	css`
		${overrideGridStyles(
			false,
			getGridColumnTemplateFromColumnWidths(SOFT_SKILLS_TABLE_COLUMNS_WIDTHS)
		)(theme).styles}

		.rdg-row {
			background-color: ${theme.palette['background-white']};
		}
	`;
