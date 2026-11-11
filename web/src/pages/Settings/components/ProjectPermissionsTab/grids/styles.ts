import { css, Theme } from '@emotion/react';

import { overrideGridStylesWithEditColumn } from '@/components/Grid';

import { PROJECT_PERMISSIONS_GRID_COLUMNS_AMOUNT } from './constants';

interface OverrideProjectPermissionsGridStyles {
	isPaginationShown: boolean;
	templateColumns?: string;
	width?: number;
	height?: number;
}

export const overrideProjectPermissionsGridStyles =
	({
		isPaginationShown,
		templateColumns,
	}: OverrideProjectPermissionsGridStyles) =>
	(theme: Theme) =>
		css`
			${overrideGridStylesWithEditColumn(
				isPaginationShown,
				PROJECT_PERMISSIONS_GRID_COLUMNS_AMOUNT + 2,
				templateColumns
			)(theme)};
		`;
