import { css, SerializedStyles, Theme } from '@emotion/react';

import { createActiveBorderColor, createBorderColor } from '@/helpers/getColor';

import {
	RESET_FILTER_COLUMN_INDEX,
	VIEW_LINK_COLUMN_CELL_CLASS_NAME,
	VIEW_LINK_COLUMN_HEADER_CLASS_NAME,
} from './constants';

export const overrideCertificatesGridStyles =
	(additionalStyles: (theme: Theme) => SerializedStyles) => (theme: Theme) =>
		css`
			${additionalStyles(theme).styles}

			.${VIEW_LINK_COLUMN_CELL_CLASS_NAME} {
				border-left: 1px solid ${createActiveBorderColor(theme)};
			}

			.${VIEW_LINK_COLUMN_HEADER_CLASS_NAME} {
				justify-content: flex-end;
				border-left: 1px solid ${createBorderColor(theme)};
			}

			.rdg-header-row
				.rdg-cell[role='columnheader'].${VIEW_LINK_COLUMN_HEADER_CLASS_NAME},
				.rdg-row
				.rdg-cell:not(
					[role='columnheader']
				).${VIEW_LINK_COLUMN_CELL_CLASS_NAME} {
				position: sticky;
				right: -0.1px;
				grid-column: ${RESET_FILTER_COLUMN_INDEX} !important;
				box-shadow: none;
				border-inline-end: none;
				border-right: none;
			}
		`;
