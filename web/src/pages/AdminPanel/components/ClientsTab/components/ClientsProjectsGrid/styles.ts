import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { CLIENT_PROJECTS_CELL_FONT_SIZE } from '@/components/Grid/constants';

import { CELL_DIVIDER_HEIGHT, CELL_SCROLLBAR_HEIGHT } from './constants';

export const overrideClientProjectsGridStyles =
	(gridTemplateColumns: string) => (theme: Theme) =>
		css`
			height: auto;
			width: 100%;
			overflow: visible;
			contain: none;
			content-visibility: visible;
			overscroll-behavior: auto;

			${gridTemplateColumns};

			.grid-cell {
				padding: 0;
			}

			.rdg-header-row {
				display: none;
			}

			.rdg-header-row > .rdg-cell::after {
				display: none;
			}

			.rdg-cell {
				border-block-end: unset;
				border-left: 0;
				display: inline-block;
				text-overflow: ellipsis;
				background-color: ${theme.palette['background-white']};
				outline: 0 solid white;
				padding: 0 ${theme.spaces.s}px;
				font-size: ${CLIENT_PROJECTS_CELL_FONT_SIZE}px;

				&::after {
					content: '';
					position: absolute;
					left: 0;
					bottom: 0;
					height: ${CELL_DIVIDER_HEIGHT}px;
					width: 100%;
					background-color: ${theme.palette['graphics-divider-2']};
				}

				& span {
					text-overflow: ellipsis;
					width: 100%;
				}

				.rdg-row-odd .rdg-cell,
				.rdg-row-even .rdg-cell {
					background-color: ${theme.palette['background-white']};
				}

				&::-webkit-scrollbar {
					height: ${CELL_SCROLLBAR_HEIGHT};
				}
			}
		`;

export const ClientsProjectsGridWrapper = styled('div')`
	& > div {
		border: none;
		block-size: 100%;
		overflow: visible;
		overscroll-behavior: auto;
		contain: none;
		border-radius: 0;
	}
`;
