import { css, Theme } from '@emotion/react';

import { overrideGridStyles } from '@/components/Grid';
import { getHoverBackgroundColor } from '@/helpers/getColor';

import { NAME_CELL_BACKGROUND } from './constants';

export const overrideClaimsGridStyles =
	(templateColumns?: string) => (theme: Theme) =>
		css`
			${overrideGridStyles(false, templateColumns, false)(theme).styles};

			& .rdg-cell[aria-colindex='1'][role='gridcell'] {
				background-color: ${NAME_CELL_BACKGROUND[theme.mode]};
			}

			.rdg-row:hover > .rdg-cell {
				background-color: ${getHoverBackgroundColor(theme)};
			}
			.rdg-cell {
				border-bottom: 1px solid ${theme.palette['graphics-divider-1']};
				& > div:not(:has(svg)) {
					font-size: ${theme.typography.body['s-600'].fontSize};
					font-weight: ${theme.typography.body['s-600'].fontWeight};
					color: ${theme.palette['text-gray']};
				}
			}
		`;
