import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { overrideGridStyles } from '@/components/Grid';
import { CROSS_LINK_CELL_CLASS } from '@/components/Grid/constants';
import { BorderRadiuses } from '@/constants/borderRadiuses';
import {
	getOddRowBackgroundColor,
	getEvenRowBackgroundColor,
	createBorderColor,
} from '@/helpers/getColor';

import {
	CALENDAR_DAY_HEADER_CLASSNAME,
	CALENDAR_ITEM_CLASSNAME,
} from './constants';
import { CalendarColumns } from './types';

const CURRENT_DAY_HEADER_ROW_HEIGHT = 24;

export const overrideCalendarGridStyles =
	(periodLength: number, columns: CalendarColumns, dateColumnWidth: number) =>
	(theme: Theme) =>
		css`
			${overrideGridStyles(true, undefined, false)(theme)}

			.rdg-cell[role='columnheader'].${CALENDAR_DAY_HEADER_CLASSNAME} {
				padding: 2px !important;

				& > div {
					background-color: ${theme.palette['graphics-accent']};
					border-radius: ${BorderRadiuses.xs2}px;
					height: ${CURRENT_DAY_HEADER_ROW_HEIGHT}px;

					& > span {
						color: ${theme.palette['text-white']};
					}
				}
			}

			& > .rdg-header-row > .rdg-cell {
				padding: 0;
				overflow: hidden;

				border-right: 1px solid ${createBorderColor(theme)};

				& > div {
					padding: 0 ${theme.spaces.s}px;
				}
			}

			& > .rdg-row > .rdg-cell:has(.${CALENDAR_ITEM_CLASSNAME}) {
				overflow: visible !important;
				background-color: transparent;
			}

			.rdg-row {
				&-odd {
					background-color: transparent !important;

					.rdg-cell {
						border-top-color: ${getOddRowBackgroundColor(theme)};
						border-bottom-color: ${getOddRowBackgroundColor(theme)};
						border-block-end: 1px solid var(--rdg-border-color);
					}
				}

				&-even {
					background-color: transparent !important;

					.rdg-cell {
						border-top-color: ${getEvenRowBackgroundColor(theme)};
						border-bottom-color: ${getEvenRowBackgroundColor(theme)};
						border-block-end: 1px solid var(--rdg-border-color);
					}
				}

				&:hover .${CROSS_LINK_CELL_CLASS} {
					text-decoration: underline;
					cursor: pointer;
				}
			}

			justify-content: stretch;
			display: grid;
			grid-template-columns: ${columns.userFullName.width}px ${columns
					.department.width}px repeat(
					${periodLength},
					minmax(${dateColumnWidth}px, 1fr)
				) !important;

			& > *:last-child {
				display: none;
			}
		`;

export const AbsenceCalendarGridWrapper = styled('div')({
	display: 'flex',
	flexDirection: 'column',
	flex: 1,
	minHeight: 0,
	overflow: 'hidden',

	'& .data-grid-wrapper': {
		height: '100%',
	},
});
